using System;
using System.Windows.Forms;

namespace ClientIMC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if ((string.IsNullOrEmpty(textBox1.Text)) || (string.IsNullOrEmpty(textBox2.Text)))
            {
                MessageBox.Show("Campo(s) vazio(s)", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ClientSocket cs = new ClientSocket();
                cs.SendCredentials(textBox1.Text, textBox2.Text);

                if (cs.Connected())
                {
                    this.Hide();
                    var form2 = new Form2();
                    form2.Closed += (s, args) => this.Close();
                    form2.Show();
                }
                StaticClientSocket.Client = cs;
            }
        }
    }
}
