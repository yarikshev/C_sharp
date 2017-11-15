using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace translator
{


    class Executer
    {
        static private Dictionary<string, int> labelsTable =  new Dictionary<string,int>();
        static private List<string> Poliz { get; set; }
        static private Dictionary<string, double> identifiers;                

        static private string[] binaryOperations = {"+","-","*","/","^"};
        static private string[] logicalOperations = { ">","<",">=","<=","==","!=" };
        static private string[] boolOperations = { "or", "and" };
        static private string[] unaryOperations = {"BP", "read", "write" };
        static private string upl = "UPL";
        static private string assignment = "=";
        

        
        static Executer()
        {
            identifiers = new Dictionary<string, double>();
        }

        public static void Init(string[] poliz)
        {
            Poliz = new List<string>(poliz);  
            MakeLabelsTable();
            Execute();           
        }

        static private void MakeLabelsTable()
        {
            for (int i = 0; i < Poliz.Count; i++)
            {
                if (Poliz[i] == ":")
                {
                    labelsTable.Add(Poliz[i - 1], i - 1);
                }
            }
        }

        private static void Execute()
        {
            Stack<string> stack = new Stack<string>();
            int i = 0;
            System.Diagnostics.Stopwatch a = new System.Diagnostics.Stopwatch();            
            while (i < Poliz.Count)
            {                
                #region Not HardCode (but + 1 switch)                
                 if (binaryOperations.Contains(Poliz[i]))
                 {
                     stack.Push(CalculateBinary(Operand(stack.Pop()), Operand(stack.Pop()), Poliz[i]).ToString());
                 }
                 else if (unaryOperations.Contains(Poliz[i]))
                 {
                     UnaryOperation(stack.Pop(), Poliz[i], ref i);
                 }
                 else if (logicalOperations.Contains(Poliz[i]))
                 {
                     stack.Push(CalculateLogicBinary(Operand(stack.Pop()), Operand(stack.Pop()), Poliz[i]).ToString());
                 }
                 else if (boolOperations.Contains(Poliz[i]))
                 {
                     stack.Push(CalculateBoolBinary(bool.Parse(stack.Pop()), bool.Parse(stack.Pop()), Poliz[i]).ToString());
                 }
                 else if (assignment == Poliz[i])
                 {
                     double b = Operand(stack.Pop());
                     identifiers[stack.Pop()] = b;
                 }
                 else if (upl == Poliz[i])
                 {
                     UPL(stack.Pop(), bool.Parse(stack.Pop()), ref i);
                 }
                 else  //can be id or number or _m or _r
                     if (Poliz[i] != ":")
                     {
                         stack.Push(Poliz[i]);
                         Operand(Poliz[i]); // added to table of identifiers
                     }
                 #endregion

                #region HardCode
                /* switch (poliz[i])
                {
                    case "+":
                        {
                            stack.Push( (Operand(stack.Pop()) + Operand(stack.Pop())).ToString() );
                        }break;
                    case "-":
                        {
                            double op2 = Operand(stack.Pop());
                            double op1 = Operand(stack.Pop());
                            stack.Push( (op1 - op2).ToString() );
                        }break;
                    case "*":
                        {
                            stack.Push((Operand(stack.Pop()) * Operand(stack.Pop())).ToString());
                        }break;
                    case "/":
                        {
                            double op2 = Operand(stack.Pop());
                            double op1 = Operand(stack.Pop());
                            stack.Push((op1 / op2).ToString());
                        }break;
                    case "^":
                        {
                            double op2 = Operand(stack.Pop());
                            double op1 = Operand(stack.Pop());
                            stack.Push(Math.Pow(op1,op2).ToString());
                        }break;
                    case "=":
                        {
                            double b = Operand(stack.Pop());
                            identifiers[stack.Pop()] = b;
                        }break;
                    case ">":                        
                        stack.Push( (Operand(stack.Pop()) < Operand(stack.Pop())).ToString() );break;
                    case "<":                        
                        stack.Push( (Operand(stack.Pop()) > Operand(stack.Pop())).ToString() );break;
                    case ">=":
                        stack.Push( (Operand(stack.Pop()) <= Operand(stack.Pop())).ToString() );break;
                    case "<=":
                        stack.Push( (Operand(stack.Pop()) >= Operand(stack.Pop())).ToString() );break;
                    case "!=":
                        stack.Push( (Operand(stack.Pop()) != Operand(stack.Pop())).ToString() );break;
                    case "==":
                        stack.Push( (Operand(stack.Pop()) == Operand(stack.Pop())).ToString() );break;
                    case "and":
                        stack.Push( (bool.Parse(stack.Pop()) && bool.Parse(stack.Pop())).ToString() );break;
                    case "or":
                        stack.Push((bool.Parse(stack.Pop()) || bool.Parse(stack.Pop())).ToString()); break;
                    case "UPL":
                            UPL(stack.Pop(), bool.Parse(stack.Pop()), ref i);break;
                    case "BP":
                        i = labelsTable[stack.Pop()];break;
                    case "read":
                        Read(stack.Pop());break;
                    case "write":
                        Write(stack.Pop());break;
                    default:
                        {
                            if (poliz[i] != ":")
                            {
                                stack.Push(poliz[i]);
                                Operand(poliz[i]); // added to table of identifiers
                            }
                        }break;   
                }*/
                #endregion
                i++;
            }
        }

        private static double Operand(string element)
        {
            double value = 0.0;
            if (double.TryParse(element, out value))
                return value;
            else
            {
                if (identifiers.TryGetValue(element, out value))
                    return value;
                identifiers.Add(element, 0);
                return 3.1419265359;  
            }            
        }

        #region Operations
        private static double CalculateBinary(double op2, double op1, string operation)
        {
            switch (operation)
            {
                case "+":                    
                        return op1 + op2;                    
                case "-":
                        return op1 - op2;
                case "*":
                        return op1 * op2;
                case "/":
                        return op1 / op2;
                case "^":
                        return Math.Pow(op1, op2);
                default:
                        {
                            Console.WriteLine("No binary operation.");
                            return 0.0000000000000000000001;
                        }
            }
        }

        private static bool CalculateLogicBinary(double op2, double op1, string operation)
        {
            switch (operation)
            {
                case ">":
                    return op1 > op2 ? true : false;
                case "<":
                    return op1 < op2 ? true : false;
                case ">=":
                    return op1 >= op2 ? true : false;
                case "<=":
                    return op1 <= op2 ? true : false;
                case "!=":
                    return op1 != op2 ? true : false;
                case "==":
                    return op1 == op2 ? true : false;
                default:
                    {
                        Console.WriteLine("No logical operation " + operation);
                        return false;
                    }
            }
        }

        private static bool CalculateBoolBinary(bool op2, bool op1, string operation)
        {
            switch (operation)
            {
                case "and":
                    return op1 && op2;
                case "or":
                    return op1 || op2;
                default:
                    {
                        Console.WriteLine("No bool operation.");
                        return false;
                    }
            }
        }

        private static void UPL(string label, bool op, ref int pointer)
        {
            if (!op)
                pointer = labelsTable[label];
        }

        private static void UnaryOperation(string op, string operation, ref int pointer)
        {
            switch (operation)
            {
                case "write":
                    {
                        Write(op);
                    }break;
                case "read":
                    {
                        Read(op);
                    }break;
                case "BP":
                    {
                        pointer = labelsTable[op];
                    }break;
                default:
                    Console.WriteLine("No unary operation.");
                    break;
            }
        }

        private static void Write(string op)
        {
            Console.WriteLine(op + " = " + identifiers[op]);
        }

        private static void Read(string op)
        {
            Console.Write("reading " + op + " : ");
            identifiers[op] = double.Parse(Console.ReadLine());
        }

        #endregion
    }
}