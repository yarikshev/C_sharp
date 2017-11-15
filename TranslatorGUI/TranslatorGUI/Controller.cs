using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translator;

namespace TranslatorGUI
{
    class Controller
    {
        public static string SettingsFilePath { get; set; }
        public static string SourseCodeFilePath { get; set; }

        public static string grammarPath = Environment.CurrentDirectory + @"\grammar.txt";
        public static string prioritiesPath = Environment.CurrentDirectory + @"\priorities.xml";
        public static string rulesPath = Environment.CurrentDirectory + @"\polizRules.xml";
        public static string compilerPath = @"D:\Лабораторные\3 курс\2 семестр\Трансляторы\TranslatorGUI\Compiler\Compiler\bin\Debug\Compiler.exe";

        public static bool ReadSourceCode()
        {
            try
            {
                Scaner.init(Controller.SourseCodeFilePath);                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> Compile(MainWindow io)
        {
            List<string> output = null;

                Scaner.scan();
                if (Scaner.ErrorsNumber == 0)
                {
                    RelationTable.Init(grammarPath);
                    Syntactic_simple_precedence_grammars.Init();
                    if (Syntactic_simple_precedence_grammars.Errors.Count == 0)
                    {
                        Poliz2.Init(prioritiesPath, rulesPath);
                        //TODO создать новый процесс
                        System.Diagnostics.Process.Start(compilerPath, String.Concat(Poliz2.Poliz.ToArray()));
                    }
                    output = Syntactic_simple_precedence_grammars.Errors;
                }
                else
                    output = Scaner.Errors;

            return output;
        }

    }
}
