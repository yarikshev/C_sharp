using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace translator
{
    struct Rule
    {
        public int key;
        public string leftPart;
        public List<string[]> rightPart;
        
       public Rule(int key, string left, string[] right)
        {
            this.key = key;
            leftPart = left;
            rightPart = new List<string[]>();
            rightPart.Add(right);
        }
    }

    enum Relation
    {
        equal = 2, less = 1, more = 3
    }

    static class RelationTable
    {
        private static List<string> lexemeList;
        private static List<Rule> rules = new List<Rule>();
        private static int[,] table; // 555 - = 777 - < , 888 - > 
        private static int symbolNumber = 51; // keys of Rules
        private static Dictionary<int,int> ticket = new Dictionary<int,int>(); // key - key of lexeme, value - # of column in the table

        public static List<Rule> Rules
        {
            get { return rules; }
        }

        public static int[,] GetRelationTable
        {
            get { return table; }
        }

        public static Dictionary<int, int> GetTickets
        {
            get { return ticket; }
        }

        public static void Init(string path)
        {
            ticket.Clear();
            rules.Clear();
            lexemeList = Scaner.LexemeList;
            symbolNumber = 51;
            ReadRules(System.IO.File.ReadAllLines(path));
            InitTable();
            SetEqualRelation();            
        }

        private static void ReadRules(string[] grammar)
        {
            for(int i=0;i<grammar.Length;i++)
            {
                string[] rule = grammar[i].Split(' ');
                int index = GetRuleIndex(rule[0]);

                if (index > 0)
                {
                    rules[index].rightPart.Add(GetRightPart(rule));
                }
                else
                {
                    rules.Add(new Rule(++symbolNumber, rule[0], GetRightPart(rule)));
                }
            }
        }

        private static void InitTable()
        {
            table = new int[rules.Count + lexemeList.Count + 4, rules.Count + lexemeList.Count + 4];
            //terminal
            for (int i = 0; i < lexemeList.Count; i++)
            {
                table[0, i + 1] = i;
                table[i + 1, 0] = i;
                ticket.Add(i, i + 1);
            }
            table[0, lexemeList.Count + 1] = 50; // const
            table[lexemeList.Count + 1, 0] = 50;

            //ticket.Add(50, 36);
            ticket.Add(50, lexemeList.Count + 1);

            table[0, lexemeList.Count + 2] = 51; // identificator
            table[lexemeList.Count + 2, 0] = 51;

            //ticket.Add(51, 37);
            ticket.Add(51, lexemeList.Count + 2);

            // non terminal
            for (int i = 0; i < rules.Count; i++)
            {
                table[0, lexemeList.Count + i + 3] = rules[i].key;
                table[lexemeList.Count + i + 3, 0] = rules[i].key;
                ticket.Add(rules[i].key,lexemeList.Count + i + 3);
            }
            table[0, (int)Math.Sqrt(table.Length) - 1] = 99;
            table[(int)Math.Sqrt(table.Length) - 1, 0] = 99;
            ticket.Add(99, lexemeList.Count + rules.Count + 3);

            for (int i = 1; i < (int)Math.Sqrt(table.Length); i++)
            {
                table[i,(int)Math.Sqrt(table.Length) - 1] = (int)Relation.more;
                table[(int)Math.Sqrt(table.Length) - 1, i] = (int)Relation.less;
            }
        }

        private static void SetEqualRelation()
        {
            for (int i = 0; i < rules.Count; i++)
            {
                for (int j = 0; j < rules[i].rightPart.Count; j++)
                {
                    if (rules[i].rightPart[j].Length > 1)
                    {
                        for (int k = 0; k < rules[i].rightPart[j].Length - 1; k++)
                        {
                           int row  = TicketOf(rules[i].rightPart[j][k]);
                           int col = TicketOf(rules[i].rightPart[j][k+1]);
                           if (table[row, col] != 0 && table[row, col] != (int)Relation.equal)
                               Console.WriteLine("Error in cell {0} , {1} ", row,col);
                           table[row, col] = (int)Relation.equal;//555;
                           SetRelation(rules[i].rightPart[j][k], rules[i].rightPart[j][k + 1]);
                        }
                    }
                }
            }
        }

        private static void SetRelation(string op1, string op2)
        {
            int isTermOp1 = GetRuleIndex(op1);
            int isTermOp2 = GetRuleIndex(op2);

            if (isTermOp1 >= 0 && isTermOp2 < 0)
            {
                // nonterm term
                int termIndex = lexemeList.IndexOf(op2);
                if (op2 == "id")
                    termIndex = 51;
                if (op2 == "con")
                    termIndex = 50;
                SetMoreRealtion(isTermOp1, termIndex);
            }
            if (isTermOp1 < 0 && isTermOp2 >= 0)
            {
                // term nonterm
                int termIndex = lexemeList.IndexOf(op1);
                if (op1 == "id")
                    termIndex = 51;
                if (op1 == "con")
                    termIndex = 50;
                SetLessRelation(termIndex, isTermOp2);
            }

            if (isTermOp1 > 0 && isTermOp2 > 0)
            {
                // nonterm term
                // term nonterm
                SetMoreRelationBetweenNonTerms(isTermOp1, isTermOp2);
                
            }
        }

        private static void SetMoreRealtion(int ruleIndex, int termIndex)
        {
            List<string> lastPlus = LastPlus(rules[ruleIndex]);
            for (int i = 0; i < lastPlus.Count; i++)
            {
                int row = TicketOf(lastPlus[i]);
                int col = ticket[termIndex];

                if (table[row, col] != 0 && table[row, col] != (int)Relation.more)
                    Console.WriteLine("Error int table [{0},{1}] Last+: {2:s} term: {3:s} relation WAS : {4}, WANT to SET {5}, Rule {6}", row, col, lastPlus[i], lexemeList[termIndex], table[row, col], ">", rules[ruleIndex].leftPart);
                else
                    table[row, col] = (int)Relation.more;
            }
        }

        private static void SetLessRelation(int termIndex, int ruleIndex)
        {
            List<string> firstPlus = FirstPlus(rules[ruleIndex]);
            for (int i = 0; i < firstPlus.Count; i++)
            {
                int row = ticket[termIndex];
                int col = TicketOf(firstPlus[i]);
                if (table[row, col] != 0 && table[row, col] != (int)Relation.less)
                {
                    Console.WriteLine("Error int table [{0},{1}] term: {2:s} first+: {3:s} relation WAS : {4}, WANT to SET {5} Rule {6}", row, col, termIndex < 50 ? lexemeList[termIndex] : "id or con", firstPlus[i], table[row, col], "<", rules[ruleIndex].leftPart);
                }
                else
                    table[row, col] = (int)Relation.less;
            }
        }

        private static void SetMoreRelationBetweenNonTerms(int NonTerm1Index, int NonTerm2Index)
        {
            List<string> lastPlus = LastPlus(rules[NonTerm1Index]);
            List<string> firstPlus = FirstPlus(rules[NonTerm2Index]);

            for (int i = 0; i < lastPlus.Count; i++)
            {
                int Row = TicketOf(lastPlus[i]);
                for (int j = 0; j < firstPlus.Count; j++)
                {
                    int Col = TicketOf(firstPlus[j]);
                    if (table[Row, Col] != (int)Relation.less)
                        table[Row, Col] = (int)Relation.more;
                    else
                        Console.WriteLine("Error☺ int table[" + Row + "," + Col + "] : last+ " + lastPlus[i] + " and first + " + firstPlus[j] + " was " + (Relation)table[Row,Col] + " want to set More");
                }
            }

            //Experiment
            int row = TicketOf(rules[NonTerm1Index].leftPart);
            for (int i = 0; i < firstPlus.Count; i++)
            {
                int col = TicketOf(firstPlus[i]);
                if (table[row, col] != (int)Relation.more)
                    table[row, col] = (int)Relation.less;
                else
                    Console.WriteLine("Error : Experiment");
            }
         
        }

        private static void GetElement(Rule item, List<string> firstPlus, bool isfirs)
        {
            for (int i = 0; i < item.rightPart.Count; i++)
            {
                int j;

                if(isfirs)
                  j = 0;
                else
                  j = item.rightPart[i].Length - 1;

                    if (lexemeList.IndexOf(item.rightPart[i][j]) >= 0 || item.rightPart[i][j] == "id" || item.rightPart[i][j] == "con")
                    {
                        if(!firstPlus.Contains(item.rightPart[i][j]))
                        firstPlus.Add(item.rightPart[i][j]);
                    }
                    else
                    {
                        if (item.rightPart[i][j] != item.leftPart)
                        {
                            int index = GetRuleIndex(item.rightPart[i][j]);
                            if (!firstPlus.Contains(item.rightPart[i][j]))
                              firstPlus.Add(item.rightPart[i][j]);  // NEW  !!!!
                            GetElement(rules[index], firstPlus, isfirs);
                        }
                    }   
            }
        }

        private static List<string> FirstPlus(Rule item)
        {
            List<string> firstPlus = new List<string>();
            GetElement(item, firstPlus, true);
            return firstPlus;
        }

        private static List<string> LastPlus(Rule item)
        {
            List<string> lastPlus = new List<string>();
            GetElement(item, lastPlus, false);
            return lastPlus;
        }

        private static void DisplayTable()
        {
            for (int i = 0; i < Math.Sqrt(table.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(table.Length); j++)
                {
                    Console.Write("{0,2:d} ", table[i, j]);
                }
                Console.WriteLine();
            }
        }

        private static void DisplayTableInExel()
        {
            Application exelTable = new Application();
            exelTable.Application.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Range columns = exelTable.get_Range("A1","BN66");
            columns.ColumnWidth = 5;

            for (int i = 1; i < Math.Sqrt(table.Length); i++)
            {
                exelTable.Cells[i + 1, 1] = GetSymbol(table[i, 0]);
                exelTable.Cells[1, i + 1] = GetSymbol(table[0, i]);
            }
            for (int i = 1; i < Math.Sqrt(table.Length); i++)
            {
                for (int j = 1; j < Math.Sqrt(table.Length); j++)
                {
                    string symbol = "";
                    switch (table[i, j])
                    {
                        case (int)Relation.equal: symbol = "=";
                            break;
                        case (int)Relation.less: symbol = "<";
                            break;
                        case (int)Relation.more: symbol = ">";
                            break;
                    }
                    exelTable.Cells[i + 1, j + 1] = symbol;
                }
            }
            exelTable.Visible = true;
        }

        private static string[] GetRightPart(string[] rule)
        {
            string[] right = new string[rule.Length - 1];

            for (int i = 0; i < right.Length; i++)
            {
                right[i] = rule[i + 1];
            }
            return right;
        }

        private static int GetRuleIndex(string name)
        {
            int index = -1;
            for (int i = 0; i < rules.Count; i++)
            {
                if (name == rules[i].leftPart)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public static int TicketOf(string name)
        {
            int index = GetRuleIndex(name);
            if (index >= 0)
            {
                return ticket[rules[index].key]; 
            }
            else
            {
                if (name == "id")
                    return ticket[51];
                if (name == "con")
                    return ticket[50];
                return ticket[lexemeList.IndexOf(name)];
            }
        }

        public static string GetSymbol(int key) // переделать бинарным поиском
        {
            if (key < 50)
                return lexemeList[key];
            else
            {
                if (key == 51)
                    return "id";
                if (key == 50)
                    return "con";
                if (key == 99)
                    return "#";
                for (int i = 0; i < rules.Count; i++)
                {
                    if (rules[i].key == key)
                        return rules[i].leftPart;
                }
                return "not found";
            }
        }
    }
}
