using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mini_excel_lab2
{
    public partial class FormLoadFile : Form
    {

        private void fillListBox()
        {
            string path = @TBpapka.Text;
            List<string> files = (from a in Directory.GetFiles(path) select Path.GetFileName(a)).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                listBoxFiles.Items.Add(files[i]);
            }
        }

        public FormLoadFile()
        {
            InitializeComponent();
            fillListBox();        
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /* Структура
         * кількість ствбчиків
         * кількість рядочків
         * iм'я клітинки1
         * значення1
         * вираз1
         * i т.д.
         */

        private void btnOK_Click(object sender, EventArgs e)
        {
            int indexFile = listBoxFiles.SelectedIndex;
            if(indexFile == -1)
            {
                MessageBox.Show("Оберіть файл");
                return;
            }
            else
            {
                
                string nameFile = listBoxFiles.Items[indexFile].ToString();
                Program.mainFormMiniExcel.label1.Text = nameFile;
                StreamReader sr = new StreamReader(TBpapka.Text + @"\" + nameFile);

                while (Program.mainFormMiniExcel.dgv.ColumnCount!=1)
                {
                    Program.mainFormMiniExcel.currCol = Program.mainFormMiniExcel.dgv.ColumnCount - 1;
                    Program.mainFormMiniExcel.DelCol();
                }


                while (Program.mainFormMiniExcel.dgv.RowCount != 1)
                {
                    Program.mainFormMiniExcel.currRow = Program.mainFormMiniExcel.dgv.RowCount - 1;
                    Program.mainFormMiniExcel.DelRow();
                }

                int newColumnCount;
                int newRowCount;
                try
                {
                    newColumnCount = Int32.Parse(sr.ReadLine());
                    newRowCount = Int32.Parse(sr.ReadLine());
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }


                for(int i=0; i<newRowCount; i++)
                {
                    Program.mainFormMiniExcel.AddRow();
                }
                for(int i=0; i<newColumnCount; i++)
                {
                    Program.mainFormMiniExcel.AddColumn();
                }
                string name;
                while ((name = sr.ReadLine()) != null)
                {
                    string value = sr.ReadLine();
                    string exp = sr.ReadLine();
                    Cell c = new Cell();
                    c.Name = name;
                    c.Value = value;
                    c.Exp = exp;
                    try
                    {
                        Program.mainFormMiniExcel.dictionary.Add(name, c);
                    }
                    catch (System.ArgumentException)
                    {
                        Program.mainFormMiniExcel.dictionary.Remove(name);
                        Program.mainFormMiniExcel.dictionary.Add(name, c);
                    }                  
                }
                Program.mainFormMiniExcel.CalcExp();
                this.Close();
            }
        }

        private void TBpapka_TextChanged(object sender, EventArgs e)
        {
            try
            {
                fillListBox();
            }
            catch { }   
        }
    }
}
