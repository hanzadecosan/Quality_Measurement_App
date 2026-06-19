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
            StartPosition = FormStartPosition.CenterScreen; //general settings of the form
            Size = new Size(820, 620);
            BackColor = Color.FromArgb(235, 238, 243);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Panel cardPanel = new Panel
            {
                Size = new Size(460, 470),
                Location = new Point((ClientSize.Width - 460) / 2, 35),
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

            Label sampleGroupLabel = new Label
            {
                Text = "Sample group",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(380, 24),
                Location = new Point(40, 315)
            };
            cardPanel.Controls.Add(sampleGroupLabel);

            ComboBox sampleGroupComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(180, 32),
                Location = new Point(40, 342)
            };
            sampleGroupComboBox.Items.Add("First Of");
            sampleGroupComboBox.Items.Add("Last Of");
            sampleGroupComboBox.SelectedIndex = 0;
            cardPanel.Controls.Add(sampleGroupComboBox);

            Label sampleNoLabel = new Label
            {
                Text = "Sample no",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(180, 24),
                Location = new Point(240, 315)
            };
            cardPanel.Controls.Add(sampleNoLabel);

            ComboBox sampleNoComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(180, 32),
                Location = new Point(240, 342)
            };
            sampleNoComboBox.Items.Add("1");
            sampleNoComboBox.Items.Add("2");
            sampleNoComboBox.Items.Add("3");
            sampleNoComboBox.SelectedIndex = 0;
            cardPanel.Controls.Add(sampleNoComboBox);

            Button continueButton = new Button
            {
                Text = "Start Session",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(380, 46),
                Location = new Point(40, 405),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            continueButton.FlatAppearance.BorderSize = 0;

            continueButton.Click += (sender, e) => // get the selected user and the selected model and direct to measurement form,next step
            {
                UserItem selectedUser = userComboBox.SelectedItem as UserItem;
                ModelItem selectedModel = modelComboBox.SelectedItem as ModelItem;
                string selectedSampleGroup = sampleGroupComboBox.Text;
                int selectedSampleNo = Convert.ToInt32(sampleNoComboBox.Text);

                if (selectedUser == null || selectedModel == null)
                {
                    MessageBox.Show("Please select a user and a model.");
                    return;
                }

                MeasurementForm measurementForm = new MeasurementForm(
     selectedUser.UserID,
     selectedUser.FullName,
     selectedModel.ModelID,
     selectedModel.ModelName,
     selectedSampleGroup,
     selectedSampleNo
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
                Location = new Point(0, 545),
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