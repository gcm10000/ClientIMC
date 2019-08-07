using System;

namespace ClientIMC
{
    class Chat
    {
        private string fromEmail;
        private string toEmail;
        private Protocol.Command command;
        private string message;
        private DateTime date;
        private bool isSend;

        public string FromEmail { get => fromEmail; set => fromEmail = value; }
        public string ToEmail { get => toEmail; set => toEmail = value; }
        public string Message { get => message; set => message = value; }
        public Protocol.Command Command { get => command; set => command = value; }
        public DateTime Date { get => date; set => date = value; }
        public bool IsSend { get => isSend; set => isSend = value; }
    }
    class Protocol
    {
        public enum Command
        {
            Invalid = 190,
            OK = 200,
            RequestFriendship = 210,
            AcceptFriendship = 220,
            RecusedFriendship = 230,
            Maintenance = 500
        }
    }
}
