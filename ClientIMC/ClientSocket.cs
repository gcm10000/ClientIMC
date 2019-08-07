using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ClientIMC
{
    class StaticClientSocket
    {
        private static ClientSocket client;

        internal static ClientSocket Client { get => client; set => client = value; }
    }

    class ClientSocket
    {
        private Socket client;
        private IPAddress[] ip;
        private EndPoint ep;
        private Credentials credentials;
        private byte[] ReceiveBuffer;
        
        private enum Command
        {
            Invalid = 190,
            OK = 200,
            Maintenance = 500
        }

        public ClientSocket()
        {
            InitializeClient();
        }

        private void InitializeClient()
        {
            try
            {
                ReceiveBuffer = new byte[1024];
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ip = Dns.GetHostAddresses("192.168.1.10");
                ep = new IPEndPoint(ip[0], 8222);
                client.BeginConnect(ep, new AsyncCallback(NewConnection), client);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "InitializeClient()", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void NewConnection(IAsyncResult AR)
        {
            try
            {
                client.EndConnect(AR);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "NewConnection()", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void ReceiveCallBack(IAsyncResult AR)
        {
            try
            {
                Socket _client = AR.AsyncState as Socket;
                int recv = _client.EndReceive(AR);
                byte[] data = new byte[recv];
                Array.Copy(ReceiveBuffer, data, recv);
                string message = Encoding.UTF8.GetString(data, 0, recv);

                Chat chat = JsonConvert.DeserializeObject<Chat>(message);
                if (chat.Command == Protocol.Command.RequestFriendship)
                {
                    if (System.Windows.Forms.MessageBox.Show("Solicitação de amizade de " + chat.FromEmail + "\n\r\n\rDeseja aceitar?", "Solicitação de amizade", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                    {

                        //Se sim, a solicitação de amizade tem de ser enviada ao servidor
                        //para adicionar cliente 1 na lista de amigos do cliente 2
                        chat.Command = Protocol.Command.AcceptFriendship;
                        string json = JsonConvert.SerializeObject(chat);
                        client.BeginSend(Encoding.UTF8.GetBytes(json), 0, json.Length, SocketFlags.None, new AsyncCallback(SendCallBack), client);
                    }
                    else
                    {
                        chat.Command = Protocol.Command.RecusedFriendship;
                        string json = JsonConvert.SerializeObject(chat);
                        client.BeginSend(Encoding.UTF8.GetBytes(json), 0, json.Length, SocketFlags.None, new AsyncCallback(SendCallBack), client);
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "ReceiveCallBack()", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                client.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), client);
            }
        }

        private void SendCallBack(IAsyncResult AR)
        {
            Socket _client = AR.AsyncState as Socket;
            try
            {
                _client.EndSend(AR);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "SendCallBack()", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public void SendCredentials(string user, string pass)
        {
            credentials = new Credentials()
            {
                Email = user,
                Password = pass
            };
            try
            {
                string json = JsonConvert.SerializeObject(credentials);
                client.BeginSend(Encoding.UTF8.GetBytes(json), 0, json.Length, SocketFlags.None, new AsyncCallback(SendCallBack), client);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "SendCredentials()", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public void SendRequestFriendShip(string email)
        {
            try
            {
                Friendship friendship = new Friendship();
                friendship.Email = email;
                string json = JsonConvert.SerializeObject(friendship);
                client.BeginSend(Encoding.UTF8.GetBytes(json), 0, json.Length, SocketFlags.None, new AsyncCallback(SendCallBack), client);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "SendRequestFriendShip()", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public bool Connected()
        {
            byte[] data = new byte[1024];
            int recv = client.Receive(data, data.Length, SocketFlags.None);
            string cmd = Encoding.UTF8.GetString(data, 0, recv);
            if ((Command)Enum.Parse(typeof(Command), cmd) == Command.OK)
            {
                client.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), client);
                return true;
            }
            return false;
        }
    }
}
