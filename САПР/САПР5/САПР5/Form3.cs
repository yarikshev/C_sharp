using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace САПР5
{
    public partial class Form3 : Form
    {
        public Form3(Form2 ff)
        {
            f = ff;
            InitializeComponent();
        }

        Form2 f;
        string[] lex = { "program", "varr", "beginn", "endd.", "long", "float", "weread", "wewrite", "if", "then", "do", "while", "enddo", ":", ",", "+", "-", "*", "/", "(", ")", "[", "]", ">", "<", ">=", "<=", "%", "<>", "=", "?", "OR", "AND", "NOT", ";", "^", "endif"};
        char[] oper = { '+', '-', '*', '/', '[', ']', '>', '<','^','=','%' };

        string PrintValues(IEnumerable List)
        {
            string values = "";
            foreach (string number in List)
            {
                values += number + " ";
            }
            return values;
        }

        string RemoveSpaces(string inputString)
        {
            inputString = inputString.Replace("  ", string.Empty);
            inputString = inputString.Trim().Replace(" ", string.Empty);

            return inputString;
        }

        string findlex(string s)
        {
            string sp=" ";
            for (int po = 0; po < lex.Length; po++)
                if (s == lex[po]) { sp = lex[po]; break; }
            return sp;
        }
        
        string findratio(List<string> myList, string str)
        {
            string s = "";
            if (str == "end")
                s = ">";
            else
                for (int l = 1; l <= f.dataGridView1.Rows.Count; l++)
                    if ((string)f.dataGridView1.Rows[l].Cells[0].Value == myList.Last<string>())
                    {
                        for (int m = 1; m <= f.dataGridView1.ColumnCount; m++)
                            if ((string)f.dataGridView1.Rows[0].Cells[m].Value == str)
                            {
                                s = (string)f.dataGridView1.Rows[l].Cells[m].Value;
                                break;
                            }
                        break;
                    }
            return s;
        }

        string findlexem(int i, string Text)
        {
            string str="";
            if (Text.Length > 1)
                do
                {
                    str += Text[i++];
                    if (str == "\n") {  str = ""; }
                    //else if (str == "-" && Text[i] == '>') { i++; str = "->"; }
                    else if (str == "=" && Text[i] == '=') { i++; str = "=="; }
                    else if (str == ">" && Text[i] == '=') { i++; str = ">="; }
                    else if (str == "<" && Text[i] == '=') { i++; str = "<="; }
                    else if (str == "<" && Text[i] == '>') { i++; str = "<>"; }
                    else if (str == "endd" && Text[i] == '.') { i++; str = "endd."; }
                    else if (str != findlex(str) && !Char.IsLetter(Text[i]) && !Char.IsDigit(Text[i]))
                    {
                        if (Char.IsLetter(str[0])) str = "id";
                        else str = "con";
                        break;
                    }
                }
                while (str != findlex(str));
            else str = "end";
            return str;
        }

        public void Analysis(string Text)
        {
            int i = 0, j = 0, k = 0;
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 4;
            string[] neterm1;
            string[][] neterm2;
            string str = "";
            bool ind = false, ind2 = true;
            List<string> stack = new List<string>();
            dataGridView1.Rows.Add(i,PrintValues(stack),"<",Text);
            do
            {
                k = j = 0;
                str = "";
                while (Text[j] == ' ') j++;
                do
                {
                    str += Text[j++];
                    if (str == "\n") { str = ""; }
                    //else if (str == "-" && Text[j] == '>') { j++; str = "->"; }
                    else if (str == "=" && Text[j] == '=') { j++; str = "=="; }
                    else if (str == ">" && Text[j] == '=') { j++; str = ">="; }
                    else if (str == "<" && Text[j] == '=') { j++; str = "<="; }
                    else if (str == "<" && Text[j] == '>') { j++; str = "<>"; }
                    else if (str == "endd" && Text[j] == '.') { j++; str = "endd."; }
                    else if (str != findlex(str) && !Char.IsLetter(Text[j]) && !Char.IsDigit(Text[j]))
                    {
                        if (Char.IsLetter(str[0])) str = "id";
                        else str = "con";
                        break;
                    }
                }
                while (str != findlex(str));
                Text = Text.Remove(0, j);
                stack.Add(str);
                if (str == "weread" || str == "wewrite") ind2 = true;
                if (ind2 && (str == ")" || str=="beginn")) ind2 = false;
                while (Text.Length > 1 && Text[k] == ' ') k++;
                str = findratio(stack, findlexem(k, Text));
                dataGridView1.Rows.Add(++i, PrintValues(stack), str, Text);
                if (str == ">")
                {
                    while (str == ">" && stack[stack.Count - 1] != "<program>" && !ind)
                    {
                        List<string> stack2 = new List<string>();
                        bool gen = false;
                        for (int jj = 0; jj <= stack.Count; jj++)
                        {
                            stack2.Clear();
                            for (int ll = jj; ll < stack.Count; ll++)
                                stack2.Add(stack[ll]);
                            for (int ii = 0; ii < f.gram.Count; ii++)
                            {
                                if (f.gram[ii] is string[][])
                                {
                                    bool ifgram;
                                    neterm2 = (string[][])f.gram[ii];
                                    for (int mas = 0; mas < neterm2.Length; mas++)
                                    {
                                        ifgram = false;
                                        if (stack2.Count == neterm2[mas].Length)
                                            for (int mas2 = 0; mas2 < neterm2[mas].Length; mas2++)
                                            {
                                                if (stack2[mas2] != neterm2[mas][mas2])
                                                {
                                                    ifgram = false;
                                                    break;
                                                }
                                                else ifgram = true;
                                            }
                                        if (ifgram)
                                        {
                                            int l = 0;
                                            for (int kk = stack.Count - 1; kk >= jj; kk--)
                                                stack.RemoveAt(kk);
                                            while (Text.Length > 1 && Text[l] == ' ') l++;
                                            if (f.masneterm[ii] == "spys_id" && !ind2)
                                                str = "<step>";
                                            else str = "<" + f.masneterm[ii] + ">";
                                            stack.Add(str);
                                            str = findratio(stack, findlexem(l, Text));
                                            dataGridView1.Rows.Add(++i, PrintValues(stack), str, Text);
                                            gen = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    neterm1 = (string[])f.gram[ii];
                                    bool ifgram = false;
                                    if (stack2.Count == neterm1.Length)
                                        for (int mas = 0; mas < neterm1.Count(); mas++)
                                        {
                                            if (stack2[mas] != neterm1[mas])
                                            {
                                                ifgram = false;
                                                break;
                                            }
                                            else ifgram = true;
                                        }
                                    if (ifgram)
                                    {
                                        int l = 0;
                                        str = "<" + f.masneterm[ii] + ">";
                                        for (int kk = stack.Count - 1; kk >= jj; kk--)
                                            stack.RemoveAt(kk);
                                        while (Text.Length > 1 && Text[l] == ' ') l++;
                                        if (f.masneterm[ii] == "spys_id"&& !ind2)
                                            str = "<step>";
                                        else str = "<" + f.masneterm[ii] + ">";
                                        stack.Add(str);
                                        str = findratio(stack, findlexem(l, Text));
                                        dataGridView1.Rows.Add(++i, PrintValues(stack), str, Text);
                                        gen = true;
                                        break;
                                    }
                                }
                                if (gen) break;
                            }
                            if (gen) break;
                        }
                        if (stack2.Count == 0 && str == ">")
                        {
                            ind = true;
                        }
                    }
                }
                else if (str == null) ind = true;
            }
            while ((string)dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value != "<program> " && !ind);
            if (ind) MessageBox.Show("Розбір завершено не успішно!");
            else MessageBox.Show("Розбір завершено успішно!");
        }
    }
}