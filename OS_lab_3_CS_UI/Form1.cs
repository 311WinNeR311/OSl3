using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace OS_lab_3_CS_UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string[] files = Directory.GetFiles(@"C:\");
        private string[] folders = Directory.GetDirectories(@"C:\");

        List<Thread> T = new List<Thread>();

        /*
         Functions
        */

        // Function that searching files 
        private List<string> SearchFiles(string[] files, string sFileName, string sWrite)
        {
            List<string> SearchedFiles = new List<string>();

            if (sFileName[sFileName.Length - 1] == '*')
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    bool isIdentity = true;
                    for (int j = 0; j < sFileName.Length; ++j)
                    {
                        if (sFileName[j] == '*')
                        {
                            break;
                        }
                        if (sFileName[j] != files[i][j])
                        {
                            isIdentity = false;
                            break;
                        }
                    }
                    if (isIdentity)
                    {
                        SearchedFiles.Add(files[i]);
                    }
                }
            }

            else
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    if (files[i].Length == sFileName.Length)
                    {
                        bool isIdentity = true;
                        for (int j = 0; j < sFileName.Length; ++j)
                        {
                            if (sFileName[j] != files[i][j])
                            {
                                isIdentity = false;
                                break;
                            }
                        }
                        if (isIdentity)
                        {
                            SearchedFiles.Add(files[i]);
                        }
                    }
                }
            }
            System.IO.StreamWriter fileT = new System.IO.StreamWriter(sWrite, false);
            fileT.Close();
            for (int i = 0; i < SearchedFiles.Count; ++i)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(sWrite, true))
                {
                    file.WriteLine(SearchedFiles[i].ToString());
                }
            }
            
            return SearchedFiles;
        }

        // Function that searching folders 
        private List<string> SearchFolders(string[] folders, string sFileName, string sWrite)
        {
            List<string> SearchedFolders = new List<string>();

            if (sFileName[sFileName.Length - 1] == '*')
            {
                for (int i = 0; i < folders.Length; ++i)
                {
                    bool isIdentity = true;
                    for (int j = 0; j < sFileName.Length; ++j)
                    {
                        if (sFileName[j] == '*')
                        {
                            break;
                        }
                        if (sFileName[j] != folders[i][j])
                        {
                            isIdentity = false;
                            break;
                        }
                    }
                    if (isIdentity)
                    {
                        SearchedFolders.Add(folders[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < folders.Length; ++i)
                {
                    if (folders[i].Length == sFileName.Length)
                    {
                        bool isIdentity = true;
                        for (int j = 0; j < sFileName.Length; ++j)
                        {
                            if (sFileName[j] != folders[i][j])
                            {
                                isIdentity = false;
                                break;
                            }
                        }
                        if (isIdentity)
                        {
                            SearchedFolders.Add(folders[i]);
                        }
                    }
                }
            }
            System.IO.StreamWriter fileT = new System.IO.StreamWriter(sWrite, false);
            fileT.Close();
            for (int i = 0; i < SearchedFolders.Count; ++i)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(sWrite, true))
                {
                    file.WriteLine(SearchedFolders[i].ToString());
                }
            }
            return SearchedFolders;
        }


        

        private void Button1_Click(object sender, System.EventArgs e)
        {
            files = Directory.GetFiles(textBox2.Text);
            folders = Directory.GetDirectories(textBox2.Text);

            // Getting template file name
            string sFileName = textBox1.Text;
            if (sFileName == "")
            {
                sFileName = "*";
            }

            // Clearing filepath from files strings
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, textBox2.Text.Length);
                for (int j = 0; files[i][j] == '\\'; ++j)
                {
                    files[i] = files[i].Remove(0, 1);
                }
            }
            // Clearing filepath from folders strings
            for (int i = 0; i < folders.Length; ++i)
            {
                folders[i] = folders[i].Remove(0, textBox2.Text.Length);
                for (int j = 0; folders[i][j] == '\\'; ++j)
                {
                    folders[i] = folders[i].Remove(0, 1);
                }
            }

            // Threads count
            int iCountOfFilesThreads = int.Parse(comboBox1.SelectedItem.ToString()) / 2;
            int iCountOfFoldersThreads = int.Parse(comboBox1.SelectedItem.ToString()) / 2;
            while (folders.Length < iCountOfFoldersThreads)
            {
                --iCountOfFoldersThreads;
            }
            while (files.Length < iCountOfFilesThreads)
            {
                --iCountOfFilesThreads;
            }

            T.Clear();
            // Creating files and threads for folders
            for (int i = 0; i < iCountOfFoldersThreads; ++i)
            {
                // Creating files
                string sFilesWrite = "Folders";
                sFilesWrite += comboBox1.SelectedItem.ToString() + "-";
                sFilesWrite += i.ToString() + ".txt";
                string[] folderstemp;
                int iDec = folders.Length / iCountOfFoldersThreads;
                if (folders.Length % iCountOfFoldersThreads != 0 && i == iCountOfFoldersThreads - 1)
                {
                    folderstemp = new string[iDec + folders.Length % iCountOfFoldersThreads];
                    Array.Copy(folders, i * iDec, folderstemp, 0, iDec + folders.Length % iCountOfFoldersThreads);
                }
                else
                {
                    folderstemp = new string[iDec];
                    Array.Copy(folders, i * iDec, folderstemp, 0, iDec);
                }

                // Starting threads
                T.Add(new Thread(() => SearchFolders(folderstemp, sFileName, sFilesWrite)));
                T[T.Count - 1].Start();
            }

            // Creating files and threads for files
            for (int i = 0; i < iCountOfFilesThreads; ++i)
            {
                // Creating files
                string sFilesWrite = "Files";
                sFilesWrite += comboBox1.SelectedItem.ToString() + "-";
                sFilesWrite += i.ToString() + ".txt";
                string[] filestemp;
                int iDec = files.Length / iCountOfFilesThreads;
                if (files.Length % iCountOfFilesThreads != 0 && i == iCountOfFilesThreads - 1)
                {
                    filestemp = new string[iDec + files.Length % iCountOfFilesThreads];
                    Array.Copy(files, i * iDec, filestemp, 0, iDec + files.Length % iCountOfFilesThreads);
                }
                else
                {
                    filestemp = new string[iDec];
                    Array.Copy(files, i * iDec, filestemp, 0, iDec);
                }

                // Starting threads
                T.Add(new Thread(() => SearchFiles(filestemp, sFileName, sFilesWrite)));
                T[T.Count - 1].Start();
            }
        }


        // Choosing folder path; get files and directories from choosed path
        private void Button2_Click(object sender, System.EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            int iCountOfThreads = int.Parse(comboBox1.SelectedItem.ToString()) / 2;

            richTextBox1.Clear();
            richTextBox1.Text += "Folders:\n";
            bool isExist = false;
            for (int i = 0; i < iCountOfThreads; ++i)
            {
                string sFilesName = "Folders";
                sFilesName += comboBox1.SelectedItem.ToString() + "-";
                sFilesName += i.ToString() + ".txt";
                if (File.Exists(sFilesName))
                {
                    if (!isExist)
                    {
                        isExist = true;
                    }
                    StreamReader sr = new StreamReader(sFilesName);
                    string line = sr.ReadLine();

                    //Continue to read until you reach end of file
                    while (line != null)
                    {
                        richTextBox1.Text += line + '\n';
                        line = sr.ReadLine();
                    }
                }
            }

            richTextBox1.Text += "\nFiles:\n";
            for (int i = 0; i < iCountOfThreads; ++i)
            {
                string sFilesName = "Files";
                sFilesName += comboBox1.SelectedItem.ToString() + "-";
                sFilesName += i.ToString() + ".txt";
                if (File.Exists(sFilesName))
                {
                    if (!isExist)
                    {
                        isExist = true;
                    }
                    StreamReader sr = new StreamReader(sFilesName);
                    string line = sr.ReadLine();

                    //Continue to read until you reach end of file
                    while (line != null)
                    {
                        richTextBox1.Text += line + '\n';
                        line = sr.ReadLine();
                    }
                }
            }
            if (!isExist)
            {
                MessageBox.Show("Please, firstly start searching by " + iCountOfThreads * 2 + " threads");
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            int iThread = int.Parse(comboBox2.SelectedItem.ToString()) - 1;
            int iPriority = comboBox3.SelectedIndex;
            if (T.Count <= iThread)
            {
                MessageBox.Show("Firstly, start threading");
                return;
            }
            if (!T[iThread].IsAlive)
            {
                MessageBox.Show("Thread is ended working, use this option when threading is in process");
                return;
            }
            switch (iPriority)
            {
                case 0:
                    T[iThread].Priority = ThreadPriority.Highest;
                    break;
                case 1:
                    T[iThread].Priority = ThreadPriority.AboveNormal;
                    break;
                case 2:
                    T[iThread].Priority = ThreadPriority.Normal;
                    break;
                case 3:
                    T[iThread].Priority = ThreadPriority.BelowNormal;
                    break;
                case 4:
                    T[iThread].Priority = ThreadPriority.Lowest;
                    break;
                default:
                    break;
            }
            
        }




        // Protection from incorrect template
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                if (textBox1.Text[0] == '.')
                {
                    textBox1.Text = textBox1.Text.Remove(0, 1);
                }
                int iStarPos = 0;
                bool isStar = false;
                for (int i = 0; i < textBox1.Text.Length; ++i)
                {
                    if (textBox1.Text[i] == '*')
                    {
                        iStarPos = i;
                        isStar = true;
                        break;
                    }
                }
                if (isStar && textBox1.Text.Length - 1 > iStarPos)
                {
                    textBox1.Text = textBox1.Text.Remove(iStarPos + 1);
                }
            }
        }
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '/' &&
                e.KeyChar != '\\' &&
                e.KeyChar != ':' &&
                e.KeyChar != '?' &&
                e.KeyChar != '"' &&
                e.KeyChar != '<' &&
                e.KeyChar != '>' &&
                e.KeyChar != '|')
                return;
            else
                e.Handled = true;
        }
    }
}