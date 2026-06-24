using Microsoft.Data.SqlClient;
using System;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

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

        private Label targetLabel, positiveToleranceLabel, negativeToleranceLabel, lowerLabel, upperLabel, unitLabel, optionsLabel;
        private TextBox targetTextBox, positiveToleranceTextBox, negativeToleranceTextBox, lowerTextBox, upperTextBox, unitTextBox;
        private ComboBox checkMethodComboBox; private ComboBox optionsComboBox;



        private const int LEFT = 200;       
        private const int COL_W = 320;      
        private const int GAP = 20;         
       

        public AddCriteriaForm()
        {
            InitializeComponent();
            BuildAddCriteriaScreen();
        }

        private void BuildAddCriteriaScreen()
        {
            Controls.Clear();

            Text = "Kriter ekle";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            int c1 = LEFT;
            int c2 = LEFT + COL_W + GAP;        
            int c3 = LEFT + (COL_W + GAP) * 2;  
            int c4 = LEFT + (COL_W + GAP) * 3;  

            // Başlık 
            Label titleLabel = new Label
            {
                Text = "YENİ KRİTER EKLE",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(900, 70),
                Location = new Point(c1, 50),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(titleLabel);

            // ── Satır 1: Model | Step No
            
            CreateLabel("Model", c1, 150);
            ComboBox modelComboBox = CreateComboBox(c1, 190, COL_W * 2 + GAP);  
            LoadModelsFromDatabase(modelComboBox);

            CreateLabel("Kaçıncı Adım?", c3, 150);
            TextBox stepTextBox = CreateTextBox(c3, 190, COL_W);

            
            CreateLabel("Kriterin Adı", c1, 270);
            TextBox criteriaNameTextBox = CreateTextBox(c1, 310, COL_W);

            CreateLabel("Ölçüm türü", c2, 270);

            ComboBox inputTypeComboBox = CreateComboBox(c2, 310, COL_W);
            inputTypeComboBox.Items.Add("Numeric");
            inputTypeComboBox.Items.Add("Dropdown");
            inputTypeComboBox.Items.Add("YesNo");
            inputTypeComboBox.Items.Add("Text");
            inputTypeComboBox.SelectedIndex = 0;

            CreateLabel("Kontrol metodu", c3, 270);
            checkMethodComboBox = CreateComboBox(c3, 310, COL_W);

            // ── Satır 3: Description
            CreateLabel("Açıklama Metni", c1, 390);
            TextBox descriptionTextBox = CreateTextBox(c1, 430, COL_W * 3 + GAP * 2);  
            descriptionTextBox.Multiline = true;
            descriptionTextBox.Height = 80;

            
            targetLabel = CreateLabel("İstenen değer", c1, 550);
            targetTextBox = CreateTextBox(c1, 590, COL_W);

            
            lowerLabel = CreateLabel("Minimum değer", c2, 550);
            lowerTextBox = CreateTextBox(c2, 590, COL_W);

            upperLabel = CreateLabel("Üst Limit", c3, 550);
            upperTextBox = CreateTextBox(c3, 590, COL_W);


            unitLabel = CreateLabel("Birim", c4, 550);
            unitTextBox = CreateTextBox(c4, 590, 160);
            unitLabel.BringToFront();
            unitTextBox.BringToFront();

            // ── Satır 5: Options | Image 
            optionsLabel = CreateLabel("Seçenekler", c1, 690);

            optionsComboBox = CreateComboBox(c1, 730, COL_W * 2 + GAP);

            optionsComboBox.Items.Add("OK;NOK");
            optionsComboBox.Items.Add("Yes;No");
            optionsComboBox.Items.Add("Pass;Fail");
            optionsComboBox.Items.Add("Good;Bad");
            optionsComboBox.SelectedIndex = 0;

            CreateLabel("Image", c3, 690);

            // Image: dropdown + Browse butonu yan yana
            ComboBox imageComboBox = CreateComboBox(c3, 730, COL_W);
            imageComboBox.Items.Add("— Resim Yok —");
            LoadImagesFromFolder(imageComboBox);
            imageComboBox.SelectedIndex = 0;

            Button browseButton = new Button
            {
                Text = "Ara...",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(120, 45),
                Location = new Point(c3 + COL_W + GAP, 730),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            browseButton.FlatAppearance.BorderSize = 0;
            Controls.Add(browseButton);

            browseButton.Click += (sender, e) =>
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Resim dosyası seçiniz";
                    dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
                    dlg.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string imagesFolder =
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                        if (!Directory.Exists(imagesFolder))
                            Directory.CreateDirectory(imagesFolder);

                        string fileName = Path.GetFileName(dlg.FileName);

                        string destinationPath =
                            Path.Combine(imagesFolder, fileName);

                        if (dlg.FileName.Equals(destinationPath,
    StringComparison.OrdinalIgnoreCase))
                        {
                            // zaten Images klasöründe
                        }
                        else
                        {
                            File.Copy(
                                dlg.FileName,
                                destinationPath,
                                true);
                        }

                        string relativePath =
                            Path.Combine("Images", fileName);

                        if (!imageComboBox.Items.Contains(relativePath))
                            imageComboBox.Items.Add(relativePath);

                        imageComboBox.SelectedItem = relativePath;
                    }
                }
            };

            // ── Butonlar
            Button saveButton = new Button
            {
                Text = "Kriteri kaydet",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Size = new Size(320, 60),
                Location = new Point(c2, 840),
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
                Size = new Size(200, 60),
                Location = new Point(c2 + 320 + GAP, 840),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            backButton.FlatAppearance.BorderSize = 0;
            Controls.Add(backButton);

            // ── Event handler
            inputTypeComboBox.SelectedIndexChanged += (sender, e) =>
            {
                UpdateFormForInputType(inputTypeComboBox.Text);
            };

            checkMethodComboBox.SelectedIndexChanged += (sender, e) =>
            {
                UpdateToleranceFields(checkMethodComboBox.Text);
            };

            UpdateFormForInputType(inputTypeComboBox.Text);

            // ── Save 
            saveButton.Click += (sender, e) =>
            {
                ModelItem selectedModel = modelComboBox.SelectedItem as ModelItem;

                if (selectedModel == null)
                {
                    MessageBox.Show("Lütfen model seçiniz", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(stepTextBox.Text.Trim(), out int stepNo) || stepNo <= 0)
                {
                    MessageBox.Show("Geçerli adım sayısı giriniz (positif sayı).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string criteriaName = criteriaNameTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(criteriaName))
                {
                    MessageBox.Show("Kriterin adını giriniz", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string inputType = inputTypeComboBox.Text;
                string checkMethod = checkMethodComboBox.Text;

                if ((inputType == "Dropdown" || inputType == "YesNo") && string.IsNullOrWhiteSpace(optionsComboBox.Text))

                    if (inputType == "Numeric")
                {
                    if (checkMethod == "NumericRange" &&
                        (string.IsNullOrWhiteSpace(lowerTextBox.Text) || string.IsNullOrWhiteSpace(upperTextBox.Text)))
                    {
                        MessageBox.Show("Please enter both Lower Limit and Upper Limit for NumericRange.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (checkMethod == "NumericMin" && string.IsNullOrWhiteSpace(lowerTextBox.Text))
                    {
                        MessageBox.Show("Please enter Lower Limit / Min for NumericMin.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (StepNoExists(selectedModel.ModelID, stepNo))
                {
                    MessageBox.Show($"Step No {stepNo} already exists for this model.", "Duplicate Step", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal? targetValue = targetTextBox.Visible ? ParseNullableDecimal(targetTextBox.Text) : null;

                decimal? positiveTolerance = null;
                decimal? negativeTolerance = null;
                decimal? lowerLimit = null;
                decimal? upperLimit = null;

                if (checkMethod == "NumericRange")
                {
                    positiveTolerance = ParseNullableDecimal(lowerTextBox.Text);
                    negativeTolerance = ParseNullableDecimal(upperTextBox.Text);

                    positiveTolerance ??= 0;
                    negativeTolerance ??= 0;

                    lowerLimit = targetValue.Value - negativeTolerance.Value;
                    upperLimit = targetValue.Value + positiveTolerance.Value;
                }
                else if (checkMethod == "NumericMin")
                {
                    lowerLimit = ParseNullableDecimal(lowerTextBox.Text);
                }
                if (checkMethod == "NumericRange")
                {
                    if (!targetValue.HasValue)
                    {
                        MessageBox.Show("Please enter Target Value for NumericRange.");
                        return;
                    }

                    positiveTolerance ??= 0;
                    negativeTolerance ??= 0;

                    lowerLimit = targetValue.Value - negativeTolerance.Value;
                    upperLimit = targetValue.Value + positiveTolerance.Value;
                }

                string selectedImage = imageComboBox.SelectedIndex == 0 ? "" : imageComboBox.Text;

                bool saved = SaveCriteriaToDatabase(
    selectedModel.ModelID,
    stepNo,
    criteriaName,
    descriptionTextBox.Text.Trim(),
    inputType,
    checkMethod,
    targetValue,
    positiveTolerance,
    negativeTolerance,
    lowerLimit,
    upperLimit,
    unitTextBox.Visible ? unitTextBox.Text.Trim() : "",
    optionsComboBox.Visible ? optionsComboBox.Text.Trim() : "",
    selectedImage
);

                if (saved)
                {
                    MessageBox.Show("Kriter başarıyla kaydedildi!.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    stepTextBox.Clear();
                    criteriaNameTextBox.Clear();
                    descriptionTextBox.Clear();
                    targetTextBox.Clear();
                    lowerTextBox.Clear();
                    upperTextBox.Clear();
                    unitTextBox.Clear();
                    optionsComboBox.SelectedIndex = 0;
                    imageComboBox.SelectedIndex = 0;
                }
            };

            backButton.Click += (sender, e) =>
            {
                ManagerForm managerForm = new ManagerForm();
                managerForm.Show();
                Close();
            };
        }

        // ── Images dropdowna yükle
        private void LoadImagesFromFolder(ComboBox comboBox)
        {
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            if (!Directory.Exists(imagesFolder))
                return;

            string[] files = Directory.GetFiles(imagesFolder);
            Array.Sort(files);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                comboBox.Items.Add(Path.Combine("Images", fileName));
            }
        }

        // ── visiblity ayarla
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
                    targetLabel.Text = "Hedef Değer";
                    lowerLabel.Text = "+ Tolerans";
                    upperLabel.Text = "- Tolerans";

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
                    lowerLabel.Text = "Min Değer";
                    lowerLabel.Visible = true;
                    lowerTextBox.Visible = true;

                    unitLabel.Visible = true;
                    unitTextBox.Visible = true;
                    break;

                case "RecordOnly":
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

            MessageBox.Show($"Invalid numeric value: \"{text}\"", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        // ── Duplicate StepNo kontrolü 
        private bool StepNoExists(int modelId, int stepNo)
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) \r\nFROM dbo.InspectionCriteria \r\nWHERE ModelID = @ModelID \r\n  AND StepNo = @StepNo\r\n  AND IsActive = 1";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ModelID", modelId);
                        command.Parameters.AddWithValue("@StepNo", stepNo);
                        return Convert.ToInt32(command.ExecuteScalar()) > 0;
                    }
                }
            }
            catch { return false; }
        }

        // ── Model yükleme 
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

        // ── Kaydetme 
        private bool SaveCriteriaToDatabase(
    int modelId,
    int stepNo,
    string criteriaName,
    string description,
    string inputType,
    string checkMethod,
    decimal? targetValue,
    decimal? positiveTolerance,
    decimal? negativeTolerance,
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
 TargetValue, PositiveTolerance, NegativeTolerance, LowerLimit, UpperLimit, Unit, Options, ImagePath)
VALUES
(@ModelID, @StepNo, @CriteriaName, @Description, @InputType, @CheckMethod,
 @TargetValue, @PositiveTolerance, @NegativeTolerance, @LowerLimit, @UpperLimit, @Unit, @Options, @ImagePath);";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ModelID", modelId);
                        command.Parameters.AddWithValue("@StepNo", stepNo);
                        command.Parameters.AddWithValue("@CriteriaName", criteriaName);
                        command.Parameters.AddWithValue("@Description",
                            string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description);
                        command.Parameters.AddWithValue("@InputType", inputType);
                        command.Parameters.AddWithValue("@CheckMethod", checkMethod);
                        command.Parameters.AddWithValue("@TargetValue",
                            targetValue.HasValue ? (object)targetValue.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@PositiveTolerance",
                    positiveTolerance.HasValue ? (object)positiveTolerance.Value : DBNull.Value);

                        command.Parameters.AddWithValue("@NegativeTolerance",
                            negativeTolerance.HasValue ? (object)negativeTolerance.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@LowerLimit",
                            lowerLimit.HasValue ? (object)lowerLimit.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@UpperLimit",
                            upperLimit.HasValue ? (object)upperLimit.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Unit",
                            string.IsNullOrWhiteSpace(unit) ? (object)DBNull.Value : unit);
                        command.Parameters.AddWithValue("@Options",
    string.IsNullOrWhiteSpace(options) ? (object)DBNull.Value : options);
                        command.Parameters.AddWithValue("@ImagePath",
                            string.IsNullOrWhiteSpace(imagePath) ? (object)DBNull.Value : imagePath);
                       

                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kayıt Başarısız:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
