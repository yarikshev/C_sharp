using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace translator
{

    struct specLexeme
    {
        public int key;
        public int? clearTo;
        public bool include;
        public bool push;
        public bool addToPoliz;
        public int? labelsNumber;
        public string poliz;

        public specLexeme(int nkey, int? nclearTo, bool ninclude, bool npush, bool naddToPoliz,int? nlabelNumber, string npoliz)
        {
            key = nkey;
            clearTo = nclearTo;
            include = ninclude;
            push = npush;
            addToPoliz = naddToPoliz;
            labelsNumber = nlabelNumber;
            poliz = npoliz;
        }
    }

    static class Poliz2
    {
        static public List<string> Poliz { get; set;}
        static private Stack<lexeme> stack;
        

        static private Stack<int> labels;
        static private int lastLabel;

        static private Stack<int> cells;
        static private int lastCell;
        static private string l;  // name of identifier for cycle // for l = a 
        static private lexeme operation ;

        static private Dictionary<int, int> priorities;
        static private Dictionary<int, specLexeme> specLexemes;

        static private List<lexeme> lexemeOutput = Scaner.LexemeOutput;
        static private List<string> lexemes = Scaner.LexemeList;

        static private List<string>[] history = new List<string>[3];

        static public List<string>[] History { get { return history; } }

        static Poliz2()
        {
            history[0] = new List<string>();
            history[1] = new List<string>();
            history[2] = new List<string>();
        }

        static public void Init(string prioritiesPath, string rulesPath)
        {
            Poliz = new List<string>();
            stack = new Stack<lexeme>();            
            labels = new Stack<int>();
            cells = new Stack<int>();
            stack.Push(new lexeme(-1,"",-1,null));
            history[0] = new List<string>();
            history[1] = new List<string>();
            history[2] = new List<string>();

            priorities = ReadPriorities(prioritiesPath);
            specLexemes = ReadRules(rulesPath);
            GeneratePoliz();
            DeleteWrongSymbols();
            MakeSplitable(' ');
        }

        static private void MakeSplitable(char symbol)
        {
            int i = 1;
            while(Poliz.Count >= i + 1)
            {
                Poliz.Insert(i, symbol.ToString());
                i = i + 2;
            }
        }

        static private bool Prediacate(string s)
        {
            return s == @"\n" ? true : false;
        }

        static private void DeleteWrongSymbols()
        {
            Poliz.RemoveAll(Prediacate);
        }

        static private void ShowPoliz()
        {
            string[] a = Poliz.ToArray();
            Console.WriteLine("Poliz:");
            for(int i=0; i< a.Length; i++)
            {
                Console.Write(a[i] + " ");
            }
        }

        static private Dictionary<int, int> ReadPriorities(string path)
        {
            XmlDocument prioritiesList = new XmlDocument();
            prioritiesList.Load(path);
            XmlNode root = prioritiesList.SelectSingleNode("priorities");
            XmlNodeList lexemesList = root.SelectNodes("lexeme");
            Dictionary<int, int> prioritiList = new Dictionary<int, int>();
            for (int i = 0; i < lexemesList.Count; i++)
            {   
                prioritiList.Add(lexemes.IndexOf(lexemesList[i].Attributes[0].InnerText), Int32.Parse(lexemesList[i].Attributes[1].InnerText));
            }
            return prioritiList;
        }

        static private Dictionary<int, specLexeme> ReadRules(string path)
        {
            XmlDocument rules = new XmlDocument();
            rules.Load(path);
            XmlNode root = rules.SelectSingleNode("rules");
            XmlNodeList lexemesList = root.SelectNodes("lexeme");

            Dictionary<int, specLexeme> spec = new Dictionary<int, specLexeme>();
            for (int i = 0; i < lexemesList.Count; i++ )
            {
                int key = lexemes.IndexOf(lexemesList[i].Attributes[0].InnerText);
                int? clearTo = lexemesList[i].SelectSingleNode("clearto").InnerText == "null" ? null :  (int?)lexemes.IndexOf(lexemesList[i].SelectSingleNode("clearto").InnerText);
                bool include = bool.Parse(lexemesList[i].SelectSingleNode("clearto").Attributes[0].InnerText);
                bool push = bool.Parse(lexemesList[i].SelectSingleNode("push").InnerText);
                bool add = bool.Parse(lexemesList[i].SelectSingleNode("addToPoliz").InnerText);
                int? labelNumber = lexemesList[i].SelectSingleNode("labels").InnerText == "null" ? null : (int?)Int32.Parse(lexemesList[i].SelectSingleNode("labels").InnerText);
                string poliz = lexemesList[i].SelectSingleNode("poliz").InnerText == "null" ? null : lexemesList[i].SelectSingleNode("poliz").InnerText;
                spec.Add(key, new specLexeme(key, clearTo, include, push, add,labelNumber, poliz));                                                                     
            }
            return spec;
        }

        static private int GetPrioritet(int indexOfLexeme)
        {
            int result = 0;
            if (priorities.TryGetValue(indexOfLexeme, out result))
                return result;
            return -1;
        }

        static private specLexeme IsSpecLexeme(lexeme op)
        {
            try
            {
                return specLexemes[op.key];
            }
            catch
            {
                return new specLexeme(-1,null,false,false,false,null,null);
            }
        }

        static private void PolizFieldGenerator(string s, lexeme lex)
        {
            string[] commands = s.Split(' ');
            string polizAdd = "";
            int a = 0;

            for(int i=0;i<commands.Length; i++)
            {
                switch (commands[i])
                {
                    case "_m":
                        {
                            polizAdd += commands[i];
                            if (Int32.TryParse(commands[i + 1], out a))
                            {
                                polizAdd += (labels.Peek() - a).ToString();
                                Poliz.Add(polizAdd);
                                polizAdd = "";
                                i++;
                            }
                        }
                        break;
                    case "_r":
                        {
                            polizAdd += commands[i];
                            if (Int32.TryParse(commands[i + 1], out a))
                            {
                                polizAdd += (labels.Peek() - a).ToString();
                                Poliz.Add(polizAdd);
                                polizAdd = "";
                                i++;
                            }
                        }
                        break;
                    case ":":
                        {
                            //labelsTable.Add(poliz[poliz.Count - 1], poliz.Count - 1);
                            Poliz.Add(commands[i]);
                        } break;
                    case "cells" :
                        {
                            int cellsnum;
                            if (Int32.TryParse(commands[i + 1], out cellsnum))
                            {
                                cells.Push(lastCell + cellsnum);
                                lastCell += cellsnum;
                                i++;
                            }
                        }break;
                    case "_l" :
                        {
                            Poliz.Add(l);
                        }break;
                    case "popLabel" :
                        {
                            labels.Pop();
                        }break;
                    case "popcell" :
                        {
                            cells.Pop();
                        }break;
                    case "rememberL" :
                        {
                            l = Poliz[Poliz.Count - 1];
                        }break;
                    case "operation":
                        {
                            operation = lex;
                            i++;
                        } break;
                    case "pushOperation":
                        {
                            stack.Push(operation);
                        } break;
                    default :
                        {
                           // if (commands[i] == "=" || commands[i] == "==" || Int32.TryParse(commands[i], out a))
                            {
                                Poliz.Add(commands[i]);
                            }       
                        } break;
                }                                                                             
            }

        }

        static private void SpecLexemeManager(specLexeme spec, lexeme lex)
        {
            if (spec.clearTo != null)
            {
                specLexeme a = IsSpecLexeme(stack.Peek());
                while (stack.Peek().key != spec.clearTo)
                {
                    a = IsSpecLexeme(stack.Peek());
                    if (a.key > 0)
                    {
                        if (a.addToPoliz)
                        {
                            Poliz.Add(stack.Pop().value);
                        }
                        else
                            stack.Pop();
                    }
                    else
                    {
                        Poliz.Add(stack.Pop().value);
                    }
                }
                if(spec.include)
                {
                    if(a.addToPoliz)
                    {
                        Poliz.Add(stack.Pop().value);
                    }
                    else
                        stack.Pop();                    
                }
            }
            if(spec.push)
            {
                stack.Push(lex);
            }
            if(spec.labelsNumber > 0)
            {                            
                labels.Push(lastLabel + (int)spec.labelsNumber);
                lastLabel += (int)spec.labelsNumber;
            }
            if(spec.poliz != null)
            {                
                PolizFieldGenerator(spec.poliz, lex);
            }

        }

        static private void GeneratePoliz()
        {
            bool ignore = false;
            for(int i = 3; i < lexemeOutput.Count - 2; i++)
            {
                if (!ignore) { ignore = lexemeOutput[i].value == "int" ? true : false; } // игнорировать объявление переменных
                if(!ignore)
                {
                    SaveParsingHistory(i);
                    if(lexemeOutput[i].key == 50 || lexemeOutput[i].key == 51)
                    {
                        Poliz.Add(lexemeOutput[i].value);
                    }
                    else
                    {
                        int prioritet = GetPrioritet(lexemeOutput[i].key);
                        specLexeme spec = IsSpecLexeme(lexemeOutput[i]);
                        if(spec.key > 0)
                        {                            
                            SpecLexemeManager((specLexeme)spec, lexemeOutput[i]);
                        }
                        else
                        {
                            while (GetPrioritet(stack.Peek().key) > prioritet)
                            {
                                Poliz.Add(stack.Pop().value);
                            }
                            stack.Push(lexemeOutput[i]);
                        }
                    }                    
                }
                else
                {
                    if (lexemeOutput[i].value == @"\n")
                        ignore = false;
                }                
            }
        }

        static private void SaveParsingHistory(int counter)
        {
            string col0 = null, col1 = null, col2 = null;
            for (int i = 0; i < Poliz.Count; i++)
                col0 += Poliz[i] + " ";
            lexeme[] tempStack = stack.ToArray();
            for(int i=0;i < tempStack.Length;i++)
                col1 += tempStack[i].value + " ";
            for (int i = counter; i < lexemeOutput.Count; i++)
                col2 += lexemeOutput[i].value + " ";
            history[0].Add(col0);
            history[1].Add(col1);
            history[2].Add(col2);
        }
    }
}
