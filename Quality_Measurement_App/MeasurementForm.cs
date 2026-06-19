using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Quality_Measurement_App
{

  
    public partial class MeasurementForm : Form
    {
        private readonly string selectedSampleGroup;
        private readonly int selectedSampleNo;
        private class CriteriaItem
        {
            public int CriteriaID { get; set; }
            public int StepNo { get; set; }
            public string CriteriaName { get; set; }
            public string Description { get; set; }
            public string InputType { get; set; }
            public string CheckMethod { get; set; }
            public decimal? TargetValue { get; set; }
            public decimal? LowerLimit { get; set; }
            public decimal? UpperLimit { get; set; }
            public string Unit { get; set; }
            public string Options { get; set; }
            public string ImagePath { get; set; }
        }
        private readonly int selectedUserId;
        private readonly string selectedUserName;
        private readonly int selectedModelId;
        private readonly string selectedModelName;

        private List<CriteriaItem> criteriaList = new List<CriteriaItem>();
        private int currentIndex = 0;
        private List<ResultItem> resultList = new List<ResultItem>();



        public MeasurementForm(
    int userId,
    string userName,
    int modelId,
    string modelName,
    string sampleGroup,
    int sampleNo)
        {
            InitializeComponent();

            selectedUserId = userId;
            selectedUserName = userName;
            selectedModelId = modelId;
            selectedModelName = modelName;
            selectedSampleGroup = sampleGroup;
            selectedSampleNo = sampleNo;

            LoadCriteriaList();
            ShowCurrentCriteria();
        }

        private void LoadCriteriaList()
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            criteriaList.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT CriteriaID, StepNo, CriteriaName, Description, InputType, CheckMethod,
                           TargetValue, LowerLimit, UpperLimit, Unit, Options, ImagePath
                    FROM dbo.InspectionCriteria
                    WHERE ModelID = @ModelID
                    ORDER BY StepNo";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ModelID", selectedModelId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            criteriaList.Add(new CriteriaItem
                            {
                                CriteriaID = Convert.ToInt32(reader["CriteriaID"]),
                                StepNo = Convert.ToInt32(reader["StepNo"]),
                                CriteriaName = reader["CriteriaName"].ToString(),
                                Description = reader["Description"] == DBNull.Value ? "" : reader["Description"].ToString(),
                                InputType = reader["InputType"].ToString(),
                                CheckMethod = reader["CheckMethod"].ToString(),
                                TargetValue = reader["TargetValue"] == DBNull.Value ? null : Convert.ToDecimal(reader["TargetValue"]),
                                LowerLimit = reader["LowerLimit"] == DBNull.Value ? null : Convert.ToDecimal(reader["LowerLimit"]),
                                UpperLimit = reader["UpperLimit"] == DBNull.Value ? null : Convert.ToDecimal(reader["UpperLimit"]),
                                Unit = reader["Unit"] == DBNull.Value ? "" : reader["Unit"].ToString(),
                                Options = reader["Options"] == DBNull.Value ? "" : reader["Options"].ToString(),
                                ImagePath = reader["ImagePath"] == DBNull.Value ? "" : reader["ImagePath"].ToString()
                            });
                        }
                    }
                }
            }
        }

        private void ShowCurrentCriteria()
        {
            Controls.Clear();

            Text = "Measurement Session";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(980, 650);
            BackColor = Color.FromArgb(245, 247, 250);

            if (criteriaList.Count == 0)
            {
                MessageBox.Show("No criteria found for this model.");
                Close();
                return;
            }

            CriteriaItem item = criteriaList[currentIndex];

            Label titleLabel = new Label
            {
                Text = "MEASUREMENT SESSION",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(500, 40),
                Location = new Point(35, 25)
            };
            Controls.Add(titleLabel);

            Label infoLabel = new Label
            {
                Text = $"Operator: {selectedUserName}   |   Model: {selectedModelName}   |   Step {currentIndex + 1}/{criteriaList.Count}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(90, 100, 110),
                Size = new Size(850, 30),
                Location = new Point(38, 65)
            };
            Controls.Add(infoLabel);

            Label criteriaLabel = new Label
            {
                Text = $"{item.StepNo}. {item.CriteriaName}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 87, 145),
                Size = new Size(850, 35),
                Location = new Point(40, 115)
            };
            Controls.Add(criteriaLabel);

            Label limitLabel = new Label
            {
                Text = BuildLimitText(item),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(850, 30),
                Location = new Point(42, 155)
            };
            Controls.Add(limitLabel);

            PictureBox tutorialImage = new PictureBox
            {
                Size = new Size(430, 280),
                Location = new Point(40, 205),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            Controls.Add(tutorialImage);

            if (!string.IsNullOrWhiteSpace(item.ImagePath) && File.Exists(item.ImagePath))
            {
                tutorialImage.Image = Image.FromFile(item.ImagePath);
            }

            Label descriptionLabel = new Label
            {
                Text = item.Description,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(80, 90, 100),
                Size = new Size(430, 70),
                Location = new Point(40, 500)
            };
            Controls.Add(descriptionLabel);

            Label inputLabel = new Label
            {
                Text = "Measurement input",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(300, 24),
                Location = new Point(530, 220)
            };
            Controls.Add(inputLabel);

            Control inputControl = CreateInputControl(item);
            inputControl.Location = new Point(530, 250);
            inputControl.Size = new Size(280, 34);
            Controls.Add(inputControl);

            Label resultLabel = new Label
            {
                Text = "Result: -",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 80, 90),
                Size = new Size(330, 35),
                Location = new Point(530, 360)
            };
            Controls.Add(resultLabel);

            Button checkButton = new Button
            {
                Text = "Check Measurement",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(280, 44),
                Location = new Point(530, 300),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            checkButton.FlatAppearance.BorderSize = 0;
            Controls.Add(checkButton);

            Button previousButton = new Button
            {
                Text = "Previous Step",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Size = new Size(135, 44),
                Location = new Point(530, 420),
                BackColor = Color.FromArgb(255, 193, 7), // Sarı
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            previousButton.FlatAppearance.BorderSize = 0;
            Controls.Add(previousButton);
            if (currentIndex == 0)
            {
                previousButton.Visible = false;
            }

            Button nextButton = new Button
            {
                Text = currentIndex == criteriaList.Count - 1 ? "Finish" : "Next Step",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(135, 44),
                Location = new Point(670, 420),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            nextButton.FlatAppearance.BorderSize = 0;
            Controls.Add(nextButton);

            checkButton.Click += (sender, e) =>
            {
                string status = CheckMeasurement(item, inputControl);

                if (status == "INVALID")
                {
                    MessageBox.Show("Please enter/select a valid value.");
                    return;
                }

                if (status == "OK")
                {
                    resultLabel.Text = "Result: OK";
                    resultLabel.ForeColor = Color.FromArgb(25, 135, 84);
                    nextButton.Enabled = true;
                }
                else
                {
                    resultLabel.Text = "Result: NOK";
                    resultLabel.ForeColor = Color.FromArgb(220, 53, 69);
                    MessageBox.Show("Cannot continue session. Call the supervisor");
                    nextButton.Enabled = false;
                }
                resultList.RemoveAll(r => r.StepNo == item.StepNo);

                resultList.Add(new ResultItem
                {
                    StepNo = item.StepNo,
                    CriteriaName = item.CriteriaName,
                    EnteredValue = GetInputValue(inputControl),
                    Status = status
                });

            };

            nextButton.Click += (sender, e) =>
            {
                if (currentIndex < criteriaList.Count - 1)
                {
                    currentIndex++;
                    ShowCurrentCriteria();
                }
                else
                {
                    ResultsForm resultsForm = new ResultsForm(
    selectedUserId,
    selectedUserName,
    selectedModelId,
    selectedModelName,
    selectedSampleGroup,
    selectedSampleNo,
    resultList
);

                    resultsForm.Show();
                    this.Close();
                }
            };
            previousButton.Click += (sender, e) =>
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                    ShowCurrentCriteria();
                }


            };

        }

        private string BuildLimitText(CriteriaItem item)
        {
            if (item.CheckMethod == "NumericRange")
            {
                if (item.LowerLimit == item.UpperLimit)
                    return $"Target: {item.TargetValue} {item.Unit}";

                return $"Range: {item.LowerLimit} - {item.UpperLimit} {item.Unit}";
            }

            if (item.CheckMethod == "NumericMin")
                return $"Minimum: {item.LowerLimit} {item.Unit}";

            if (item.CheckMethod == "RecordOnly")
                return $"Target: {item.TargetValue} {item.Unit}";

            if (item.CheckMethod == "Option")
                return $"Options: {item.Options}";

            return "";
        }

        private Control CreateInputControl(CriteriaItem item)
        {
            if (item.InputType == "Dropdown" || item.InputType == "YesNo")
            {
                ComboBox comboBox = new ComboBox
                {
                    Font = new Font("Segoe UI", 11),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                string[] options = item.Options.Split(';');
                foreach (string option in options)
                    comboBox.Items.Add(option.Trim());

                if (comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;

                return comboBox;
            }

            return new TextBox
            {
                Font = new Font("Segoe UI", 12)
            };
        }

        private string CheckMeasurement(CriteriaItem item, Control inputControl)
        {

            string inputText = "";

            if (inputControl is TextBox textBox)
                inputText = textBox.Text.Trim();

            if (inputControl is ComboBox comboBox)
                inputText = comboBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(inputText))
                return "INVALID";

            if (item.InputType == "Numeric")
            {
                if (!decimal.TryParse(inputText, out decimal value))
                    return "INVALID";



                if (item.CheckMethod == "NumericRange")
                {
                    if (item.LowerLimit == null || item.UpperLimit == null)
                        return "INVALID";

                    return value >= item.LowerLimit && value <= item.UpperLimit ? "OK" : "NOK";
                }

                if (item.CheckMethod == "NumericMin")
                {
                    if (item.LowerLimit == null)
                        return "INVALID";

                    return value >= item.LowerLimit ? "OK" : "NOK";
                }

                if (item.CheckMethod == "RecordOnly")
                    return "OK";


            }

            if (item.CheckMethod == "Option")
            {
                return inputText.Equals("OK", StringComparison.OrdinalIgnoreCase) ||
                       inputText.Equals("Yes", StringComparison.OrdinalIgnoreCase)
                    ? "OK"
                    : "NOK";
            }

            return "OK";
        }
        private string GetInputValue(Control inputControl)
        {
            if (inputControl is TextBox textBox)
                return textBox.Text.Trim();

            if (inputControl is ComboBox comboBox)
                return comboBox.Text.Trim();

            return "";
        }
    }
}