using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class ManageCriteriaForm : Form
    {
        private class ModelItem
        {
            public int ModelID { get; set; }
            public string ModelName { get; set; }
            public override string ToString() => ModelName;
        }

        private class CriteriaItem
        {
            public int CriteriaID { get; set; }
            public int StepNo { get; set; }
            public string CriteriaName { get; set; }
            public override string ToString() => $"Step {StepNo} - {CriteriaName}";
        }

        private ComboBox modelComboBox, criteriaComboBox, inputTypeComboBox, checkMethodComboBox, optionsComboBox, imageComboBox;
        private TextBox stepTextBox, criteriaNameTextBox, descriptionTextBox;
        private TextBox targetTextBox, lowerTextBox, upperTextBox, unitTextBox;
        private Label targetLabel, lowerLabel, upperLabel, unitLabel, optionsLabel;

        private int selectedCriteriaId = 0;

        private const int LEFT = 200;
        private const int COL_W = 320;
        private const int GAP = 20;

        public ManageCriteriaForm()
        {
            InitializeComponent();
            BuildScreen();
        }

        private void BuildScreen()
        {
            Controls.Clear();

            Text = "Manage Criteria";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            int c1 = LEFT;
            int c2 = LEFT + COL_W + GAP;
            int c3 = LEFT + (COL_W + GAP) * 2;
            int c4 = LEFT + (COL_W + GAP) * 3;

            Label titleLabel = new Label
            {
                Text = "MANAGE INSPECTION CRITERIA",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(1000, 70),
                Location = new Point(c1, 45)
            };
            Controls.Add(titleLabel);

            CreateLabel("Model", c1, 135);
            modelComboBox = CreateComboBox(c1, 175, COL_W * 2 + GAP);
            LoadModels();

            CreateLabel("Criteria", c3, 135);
            criteriaComboBox = CreateComboBox(c3, 175, COL_W * 2 + GAP);

            modelComboBox.SelectedIndexChanged += (sender, e) =>
            {
                LoadCriteriaForSelectedModel();
            };

            criteriaComboBox.SelectedIndexChanged += (sender, e) =>
            {
                CriteriaItem selected = criteriaComboBox.SelectedItem as CriteriaItem;
                if (selected != null)
                    LoadCriteriaDetails(selected.CriteriaID);
            };

            CreateLabel("Step No", c1, 255);
            stepTextBox = CreateTextBox(c1, 295, COL_W);

            CreateLabel("Criteria Name", c2, 255);
            criteriaNameTextBox = CreateTextBox(c2, 295, COL_W);

            CreateLabel("Input Type", c3, 255);
            inputTypeComboBox = CreateComboBox(c3, 295, COL_W);
            inputTypeComboBox.Items.Add("Numeric");
            inputTypeComboBox.Items.Add("Dropdown");
            inputTypeComboBox.Items.Add("YesNo");
            inputTypeComboBox.Items.Add("Text");

            CreateLabel("Check Method", c4, 255);
            checkMethodComboBox = CreateComboBox(c4, 295, COL_W);

            CreateLabel("Description", c1, 370);
            descriptionTextBox = CreateTextBox(c1, 410, COL_W * 3 + GAP * 2);
            descriptionTextBox.Multiline = true;
            descriptionTextBox.Height = 80;

            targetLabel = CreateLabel("Target Value", c1, 535);
            targetTextBox = CreateTextBox(c1, 575, COL_W);

            lowerLabel = CreateLabel("Minimum Value", c2, 535);
            lowerTextBox = CreateTextBox(c2, 575, COL_W);

            upperLabel = CreateLabel("Upper Limit", c3, 535);
            upperTextBox = CreateTextBox(c3, 575, COL_W);

            unitLabel = CreateLabel("Unit", c4, 535);
            unitTextBox = CreateTextBox(c4, 575, 160);

            optionsLabel = CreateLabel("Options", c1, 675);
            optionsComboBox = CreateComboBox(c1, 715, COL_W * 2 + GAP);
            optionsComboBox.Items.Add("OK;NOK");
            optionsComboBox.Items.Add("Yes;No");
            optionsComboBox.Items.Add("Pass;Fail");
            optionsComboBox.Items.Add("Good;Bad");

            CreateLabel("Image", c3, 675);
            imageComboBox = CreateComboBox(c3, 715, COL_W);
            imageComboBox.Items.Add("— No image —");
            LoadImagesFromFolder();
            imageComboBox.SelectedIndex = 0;

            Button browseButton = new Button
            {
                Text = "Browse...",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(120, 45),
                Location = new Point(c3 + COL_W + GAP, 715),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            browseButton.FlatAppearance.BorderSize = 0;
            Controls.Add(browseButton);

            browseButton.Click += (sender, e) =>
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Select Image File";
                    dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
                    dlg.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;
                        string fullPath = dlg.FileName;

                        string relativePath = fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)
                            ? fullPath.Substring(basePath.Length)
                            : fullPath;

                        if (!imageComboBox.Items.Contains(relativePath))
                            imageComboBox.Items.Add(relativePath);

                        imageComboBox.SelectedItem = relativePath;
                    }
                }
            };

            Button updateButton = new Button
            {
                Text = "Update Criteria",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Size = new Size(300, 60),
                Location = new Point(c1, 835),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            updateButton.FlatAppearance.BorderSize = 0;
            Controls.Add(updateButton);

            Button deactivateButton = new Button
            {
                Text = "Deactivate",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Size = new Size(260, 60),
                Location = new Point(c1 + 320, 835),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            deactivateButton.FlatAppearance.BorderSize = 0;
            Controls.Add(deactivateButton);

            Button backButton = new Button
            {
                Text = "Back",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Size = new Size(220, 60),
                Location = new Point(c1 + 600, 835),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            backButton.FlatAppearance.BorderSize = 0;
            Controls.Add(backButton);

            inputTypeComboBox.SelectedIndexChanged += (sender, e) =>
            {
                UpdateFormForInputType(inputTypeComboBox.Text);
            };

            checkMethodComboBox.SelectedIndexChanged += (sender, e) =>
            {
                UpdateToleranceFields(checkMethodComboBox.Text);
            };

            updateButton.Click += (sender, e) =>
            {
                UpdateCriteria();
            };

            deactivateButton.Click += (sender, e) =>
            {
                DeactivateCriteria();
            };

            backButton.Click += (sender, e) =>
            {
                ManagerForm managerForm = new ManagerForm();
                managerForm.Show();
                Close();
            };

            if (modelComboBox.Items.Count > 0)
                modelComboBox.SelectedIndex = 0;
        }

        private void LoadCriteriaForSelectedModel()
        {
            criteriaComboBox.Items.Clear();
            selectedCriteriaId = 0;

            ModelItem selectedModel = modelComboBox.SelectedItem as ModelItem;
            if (selectedModel == null) return;

            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT CriteriaID, StepNo, CriteriaName
                    FROM dbo.InspectionCriteria
                    WHERE ModelID = @ModelID AND IsActive = 1
                    ORDER BY StepNo;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ModelID", selectedModel.ModelID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            criteriaComboBox.Items.Add(new CriteriaItem
                            {
                                CriteriaID = Convert.ToInt32(reader["CriteriaID"]),
                                StepNo = Convert.ToInt32(reader["StepNo"]),
                                CriteriaName = reader["CriteriaName"].ToString()
                            });
                        }
                    }
                }
            }

            if (criteriaComboBox.Items.Count > 0)
                criteriaComboBox.SelectedIndex = 0;
        }

        private void LoadCriteriaDetails(int criteriaId)
        {
            selectedCriteriaId = criteriaId;

            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM dbo.InspectionCriteria WHERE CriteriaID = @CriteriaID;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CriteriaID", criteriaId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stepTextBox.Text = reader["StepNo"].ToString();
                            criteriaNameTextBox.Text = reader["CriteriaName"].ToString();
                            descriptionTextBox.Text = reader["Description"] == DBNull.Value ? "" : reader["Description"].ToString();

                            inputTypeComboBox.Text = reader["InputType"].ToString();
                            UpdateFormForInputType(inputTypeComboBox.Text);

                            checkMethodComboBox.Text = reader["CheckMethod"].ToString();
                            UpdateToleranceFields(checkMethodComboBox.Text);

                            targetTextBox.Text = reader["TargetValue"] == DBNull.Value ? "" : reader["TargetValue"].ToString();
                            lowerTextBox.Text = "";

                            if (reader["PositiveTolerance"] != DBNull.Value && checkMethodComboBox.Text == "NumericRange")
                                lowerTextBox.Text = reader["PositiveTolerance"].ToString();
                            else if (reader["LowerLimit"] != DBNull.Value)
                                lowerTextBox.Text = reader["LowerLimit"].ToString();

                            upperTextBox.Text = "";

                            if (reader["NegativeTolerance"] != DBNull.Value && checkMethodComboBox.Text == "NumericRange")
                                upperTextBox.Text = reader["NegativeTolerance"].ToString();
                            else if (reader["UpperLimit"] != DBNull.Value)
                                upperTextBox.Text = reader["UpperLimit"].ToString();

                            unitTextBox.Text = reader["Unit"] == DBNull.Value ? "" : reader["Unit"].ToString();

                            string options = reader["Options"] == DBNull.Value ? "" : reader["Options"].ToString();
                            if (!string.IsNullOrWhiteSpace(options))
                            {
                                if (!optionsComboBox.Items.Contains(options))
                                    optionsComboBox.Items.Add(options);

                                optionsComboBox.SelectedItem = options;
                            }
                            else if (optionsComboBox.Items.Count > 0)
                            {
                                optionsComboBox.SelectedIndex = 0;
                            }

                            string imagePath = reader["ImagePath"] == DBNull.Value ? "" : reader["ImagePath"].ToString();
                            if (!string.IsNullOrWhiteSpace(imagePath))
                            {
                                if (!imageComboBox.Items.Contains(imagePath))
                                    imageComboBox.Items.Add(imagePath);

                                imageComboBox.SelectedItem = imagePath;
                            }
                            else
                            {
                                imageComboBox.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateCriteria()
        {
            if (selectedCriteriaId == 0)
            {
                MessageBox.Show("Please select a criteria.");
                return;
            }

            if (!int.TryParse(stepTextBox.Text.Trim(), out int stepNo) || stepNo <= 0)
            {
                MessageBox.Show("Please enter a valid step number.");
                return;
            }

            string inputType = inputTypeComboBox.Text;
            string checkMethod = checkMethodComboBox.Text;

            decimal? targetValue = targetTextBox.Visible ? ParseNullableDecimal(targetTextBox.Text) : null;
            decimal? positiveTolerance = null;
            decimal? negativeTolerance = null;
            decimal? lowerLimit = null;
            decimal? upperLimit = null;

            if (checkMethod == "NumericRange")
            {
                if (!targetValue.HasValue)
                {
                    MessageBox.Show("Please enter Target Value for NumericRange.");
                    return;
                }

                positiveTolerance = ParseNullableDecimal(lowerTextBox.Text) ?? 0;
                negativeTolerance = ParseNullableDecimal(upperTextBox.Text) ?? 0;

                lowerLimit = targetValue.Value - negativeTolerance.Value;
                upperLimit = targetValue.Value + positiveTolerance.Value;
            }
            else if (checkMethod == "NumericMin")
            {
                lowerLimit = ParseNullableDecimal(lowerTextBox.Text);
            }

            string imagePath = imageComboBox.SelectedIndex == 0 ? "" : imageComboBox.Text;

            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE dbo.InspectionCriteria
                        SET
                            StepNo = @StepNo,
                            CriteriaName = @CriteriaName,
                            Description = @Description,
                            InputType = @InputType,
                            CheckMethod = @CheckMethod,
                            TargetValue = @TargetValue,
                            PositiveTolerance = @PositiveTolerance,
                            NegativeTolerance = @NegativeTolerance,
                            LowerLimit = @LowerLimit,
                            UpperLimit = @UpperLimit,
                            Unit = @Unit,
                            Options = @Options,
                            ImagePath = @ImagePath
                        WHERE CriteriaID = @CriteriaID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CriteriaID", selectedCriteriaId);
                        command.Parameters.AddWithValue("@StepNo", stepNo);
                        command.Parameters.AddWithValue("@CriteriaName", criteriaNameTextBox.Text.Trim());
                        command.Parameters.AddWithValue("@Description",
                            string.IsNullOrWhiteSpace(descriptionTextBox.Text) ? (object)DBNull.Value : descriptionTextBox.Text.Trim());
                        command.Parameters.AddWithValue("@InputType", inputType);
                        command.Parameters.AddWithValue("@CheckMethod", checkMethod);
                        command.Parameters.AddWithValue("@TargetValue", targetValue.HasValue ? (object)targetValue.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@PositiveTolerance", positiveTolerance.HasValue ? (object)positiveTolerance.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@NegativeTolerance", negativeTolerance.HasValue ? (object)negativeTolerance.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@LowerLimit", lowerLimit.HasValue ? (object)lowerLimit.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@UpperLimit", upperLimit.HasValue ? (object)upperLimit.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Unit",
                            string.IsNullOrWhiteSpace(unitTextBox.Text) ? (object)DBNull.Value : unitTextBox.Text.Trim());
                        command.Parameters.AddWithValue("@Options",
                            optionsComboBox.Visible && !string.IsNullOrWhiteSpace(optionsComboBox.Text) ? (object)optionsComboBox.Text.Trim() : DBNull.Value);
                        command.Parameters.AddWithValue("@ImagePath",
                            string.IsNullOrWhiteSpace(imagePath) ? (object)DBNull.Value : imagePath);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Criteria updated successfully.");
                LoadCriteriaForSelectedModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed:\n\n" + ex.Message);
            }
        }

        private void DeactivateCriteria()
        {
            if (selectedCriteriaId == 0)
            {
                MessageBox.Show("Please select a criteria.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "This criteria will be deactivated. It will not appear in new measurements, but old records will stay valid.\n\nContinue?",
                "Confirm Deactivate",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    UPDATE dbo.InspectionCriteria
                    SET IsActive = 0
                    WHERE CriteriaID = @CriteriaID;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CriteriaID", selectedCriteriaId);
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Criteria deactivated.");
            LoadCriteriaForSelectedModel();
        }

        private void UpdateFormForInputType(string inputType)
        {
            checkMethodComboBox.Items.Clear();

            switch (inputType)
            {
                case "Numeric":
                    checkMethodComboBox.Items.Add("NumericMin");
                    checkMethodComboBox.Items.Add("NumericRange");
                    checkMethodComboBox.Items.Add("RecordOnly");
                    checkMethodComboBox.SelectedIndex = 0;
                    checkMethodComboBox.Enabled = true;
                    SetOptionsVisible(false);
                    break;

                case "Dropdown":
                case "YesNo":
                    checkMethodComboBox.Items.Add("Option");
                    checkMethodComboBox.SelectedIndex = 0;
                    checkMethodComboBox.Enabled = false;
                    SetNumericFieldsVisible(false);
                    SetOptionsVisible(true);
                    break;

                case "Text":
                    checkMethodComboBox.Items.Add("RecordOnly");
                    checkMethodComboBox.SelectedIndex = 0;
                    checkMethodComboBox.Enabled = false;
                    SetNumericFieldsVisible(false);
                    SetOptionsVisible(false);
                    break;
            }

            if (inputType == "Numeric")
                UpdateToleranceFields(checkMethodComboBox.Text);
        }

        private void UpdateToleranceFields(string checkMethod)
        {
            SetNumericFieldsVisible(false);

            switch (checkMethod)
            {
                case "NumericRange":
                    targetLabel.Text = "Target Value";
                    lowerLabel.Text = "+ Tolerance";
                    upperLabel.Text = "- Tolerance";

                    targetLabel.Visible = true;
                    targetTextBox.Visible = true;
                    lowerLabel.Visible = true;
                    lowerTextBox.Visible = true;
                    upperLabel.Visible = true;
                    upperTextBox.Visible = true;
                    unitLabel.Visible = true;
                    unitTextBox.Visible = true;
                    break;

                case "NumericMin":
                    lowerLabel.Text = "Minimum Value";
                    lowerLabel.Visible = true;
                    lowerTextBox.Visible = true;
                    unitLabel.Visible = true;
                    unitTextBox.Visible = true;
                    break;

                case "RecordOnly":
                    targetLabel.Text = "Target Value";
                    targetLabel.Visible = true;
                    targetTextBox.Visible = true;
                    unitLabel.Visible = true;
                    unitTextBox.Visible = true;
                    break;
            }
        }

        private void SetNumericFieldsVisible(bool visible)
        {
            targetLabel.Visible = visible;
            targetTextBox.Visible = visible;
            lowerLabel.Visible = visible;
            lowerTextBox.Visible = visible;
            upperLabel.Visible = visible;
            upperTextBox.Visible = visible;
            unitLabel.Visible = visible;
            unitTextBox.Visible = visible;
        }

        private void SetOptionsVisible(bool visible)
        {
            optionsLabel.Visible = visible;
            optionsComboBox.Visible = visible;
        }

        private void LoadModels()
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            modelComboBox.Items.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ModelID, ModelName FROM dbo.Models ORDER BY ModelName";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        modelComboBox.Items.Add(new ModelItem
                        {
                            ModelID = Convert.ToInt32(reader["ModelID"]),
                            ModelName = reader["ModelName"].ToString()
                        });
                    }
                }
            }
        }

        private void LoadImagesFromFolder()
        {
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            if (!Directory.Exists(imagesFolder))
                return;

            string[] files = Directory.GetFiles(imagesFolder, "*.png");
            Array.Sort(files);

            foreach (string file in files)
            {
                imageComboBox.Items.Add(Path.Combine("Images", Path.GetFileName(file)));
            }
        }

        private Label CreateLabel(string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(500, 35),
                Location = new Point(x, y)
            };
            Controls.Add(label);
            label.BringToFront();
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

            text = text.Trim().Replace(",", ".");

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
                return value;

            MessageBox.Show($"Invalid numeric value: \"{text}\"");
            return null;
        }
    }
}