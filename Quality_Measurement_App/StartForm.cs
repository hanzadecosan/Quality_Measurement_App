using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class StartForm : Form
    {
        private class UserItem
        {
            public int UserID { get; set; }
            public string FullName { get; set; }
            public string DisplayText { get; set; }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        private class ModelItem
        {
            public int ModelID { get; set; }
            public string ModelName { get; set; }

            public override string ToString()
            {
                return ModelName;
            }
        }

        public StartForm()
        {
            InitializeComponent();
            BuildStartScreen();
        }
        private void BuildStartScreen()
        {
            Controls.Clear();

            Text = "Quality Control";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(820, 560);
            BackColor = Color.FromArgb(235, 238, 243);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Panel cardPanel = new Panel
            {
                Size = new Size(460, 390),
                Location = new Point((ClientSize.Width - 460) / 2, 55),
                BackColor = Color.White
            };
            Controls.Add(cardPanel);

            Label titleLabel = new Label
            {
                Text = "QUALITY CONTROL",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(380, 42),
                Location = new Point(40, 28)
            };
            cardPanel.Controls.Add(titleLabel);

            Label subtitleLabel = new Label
            {
                Text = "Production Measurement Station",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(120, 130, 140),
                Size = new Size(380, 25),
                Location = new Point(42, 70)
            };
            cardPanel.Controls.Add(subtitleLabel);

            string currentShift = GetCurrentShift();

            Label shiftLabel = new Label
            {
                Text = "Current shift: " + currentShift,
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(31, 87, 145),
                Size = new Size(380, 34),
                Location = new Point(40, 108),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(shiftLabel);


            Label dateTimeLabel = new Label
            {
                Text = DateTime.Now.ToString("dd.MM.yyyy | HH:mm"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(120, 130, 140),
                Size = new Size(380, 22),
                Location = new Point(40, 145),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(dateTimeLabel);

            Label userLabel = new Label
            {
                Text = "Select user",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(380, 24),
                Location = new Point(40, 165)
            };
            cardPanel.Controls.Add(userLabel);

            ComboBox userComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(380, 32),
                Location = new Point(40, 192)
            };
            cardPanel.Controls.Add(userComboBox);
            LoadUsersFromDatabase(userComboBox);

            Label modelLabel = new Label
            {
                Text = "Select model",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(380, 24),
                Location = new Point(40, 238)
            };
            cardPanel.Controls.Add(modelLabel);

            ComboBox modelComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(380, 32),
                Location = new Point(40, 265)
            };
            cardPanel.Controls.Add(modelComboBox);
            LoadModelsFromDatabase(modelComboBox);

            Button continueButton = new Button
            {
                Text = "Start Session",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(380, 46),
                Location = new Point(40, 322),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            continueButton.FlatAppearance.BorderSize = 0;

            continueButton.Click += (sender, e) =>
            {
                UserItem selectedUser = userComboBox.SelectedItem as UserItem;
                ModelItem selectedModel = modelComboBox.SelectedItem as ModelItem;

                if (selectedUser == null || selectedModel == null)
                {
                    MessageBox.Show("Please select a user and a model.");
                    return;
                }

                MeasurementForm measurementForm = new MeasurementForm(
                    selectedUser.UserID,
                    selectedUser.FullName,
                    selectedModel.ModelID,
                    selectedModel.ModelName
                );

                measurementForm.Show();
                Hide();
            };

            cardPanel.Controls.Add(continueButton);

            Label footerLabel = new Label
            {
                Text = "v1.0  •  Internship Project",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(150, 155, 160),
                Size = new Size(820, 25),
                Location = new Point(0, 475),
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

                        string roleName = roleId == 0 ? "Operator" : "Manager";

                        comboBox.Items.Add(new UserItem
                        {
                            UserID = userId,
                            FullName = fullName,
                            DisplayText = fullName + "   •   " + roleName
                        });
                    }
                }
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void LoadModelsFromDatabase(ComboBox comboBox)
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            comboBox.Items.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ModelID, ModelName FROM dbo.Models";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int modelId = Convert.ToInt32(reader["ModelID"]);
                        string modelName = reader["ModelName"].ToString();

                        comboBox.Items.Add(new ModelItem
                        {
                            ModelID = modelId,
                            ModelName = modelName
                        });
                    }
                }
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }
        private string GetCurrentShift()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (now >= new TimeSpan(8, 0, 0) && now < new TimeSpan(16, 0, 0))
                return "Morning  Shift";

            if (now >= new TimeSpan(16, 0, 0) && now <= new TimeSpan(23, 59, 59))
                return "Noon Shift";

            if (now >= new TimeSpan(0, 0, 0) && now <= new TimeSpan(7, 59, 59))
                return "Night Shift";

            return "Out of Shift";
        }

   
    }
}