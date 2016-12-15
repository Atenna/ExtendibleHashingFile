using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ExtendibleHashingFile.DataStructure;
using ExtendibleHashingFile.Model;

namespace ExtendibleHashingFile
{
    public partial class Form1 : Form
    {
        public Table<TestClass> Hashing;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void buttonAdd_Click(object sender, System.EventArgs e)
        {
            int key = int.Parse(textBoxKey.Text);
            int data = int.Parse(textBoxData.Text);
            
        }

        private void GenerateInsertions(Table<TestClass> hashing)
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                
            }

            SearchInserted();
        }

        private void SearchInserted()
        {
            
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
           
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            
        }
    }
}

// sekvencny vypis suboru, nie podla adresara
