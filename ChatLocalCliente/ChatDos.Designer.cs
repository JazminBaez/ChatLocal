namespace ChatLocalCliente
{
    partial class ChatDos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtMessage = new TextBox();
            txtMessages = new TextBox();
            btnSend = new Button();
            SuspendLayout();
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(30, 413);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(670, 23);
            txtMessage.TabIndex = 0;
            // 
            // txtMessages
            // 
            txtMessages.BackColor = SystemColors.Control;
            txtMessages.Location = new Point(30, 48);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ReadOnly = true;
            txtMessages.Size = new Size(698, 323);
            txtMessages.TabIndex = 1;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(713, 416);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 2;
            btnSend.Text = "button1";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // ChatDos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSend);
            Controls.Add(txtMessages);
            Controls.Add(txtMessage);
            Name = "ChatDos";
            Text = "ChatDos";
            Load += ChatForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMessage;
        private TextBox txtMessages;
        private Button btnSend;
    }
}