using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serialization;

namespace ChatLocalCliente
{
    public partial class ChatDos : Form
    {
        private Client c;
        public string UserAlias { get; set; }


        public ChatDos()
        {
            InitializeComponent();
        }

        private async void ChatForm_Load(object sender, EventArgs e)
        {
            c = new Client("localhost", 4404);
            await c.Start(this.UserAlias); // Espera a que se conecte el cliente
            Task.Run(() => ListenForMessages()); // Inicia la escucha de mensajes
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMessage.Text; // Obtén el mensaje del TextBox
            if (!string.IsNullOrWhiteSpace(msg))
            {
                c.SendMessage(msg, this.UserAlias); // userAlias es el alias del usuario
                txtMessage.Clear(); // Limpia el TextBox después de enviar
            }
        }

        private async Task ListenForMessages()
        {
            Message received = await c.Received();
            MessageBox.Show("Debbug: LLEGA..");
            string msg = received.userFrom + ": " + received.msg;
            UpdateMessageBox(msg);

        }
        private void UpdateMessageBox(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateMessageBox), message);
                return;
            }

            txtMessages.AppendText(message + Environment.NewLine); 
        }
    }
}
