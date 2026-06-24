using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
            BuildAddUserScreen();
        }

        private void BuildAddUserScreen()
        {
            Controls.Clear();

            Text = "Kullanıcı Ekle";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            Label titleLabel = new Label
            {
                Text = "YENİ KULLANICI",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(700, 70),
                Location = new Point(610, 160),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(titleLabel);

            Label nameLabel = new Label
            {
                Text = "Ad Soyad",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(500, 35),
                Location = new Point(710, 290)
            };
            Controls.Add(nameLabel);

            TextBox fullNameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 15),
                Size = new Size(500, 45),
                Location = new Point(710, 330)
            };
            Controls.Add(fullNameTextBox);

            Label roleLabel = new Label
            {
                Text = "Yetki",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(500, 35),
                Location = new Point(710, 410)
            };
            Controls.Add(roleLabel);

            ComboBox roleComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 15),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(500, 45),
                Location = new Point(710, 450)
            };
            roleComboBox.Items.Add("Operator");
            roleComboBox.Items.Add("Manager");
            roleComboBox.SelectedIndex = 0;
            Controls.Add(roleComboBox);

            Button saveButton = new Button
            {
                Text = "Kaydet",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Size = new Size(500, 60),
                Location = new Point(710, 550),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderSize = 0;
            Controls.Add(saveButton);

            Button backButton = new Button
            {
                Text = "Geri",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Size = new Size(500, 50),
                Location = new Point(710, 630),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            backButton.FlatAppearance.BorderSize = 0;
            Controls.Add(backButton);

            saveButton.Click += (sender, e) =>
            {
                string fullName = fullNameTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    MessageBox.Show("Lütfen ad soyad giriniz");
                    return;
                }

                int roleId = roleComboBox.Text == "Yönetici" ? 1 : 0;

                SaveUserToDatabase(fullName, roleId);

                fullNameTextBox.Clear();
                roleComboBox.SelectedIndex = 0;
                ManagerForm managerForm = new ManagerForm();
                managerForm.Show();
                this.Close();

            };

            backButton.Click += (sender, e) =>
            {
                ManagerForm managerForm = new ManagerForm();
                managerForm.Show();
                Close();
            };
        }

        private void SaveUserToDatabase(string fullName, int roleId)
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO dbo.Users (FullName, RoleID)
                        VALUES (@FullName, @RoleID);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", fullName);
                        command.Parameters.AddWithValue("@RoleID", roleId);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("User saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("User could not be saved:\n\n" + ex.Message);
            }
        }
    }
}