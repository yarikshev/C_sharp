using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace translator
{
    public struct lexeme
    {
        public int line;
        public string value;
        public int key;
        public int? index;

        public lexeme(int line, string value, int key, int? index)
        {
            this.line = line;
            this.value = value;
            this.key = key;
            this.index = index;
        }
    }

   static class Scaner
    {
        private static string path; //source code
        private static string[] sourceCode;
       //  code of lexeme                    0      1     2      3       4    5      6     7      8    9      10     11    12   13   14  15  16  17  18  19  20   21  22  23   24   25   26   27 28  29  30  31   32    33 34    35
        private static string[] lexemes = {"main","int","read","write","for","to","step","next","if","else","endif","not","or","and","^","+","-","*","/","=","==",">","<",">","<=",">=","!=","(",")",",","{","}",@"\n","[","]", "then", "do", "continue"};
        private static char[] separators = { ',', '+', '-', '*', '/', '(', ')', '^', '{', '}','\n','[',']'};
        private static char[] lettersNumbers = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static char[] logical = {'>','<','=','!'};
        private static char space = ' ';
        private static List<string> lexemeList = new List<string>(lexemes);
        private static List<lexeme> lexemeOutput = new List<lexeme>();
        private static List<string> idOutput = new List<string>();
        private static List<string> conOutput = new List<string>();
        private static List<string> errorOutput = new List<string>();

        public static List<string> Errors { get { return errorOutput; } }
        public static List<string> LexemeList
        {
            get { return lexemeList; }
        }

        public static List<lexeme> LexemeOutput
        {
            get { return lexemeOutput; }
        }

       public static List<string> IdOutput
        {
            get { return idOutput; }
        }
       public static List<string> ConOutput
       {
           get { return conOutput; }
       }
        public static int ErrorsNumber
        {
            get { return errorOutput.Count; }
        }

        public static void init(string pathfile)
        {
            sourceCode = null;
            lexemeOutput.Clear();
            idOutput.Clear();
            conOutput.Clear();
            errorOutput.Clear();

            path = pathfile;
            readCode();
        }

        private static void readCode()
        {
            sourceCode = System.IO.File.ReadAllLines(path);
        }

        public static void displayLexeme()
        {
            for (int i = 0; i < lexemes.Length; i++)
            {
                Console.WriteLine("{0}:{1}",lexemes[i],i);
            }
        }

        public static void displayCode()
        {
            for (int i = 0; i < sourceCode.Length; i++)
                Console.WriteLine(sourceCode[i]);
        }

        private static bool isIdentifier(string lex)
        {
            Regex reg = new  Regex(@"^([a-z])([a-z0-9])*$",RegexOptions.IgnoreCase);
            return reg.IsMatch(lex);
        }

        private static int Class(char symbol)
        {
           int klass=0;
           if (lettersNumbers.Contains(symbol))
               klass = 1;
           else
               if (separators.Contains(symbol))
                       klass = 3;
                   else
                       if (space == symbol)
                           klass = 0;
                       else
                           if (logical.Contains(symbol))
                               klass = 4;
                           else
                               klass = -1;

            
           return klass;
        }

        private static void addLexeme(string lex,int num)
        {
            
            int key = lexemeList.IndexOf(lex);
            if (key >= 0)
            {
                lexemeOutput.Add(new lexeme(num + 1, lex, key, null));
            }
            else
            {
                try
                {
                    Convert.ToInt32(lex);
                    key = conOutput.IndexOf(lex);
                    if (key >= 0)
                        lexemeOutput.Add(new lexeme(num + 1, lex, 50, key));
                    else
                    {
                        conOutput.Add(lex);
                        lexemeOutput.Add(new lexeme(num + 1, lex, 50, conOutput.Count - 1));
                    }
                }
                catch (FormatException)
                {
                    key = idOutput.IndexOf(lex);
                    if (key >= 0)
                    {
                        if (lexemeOutput.IndexOf(new lexeme(num + 1, "int", lexemeList.IndexOf("int"), null)) < 0)
                        lexemeOutput.Add(new lexeme(num + 1, lex, 51, key));
                        else
                            errorOutput.Add("Error : Duplicate declaration " + lex + " on line " + (num + 1).ToString());
                    }
                    else
                    {
                        if (lexemeOutput.IndexOf(new lexeme(num + 1, "int", lexemeList.IndexOf("int"), null)) > 0 )//|| idOutput.Count == 0)
                        {

                            if (isIdentifier(lex))
                            {
                                idOutput.Add(lex);
                                lexemeOutput.Add(new lexeme(num + 1, lex, 51, idOutput.Count - 1));
                            }
                            else
                                errorOutput.Add("Error : Identifier " + lex + " begins with a number on line " + (num + 1).ToString());
                        }
                        else
                            errorOutput.Add("Error : Undefined identificator " + lex + " on line " + (num + 1).ToString());

                    }
                }
            }
        }

        public static void scan()
        {
            string word = "";
            bool sp = true;
            int length = 0;
            int lineNum = 0;
            int klass = 0;
            foreach (string line in sourceCode)
            {
                length = line.Length;
                sp = true;
                for (int i = 0; i < length ; i++)
                {
                    klass = Class(line[i]);
                    switch (klass)
                    {
                        case 0: if (!sp)
                                addLexeme(word, lineNum);
                                word = "";
                                sp = true;
                                break;
                        case 3: if(!sp)
                                addLexeme(word, lineNum);
                                addLexeme("" + line[i], lineNum);
                                word = "";
                                sp = true;
                                break;
                        case 4: if(!sp)
                                addLexeme(word, lineNum);
                                if(i+1<length)
                                    if (line[i + 1] == '=')
                                    {
                                        addLexeme("" + line[i] + line[i + 1], lineNum);
                                        i++;
                                    }
                                    else
                                        addLexeme("" + line[i], lineNum);
                                else
                                    addLexeme("" + line[i], lineNum);
                                word = "";
                                sp = true;
                                break;
                        case 1: word += line[i];
                                sp = false;
                                break;
                        case -1: errorOutput.Add("Error: Unknown symbol "+line[i]+" on line "+ (lineNum+1).ToString() + " in column "+(i+1).ToString());
                                break;
                    } 
                }
                if ((word != "") && (word != null))
                    addLexeme(word, lineNum);
                if(line.Length > 0)
                addLexeme(@"\n", lineNum);
                word = null;
                ++lineNum;
            }
        }

        public static void displayLexemeOutput()
        {
            Console.WriteLine("Lexemes:\n\n{0,8}|{1,8}|{2,5}|{3,5}","Line","Value","key","Index con/id");
            Console.WriteLine("----------------------------------");
            for (int i = 0; i < lexemeOutput.Count; i++)
            {
                Console.WriteLine("{0,8}|{1,8}|{2,5}|{3,5}",lexemeOutput[i].line,lexemeOutput[i].value, lexemeOutput[i].key,lexemeOutput[i].index);
            }
        }

        public static void displayIdOutput()
        {
            Console.WriteLine("\nIdentificators:\n\n{0,8}|{1,5}","Value","Index");
            Console.WriteLine("--------------------------");
            for (int i = 0; i < idOutput.Count; i++)
            {
                Console.WriteLine("{0,8}|{1,5}",idOutput[i],i);
            }
        }

        public static void displayConst()
        {
            Console.WriteLine("\nConstants\n\n{0,8}|{1,5}","Value","Index");
            Console.WriteLine("----------------------------------");
            for (int i = 0; i < conOutput.Count; i++)
            {
                Console.WriteLine("{0,8}|{1,5}",conOutput[i],i);
            }
        }

        public static void displayErrors()
        {
            if (errorOutput.Count == 0)
            {
                Console.WriteLine("\nLexical analysis is done successfully \n");
            }
            else
            {
                for (int i = 0; i < errorOutput.Count; i++)
                {
                    Console.WriteLine(errorOutput[i]);
                }
            }
        }

    }
}
