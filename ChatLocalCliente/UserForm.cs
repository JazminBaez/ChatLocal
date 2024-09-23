using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace ChatLocalCliente
{
    public partial class UserForm : Form
    {
        public UserForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtJoin_Click(object sender, EventArgs e)
        {
            string alias = txtAlias.Text; 
            ChatDos chatForm = new ChatDos();
            chatForm.UserAlias = alias;
            chatForm.Show();
            this.Hide();
        }
    }
}
