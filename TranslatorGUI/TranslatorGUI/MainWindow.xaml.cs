using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using translator;

namespace TranslatorGUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {     

        public MainWindow()
        {
            InitializeComponent();          
        }

        // вспомогательные методы
        //открыть файл через OpenFileDialog
        private void OpenFile(RichTextBox recipient)
        {
            //Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.ShowDialog();
            try
            {
                Controller.SourseCodeFilePath = openFileDialog.FileName;
                SetText(recipient, System.IO.File.ReadAllText(openFileDialog.FileName));
            }
            catch
            {
                MessageBox.Show("Cant open file");
            }
        }

        //установить текст объекту RichTextBox
        private void SetText(RichTextBox recipient, string text)
        {
            if (recipient != null)
            {
                recipient.SelectAll();
                recipient.Selection.Text = text;
            }
        }

        // добавить данные в таблицу
        private void SetDataToTable(DataGrid table, params List<string>[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {                
                table.Columns.Add(new DataGridTextColumn());
            }
            
        }

        // добавить лексмемы в таблицу
        private void SetLexemesToTable(DataGrid table, List<lexeme> list, params string[] headers)
        {
            System.Data.DataTable view = new System.Data.DataTable();
            for(int i = 0;i<headers.Length;i++)
            {
                view.Columns.Add(new System.Data.DataColumn());
                view.Columns[i].ColumnName = headers[i];                                
            }
            for(int i=0;i<list.Count;i++)
            {
                view.Rows.Add(list[i].line.ToString(), list[i].value, list[i].key.ToString(), list[i].index);                                
            }
            table.ItemsSource = view.DefaultView;
            table.ColumnWidth = this.Width / headers.Length;
            table.RowHeight = 25;
                        
        }

        //добавить историю разбора синтаксического анализатора и генератора полиза
        private void SetStackHistoryToTable(DataGrid table, List<string>[] list, params string[] headers)
        {
            System.Data.DataTable view = new System.Data.DataTable();
            for (int i = 0; i < headers.Length; i++)
            {
                view.Columns.Add(new System.Data.DataColumn());
                view.Columns[i].ColumnName = headers[i];
            }
            for (int i = 0; i < list[0].Count; i++) // length of Lists in array mabe is equal
            {
                view.Rows.Add(list[0][i], list[1][i], list[2][i]);
            }
            table.ItemsSource = view.DefaultView;
            table.ColumnWidth = this.Width / headers.Length;
            table.RowHeight = 25;
        }

        //добавить строку в нижнюю панель
        public  void WriteLine(string text)
        {
            messagePanel.AppendText(text + "\n");
        }
        public void Write(string text)
        {
            messagePanel.AppendText(text);
        }

        // display errors 
        private void DisplayList(List<string> list)
        {
            if ( list.Count != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    WriteLine(list[i]);
                }
            }
            else
                WriteLine("Success.");
        }
        
        //сохранить файл исходного кода
        private void SaveSourseCode()
        {
            try
            {
                codePanel.SelectAll();
                if(codePanel.Selection.Text.Length > 7)
                    System.IO.File.WriteAllText(Controller.SourseCodeFilePath, codePanel.Selection.Text);

            }
            catch
            {
                MessageBox.Show("Can't save sourcecode");
                MessageBox.Show(">" + codePanel.Selection.Text + "<");
            }
        }
        // вспомогательные методы end

        private void Compile_Click(object sender, RoutedEventArgs e)
        {
            SaveSourseCode();
            SetText(messagePanel, "\n");
            if(Controller.ReadSourceCode())
            {                
                DisplayList(Controller.Compile(this));
                WriteLine("Compiled.");
            }
            else
            {
                WriteLine("Can't compile.");
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(codePanel);            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SaveSourseCode();
            }
            else
            {
                switch (e.Key)
                {
                    case Key.F5:
                        Compile_Click(sender, e); break;
                    case Key.Escape:
                        {
                            SaveSourseCode();
                            this.Close();
                        } break;
                    default: break;
                }
            }

        }

        private void Lexemes_Click(object sender, RoutedEventArgs e)
        {
            Window lexemeTable = new Window();
            DataGrid table = new DataGrid();
            SetLexemesToTable(table, Scaner.LexemeOutput, "Line", "Value", "Key", "Index");
            lexemeTable.Content = table;
            lexemeTable.Show();
        }

        private void codePanel_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetText(messagePanel, "\n"); //TODO проверить на быстродействие
        }

        private void StackHistory_Click(object sender, RoutedEventArgs e)
        {
            Window lexemeTable = new Window();
            DataGrid table = new DataGrid();
            SetStackHistoryToTable(table, Syntactic_simple_precedence_grammars.ParsingHistory, "Stack", "Retation", "Lexemes");            
            lexemeTable.Content = table;
            lexemeTable.Show();
        }

        private void PolizHistory_Click(object sender, RoutedEventArgs e)
        {
            Window lexemeTable = new Window();
            DataGrid table = new DataGrid();
            SetStackHistoryToTable(table, Poliz2.History, "Poliz", "Stack", "Lexemes");            
            lexemeTable.Content = table;
            lexemeTable.Show();
        }
    }
}
