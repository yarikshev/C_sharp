using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace translator
{
    struct RuleInNum
    {
        public int key;
        public int leftPart;
        public List<int[]> rightPart;

        public RuleInNum(int key, int leftPart, List<int[]> rightPart)
        {
            this.key = key;
            this.leftPart = leftPart;
            this.rightPart = rightPart;
        }
    }

    static class Syntactic_simple_precedence_grammars
    {
        private static List<Rule> rules = RelationTable.Rules;
        private static List<RuleInNum> rulesInNum = new List<RuleInNum>();
        private static int[,] table = RelationTable.GetRelationTable;
        private static List<lexeme> lexemeOutput = Scaner.LexemeOutput;
        private static Dictionary<int, int> ticket = RelationTable.GetTickets;

        private static Stack<int> stack = new Stack<int>();
        private static Stack<Relation> relation = new Stack<Relation>();
        private static List<string>[] stackHistory = new List<string>[3];

        private static bool isOk = true;
        public static bool Success { get { return isOk; } }

        private static List<string> errors = new List<string>();

        public static List<string> Errors { get { return errors; } }
        public static List<string>[] ParsingHistory { get { return stackHistory; } }

        static Syntactic_simple_precedence_grammars() // не созданы объекты листов, без вызова Init() при попытке просмотреть историю разбора выкинет исключение
        {
            stackHistory[0] = new List<string>();
            stackHistory[1] = new List<string>();
            stackHistory[2] = new List<string>();
        }

        public static void Init()
        {
            rulesInNum.Clear();
            errors.Clear();
            stackHistory[0].Clear();
            stackHistory[1].Clear();
            stackHistory[2].Clear();
            stack.Clear();
            relation.Clear();
            TranslateRulesIntoNumbers();            
            lexemeOutput.Add(new lexeme(99, "#", 99, null));
            Begin();
        }

        private static void TranslateRulesIntoNumbers()
        {
            for (int i = 0; i < rules.Count; i++)
            {
                int key = ticket[rules[i].key];
                int leftPart = RelationTable.TicketOf(rules[i].leftPart);
                List<int[]> rightPart = new List<int[]>();

                for (int j = 0; j < rules[i].rightPart.Count;j++ )
                {                    
                    int[] onePart = new int[rules[i].rightPart[j].Length];

                    for (int k = 0; k < rules[i].rightPart[j].Length; k++)
                    {
                        onePart[k] = RelationTable.TicketOf(rules[i].rightPart[j][k]);    
                    }
                    rightPart.Add(onePart);
                }
                rulesInNum.Add(new RuleInNum(key,leftPart,rightPart));
            }
        }

        private static int GetLeftPart(List<int> rightPart)
        {
            bool ok = true;
            for (int i = 0; i < rulesInNum.Count; i++)
            {
                for (int j = 0; j < rulesInNum[i].rightPart.Count; j++)
                {
                    if (rulesInNum[i].rightPart[j].Length == rightPart.Count)
                    {
                        for (int k = 0; k < rightPart.Count; k++)
                        {
                            if (rulesInNum[i].rightPart[j][k] != rightPart[k])
                            {
                                ok = false;
                                break;
                            }
                            else
                                ok = true;
                        }
                        if(ok)
                        return rulesInNum[i].leftPart;
                    }
                }
            }
            return 0;
        }

        private static void PickOutBasis(int currLexeme)
        {
            List<int> rightPart = new List<int>();
            do
            {
                rightPart.Add(stack.Pop());
                
            }
            while (relation.Pop() != Relation.less);

            rightPart.Reverse();
            int basis = GetLeftPart(rightPart);
            relation.Push((Relation)table[stack.First(), basis]);
            stack.Push(basis);
        }

        private static void Begin()
        {
            stack.Push(RelationTable.Rules.Count + Scaner.LexemeList.Count + 3); // code of symbol "#"
            relation.Push(Relation.less);
            stack.Push(ticket[lexemeOutput[0].key]); // push first lexeme(relation less with previous #)

            for (int i = 1; i < lexemeOutput.Count; i++)
            {                
                Relation currRelation;                
                currRelation = (Relation)table[stack.First(), ticket[lexemeOutput[i].key]];
                ShowBasis(stack, relation, currRelation, i);

                if (stack.Peek() == Scaner.LexemeList.Count + 3)
                    break;

                switch (currRelation)
                {
                    case Relation.equal :
                        stack.Push(ticket[lexemeOutput[i].key]);
                        relation.Push(currRelation);
                        break;

                    case Relation.less :
                        stack.Push(ticket[lexemeOutput[i].key]);
                        relation.Push(currRelation);
                        break;

                    case Relation.more :
                        //сначала выделить основу, а затем занести в стек новое отношение между получившейся основой и символов в цепочке.
                        PickOutBasis(i);
                        --i;
                        break;

                    default: 
                             //Console.WriteLine("Error after " + lexemeOutput[i-1].value + " on line " + lexemeOutput[i-1].line);
                             errors.Add("Error after " + lexemeOutput[i - 1].value + " on line " + lexemeOutput[i - 1].line);
                             i = lexemeOutput.Count - 1;
                             isOk = false;
                        break;
                }                
            }
        }

        private static void ShowBasis(Stack<int> stack, Stack<Relation> relation, Relation currRelation, int currLexeme)
        {
           // int Number = 4; // limit for output
           // int NumberStack = 3; // -//-
            int[] Stack = stack.ToArray();
            Relation[] Relation = relation.ToArray();
            Stack.Reverse();
            Relation.Reverse();
            string col0 = null, col1 = null, col2 = null; // columns of the table

            int n = Relation.Length;// < NumberStack ? Relation.Length : NumberStack;
            int k = lexemeOutput.Count -currLexeme - 1;// > Number ? Number : lexemeOutput.Count - currLexeme - 1;            
            //stack
            for( int i =  n - 1; i >= 0; i--)
            {
                col0 += (String.Format("{1} {0} ",GetRelationSymbol(Relation[i]),RelationTable.GetSymbol(table[Stack[i + 1],0])));
            }
            col0 += String.Format("{0} ", RelationTable.GetSymbol(table[Stack[0], 0]));
            //current relation
            col1 += currRelation.ToString();
            //lexemes
            for (int i = currLexeme; i < currLexeme + k; i++)
            {
                col2 += lexemeOutput[i].value + " ";
            }
            stackHistory[0].Add(col0);
            stackHistory[1].Add(col1);
            stackHistory[2].Add(col2);
        }

        private static string GetRelationSymbol(Relation relation)
        {
            switch (relation)
            {
                case Relation.equal:
                    return "=";
                case Relation.less :
                    return "<";
                case Relation.more :
                    return ">";
                default: return null;
            }
        }

    }
}
