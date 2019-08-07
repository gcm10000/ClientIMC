using System;
using System.Windows.Forms;

namespace ClientIMC
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            new Form1().Close();
            InitializeComponent();
            listBox1.Items.Add("123");
            listBox1.Items.Add("1235");
            listBox1.Items.Add("123100");
            listBox1.Items.Add("1231020");
        }

        private void AdicionarAmigoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFriend addFriend = new AddFriend();
            addFriend.Show();
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(listBox1.SelectedItem.ToString());
        }
    }
}
