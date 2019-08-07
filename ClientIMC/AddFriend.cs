using System;
using System.Windows.Forms;

namespace ClientIMC
{
    public partial class AddFriend : Form
    {
        ClientSocket clientSocket;

        public AddFriend()
        {

            clientSocket = StaticClientSocket.Client;

            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            RequestFriendShip();
            this.Close();
        }

        private void RequestFriendShip()
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                clientSocket.SendRequestFriendShip(textBox1.Text);
                MessageBox.Show("Solicitação enviada", "Mensagem");
            }
        }
    }
}
