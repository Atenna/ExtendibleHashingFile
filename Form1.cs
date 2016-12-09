using System;
using System.Windows.Forms;
using ExtendibleHashingFile.DataStructure;

namespace ExtendibleHashingFile
{
    public partial class Form1 : Form
    {
        public ExtendibleHashing<TestClass> Hashing;
        public Form1()
        {
            InitializeComponent();
            Hashing = new ExtendibleHashing<TestClass>("test17.txt", 2);
            GenerateInsertions(Hashing);
        }

        private void buttonAdd_Click(object sender, System.EventArgs e)
        {
            int key = int.Parse(textBoxKey.Text);
            int data = int.Parse(textBoxData.Text);
            var t = new TestClass();
            t.Key = key;
            t.Data = data;
            if (Hashing.Add(t))
            {
                labelMessage.Text = "Successfully added";
            }
        }

        private void GenerateInsertions(ExtendibleHashing<TestClass> hashing)
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                var t = new TestClass();
                t.Key = rnd.Next(0, 50);
                t.Data = 1;
                hashing.Add(t);
            }
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            Hashing.PrintDirectory();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            int key = int.Parse(textBoxKey.Text);
            var t = new TestClass();
            t.Key = key;
            labelMessage.Text = Hashing.SearchKey(t);
        }
    }
}

// sekvencny vypis suboru, nie podla adresara
