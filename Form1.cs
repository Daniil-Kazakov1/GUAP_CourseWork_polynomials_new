using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using operationsPolynomials;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;


namespace courseWork_polinom
{
    public partial class Form1 : Form
    {
        Stopwatch stopwatch = new Stopwatch();
        public Form1()
        {
            
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            button9.Click += button9_Click;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            

            int tmp_agree1 = Convert.ToInt32(comboBox1.Text);
            int tmp_agree2 = Convert.ToInt32(comboBox2.Text);

            for (int j = 0; j <= tmp_agree1; j++)
            {
                dataGridView1.Columns.Add($"Columns{j}", $"{j}");
                dataGridView1.RowCount = 1;
                dataGridView1.AllowUserToAddRows = false;
            }
            for (int i = 0; i <= tmp_agree2; i++)
            {
                dataGridView2.Columns.Add($"Columns{i}", $"{i}");
                dataGridView2.RowCount = 1;
                dataGridView2.AllowUserToAddRows = false;
            }

        }
        private void button9_Click(object sender, EventArgs e)
        {
            RandomNumbers(dataGridView1);
            RandomNumbers(dataGridView2);
        }
        private Random random = new Random();
        private void RandomNumbers(DataGridView dataGridView)
        {
            for (int row = 0; row < dataGridView.Rows.Count; row++)
            {
                for (int col = 0; col < dataGridView.Columns.Count; col++)
                {
                    double randomNumber = random.NextDouble() * 99.9 + 0.1;
                    dataGridView.Rows[row].Cells[col].Value = randomNumber.ToString("F3");
                }
            }
        }

        private Polynomial NewPolinom1()
        {
            double[] coefficients1 = new double[dataGridView1.Columns.Count];
            int agree1 = dataGridView1.Columns.Count;

            try
            {
                bool hasValues = false;
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    hasValues = true;
                    if (double.TryParse(dataGridView1.Rows[0].Cells[i].Value?.ToString(), out double coefficient))
                    {
                        coefficients1[i] = coefficient;
                    }
                    else
                    {
                        throw new FormatException($"Ошибка ввода в столбце {i + 1}. Введите вещественное число через запятую");
                    }
                }
                if (!hasValues) 
                {
                    coefficients1 = new double[1];
                    coefficients1[0] = 1; 
                }

                Polynomial polynomial1 = new Polynomial(coefficients1, agree1);
                return polynomial1;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; 
            }
        }
        private Polynomial NewPolinom2()
        {
            double[] coefficients2 = new double[dataGridView2.Columns.Count];
            int agree2 = dataGridView2.Columns.Count;

            try
            {
                bool hasValues = false; 

                for (int i = 0; i < dataGridView2.Columns.Count; i++)
                {
                    if (dataGridView2.Rows[0].Cells[i].Value != null &&
                        !string.IsNullOrWhiteSpace(dataGridView2.Rows[0].Cells[i].Value.ToString()))
                    {
                        hasValues = true; 
                        if (double.TryParse(dataGridView2.Rows[0].Cells[i].Value?.ToString(), out double coefficient))
                        {
                            coefficients2[i] = coefficient;
                        }
                        else
                        {
                            throw new FormatException($"Ошибка ввода в столбце {i + 1}. Пожалуйста, введите вещественное число через запятую");
                        }
                    }
                }

                if (!hasValues) 
                {
                    coefficients2 = new double[1];
                    coefficients2[0] = 1; 
                }

                Polynomial polynomial2 = new Polynomial(coefficients2, agree2);
                return polynomial2;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; 
            }
        }

        private void data_Base_write(string s1, string s2, string s3, string s4)
        {
            DB dataBase = new DB();
            string querystring = $"insert into operationsPolynomials(polinom1,polinom2,operation,result) values ('{s1}','{s2}','{s3}','{s4}') ";
            SqlCommand command = new SqlCommand(querystring, dataBase.GetConnection());
            dataBase.openConnection();
           /* if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Успешно! Результаты записаны в БД");
            }
            else
            {
                MessageBox.Show("Ошибка записи в БД!");
            } */
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            Process currentProcess = Process.GetCurrentProcess();
            Polynomial polynomial1 = NewPolinom1();
            Polynomial polynomial2 = NewPolinom2();
            if (polynomial1 != null && polynomial2 != null)
            {
                
                double[] resultArray = PolynomialOperations.AddArrays(polynomial1.coefficients, polynomial2.coefficients);
                
                Polynomial polynomial3 = new Polynomial(resultArray, resultArray.Length);
                string polynomial1String = PolynomialToString(polynomial1);
                string polynomial2String = PolynomialToString(polynomial2);
                string polynomialResult = PolynomialToString(polynomial3);
                
                string operation = " Сложение ";

                data_Base_write(polynomial1String, polynomial2String, operation, polynomialResult);


                
                textBox2.AppendText("Полином 1: " + polynomial1String + Environment.NewLine);
                textBox2.AppendText("Полином 2: " + polynomial2String + Environment.NewLine);
                textBox2.AppendText("Результат сложения: " + Environment.NewLine + polynomialResult + Environment.NewLine);
                
            }
            long memoryUsed = currentProcess.PrivateMemorySize64;
            DateTime end = DateTime.Now;
            TimeSpan ts = (end - start);
            textBox2.AppendText("затраченное время " + ts.TotalMilliseconds + " мс" + Environment.NewLine);
            textBox2.AppendText("затраченное память " + memoryUsed + " байт" + Environment.NewLine);
            textBox2.AppendText("--------------------------" + Environment.NewLine);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            Process currentProcess = Process.GetCurrentProcess();
            Polynomial polynomial1 = NewPolinom1();
            Polynomial polynomial2 = NewPolinom2();
            if (polynomial1 != null && polynomial2 != null)
            {
                
                stopwatch.Start();
                double[] resultArray = PolynomialOperations.SubtractArrays(polynomial1.coefficients, polynomial2.coefficients);
                Polynomial polynomial3 = new Polynomial(resultArray, resultArray.Length);
                string polynomial1String = PolynomialToString(polynomial1);
                string polynomial2String = PolynomialToString(polynomial2);
                string polynomialResult = PolynomialToString(polynomial3);

                string operation = "Вычитание";
                data_Base_write(polynomial1String, polynomial2String, operation, polynomialResult);

                
                textBox2.AppendText("Полином 1: " + polynomial1String + Environment.NewLine);
                textBox2.AppendText("Полином 2: " + polynomial2String + Environment.NewLine);
                textBox2.AppendText("Результат вычитания: " + Environment.NewLine + polynomialResult + Environment.NewLine);
                
            }
            long memoryUsed = currentProcess.PrivateMemorySize64;
            DateTime end = DateTime.Now;
            TimeSpan ts = (end - start);
            textBox2.AppendText("затраченное время " + ts.TotalMilliseconds + " мс" + Environment.NewLine);
            textBox2.AppendText("затраченное память " + memoryUsed + " байт" + Environment.NewLine);
            textBox2.AppendText("--------------------------" + Environment.NewLine);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            Process currentProcess = Process.GetCurrentProcess();
            Polynomial polynomial1 = NewPolinom1();
            Polynomial polynomial2 = NewPolinom2();
            if (polynomial1 != null && polynomial2 != null)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                double[] resultArray = PolynomialOperations.KaratsubaMultiply(polynomial1.coefficients, polynomial2.coefficients);
                Polynomial polynomial3 = new Polynomial(resultArray, resultArray.Length);
                string polynomial1String = PolynomialToString(polynomial1);
                string polynomial2String = PolynomialToString(polynomial2);
                string polynomialResult = PolynomialToString(polynomial3);

                string operation = "Умножение";
                data_Base_write(polynomial1String, polynomial2String, operation, polynomialResult);

               
                textBox2.AppendText("Полином 1: " + polynomial1String + Environment.NewLine);
                textBox2.AppendText("Полином 2: " + polynomial2String + Environment.NewLine);
                textBox2.AppendText("Результат умножения: " + Environment.NewLine + polynomialResult + Environment.NewLine);
                
            }
            long memoryUsed = currentProcess.PrivateMemorySize64;
            DateTime end = DateTime.Now;
            TimeSpan ts = (end - start);
            textBox2.AppendText("затраченное время " + ts.TotalMilliseconds + " мс" + Environment.NewLine);
            textBox2.AppendText("затраченное память " + memoryUsed + " байт" + Environment.NewLine);
            textBox2.AppendText("--------------------------" + Environment.NewLine);
        }

            private void button5_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            Process currentProcess = Process.GetCurrentProcess();
            Polynomial polynomial1 = NewPolinom1();
            Polynomial polynomial2 = NewPolinom2();

            if (polynomial1 != null && polynomial2 != null)
            {
                
                var result = PolynomialOperations.DividePolynomials(polynomial1.coefficients, polynomial2.coefficients);
                double[] quotientCoefficients = result.Item1; 
                double[] remainderCoefficients = result.Item2; 

                Polynomial quotientPolynomial = new Polynomial(quotientCoefficients, quotientCoefficients.Length);
                Polynomial remainderPolynomial = new Polynomial(remainderCoefficients, remainderCoefficients.Length);

                
                string polynomial1String = PolynomialToString(polynomial1);
                string polynomial2String = PolynomialToString(polynomial2);
                string quotientString = PolynomialToString(quotientPolynomial);
                string remainderString = PolynomialToString(remainderPolynomial);

                string operation = "частное от деления";
                
                data_Base_write(polynomial1String, polynomial2String, operation, quotientString);

                stopwatch.Stop();
                textBox2.AppendText("Полином 1: " + polynomial1String + Environment.NewLine);
                textBox2.AppendText("Полином 2: " + polynomial2String + Environment.NewLine);
                textBox2.AppendText("Частное: " + quotientString + Environment.NewLine);
                textBox2.AppendText("Остаток: " + remainderString + Environment.NewLine);
                
            }
            long memoryUsed = currentProcess.PrivateMemorySize64;
            DateTime end = DateTime.Now;
            TimeSpan ts = (end - start);
            textBox2.AppendText("затраченное время " + ts.TotalMilliseconds + " мс" + Environment.NewLine);
            textBox2.AppendText("затраченное память " + memoryUsed + " байт" + Environment.NewLine);
            textBox2.AppendText("--------------------------" + Environment.NewLine);
        }
        private string PolynomialToString(Polynomial polynomial)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirstTerm = true;

            for (int i = 0; i < polynomial.coefficients.Length; i++)
            {
                int index = polynomial.coefficients.Length - 1 - i;
                if (polynomial.coefficients[index] != 0)
                {
                    string term = $"{Math.Abs(polynomial.coefficients[index])}";
                    if (index > 0)
                    {
                        term += $"x^{index}";
                    }

                    if (polynomial.coefficients[index] < 0)
                    {
                        if (!isFirstTerm)
                        {
                            sb.Append($" - {term}");
                        }
                        else
                        {
                            sb.Append($"-{term}");
                            isFirstTerm = false;
                        }
                    }
                    else
                    {
                        if (!isFirstTerm)
                        {
                            sb.Append($" + {term}");
                        }
                        else
                        {
                            sb.Append($"{term}");
                            isFirstTerm = false;
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            Process currentProcess = Process.GetCurrentProcess();
            try
            {
                string text = textBox1.Text;

                if (double.TryParse(text, out double value))
                {
                    Polynomial polynomial1 = NewPolinom1();
                    Polynomial polynomial2 = NewPolinom2();

                    if (polynomial1 != null && polynomial2 != null)
                    {
                        double result1 = PolynomialOperations.EvaluatePolynomial(polynomial1.coefficients, value);
                        double result2 = PolynomialOperations.EvaluatePolynomial(polynomial2.coefficients, value);
                       

                        string polynomial1String = PolynomialToString(polynomial1);
                        string polynomial2String = PolynomialToString(polynomial2);
                        string polynomialResult = result1.ToString() + " (1)   " + result2.ToString() +" (2)";

                        string operation = "при х = " + text;
                        data_Base_write(polynomial1String, polynomial2String, operation, polynomialResult);

                        textBox2.AppendText("Полином 1 при x = " + value + Environment.NewLine + polynomial1String + " = " + result1 + Environment.NewLine);
                        textBox2.AppendText("Полином 2 при x = " + value + Environment.NewLine + polynomial2String + " = " + result2 + Environment.NewLine);
                        
                    }
                    long memoryUsed = currentProcess.PrivateMemorySize64;
                    DateTime end = DateTime.Now;
                    TimeSpan ts = (end - start);
                    textBox2.AppendText("затраченное время " + ts.TotalMilliseconds + " мс" + Environment.NewLine);
                    textBox2.AppendText("затраченное память " + memoryUsed + " байт" + Environment.NewLine);
                    textBox2.AppendText("--------------------------" + Environment.NewLine);
                }
                else
                {
                    MessageBox.Show("Ошибка ввода в поле 'x'. Пожалуйста, введите вещественное число через запятую", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            textBox1.Clear();
            textBox2.Clear();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"; 
                saveFileDialog.Title = "Выберите файл для сохранения"; 
                saveFileDialog.FileName = "myfile.txt"; 

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = saveFileDialog.FileName;
                        string textToWrite = textBox2.Text;
                        System.IO.File.WriteAllText(filePath, textToWrite);
                        MessageBox.Show("Текст успешно записан в файл.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при записи файла: {ex.Message}");
                    }
                }
            }
        }
    }

}

