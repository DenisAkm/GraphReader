﻿using GraphReader.Classes;
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

namespace GraphReader
{
    public partial class NumbPad : Form
    {
        int SelectedColumn;
        MainForm mainForm;        
        string[] ColNames;
        List<DataElement2> DataStore = new List<DataElement2>();

        public NumbPad(MainForm mf)
        {
            mainForm = mf;
            InitializeComponent();            
            InitializeCheckComboBox();
            InitializeDataStore(mainForm.LoadedData);
            InitializeColumnsList();
            InitializeComboBox();
        }

        #region Initialization
        
        private void InitializeComboBox()
        {
            int max = 0;
            for (int i = 0; i < DataStore.Count; i++)
            {
                if (DataStore[i].ColumnsCount > max)
                {
                    max = DataStore[i].ColumnsCount;
                }
            }

            comboBox1.Items.Clear();
            for (int i = 0; i < max; i++)
            {
                comboBox1.Items.Add("Колонка " + ColNames[i]);
            }
            comboBox1.SelectedIndex = 0;
            SelectedColumn = comboBox1.SelectedIndex;
        }
        
        private void InitializeCheckComboBox()
        {
            //Загрузка списка файлов по адресам
            for (int i = 0; i < mainForm.treeViewFilesBrowser.Nodes.Count; i++)
            {
                string name = mainForm.treeViewFilesBrowser.Nodes[i].Tag.ToString();
                checkBoxComboBox1.Items.Add(name.Remove(name.LastIndexOf(".")).Substring(name.LastIndexOf("\\") + 1));
            }

            //Отметить выделенный файл
            //if (mainForm.treeViewFilesBrowser.SelectedNode != null)//&& mainForm.listViewGraphBrowser.SelectedItems[0].Text != "(Выделить все)"
            //{
            //    for (int i = 0; i < mainForm.treeViewFilesBrowser.SelectedItems.Count; i++)
            //    {
            //        if (mainForm.listViewGraphBrowser.SelectedItems[i].Checked)
            //        {
            //            checkBoxComboBox1.CheckBoxItems[i].Checked = true;
            //        }                    
            //    }                
            //}
        }

        private void InitializeDataStore(List<DataElement2> data)//
        {
            for (int k = 0; k < mainForm.LoadedData.Count; k++)
            {
                DataStore.Add(mainForm.LoadedData[k]);
            }
        }

        private void InitializeColumnsList()
        {
            ColNames = new string[mainForm.dataGridView1.Columns.Count];
            for (int i = 0; i < mainForm.dataGridView1.Columns.Count; i++)
            {
                ColNames[i] = mainForm.dataGridView1.Columns[i].Name;
            }
        }   
        
         
        #endregion        

        #region Events
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if (CheckColumns())
            {
                LoadTextBoxInfo();
                SelectedColumn = comboBox1.SelectedIndex;
            }
            else
            {
                MessageBox.Show("Не во всех файлах есть выбранная колонка", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                comboBox1.SelectedIndex = SelectedColumn;
            }
        }
        private void RadioCheckedChanged(object sender, EventArgs e)
        {
            LoadTextBoxInfo();
        }
        private void OnlyNumbers(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 44 && number != 45 && number != 8) // цифры, клавиша BackSpace, минус('45')
            {
                e.Handled = true;                
            }
        }
        private void checkBoxComboBox1_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            LoadTextBoxInfo();
        }
        private void NumbPad_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int k = 0; k < DataStore.Count; k++)
            {
                if (DataStore[k].Changed)
                {
                    if (MessageBox.Show("Вы уверены, что хотите закрыть без сохраниения?", "Подтверждение", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        e.Cancel = false;
                        break;
                    }
                    else
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }

        }
        #endregion 

        #region NumbOperations
        private void buttonShift_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(CheckEnteredValue()))
                {                    
                    for (int k = 0; k < checkBoxComboBox1.CheckBoxItems.Count; k++)
                    {
                        if (checkBoxComboBox1.CheckBoxItems[k].Checked)
                        {
                            float changer = Convert.ToSingle(textBoxNumb.Text);
                            float newValue;
                            for (int i = 0; i < DataStore[k].RowsCount; i++)
                            {
                                if (GateCondition(DataStore[k], i))
                                {
                                    newValue = Convert.ToSingle(DataStore[k][comboBox1.SelectedIndex, i].Replace(".", ",")) + changer;
                                    DataStore[k][comboBox1.SelectedIndex, i] = Convert.ToString(newValue);
                                }
                            }
                            //mainForm.StringTable = DataStore[k];
                            //mainForm.FirstLoadRows();                            
                            DataStore[k].Changed = true;                         
                        }                        
                    }
                    LoadTextBoxInfo();
                }
            }
            catch (Exception h)
            {
                MessageBox.Show(h.ToString(), "Что-то пошло не так", MessageBoxButtons.OK);
            }
        }
        private void buttonMultiply_Click(object sender, EventArgs e)
        {
            try
            {                
                if (!(CheckEnteredValue()))
                {
                    for (int k = 0; k < checkBoxComboBox1.CheckBoxItems.Count; k++)
                    {
                        if (checkBoxComboBox1.CheckBoxItems[k].Checked)
                        {
                            float changer = Convert.ToSingle(textBoxNumb.Text);
                            float newValue;
                            for (int i = 0; i < DataStore[k].RowsCount; i++)
                            {
                                if (GateCondition(DataStore[k], i))
                                {
                                    newValue = Convert.ToSingle(DataStore[k][comboBox1.SelectedIndex, i].Replace(".", ",")) * changer;
                                    DataStore[k][comboBox1.SelectedIndex, i] = Convert.ToString(newValue);
                                }
                            }
                            //mainForm.StringTable = DataStore[k];
                            //mainForm.FirstLoadRows();
                            
                            DataStore[k].Changed = true;
                        }
                    }
                    LoadTextBoxInfo();
                }
            }
            catch (Exception h)
            {
                MessageBox.Show(h.ToString(), "Что-то пошло не так", MessageBoxButtons.OK);
            }
        }
        private void buttonTrim_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxNumb.Text = "0";
                if (!(CheckEnteredValue()))
                {
                    for (int k = 0; k < checkBoxComboBox1.CheckBoxItems.Count; k++)
                    {
                        if (checkBoxComboBox1.CheckBoxItems[k].Checked)
                        {
                            int counter = 0;
                            for (int i = 0; i < DataStore[k].RowsCount; i++)
                            {
                                if (GateCondition(DataStore[k], i))
                                {
                                    counter++;
                                }
                            }
                            StringTable st = new StringTable(DataStore[k].Name, DataStore[k].ColumnsCount, counter);
                            counter = 0; 
                            for (int i = 0; i < DataStore[k].RowsCount; i++)
                            {
                                if (GateCondition(DataStore[k], i))
                                {
                                    for (int j = 0; j < DataStore[k].ColumnsCount; j++)
                                    {
                                        st.Cell[j, counter] = DataStore[k][j, i];                                        
                                    }
                                    counter++;
                                }
                            }


                            DataStore[k].Table = new string[st.ColomnsCount, st.RowsCount];
                            DataStore[k].Name = st.Name;
                            
                            for (int i = 0; i < st.RowsCount; i++)
                            {
                                for (int j = 0; j < st.ColomnsCount; j++)
                                {
                                    DataStore[k][j, i] = st.Cell[j, i];
                                }                                
                            }
                            
                            
                            DataStore[k].Changed = true;
                        }
                        //mainForm.StringTable = DataStore[k];
                        //mainForm.FirstLoadRows();                        
                    }
                    LoadTextBoxInfo();
                }
                textBoxNumb.Text = "";
            }
            catch (Exception h)
            {
                MessageBox.Show(h.ToString(), "Что-то пошло не так", MessageBoxButtons.OK);
            }
        }
        private void buttonTodB_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxNumb.Text = "0";
                if (!(CheckEnteredValue()))
                {
                    for (int k = 0; k < checkBoxComboBox1.CheckBoxItems.Count; k++)
                    {
                        if (checkBoxComboBox1.CheckBoxItems[k].Checked)
                        {
                            if (OnlyPositive(DataStore[k], comboBox1.SelectedIndex))
                            {        
                                for (int i = 0; i < DataStore[k].RowsCount; i++)
                                {
                                    if (GateCondition(DataStore[k], i))
                                    {
                                        Double value = Convert.ToDouble(DataStore[k][comboBox1.SelectedIndex, i].Replace(".", ","));
                                        Double newVal = 20 * Math.Log10(value);
                                        DataStore[k][comboBox1.SelectedIndex, i] = Convert.ToString(newVal);
                                    }
                                }
                                //mainForm.StringTable = DataStore[k];
                                //mainForm.FirstLoadRows();                                
                                DataStore[k].Changed = true;
                            }
                            else
                            {
                                MessageBox.Show("В файле " + DataStore[k].Name + " в колонке " + (comboBox1.SelectedIndex + 1)  + "не все значения можно в логарифмическую шкалу.", "Операция прервана", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                    LoadTextBoxInfo();
                }
                textBoxNumb.Text = "";
            }
            catch (Exception h)
            {
                MessageBox.Show(h.ToString(), "Что-то пошло не так", MessageBoxButtons.OK);
            }
        }
        private void buttonNorma_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxNumb.Text = "0";
                if (!(CheckEnteredValue()))
                {
                    for (int k = 0; k < checkBoxComboBox1.CheckBoxItems.Count; k++)
                    {
                        if (checkBoxComboBox1.CheckBoxItems[k].Checked)
                        {
                            float max = FindMax(DataStore[k], comboBox1.SelectedIndex);
                            for (int i = 0; i < DataStore[k].RowsCount; i++)
                            {
                                if (GateCondition(DataStore[k], i))
                                {
                                    float val = Convert.ToSingle(DataStore[k][comboBox1.SelectedIndex, i].Replace(".", ","));
                                    DataStore[k][comboBox1.SelectedIndex, i] = Convert.ToString(val / max);
                                }
                            }
                            //mainForm.StringTable = DataStore[k];
                            //mainForm.FirstLoadRows();                            
                            DataStore[k].Changed = true;
                        }
                    }
                    LoadTextBoxInfo();
                }
                textBoxNumb.Text = "";
            }
            catch (Exception h)
            {
                MessageBox.Show(h.ToString(), "Что-то пошло не так", MessageBoxButtons.OK);
            }
        }
        private void buttonRound_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(CheckEnteredValue()))
                {
                    for (int k = 0; k < checkBoxComboBox1.CheckBoxItems.Count; k++)
                    {
                        if (checkBoxComboBox1.CheckBoxItems[k].Checked)
                        {
                            int numb = Convert.ToInt32(textBoxNumb.Text);
                            
                            for (int i = 0; i < DataStore[k].RowsCount; i++)
                            {
                                if (GateCondition(DataStore[k], i))
                                {
                                    float value = Convert.ToSingle(DataStore[k][comboBox1.SelectedIndex, i].Replace(".", ","));
                                    string newVal = String.Format(CallFormat(numb), value);
                                    DataStore[k][comboBox1.SelectedIndex, i] = newVal;
                                }
                            }
                            //mainForm.StringTable = DataStore[k];
                            //mainForm.FirstLoadRows();                            
                            DataStore[k].Changed = true;
                        }
                    }
                    LoadTextBoxInfo();
                }
            }
            catch (Exception h)
            {
                MessageBox.Show(h.ToString(), "Что-то пошло не так", MessageBoxButtons.OK);
            }
        }
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxNumb.Text = "0";
                if (!(CheckEnteredValue()))
                {
                    if (FindMinimumColumnNumber() > 1)
                    {
                        for (int k = 0; k < checkBoxComboBox1.CheckBoxItems.Count; k++)
                        {
                            if (checkBoxComboBox1.CheckBoxItems[k].Checked)
                            {
                                StringTable st = new StringTable(DataStore[k].Name, DataStore[k].ColumnsCount - 1, DataStore[k].RowsCount);

                                for (int i = 0; i < DataStore[k].RowsCount; i++)
                                {
                                    int col = 0;
                                    for (int j = 0; j < DataStore[k].ColumnsCount; j++)
                                    {
                                        if (j != SelectedColumn)
                                        {
                                            st.Cell[col, i] = DataStore[k][j, i];
                                            col++;
                                        }
                                    }
                                }


                                DataStore[k].Table = new string[st.ColomnsCount, st.RowsCount];
                                DataStore[k].Name = st.Name;

                                for (int i = 0; i < st.RowsCount; i++)
                                {
                                    for (int j = 0; j < st.ColomnsCount; j++)
                                    {
                                        DataStore[k][j, i] = st.Cell[j, i];
                                    }
                                }

                                mainForm.NumberOfColumns = DataStore[k].ColumnsCount;
                                //mainForm.StringTable = DataStore[k];
                                //mainForm.FirstLoadRows();                                
                                DataStore[k].Changed = true;
                            }
                        }
                        if (SelectedColumn > 0)
                        {
                            SelectedColumn--;
                            comboBox1.SelectedIndex = SelectedColumn;
                        }                        
                    }
                    LoadTextBoxInfo();
                }
                textBoxNumb.Text = "";
            }
            catch (Exception h)
            {
                MessageBox.Show(h.ToString(), "Что-то пошло не так", MessageBoxButtons.OK);
            }
        }

        private void buttonSwipe_Click(object sender, EventArgs e)
        {
            try
            {                
                if (!(CheckEnteredValue()))
                {
                    for (int k = 0; k < checkBoxComboBox1.CheckBoxItems.Count; k++)
                    {
                        if (checkBoxComboBox1.CheckBoxItems[k].Checked)
                        {                            
                            StringTable st = new StringTable(DataStore[k].Name, DataStore[k].ColumnsCount, DataStore[k].RowsCount);
                            
                            
                            int col = comboBox1.SelectedIndex;
                            float val = Convert.ToSingle(textBoxNumb.Text);
                            int brow = FindBreakRow(DataStore[k], col);

                            int counter = 0;
                            for (int i = brow; i < DataStore[k].RowsCount; i++)
                            {
                                for (int j = 0; j < DataStore[k].ColumnsCount; j++)
                                {
                                    if (j == col)
                                    {
                                        float newValue = Convert.ToSingle(DataStore[k][col, i].Replace(".", ",")) - val;
                                        st.Cell[j, counter] = Convert.ToString(newValue);
                                    }
                                    else
                                    {
                                        st.Cell[j, counter] = DataStore[k][j, i];
                                    }
                                    
                                }
                                counter++;
                            }


                            for (int i = 0; i < brow; i++)
                            {
                                for (int j = 0; j < DataStore[k].ColumnsCount; j++)
                                {
                                    if (j == col)
                                    {
                                        float newValue = Convert.ToSingle(DataStore[k][col, i].Replace(".", ",")) + val;
                                        st.Cell[j, counter] = Convert.ToString(newValue);
                                    }
                                    else
                                    {
                                        st.Cell[j, counter] = DataStore[k][j, i];
                                    }
                                    
                                }
                                counter++;
                            }


                            DataStore[k].Table = new string[st.ColomnsCount, st.RowsCount];
                            DataStore[k].Name = st.Name;

                            for (int i = 0; i < st.RowsCount; i++)
                            {
                                for (int j = 0; j < st.ColomnsCount; j++)
                                {
                                    DataStore[k][j, i] = st.Cell[j, i];
                                }
                            }


                            DataStore[k].Changed = true;
                        }
                        //mainForm.StringTable = DataStore[k];
                        //mainForm.FirstLoadRows();
                    }
                    LoadTextBoxInfo();
                }
                textBoxNumb.Text = "";
            }
            catch (Exception h)
            {
                MessageBox.Show(h.ToString(), "Что-то пошло не так", MessageBoxButtons.OK);
            }
        }

        

        #endregion

        #region Methods
        private int FindBreakRow(DataElement2 st, int col)
        {
            int numb = -1;
            for (int i = 0; i < st.RowsCount; i++)
            {
                if (Convert.ToSingle(st[col, i].Replace(".", ",")) > 0)
                {
                    numb = i;
                    break;
                }
            }
            return numb;
        }
        private bool CheckEnteredValue()
        {
            if (textBoxNumb.Text == "-")
            {
                textBoxNumb.Text = "-1";
            }

            bool error = false;
            if (radioButton1.Checked)
            {
                try
                {
                    int start = Convert.ToInt32(textBoxStart.Text);
                    int finish = Convert.ToInt32(textBoxFinish.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Некорректно введён диапазон значений", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    error = true;
                }
            }
            else if (radioButton2.Checked)
            {
                try
                {
                    float start = Convert.ToSingle(textBoxStart.Text);
                    float finish = Convert.ToSingle(textBoxFinish.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Некорректно введён диапазон значений", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    error = true;
                }
            }


            if (textBoxNumb.Text == "")
            {
                string mess = "Введите величину сдвига значений колонки";
                MessageBox.Show(mess, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                error = true;
            }
            else
            {
                try
                {
                    float var = Convert.ToSingle(textBoxNumb.Text);
                }
                catch (Exception)
                {
                    string mess = "Ошибка ввода данных";
                    MessageBox.Show(mess, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    error = true;
                }
            }
            
            return error;
        }
        private bool GateCondition(DataElement2 tab, int i)
        {
            bool answer = false;

            if (radioButton1.Checked)
            {
                if (i >= Convert.ToInt32(textBoxStart.Text) - 1 && i <= Convert.ToInt32(textBoxFinish.Text) - 1)
                {
                    if (i < tab.RowsCount)
                    {
                        answer = true;    
                    }                    
                }
            }
            if (radioButton2.Checked)
            {
                float celldata = Convert.ToSingle(tab[comboBox1.SelectedIndex, i].Replace(".", ","));
                float left = Convert.ToSingle(textBoxStart.Text.Replace(".", ","));
                float right = Convert.ToSingle(textBoxFinish.Text.Replace(".", ","));
                if (celldata >= left && celldata <= right)
                {
                    answer = true;
                }
            }
            return answer;
        }
        private bool OnlyPositive(DataElement2 tab, int col)
        {
            for (int i = 0; i < tab.RowsCount; i++)
            {
                float val = Convert.ToSingle(tab[col, i].Replace(".", ","));
                if (val < 0)
                {
                    return false;
                }
            }
            return true;        
        }
        private float FindMax(DataElement2 st, int col)
        {
            float max = Convert.ToSingle(st[col, 0].Replace(".", ","));
            for (int i = 1; i < st.RowsCount; i++)
            {
               float val = Convert.ToSingle(st[col, i].Replace(".", ","));
               if (val > max)
               {
                   max = val;
               }
            }
            return max;
        }
        private string CallFormat(int dec)
        {
            string fA = "{0:0";
            string fB = "}";

            string format = fA;
            if (dec > 0)
            {
                format += ".";
                for (int q = 0; q < dec; q++)
                {
                    format += "0";
                }
            }
            format += fB;
            return format;
        }
        public void LoadTextBoxInfo()
        {
            if (comboBox1.SelectedIndex != -1)
            {
                if (radioButton1.Checked)
                {
                    int count = 0;

                    for (int i = 0; i < checkBoxComboBox1.CheckBoxItems.Count; i++)
                    {
                        if (checkBoxComboBox1.CheckBoxItems[i].Checked)
                        {
                            //DataElement datElement = new DataElement(DataStore[i], 0, comboBox1.SelectedIndex);
                            if (count == 0)
                            {
                                count = DataStore[i].RowsCount;
                            }
                            if (count < DataStore[i].RowsCount)
                            {
                                count = DataStore[i].RowsCount;
                            }
                        }
                    }
                    if (count == 0)
                    {
                        textBoxStart.Text = "0";
                        textBoxFinish.Text = "0";
                    }
                    else
                    {
                        textBoxStart.Text = "1";
                        textBoxFinish.Text = "" + count;
                    }
                }
                if (radioButton2.Checked)
                {
                    List<double> maximum = new List<double>();
                    List<double> minimum = new List<double>();
                    for (int i = 0; i < DataStore.Count; i++)
                    {
                        if (checkBoxComboBox1.CheckBoxItems[i].Checked)
                        {
                            DataElement2 datElement = DataStore[i];
                            double max = datElement.Max;
                            double min = datElement.Min;
                            maximum.Add(max);
                            minimum.Add(min);
                        }
                    }

                    if (maximum.Count == 0 || minimum.Count == 0)
                    {
                        textBoxStart.Text = "";
                        textBoxFinish.Text = "";
                    }
                    else
                    {
                        double max = maximum[0];
                        double min = minimum[0];
                        for (int i = 1; i < maximum.Count; i++)
                        {
                            if (max < maximum[i])
                            {
                                max = maximum[i];
                            }
                            if (min > minimum[i])
                            {
                                min = minimum[i];
                            }
                        }
                        textBoxStart.Text = "" + min;
                        textBoxFinish.Text = "" + max;                        
                    }
                }
            }
        }
        private bool CheckColumns()
        {
            for (int i = 0; i < DataStore.Count; i++)
            {
                if (checkBoxComboBox1.CheckBoxItems[i].Checked)
                {
                    if (DataStore[i].ColumnsCount < comboBox1.SelectedIndex + 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int FindMinimumColumnNumber()
        {
            int min = 10000;
            for (int i = 0; i < DataStore.Count; i++)
            {
                if (checkBoxComboBox1.CheckBoxItems[i].Checked)
                {
                    if (DataStore[i].ColumnsCount < min)
                    {
                        min = DataStore[i].ColumnsCount;
                    }
                }
            }
            if (min == 10000)
            {
                return 0;
            }
            else
            {
                return min;
            }
            
        }
        #endregion        

        #region NumbersPad
        private void button0_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "0";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "1";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "2";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "3";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "4";
        }
        private void button5_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "5";
        }
        private void button6_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "6";
        }
        private void button7_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "7";
        }
        private void button8_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "8";
        }
        private void button9_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + "9";
        }
        private void buttonBackspace_Click(object sender, EventArgs e)
        {
            if (!(textBoxNumb.Text.Length == 0))
            {
                textBoxNumb.Text = textBoxNumb.Text.Remove(textBoxNumb.Text.Length - 1);
            }
        }
        private void buttonC_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = "";
        }
        private void buttonPoint_Click(object sender, EventArgs e)
        {
            textBoxNumb.Text = textBoxNumb.Text + ",";
        }
        private void buttonPlusMinus_Click(object sender, EventArgs e)
        {
            bool sign = false;
            if (!(textBoxNumb.Text.IndexOf(Convert.ToChar("-")) == -1))
            {
                sign = true;
            }

            if (sign)
            {
                textBoxNumb.Text = textBoxNumb.Text.Substring(1);
            }
            else
            {
                textBoxNumb.Text = String.Concat("-", textBoxNumb.Text);
            }

        }
        #endregion    

        #region Buttons
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();     
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            for (int k = 0; k < DataStore.Count; k++)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(DataStore[k].Name))
                    {
                        for (int i = 0; i < DataStore[k].RowsCount; i++)
                        {
                            string line = "";
                            for (int j = 0; j < DataStore[k].ColumnsCount; j++)
                            {
                                if (j != 0)
                                {
                                    line += "\t";
                                }
                                
                                line += DataStore[k][j, i];                                                           
                            }
                            if (SettingsForm.DecimalDelimeter == "Точка")
                            {                                
                                line = line.Replace(",", ".");                                
                            }     
                            sw.WriteLine(line);
                        }
                    }
                    DataStore[k].Changed = false;
                }
                catch (Exception h)
                {
                    MessageBox.Show(h.ToString(), "Что-то пошло не так", MessageBoxButtons.OK);
                }
            }
        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            for (int k = 0; k < DataStore.Count; k++)
            {
                if (DataStore[k].Changed)
                {
                    saveFileDialog1.FileName = DataStore[k].Name;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                        {
                            for (int i = 0; i < DataStore[k].RowsCount; i++)
                            {
                                string line = "";
                                for (int j = 0; j < DataStore[k].ColumnsCount; j++)
                                {
                                    if (j != 0)
                                    {
                                        line += "\t";
                                    }
                                    line += DataStore[k][j, i].Replace(".", ",");
                                }
                                sw.WriteLine(line);
                            }
                        }
                    }                    
                }
                DataStore[k].Changed = false;
            }
        }        

        private void buttonCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkBoxComboBox1.CheckBoxItems.Count; i++)
            {
                checkBoxComboBox1.CheckBoxItems[i].Checked = true;
            }
        }

        private void buttonUnCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkBoxComboBox1.CheckBoxItems.Count; i++)
            {
                checkBoxComboBox1.CheckBoxItems[i].Checked = false;
            }
        }

        #endregion 

        private void textBoxFinish_TextChanged(object sender, EventArgs e)
        {
            int a = 0;
            a++;
        }

        








    }
}
