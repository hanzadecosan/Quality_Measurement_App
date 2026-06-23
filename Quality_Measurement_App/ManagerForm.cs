using System;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class ManagerForm : Form
    {
        public ManagerForm()
        {
            InitializeComponent();
            BuildManagerScreen();
        }

        private void BuildManagerScreen()
        {
            Controls.Clear();

            Text = "Manager Panel";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            

            Label titleLabel = new Label
            {
                Text = "MANAGER PANEL",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(620, 70),
                Location = new Point(650, 130),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(titleLabel);

            Label subtitleLabel = new Label
            {
                Text = "Quality measurement system management",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(120, 130, 140),
                Size = new Size(620, 35),
                Location = new Point(650, 205),
                TextAlign = ContentAlignment.MiddleCenter
            };
          Controls.Add(subtitleLabel);

            Button addModelButton = CreateMenuButton("Add New Model", 330);
            Button addUserButton = CreateMenuButton("Add New User", 420);
            Button manageCriteriaButton = CreateMenuButton("Add Inspection Criteria", 510);
            Button viewRecordsButton = CreateMenuButton("View Measurement Records", 600);
            Button backButton = CreateMenuButton("Back to Start", 710);

            backButton.BackColor = Color.FromArgb(52, 58, 64);

            Controls.Add(addModelButton);
            Controls.Add(addUserButton);
            Controls.Add(manageCriteriaButton);
            Controls.Add(viewRecordsButton);
            Controls.Add(backButton);

            addModelButton.Click += (sender, e) =>
            {
                AddModelForm addModelForm = new AddModelForm();
                addModelForm.Show();
                this.Hide();
            };
            addUserButton.Click += (sender, e) =>
            {
                AddUserForm addUserForm = new AddUserForm();
                addUserForm.Show();
                this.Hide();
            };

            manageCriteriaButton.Click += (sender, e) =>
            {
                manageCriteriaButton.Click += (sender, e) =>
                {
                    AddCriteriaForm addCriteriaForm = new AddCriteriaForm();
                    addCriteriaForm.Show();
                    Hide();
                };
            };

            viewRecordsButton.Click += (sender, e) =>
            {
                MessageBox.Show("Measurement records screen will open here.");
            };

            backButton.Click += (sender, e) =>
            {
                LoginForm loginform=new LoginForm();
                loginform.Show();
                Close();
            };
        }

        private Button CreateMenuButton(string text, int y)
        {
            Button button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Size = new Size(500, 65),
                Location = new Point(710, y),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }
    }
}