namespace Quality_Measurement_App
{
    partial class mainForm
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
            button1 = new Button();
            cmbUsers = new ComboBox();
            Next = new Button();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(97, 320);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
           
            // 
            // cmbUsers
            // 
            cmbUsers.FormattingEnabled = true;
            cmbUsers.Location = new Point(215, 201);
            cmbUsers.Name = "cmbUsers";
            cmbUsers.Size = new Size(225, 28);
            cmbUsers.TabIndex = 1;
            // 
            // Next
            // 
            Next.Location = new Point(546, 320);
            Next.Name = "Next";
            Next.Size = new Size(94, 29);
            Next.TabIndex = 2;
            Next.Text = "button2";
            Next.UseVisualStyleBackColor = true;
            Next.Click += button2_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(cmbUsers);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(Next);
            panel1.Location = new Point(53, 40);
            panel1.Name = "panel1";
            panel1.Size = new Size(705, 386);
            panel1.TabIndex = 3;
            // 
            // mainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Name = "mainForm";
            Text = "mainForm";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private ComboBox cmbUsers;
        private Button Next;
        private Panel panel1;
    }
}