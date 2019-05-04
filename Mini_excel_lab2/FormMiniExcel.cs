using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Mini_excel_lab2
{
    public  partial class FormMiniExcel : Form
    {
        Indexator indexator = new Indexator();
        Parser parser = new Parser();
        Namer handler = new Namer();
        public  int currRow, currCol;
        public  Dictionary<string, Cell> dictionary = new Dictionary<string, Cell>();
        public  Dictionary<string, int> entrance = new Dictionary<string, int>();

        public  DataGridView mainTable()
        {
            return this.dgv;
        }

        public Dictionary<string, Cell> getDict()
        {
            return this.dictionary;
        }

        public FormMiniExcel()
        {
            InitializeComponent();
            DataGridViewColumn A = new DataGridViewColumn();
            DataGridViewColumn B = new DataGridViewColumn();
            DataGridViewColumn C = new DataGridViewColumn();
            DataGridViewColumn D = new DataGridViewColumn();
            DataGridViewColumn E = new DataGridViewColumn();
            DataGridViewColumn F = new DataGridViewColumn();
            DataGridViewColumn G = new DataGridViewColumn();
            DataGridViewColumn H = new DataGridViewColumn();

            Cell cellA = new Cell(); A.CellTemplate = cellA;
            Cell cellB = new Cell(); B.CellTemplate = cellB;
            Cell cellC = new Cell(); C.CellTemplate = cellC;
            Cell cellD = new Cell(); D.CellTemplate = cellD;
            Cell cellE = new Cell(); E.CellTemplate = cellE;
            Cell cellF = new Cell(); F.CellTemplate = cellF;
            Cell cellG = new Cell(); G.CellTemplate = cellG;
            Cell cellH = new Cell(); H.CellTemplate = cellH;


            A.HeaderText = "A"; A.Name = "A";
            B.HeaderText = "B"; B.Name = "B";
            C.HeaderText = "C"; C.Name = "C";
            D.HeaderText = "D"; D.Name = "D";
            E.HeaderText = "E"; E.Name = "E";
            F.HeaderText = "F"; F.Name = "F";
            G.HeaderText = "G"; G.Name = "G";
            H.HeaderText = "H"; H.Name = "H";

            dgv.Columns.Add(A);
            dgv.Columns.Add(B);
            dgv.Columns.Add(C);
            dgv.Columns.Add(D);
            dgv.Columns.Add(E);
            dgv.Columns.Add(F);
            dgv.Columns.Add(G);
            dgv.Columns.Add(H);

            DataGridViewRow row = new DataGridViewRow();
            dgv.Rows.Add(row);
            dgv.Rows.Add();
            dgv.Rows.Add();
            dgv.Rows.Add();
            dgv.Rows.Add();
            dgv.Rows.Add();
            dgv.Rows.Add();
            dgv.Rows.Add();
            dgv.Rows.Add();
            dgv.Rows.Add();
            dgv.Rows.Add();
            dgv.Rows.Add();
            AddRow();
        }

        //---------------------------------------------------------------------------------------//

        private void dgv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            currRow = dgv.CurrentCell.RowIndex;
            currCol = dgv.CurrentCell.ColumnIndex;
            string currCellName = indexator.FromNumberToWord(currCol + 1) + (currRow + 1).ToString();
            if (!dictionary.ContainsKey(currCellName))
            {
                Cell cell = new Cell();
                cell.Value = "0";
                cell.Exp = "0";
                dictionary.Add(currCellName, cell);
            }
            label3.Text = currCellName;
            textBoxFormula.Text = dictionary[currCellName].Exp;    
        }

        //---------------------------------------------------------------------------------------//

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string currCellName = indexator.FromNumberToWord(currCol + 1) + (currRow + 1).ToString();

                if (dgv.Rows[currRow].Cells[currCol].Value != null)
                {
                    string currCellValue = dgv.Rows[currRow].Cells[currCol].Value.ToString();
                    dictionary[currCellName].Value = parser.ExpressionStart(currCellValue).ToString();
                }
                else
                {
                    dictionary[currCellName].Value = 0.ToString();
                }
                dgv.Rows[currRow].Cells[currCol].Value = dictionary[currCellName].Value;
                calculateExp_Click(sender, e);
            }
            catch { };
            
        }

        //------------------------------------------------------------------------------------//

        public void AddColumn()
        {
            int n = ++dgv.ColumnCount;
            dgv.Columns[n - 1].HeaderText = indexator.NextWord(dgv.Columns[n - 2].HeaderText);
        }

        //-----------------------------------------------------------------------------//
        private void buttonAddColumn_Click(object sender, EventArgs e)//додати колонку
        {
            AddColumn();
        }

        //---------------------------------------------------------------------------------//

        public void AddRow()
        {
            int n = ++dgv.RowCount;
            for (int i = 0; i < n; i++)
            {
                dgv.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }

        //----------------------------------------------------------------------------------//

        private void buttonAddRow_Click(object sender, EventArgs e)//додати рядочок
        {
            AddRow();          
        }

        //----------------------------------------------------------------------------------//

        private void textBoxFormula_TextChanged(object sender, EventArgs e) // записать формулу
        {
            try
            {
                string currCellName = indexator.FromNumberToWord(currCol + 1) + (currRow + 1).ToString();
                dictionary[currCellName].Exp = textBoxFormula.Text;
            }
            catch{ }            
        }
        
        //---------------------------------------------------------------------------------------//

        private void buildEntranceDictionary()
        {
            entrance = new Dictionary<string, int>();
            for (int j = 0; j < dgv.ColumnCount; j++)
            {
                for (int i = 0; i < dgv.RowCount; i++)
                {
                    string currCellName = indexator.FromNumberToWord(j + 1) + (i + 1).ToString();
                    entrance.Add(currCellName, 100);
                }
            }
        }

        //------------------------------------------------------------------------------------//

        public void CalcExp()
        {
            entrance = new Dictionary<string, int>();
            buildEntranceDictionary();

            for (int j = 0; j < dgv.ColumnCount; j++)
            {
                for (int i = 0; i < dgv.RowCount; i++)
                {
                    string currCellName = indexator.FromNumberToWord(j + 1) + (i + 1).ToString();
                    if (dictionary.ContainsKey(currCellName) && dictionary[currCellName].Value != null)
                    {
                        try
                        {
                            dgv.Rows[i].Cells[j].Value = CalcCell(dictionary[currCellName], dgv).ToString();
                            dictionary[currCellName].Value = dgv.Rows[i].Cells[j].Value.ToString();
                        }
                        catch (ClassOfExceptions.ExceptionRecursion)
                        {
                            MessageBox.Show(ClassOfExceptions.ExceptionRecursion.what);
                            return;
                        }

                    }
                }
            }

            entrance = new Dictionary<string, int>();

        }

        //-----------------------------------------------------------------------------//

        private void calculateExp_Click(object sender, EventArgs e)
        {
            CalcExp();
        }

        //-----------------------------------------------------------------------------//

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSaveFile formSaveFile = new FormSaveFile();
            formSaveFile.Show();
        }

        //-------------------------------------------------------------------------//

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormLoadFile formLoadFile = new FormLoadFile();
            formLoadFile.Show();
        }

        //----------------------------------------------------------------------------//

        public void DelCol()
        {
            if (currCol == dgv.ColumnCount - 1 && currCol != 0)
            {
                for (int i = 0; i < dgv.RowCount - 1; i++)
                {
                    string currCellName = indexator.FromNumberToWord(currCol + 1) + (i + 1).ToString();
                    if (dictionary.ContainsKey(currCellName))
                    {
                        dictionary.Remove(currCellName);
                    }
                }

                dgv.ColumnCount--;

                currCol -= 1;

                label3.Text = "NULL";
                CalcExp();
                return;
            }
            else
            {
                for (int i = 0; i < dgv.RowCount; i++)
                {
                    string currCellName = indexator.FromNumberToWord(currCol + 1) + (i + 1).ToString();
                    if (dictionary.ContainsKey(currCellName))
                    {
                        dictionary.Remove(currCellName);
                        dgv.Rows[i].Cells[currCol].Value = null;
                    }
                }
                label3.Text = "NULL";
                CalcExp();
            }
        }


        //------------------------------------------------------------------------------//


        public void DelRow()
        {
            if (currRow == dgv.RowCount - 1 && currRow != 0)
            {
                for (int i = 0; i < dgv.ColumnCount - 1; i++)
                {
                    string currCellName = indexator.FromNumberToWord(i + 1) + (currRow + 1).ToString();
                    if (dictionary.ContainsKey(currCellName))
                    {
                        dictionary.Remove(currCellName);
                    }
                }

                dgv.RowCount--;
                currRow -= 1;
                label3.Text = "NULL";
                CalcExp();
                return;
            }
            else
            {
                for (int i = 0; i < dgv.ColumnCount; i++)
                {
                    string currCellName = indexator.FromNumberToWord(i + 1) + (currRow + 1).ToString();
                    if (dictionary.ContainsKey(currCellName))
                    {
                        dictionary.Remove(currCellName);
                        dgv.Rows[currRow].Cells[i].Value = null;
                    }
                }
                label3.Text = "NULL";
                CalcExp();
            }
        }

        //----------------------------------------------------------------------------//
        private void btnDelCol_Click(object sender, EventArgs e)
        {
            DelCol();
        }

        //----------------------------------------------------------------------------//

        private void btnDelRow_Click(object sender, EventArgs e)
        {
            DelRow();
        }

        //---------------------------------------------------------------------------//

        private void FormMiniExcel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Закрити?", "Message", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        //-------------------------------------------------------------------------------//


        public double CalcCell(Cell c, DataGridView dgv)
        {
            try
            {
                entrance[c.Name]--;
                if (entrance[c.Name] < 0)
                {
                    c.Value = "0";
                    throw new ClassOfExceptions.ExceptionRecursion();
                }
            }
            catch (KeyNotFoundException) { }
            
            string saveFormula = c.Exp;
            List<string> anotherCells = handler.ListNameCells(c);
            if (anotherCells.Count == 0)
            {
                return parser.ExpressionStart(c.Value);
            }
            else
            {
                for(int i=0; i< anotherCells.Count; i++)
                {

                    if (dictionary.ContainsKey(anotherCells[i]))
                    {
                         c.Exp = c.Exp.Replace(anotherCells[i], " " + CalcCell(dictionary[anotherCells[i]], dgv).ToString() + " ");
                    }
                    else
                    {
                         c.Exp = c.Exp.Replace(anotherCells[i], " 0 ");
                    }                           
                }
                string finishFormula = c.Exp;
                c.Exp = saveFormula;
             // MessageBox.Show(finishFormula);
                return parser.ExpressionStart(finishFormula);
            }
        }

    } 

}
