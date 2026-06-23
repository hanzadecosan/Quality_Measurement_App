using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class AddCriteriaForm : Form
    {
        private class ModelItem
        {
            public int ModelID { get; set; }
            public string ModelName { get; set; }

            public override string ToString()
            {
                return ModelName;
            }
        }

        public AddCriteriaForm()
        {
            InitializeComponent();
            BuildAddCriteriaScreen();
        }

        private void BuildAddCriteriaScreen()
        {
            Controls.Clear();

            Text = "Add Criteria";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            Label titleLabel = new Label
            {
                Text = "ADD INSPECTION CRITERIA",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(900, 70),
                Location = new Point(510, 70),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(titleLabel);

            Label modelLabel = CreateLabel("Model", 420, 180);
            ComboBox modelComboBox = CreateComboBox(420, 220, 500);
            LoadModelsFromDatabase(modelComboBox);

            Label stepLabel = CreateLabel("Step No", 1000, 180);
            TextBox stepTextBox = CreateTextBox(1000, 220, 220);

            Label criteriaNameLabel = CreateLabel("Criteria Name", 420, 300);
            TextBox criteriaNameTextBox = CreateTextBox(420, 340, 500);

            Label inputTypeLabel = CreateLabel("Input Type", 1000, 300);
            ComboBox inputTypeComboBox = CreateComboBox(1000, 340, 300);
            inputTypeComboBox.Items.Add("Numeric");
            inputTypeComboBox.Items.Add("Dropdown");
            inputTypeComboBox.Items.Add("YesNo");
            inputTypeComboBox.Items.Add("Text");
            inputTypeComboBox.SelectedIndex = 0;

            Label checkMethodLabel = CreateLabel("Check Method", 1340, 300);
            ComboBox checkMethodComboBox = CreateComboBox(1340, 340, 300);
            checkMethodComboBox.Items.Add("NumericMin");
            checkMethodComboBox.Items.Add("NumericRange");
            checkMethodComboBox.Items.Add("RecordOnly");
            checkMethodComboBox.Items.Add("Option");
            checkMethodComboBox.SelectedIndex = 0;

            Label descriptionLabel = CreateLabel("Description", 420, 420);
            TextBox descriptionTextBox = CreateTextBox(420, 460, 1220);
            descriptionTextBox.Multiline = true;
            descriptionTextBox.Height = 90;

            Label targetLabel = CreateLabel("Target Value", 420, 590);
            TextBox targetTextBox = CreateTextBox(420, 630, 250);

            Label lowerLabel = CreateLabel("Lower Limit / Min", 700, 590);
            TextBox lowerTextBox = CreateTextBox(700, 630, 250);

            Label upperLabel = CreateLabel("Upper Limit / Max", 980, 590);
            TextBox upperTextBox = CreateTextBox(980, 630, 250);

            Label unitLabel = CreateLabel("Unit", 1260, 590);
            TextBox unitTextBox = CreateTextBox(1260, 630, 180);

            Label optionsLabel = CreateLabel("Options", 420, 710);
            TextBox optionsTextBox = CreateTextBox(420, 750, 500);
            optionsTextBox.PlaceholderText = "Example: OK;NOK or Yes;No";

            Label imagePathLabel = CreateLabel("Image Path", 1000, 710);
            TextBox imagePathTextBox = CreateTextBox(1000, 750, 640);
            imagePathTextBox.PlaceholderText = @"Images\v28_step01.png";

            Button saveButton = new Button
            {
                Text = "Save Criteria",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Size = new Size(360, 60),
                Location = new Point(900, 870),
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
                Size = new Size(260, 55),
                Location = new Point(1280, 872),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            backButton.FlatAppearance.BorderSize = 0;
            Controls.Add(backButton);

            saveButton.Click += (sender, e) =>
            {
                ModelItem selectedModel = modelComboBox.SelectedItem as ModelItem;

                if (selectedModel == null)
                {
                    MessageBox.Show("Please select a model.");
                    return;
                }

                if (!int.TryParse(stepTextBox.Text.Trim(), out int stepNo))
                {
                    MessageBox.Show("Please enter a valid step number.");
                    return;
                }

                string criteriaName = criteriaNameTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(criteriaName))
                {
                    MessageBox.Show("Please enter criteria name.");
                    return;
                }

                decimal? targetValue = ParseNullableDecimal(targetTextBox.Text);
                decimal? lowerLimit = ParseNullableDecimal(lowerTextBox.Text);
                decimal? upperLimit = ParseNullableDecimal(upperTextBox.Text);

                bool saved = SaveCriteriaToDatabase(
                    selectedModel.ModelID,
                    stepNo,
                    criteriaName,
                    descriptionTextBox.Text.Trim(),
                    inputTypeComboBox.Text,
                    checkMethodComboBox.Text,
                    targetValue,
                    lowerLimit,
                    upperLimit,
                    unitTextBox.Text.Trim(),
                    optionsTextBox.Text.Trim(),
                    imagePathTextBox.Text.Trim()
                );

                if (saved)
                {
                    MessageBox.Show("Criteria saved successfully.");

                    stepTextBox.Clear();
                    criteriaNameTextBox.Clear();
                    descriptionTextBox.Clear();
                    targetTextBox.Clear();
                    lowerTextBox.Clear();
                    upperTextBox.Clear();
                    unitTextBox.Clear();
                    optionsTextBox.Clear();
                    imagePathTextBox.Clear();
                }
            };

            backButton.Click += (sender, e) =>
            {
                ManagerForm managerForm = new ManagerForm();
                managerForm.Show();
                Close();
            };
        }

        private Label CreateLabel(string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(500, 35),
                Location = new Point(x, y)
            };

            Controls.Add(label);
            return label;
        }

        private TextBox CreateTextBox(int x, int y, int width)
        {
            TextBox textBox = new TextBox
            {
                Font = new Font("Segoe UI", 14),
                Size = new Size(width, 45),
                Location = new Point(x, y)
            };

            Controls.Add(textBox);
            return textBox;
        }

        private ComboBox CreateComboBox(int x, int y, int width)
        {
            ComboBox comboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 14),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(width, 45),
                Location = new Point(x, y)
            };

            Controls.Add(comboBox);
            return comboBox;
        }

        private decimal? ParseNullableDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            text = text.Replace(".", ",");

            if (decimal.TryParse(text, out decimal value))
                return value;

            MessageBox.Show("Invalid decimal value: " + text);
            return null;
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

        private bool SaveCriteriaToDatabase(
            int modelId,
            int stepNo,
            string criteriaName,
            string description,
            string inputType,
            string checkMethod,
            decimal? targetValue,
            decimal? lowerLimit,
            decimal? upperLimit,
            string unit,
            string options,
            string imagePath)
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO dbo.InspectionCriteria
                        (ModelID, StepNo, CriteriaName, Description, InputType, CheckMethod,
                         TargetValue, LowerLimit, UpperLimit, Unit, Options, ImagePath)
                        VALUES
                        (@ModelID, @StepNo, @CriteriaName, @Description, @InputType, @CheckMethod,
                         @TargetValue, @LowerLimit, @UpperLimit, @Unit, @Options, @ImagePath);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ModelID", modelId);
                        command.Parameters.AddWithValue("@StepNo", stepNo);
                        command.Parameters.AddWithValue("@CriteriaName", criteriaName);
                        command.Parameters.AddWithValue("@Description",
                            string.IsNullOrWhiteSpace(description) ? DBNull.Value : description);
                        command.Parameters.AddWithValue("@InputType", inputType);
                        command.Parameters.AddWithValue("@CheckMethod", checkMethod);

                        command.Parameters.AddWithValue("@TargetValue",
                            targetValue.HasValue ? targetValue.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@LowerLimit",
                            lowerLimit.HasValue ? lowerLimit.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@UpperLimit",
                            upperLimit.HasValue ? upperLimit.Value : DBNull.Value);

                        command.Parameters.AddWithValue("@Unit",
                            string.IsNullOrWhiteSpace(unit) ? DBNull.Value : unit);
                        command.Parameters.AddWithValue("@Options",
                            string.IsNullOrWhiteSpace(options) ? DBNull.Value : options);
                        command.Parameters.AddWithValue("@ImagePath",
                            string.IsNullOrWhiteSpace(imagePath) ? DBNull.Value : imagePath);

                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Criteria could not be saved:\n\n" + ex.Message);
                return false;
            }
        }
    }
}