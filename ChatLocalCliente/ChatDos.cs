using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatLocalCliente
{
    public partial class ChatDos : Form
    {
        private Client c;
        public ChatDos()
        {
            InitializeComponent();
        }

        public async void ChatForm_Load(object sender, EventArgs e)
        {
            try
            {
                c = new Client("localhost", 4404);
                await c.Start();
                Task.Run(() => ListenForMessages());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar al servidor: {ex.Message}");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text;
            c.SendMessage(message, "userPrueba"); 
            txtMessage.Clear();
        }

        private async Task ListenForMessages() 
        {
            while (true)
            {
                var receivedMessage = await c.Received();
                AppendMessage(receivedMessage);
            }
        }
        private void AppendMessage(Message message)
        {
            if (txtMessages.InvokeRequired)
            {
                txtMessages.Invoke(new Action(() => AppendMessage(message)));
            }
            else
            {
                txtMessages.AppendText($"{message.userFrom}: {message.msg}{Environment.NewLine}");
            }
        }
    }
}
