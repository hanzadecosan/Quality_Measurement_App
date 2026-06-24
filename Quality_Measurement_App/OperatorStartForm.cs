using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class OperatorStartForm : Form
    {
        private readonly int selectedUserId;
        private readonly string selectedUserName;

        private class ModelItem
        {
            public int ModelID { get; set; }
            public string ModelName { get; set; }

            public override string ToString()
            {
                return ModelName;
            }
        }

        public OperatorStartForm(int userId, string userName)
        {
            InitializeComponent();

            selectedUserId = userId;
            selectedUserName = userName;

            BuildOperatorStartScreen();
        }

        private void BuildOperatorStartScreen()
        {
            Controls.Clear();

            Text = "Operatör başlangıcı";
          
StartPosition = FormStartPosition.CenterScreen;
WindowState = FormWindowState.Maximized;

BackColor = Color.FromArgb(235, 238, 243);

            Panel cardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White

            };
            cardPanel.Left = (ClientSize.Width - cardPanel.Width) / 2;
            cardPanel.Top = (ClientSize.Height - cardPanel.Height) / 2;
            Controls.Add(cardPanel);


            Label titleLabel = new Label
            {
                Text = "OPERATÖR PANELİ",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(620, 60),
                Location = new Point(650, 120),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(titleLabel);

            Label operatorLabel = new Label
            {
                Text = "Operatör: " + selectedUserName,
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(120, 130, 140),
                Size = new Size(620, 30),
                Location = new Point(650, 190),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(operatorLabel);

            Label shiftLabel = new Label
            {
                Text = "Vardiya: " + GetCurrentShift(),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(31, 87, 145),
                Size = new Size(500, 45),
                Location = new Point(710, 240),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(shiftLabel);

            Label dateTimeLabel = new Label
            {
                Text = DateTime.Now.ToString("dd.MM.yyyy | HH:mm"),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(120, 130, 140),
                
              Size = new Size(500, 25),
                Location = new Point(710, 300),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(dateTimeLabel);

            Label modelLabel = new Label
            {
                Text = "Model seçiniz",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(380, 24),
                Location = new Point(710, 390)
            };
            cardPanel.Controls.Add(modelLabel);

            ComboBox modelComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 13),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(500, 45),
                Location = new Point(710, 425)
            };
            cardPanel.Controls.Add(modelComboBox);
            LoadModelsFromDatabase(modelComboBox);

            Label sampleGroupLabel = new Label
            {
                Text = "Numune grubu",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(180, 24),
                Location = new Point(710, 510)
            };
            cardPanel.Controls.Add(sampleGroupLabel);

            ComboBox sampleGroupComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 13),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(240, 45),
                Location = new Point(710, 560)
            };
            sampleGroupComboBox.Items.Add("First Of");
            sampleGroupComboBox.Items.Add("Last Of");
            sampleGroupComboBox.SelectedIndex = 0;
            cardPanel.Controls.Add(sampleGroupComboBox);

            Label sampleNoLabel = new Label
            {
                Text = "Kaçıncı numune",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(180, 24),
                Location = new Point(970, 510)
            };
            cardPanel.Controls.Add(sampleNoLabel);

            ComboBox sampleNoComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 13),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(240, 45),
                Location = new Point(970, 560)
            };
            sampleNoComboBox.Items.Add("1");
            sampleNoComboBox.Items.Add("2");
            sampleNoComboBox.Items.Add("3");
            sampleNoComboBox.SelectedIndex = 0;
            cardPanel.Controls.Add(sampleNoComboBox);

            Button startButton = new Button
            {
                Text = "Ölçüme Başla",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(500, 55),
                Location = new Point(710, 650),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            startButton.FlatAppearance.BorderSize = 0;
            cardPanel.Controls.Add(startButton);

            Button backButton = new Button
            {
                Text = "Geri",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(500, 45),
                Location = new Point(710, 720),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            backButton.FlatAppearance.BorderSize = 0;
            cardPanel.Controls.Add(backButton);

            startButton.Click += (sender, e) =>
            {
                ModelItem selectedModel = modelComboBox.SelectedItem as ModelItem;

                if (selectedModel == null)
                {
                    MessageBox.Show("Lütfen model seçiniz");
                    return;
                }

                MeasurementForm measurementForm = new MeasurementForm(
                    selectedUserId,
                    selectedUserName,
                    selectedModel.ModelID,
                    selectedModel.ModelName,
                    sampleGroupComboBox.Text,
                    Convert.ToInt32(sampleNoComboBox.Text)
                );

                measurementForm.Show();
                Hide();
            };

            backButton.Click += (sender, e) =>
            {
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                Close();
            };
        }

        private void LoadModelsFromDatabase(ComboBox comboBox)
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            comboBox.Items.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ModelID, ModelName FROM dbo.Models ORDER BY ModelName";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comboBox.Items.Add(new ModelItem
                        {
                            ModelID = Convert.ToInt32(reader["ModelID"]),
                            ModelName = reader["ModelName"].ToString()
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
                return "Morning Shift";

            if (now >= new TimeSpan(16, 0, 0) && now <= new TimeSpan(23, 59, 59))
                return "Noon Shift";

            if (now >= new TimeSpan(0, 0, 0) && now <= new TimeSpan(7, 59, 59))
                return "Night Shift";

            return "Out of Shift";
        }
    }
}