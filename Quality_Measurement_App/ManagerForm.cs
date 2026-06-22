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
            Size = new Size(820, 560);
            BackColor = Color.FromArgb(235, 238, 243);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Panel cardPanel = new Panel
            {
                Size = new Size(460, 380),
                Location = new Point((ClientSize.Width - 460) / 2, 70),
                BackColor = Color.White
            };
            Controls.Add(cardPanel);

            Label titleLabel = new Label
            {
                Text = "MANAGER PANEL",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(380, 42),
                Location = new Point(40, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(titleLabel);

            Label subtitleLabel = new Label
            {
                Text = "Quality measurement system management",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(120, 130, 140),
                Size = new Size(380, 25),
                Location = new Point(40, 78),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(subtitleLabel);

            Button addModelButton = CreateMenuButton("Add New Model", 125);
            Button manageCriteriaButton = CreateMenuButton("Manage Inspection Criteria", 185);
            Button viewRecordsButton = CreateMenuButton("View Measurement Records", 245);
            Button backButton = CreateMenuButton("Back to Start", 305);

            backButton.BackColor = Color.FromArgb(52, 58, 64);

            cardPanel.Controls.Add(addModelButton);
            cardPanel.Controls.Add(manageCriteriaButton);
            cardPanel.Controls.Add(viewRecordsButton);
            cardPanel.Controls.Add(backButton);

            addModelButton.Click += (sender, e) =>
            {
                MessageBox.Show("Add Model screen will open here.");
            };

            manageCriteriaButton.Click += (sender, e) =>
            {
                MessageBox.Show("Criteria management screen will open here.");
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
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(360, 44),
                Location = new Point(50, y),
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