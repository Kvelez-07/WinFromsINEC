using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFromsINEC
{
    public partial class frmINEC : Form
    {
        #region Attributes
        static PopulationAge[] Age_Arr = new PopulationAge[40];
        static int[] Male_Arr = { 0, 0, 0, 0 }; // Summary of age group 0-4, etc.
        static int[] Female_Arr = { 0, 0, 0, 0 };
        static int[] Scholarship_Arr = { 0, 0, 0, 0, 0, 0, 0 }; // Summary of scholarship groups, takes 7 positions in text file

        static string[] Scholarship_Label_Arr =
        {
            "Primaria Completa",
            "Primaria Incompleta",
            "Secundaria Completa",
            "Secundaria Incompleta",
            "Universitaria Completa",
            "Universitaria Incompleta",
            "Sin Estudios"
        };

        static int Age_Index = 0;
        static int Male_Total = 0;
        static int Female_Total = 0;

        UtilitiesINEC Census_Utils = new UtilitiesINEC();

        Thread Male_Age_Thread; // Summary of age groups for men
        Thread Female_Age_Thread; // Summary of age groups for women
        Thread Scholarship_Thread; // Summary of scholarship groups
        #endregion

        public frmINEC()
        {
            InitializeComponent();
        }

        #region Buttons
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            ReadFile();
            FillPeopleDataGrid();

            Male_Age_Thread = new Thread(new ThreadStart(CalculateMen));
            Female_Age_Thread = new Thread(new ThreadStart(CalculateWomen));
            Scholarship_Thread = new Thread(new ThreadStart(CalculateScholarship));

            ActivateProgressBar(true, "Calculando", 1, 3, 1);

            if (!Male_Age_Thread.IsAlive)
                Male_Age_Thread.Start();
            else Census_Utils.MessageThread(Male_Age_Thread);

            if (!Female_Age_Thread.IsAlive)
                Female_Age_Thread.Start();
            else
                Census_Utils.MessageThread(Female_Age_Thread);

            if (!Scholarship_Thread.IsAlive)
                Scholarship_Thread.Start();
            else
                Census_Utils.MessageThread(Scholarship_Thread);

            gridAgeToGender.Invoke(new Action(() =>
            {
                while (Male_Age_Thread.ThreadState == ThreadState.Running)
                {
                    Thread.Sleep(1000);
                }

                while (Female_Age_Thread.ThreadState == ThreadState.Running)
                {
                    Thread.Sleep(1000);
                }

                Census_Utils.LogMessage("Thread execution finished");
                ActivateProgressBar(false, string.Empty, 0, 0, 0);
                FillGenderDataGrid();
                FillScholarshipDataGrid();

            }));
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (Male_Age_Thread.ThreadState == ThreadState.Running)
            {
                Census_Utils.MessageThread(Male_Age_Thread, "is still running");
            }
            else if (Female_Age_Thread.ThreadState == ThreadState.Running)
            {
                Census_Utils.MessageThread(Female_Age_Thread, "is still running");
            }
            else if (Scholarship_Thread.ThreadState == ThreadState.Running)
            {
                Census_Utils.MessageThread(Scholarship_Thread, "is still running");
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion

        public void AddScholarship(int index, int age_index)
        {
            for (int i = 0; i < Age_Arr[age_index].Scholarship.Length; i++)
                Scholarship_Arr[index] += Age_Arr[age_index].Scholarship[i];
        }

        public void CalculateScholarship()
        {
            txtThreadId.Invoke(new Action(() =>
            {
                txtThreadId.Text = Scholarship_Thread.ManagedThreadId.ToString();
            }));

            for (int i = 0; i < Age_Arr.Length; i++)
            {
                if (Age_Arr[i] != null)
                {
                    lock (Scholarship_Arr.SyncRoot)
                    {
                        if ((Age_Arr[i].Age >= 0) && (Age_Arr[i].Age <= 4))
                            AddScholarship(0, Age_Arr[i].Age);
                        else if ((Age_Arr[i].Age >= 5) && (Age_Arr[i].Age <= 9))
                            AddScholarship(1, Age_Arr[i].Age);
                        else if ((Age_Arr[i].Age >= 10) && (Age_Arr[i].Age <= 14))
                            AddScholarship(2, Age_Arr[i].Age);
                        else if ((Age_Arr[i].Age >= 15) && (Age_Arr[i].Age <= 19))
                            AddScholarship(3, Age_Arr[i].Age);
                        else if (Age_Arr[i].Age == 20)
                            AddScholarship(4, 20);
                        else if (Age_Arr[i].Age == 50)
                            AddScholarship(5, 21);
                        else if (Age_Arr[i].Age == 60)
                            AddScholarship(6, 22);
                    }
                }
            }
        }

        public void FillPeopleDataGrid()
        {
            gridAgeToGender.Font = new Font("Arial", 10);
            gridAgeToGender.Columns.Add("Age", "Age");
            gridAgeToGender.Columns.Add("Male", "Male");
            gridAgeToGender.Columns.Add("Female", "Female");

            gridAgeToGender.Columns["Male"].DefaultCellStyle.Format = "#.##";
            gridAgeToGender.Columns["Female"].DefaultCellStyle.Format = "#.##";

            gridAgeToGender.Columns["Male"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridAgeToGender.Columns["Female"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            for (int i = 0; i < Age_Arr.Length; i++)
            {
                if (Age_Arr[i] != null)
                {
                    gridAgeToGender.Rows.Add(Age_Arr[i].Age, Age_Arr[i].Male, Age_Arr[i].Female);
                }
            }

            gridAgeToGender.Refresh();
        }

        public void WriteSubTotal(string heading, int maleTotal, int femaleTotal, bool status, int type)
        {
            if (status)
            {
                if (type == 1)
                    gridGenderAge.Rows.Add(); // blank
                else
                    gridScholarship.Rows.Add();
            }

            if (type == 1)
            {
                gridGenderAge.Rows.Add(heading, maleTotal, femaleTotal, maleTotal + femaleTotal);
                gridGenderAge.Rows.Add();
            }
            else
            {
                gridGenderAge.Rows.Add(heading, string.Empty, maleTotal);
                gridGenderAge.Rows.Add();
            }
        }

        public void FillGenderDataGrid()
        {
            gridGenderAge.Font = new Font("Arial", 10);
            gridGenderAge.Columns.Add("Age", "Age");
            gridGenderAge.Columns.Add("Male", "Male");
            gridGenderAge.Columns.Add("Female", "Female");
            gridGenderAge.Columns.Add("Total", "Total");

            gridGenderAge.Columns["Female"].DefaultCellStyle.Format = "#.##";
            gridGenderAge.Columns["Male"].DefaultCellStyle.Format = "#.##";
            gridGenderAge.Columns["Total"].DefaultCellStyle.Format = "#.##";

            gridGenderAge.Columns["Male"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridGenderAge.Columns["Female"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridGenderAge.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            for (int i = 0; i < Age_Arr.Length; i++)
            {
                if (Age_Arr[i] != null)
                {
                    string ageStr = Age_Arr[i].Age.ToString();

                    if (i == 0)
                        WriteSubTotal("Total", Male_Total, Female_Total, false, 1);

                    if (Age_Arr[i].Age == 0)
                        WriteSubTotal("0-4", Male_Arr[0], Female_Arr[0], false, 1);
                    else if (Age_Arr[i].Age == 5)
                        WriteSubTotal("5-9", Male_Arr[1], Female_Arr[1], true, 1);
                    else if (Age_Arr[i].Age == 10)
                        WriteSubTotal("10-14", Male_Arr[2], Female_Arr[2], true, 1);
                    else if (Age_Arr[i].Age == 15)
                        WriteSubTotal("15-19", Male_Arr[3], Female_Arr[3], true, 1);
                    else if (Age_Arr[i].Age == 20)
                    {
                        gridGenderAge.Rows.Add();
                        ageStr = "20-49";
                    }
                    else if (Age_Arr[i].Age == 50)
                    {
                        gridGenderAge.Rows.Add();
                        ageStr = "50-59";
                    }
                    else if (Age_Arr[i].Age == 60)
                    {
                        gridGenderAge.Rows.Add();
                        ageStr = "60+";
                    }

                    gridGenderAge.Rows.Add(ageStr, Age_Arr[i].Male, Age_Arr[i].Female, (Age_Arr[i].Male + Age_Arr[i].Female));
                }
            }

            gridAgeToGender.Refresh();
        }

        public void FillScholarshipDataGrid()
        {
            gridScholarship.Font = new Font("Arial", 10);
            gridScholarship.Columns.Add("Age", "Age");
            gridScholarship.Columns.Add("Scholarship", "Scholarship");
            gridScholarship.Columns.Add("Total", "Total");

            gridScholarship.Columns["Total"].DefaultCellStyle.Format = "#.##";

            gridScholarship.Columns["Age"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridScholarship.Columns["Scholarship"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridScholarship.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            for (int i = 0; i < Age_Arr.Length; i++)
            {
                if (Age_Arr[i] != null)
                {
                    if (i == 0)
                        WriteSubTotal("Total", Male_Total + Female_Total, 0, false, 2);

                    if (Age_Arr[i].Age == 0)
                        WriteSubTotal("0-4", Scholarship_Arr[0], 0, false, 2);
                    else if (Age_Arr[i].Age == 5)
                        WriteSubTotal("5-9", Scholarship_Arr[1], 0, true, 2);
                    else if (Age_Arr[i].Age == 10)
                        WriteSubTotal("10-14", Scholarship_Arr[2], 0, true, 2);
                    else if (Age_Arr[i].Age == 15)
                        WriteSubTotal("15-19", Scholarship_Arr[3], 0, true, 2);
                    else if (Age_Arr[i].Age == 20)
                        WriteSubTotal("20-49", Scholarship_Arr[4], 0, true, 2);
                    else if (Age_Arr[i].Age == 50)
                        WriteSubTotal("50-59", Scholarship_Arr[5], 0, true, 2);
                    else if (Age_Arr[i].Age == 60)
                        WriteSubTotal("60+", Scholarship_Arr[6], 0, true, 2);

                    for (int j = 0; j < Age_Arr[i].Scholarship.Length; j++)
                    {
                        gridScholarship.Rows.Add(Age_Arr[i].Age, Scholarship_Label_Arr[j], Age_Arr[i].Scholarship[j]);
                    }
                }
            }

            gridScholarship.Refresh();
        }

        public void CalculateMen()
        {
            txtThreadId.Invoke(new Action(() =>
            {
                txtThreadId.Text = Male_Age_Thread.ManagedThreadId.ToString();
            }));

            for (int i = 0; i < Age_Arr.Length; i++)
            {
                if (Age_Arr[i] != null)
                {
                    lock (Male_Arr.SyncRoot)
                    {
                        if ((Age_Arr[i].Age >= 0) && (Age_Arr[i].Age <= 4))
                            Male_Arr[0] += Age_Arr[i].Male;
                        else if ((Age_Arr[i].Age >= 5) && (Age_Arr[i].Age <= 9))
                            Male_Arr[1] += Age_Arr[i].Male;
                        else if ((Age_Arr[i].Age >= 10) && (Age_Arr[i].Age <= 14))
                            Male_Arr[2] += Age_Arr[i].Male;
                        else if ((Age_Arr[i].Age >= 15) && (Age_Arr[i].Age <= 19))
                            Male_Arr[3] += Age_Arr[i].Male;
                    }
                }
            }

            Console.WriteLine("Male");
            Console.WriteLine(Male_Arr[0]);
            Console.WriteLine(Male_Arr[1]);
            Console.WriteLine(Male_Arr[2]);
            Console.WriteLine(Male_Arr[3]);

            prgAge.Invoke(new Action(() =>
            {
                prgAge.PerformStep();
            }));
        }

        public void CalculateWomen()
        {
            txtThreadId.Invoke(new Action(() =>
            {
                txtThreadId.Text = Female_Age_Thread.ManagedThreadId.ToString();
            }));

            for (int i = 0; i < Age_Arr.Length; i++)
            {
                if (Age_Arr[i] != null)
                {
                    lock (Female_Arr.SyncRoot)
                    {
                        if ((Age_Arr[i].Age >= 0) && (Age_Arr[i].Age <= 4))
                            Female_Arr[0] += Age_Arr[i].Female;
                        else if ((Age_Arr[i].Age >= 5) && (Age_Arr[i].Age <= 9))
                            Female_Arr[1] += Age_Arr[i].Female;
                        else if ((Age_Arr[i].Age >= 10) && (Age_Arr[i].Age <= 14))
                            Female_Arr[2] += Age_Arr[i].Female;
                        else if ((Age_Arr[i].Age >= 15) && (Age_Arr[i].Age <= 19))
                            Female_Arr[3] += Age_Arr[i].Female;
                    }
                }
            }

            Console.WriteLine("Female");
            Console.WriteLine(Female_Arr[0]);
            Console.WriteLine(Female_Arr[1]);
            Console.WriteLine(Female_Arr[2]);
            Console.WriteLine(Female_Arr[3]);

            prgAge.Invoke(new Action(() =>
            {
                prgAge.PerformStep();
            }));
        }

        public void ActivateProgressBar(bool status, string message, int min, int max, int steps)
        {
            if (status)
            {
                lblProgress.Text = message;
                lblProgress.Visible = true;
                prgAge.Visible = true;
                prgAge.Minimum = min;
                prgAge.Maximum = max;
                prgAge.Value = min;
                prgAge.Step = steps;
            }
            else
            {
                lblProgress.Visible = false;
                prgAge.Visible = false;
            }
        }

        public void GetFileData(string fileLine)
        {
            Age_Arr[Age_Index] = new PopulationAge();
            Age_Arr[Age_Index].Age = Convert.ToInt32(fileLine.Substring(0, 2));
            Age_Arr[Age_Index].Male = Convert.ToInt32(fileLine.Substring(2, 7));
            Age_Arr[Age_Index].Female = Convert.ToInt32(fileLine.Substring(9, 7));
            Age_Arr[Age_Index].Scholarship[0] = Convert.ToInt32(fileLine.Substring(16, 6));
            Age_Arr[Age_Index].Scholarship[1] = Convert.ToInt32(fileLine.Substring(22, 6));
            Age_Arr[Age_Index].Scholarship[2] = Convert.ToInt32(fileLine.Substring(28, 6));
            Age_Arr[Age_Index].Scholarship[3] = Convert.ToInt32(fileLine.Substring(34, 6));
            Age_Arr[Age_Index].Scholarship[4] = Convert.ToInt32(fileLine.Substring(40, 6));
            Age_Arr[Age_Index].Scholarship[5] = Convert.ToInt32(fileLine.Substring(46, 6));
            Age_Arr[Age_Index].Scholarship[6] = Convert.ToInt32(fileLine.Substring(52, 6));

            Male_Total += Age_Arr[Age_Index].Male;
            Female_Total += Age_Arr[Age_Index].Female;

            Console.WriteLine($"**** {Age_Arr[Age_Index].Age} ****");
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[0]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[1]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[2]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[3]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[4]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[5]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[6]);
            Console.WriteLine("**** UL ****");

            Age_Index++;
        }

        public void ReadFile()
        {
            string fileName = @".\DB\Proyeccion_2025.txt";
            int legnth = Convert.ToInt32(new FileInfo(fileName).Length);
            int step = 0;
            string line = string.Empty;

            try
            {
                var reader = new StreamReader(fileName);
                line = reader.ReadLine();
                step = Convert.ToInt32(line.Length);

                ActivateProgressBar(true, "Reading File", 1, legnth, step);
                prgAge.Refresh();

                while (line != null)
                {
                    GetFileData(line);
                    line = reader.ReadLine();
                    prgAge.PerformStep();
                }

                reader.Close();
                prgAge.Value = legnth;
                Census_Utils.LogMessage("File read successfully");
                ActivateProgressBar(false, string.Empty, 0, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void txtThreadId_TextChanged(object sender, EventArgs e)
        {

        }

        private void gridAgeToGender_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gridGenderAge_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gridScholarship_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void prgAge_Click(object sender, EventArgs e)
        {

        }

        private void btnLoadFile_Click_1(object sender, EventArgs e)
        {
            ReadFile();
            FillPeopleDataGrid();

            Male_Age_Thread = new Thread(new ThreadStart(CalculateMen));
            Female_Age_Thread = new Thread(new ThreadStart(CalculateWomen));
            Scholarship_Thread = new Thread(new ThreadStart(CalculateScholarship));

            ActivateProgressBar(true, "Calculando", 1, 3, 1);

            if (!Male_Age_Thread.IsAlive)
                Male_Age_Thread.Start();
            else Census_Utils.MessageThread(Male_Age_Thread);

            if (!Female_Age_Thread.IsAlive)
                Female_Age_Thread.Start();
            else
                Census_Utils.MessageThread(Female_Age_Thread);

            if (!Scholarship_Thread.IsAlive)
                Scholarship_Thread.Start();
            else
                Census_Utils.MessageThread(Scholarship_Thread);

            gridAgeToGender.Invoke(new Action(() =>
            {
                while (Male_Age_Thread.ThreadState == ThreadState.Running)
                {
                    Thread.Sleep(1000);
                }

                while (Female_Age_Thread.ThreadState == ThreadState.Running)
                {
                    Thread.Sleep(1000);
                }

                Census_Utils.LogMessage("Thread execution finished");
                ActivateProgressBar(false, string.Empty, 0, 0, 0);
                FillGenderDataGrid();
                FillScholarshipDataGrid();

            }));
        }

        private void btnExit_Click_1(object sender, EventArgs e)
        {
            if (Male_Age_Thread.ThreadState == ThreadState.Running)
            {
                Census_Utils.MessageThread(Male_Age_Thread, "is still running");
            }
            else if (Female_Age_Thread.ThreadState == ThreadState.Running)
            {
                Census_Utils.MessageThread(Female_Age_Thread, "is still running");
            }
            else if (Scholarship_Thread.ThreadState == ThreadState.Running)
            {
                Census_Utils.MessageThread(Scholarship_Thread, "is still running");
            }
            else
            {
                Application.Exit();
            }
        }
    }

    public class PopulationAge
    {
        public int Age { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int[] Scholarship = { 0, 0, 0, 0, 0, 0, 0 };
    }

    public class UtilitiesINEC
    {
        // Message function that indicates that a thread is executing and the managed threadid
        public void MessageThread(Thread thread)
        {
            MessageBox.Show($"Thread {thread.ManagedThreadId} is executing");
        }

        public void MessageThread(Thread thread, string message)
        {
            MessageBox.Show($"Thread {thread.ManagedThreadId} is executing {message}");
        }

        public void LogMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
