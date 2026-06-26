using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.Logging;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;


namespace Quality_Measurement_App
{
    public partial class LoginForm : Form
    {
        
        private class UserItem
        {
            public int UserID { get; set; }
            public string FullName { get; set; }
            public int RoleID { get; set; }
            public string DisplayText { get; set; }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        public LoginForm()
        {
            InitializeComponent();
            BuildLoginScreen();
            this.ControlBox = false;
        }

        private void BuildLoginScreen()
        {
            Controls.Clear();
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            Text = "Kullanıcı Seçiniz";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(760, 470);
            BackColor = Color.FromArgb(235, 238, 243);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Panel cardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            Controls.Add(cardPanel);

            Label titleLabel = new Label
            {
                Text = "KALİTE KONTROL",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(700, 70),
                Location = new Point(610, 150),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(titleLabel);

            Label subtitleLabel = new Label
            {
                Text = "Devam etmek için kullanıcı seçin",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(120, 130, 140),
                Size = new Size(700, 40),
                Location = new Point(610, 225),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(subtitleLabel);

            Label userLabel = new Label
            {
                Text = "Kullanıcı seçimi",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(500, 45),
                Location = new Point(710,380)
            };
            cardPanel.Controls.Add(userLabel);

            ComboBox userComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(500, 55),
               Location = new Point(710, 450),
            };
            cardPanel.Controls.Add(userComboBox);

            LoadUsersFromDatabase(userComboBox);

            Button continueButton = new Button
            {
                Text = "Devam Et",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(500, 60),
                Location = new Point(710, 500),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            continueButton.FlatAppearance.BorderSize = 0;
            cardPanel.Controls.Add(continueButton);

            Button exitButton = new Button
            {
                Text = "Çıkış",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(500, 60),
                BackColor = Color.FromArgb(173, 216, 230),
                Location = new Point(710, 580),
                ForeColor = Color.FromArgb(30, 30, 30),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            cardPanel.Controls.Add(exitButton);

            exitButton.FlatAppearance.BorderSize = 0;

            PictureBox leftLogo = new PictureBox
            {
                Image = Image.FromFile(Path.Combine(Application.StartupPath, "Logo", "companyLogo.jpg")),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(300,140),
                Location = new Point(30, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.White
            };

            Controls.Add(leftLogo);
            leftLogo.BringToFront();

            

            PictureBox rightLogo = new PictureBox
            {
                Image = Image.FromFile(Path.Combine(Application.StartupPath, "Logo", "centro-motion.png")),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(300,140),
                Location = new Point(200, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.White
            };

            Controls.Add(rightLogo);
            rightLogo.BringToFront();



            continueButton.Click += (sender, e) =>
            {
                UserItem selectedUser = userComboBox.SelectedItem as UserItem;

                if (selectedUser == null)
                {
                    MessageBox.Show("Kullanıcı seçiniz.");
                    return;
                }

                if (selectedUser.RoleID == 1)
                {
                    ManagerForm managerForm = new ManagerForm();
                    managerForm.Show();
                    Hide();
                }
                else
                {
                    OperatorStartForm operatorStartForm = new OperatorStartForm(
                        selectedUser.UserID,
                        selectedUser.FullName
                    );

                    operatorStartForm.Show();
                    Hide();
                }
            };
            exitButton.Click += (s, e) =>
            {
                DialogResult result = MessageBox.Show(
                    "Uygulamayı kapatmak istediğinize emin misiniz?",
                    "Exit Application",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };

            Label footerLabel = new Label
            {
                Text = "v1.0  •  Internship Project",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(150, 155, 160),
                Size = new Size(1920, 30),
                Location = new Point(0, 980),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(footerLabel);
        }

        private void LoadUsersFromDatabase(ComboBox comboBox)
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            comboBox.Items.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT UserID, FullName, RoleID FROM dbo.Users";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int userId = Convert.ToInt32(reader["UserID"]);
                        string fullName = reader["FullName"].ToString();
                        int roleId = Convert.ToInt32(reader["RoleID"]);

                        string roleName = roleId == 0 ? "Operatör" : "Admin";

                        comboBox.Items.Add(new UserItem
                        {
                            UserID = userId,
                            FullName = fullName,
                            RoleID = roleId,
                            DisplayText = fullName + "   •   " + roleName
                        });
                    }
                }
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }
    }
}