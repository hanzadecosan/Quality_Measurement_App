using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class AddModelForm : Form
    {
        public AddModelForm()
        {
            InitializeComponent();
            BuildAddModelScreen();
        }

        private void BuildAddModelScreen()
        {
            Controls.Clear();

            Text = "Add Model";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            Label titleLabel = new Label
            {
                Text = "ADD NEW MODEL",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(700, 70),
                Location = new Point(610, 150),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(titleLabel);

            Label modelNameLabel = new Label
            {
                Text = "Model Name",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(500, 35),
                Location = new Point(710, 290)
            };
            Controls.Add(modelNameLabel);

            TextBox modelNameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 15),
                Size = new Size(500, 45),
                Location = new Point(710, 330)
            };
            Controls.Add(modelNameTextBox);

            Label descriptionLabel = new Label
            {
                Text = "Description",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(500, 35),
                Location = new Point(710, 410)
            };
            Controls.Add(descriptionLabel);

            TextBox descriptionTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 15),
                Size = new Size(500, 90),
                Location = new Point(710, 450),
                Multiline = true
            };
            Controls.Add(descriptionTextBox);

            Button saveButton = new Button
            {
                Text = "Save Model",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Size = new Size(500, 60),
                Location = new Point(710, 580),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderSize = 0;
            Controls.Add(saveButton);

            Button backButton = new Button
            {
                Text = "Back",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Size = new Size(500, 50),
                Location = new Point(710, 660),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            backButton.FlatAppearance.BorderSize = 0;
            Controls.Add(backButton);

            saveButton.Click += (sender, e) =>
            {
                string modelName = modelNameTextBox.Text.Trim();
                string description = descriptionTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(modelName))
                {
                    MessageBox.Show("Please enter model name.");
                    return;
                }

                bool saved = SaveModelToDatabase(modelName, description);

                if (saved)
                {
                    MessageBox.Show("Model saved successfully.");

                    ManagerForm managerForm = new ManagerForm();
                    managerForm.Show();
                    Close();
                }
            };

            backButton.Click += (sender, e) =>
            {
                ManagerForm managerForm = new ManagerForm();
                managerForm.Show();
                Close();
            };
        }

        private bool SaveModelToDatabase(string modelName, string description)
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO dbo.Models (ModelName, Description)
                        VALUES (@ModelName, @Description);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ModelName", modelName);
                        command.Parameters.AddWithValue("@Description",
                            string.IsNullOrWhiteSpace(description) ? DBNull.Value : description);

                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Model could not be saved:\n\n" + ex.Message);
                return false;
            }
        }
    }
}