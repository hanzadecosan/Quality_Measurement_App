using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class ResultsForm : Form
    {

        private int userId;
        private string userName;
        private int modelId;
        private string modelName;
        private string sampleGroup;
        private int sampleNo;
        private List<ResultItem> results;

        public ResultsForm(
     int userId,
     string userName,
     int modelId,
     string modelName,
     string sampleGroup,
     int sampleNo,
     List<ResultItem> results)
        {
            InitializeComponent();

            this.userId = userId;
            this.userName = userName;
            this.modelId = modelId;
            this.modelName = modelName;
            this.sampleGroup = sampleGroup;
            this.sampleNo = sampleNo;
            this.results = results;

            BuildResultsScreen();
        }

        private void BuildResultsScreen()
        {
            Controls.Clear();

            Text = "Measurement Results";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(245, 247, 250);

            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 170,
                BackColor = Color.White
            };
            Controls.Add(headerPanel);

            Label titleLabel = new Label
            {
                Text = "Measurement Results",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                AutoSize = true,
                Location = new Point(90, 45)
            };
            headerPanel.Controls.Add(titleLabel);

            Label infoLabel = new Label
            {
                Text = $"Operator: {userName}   •   Model: {modelName}   •   Sample: {sampleGroup} {sampleNo}",
                Font = new Font("Segoe UI", 13),
                ForeColor = Color.FromArgb(100, 110, 120),
                AutoSize = true,
                Location = new Point(95, 105)
            };
            headerPanel.Controls.Add(infoLabel);

            Panel cardPanel = new Panel
            {
                Location = new Point(90, 220),
                Size = new Size(1720, 650),
                BackColor = Color.White
            };
            Controls.Add(cardPanel);

            Label tableTitle = new Label
            {
                Text = "Inspection Summary",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                AutoSize = true,
                Location = new Point(30, 25)
            };
            cardPanel.Controls.Add(tableTitle);

            DataGridView resultsGrid = new DataGridView
            {
                Location = new Point(30, 80),
                Size = new Size(1660, 540),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 235, 240),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EnableHeadersVisualStyles = false
            };

            resultsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 87, 145);
            resultsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            resultsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            resultsGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            resultsGrid.ColumnHeadersHeight = 45;

            resultsGrid.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            resultsGrid.DefaultCellStyle.ForeColor = Color.FromArgb(45, 55, 65);
            resultsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 250);
            resultsGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            resultsGrid.RowTemplate.Height = 42;

            resultsGrid.Columns.Add("StepNo", "Step");
            resultsGrid.Columns.Add("CriteriaName", "Criteria");
            resultsGrid.Columns.Add("EnteredValue", "Entered Value");
            resultsGrid.Columns.Add("Status", "Status");

            foreach (ResultItem result in results)
            {
                int rowIndex = resultsGrid.Rows.Add(
                    result.StepNo,
                    result.CriteriaName,
                    result.EnteredValue,
                    result.Status
                );

                DataGridViewRow row = resultsGrid.Rows[rowIndex];

                if (result.Status.ToLower().Contains("ok") || result.Status.ToLower().Contains("pass"))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(235, 248, 239);
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(34, 139, 84);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240);
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(190, 55, 55);
                }

                row.Cells["Status"].Style.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            }

            cardPanel.Controls.Add(resultsGrid);

            Button saveButton = new Button
            {
                Text = "Save Results",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(230, 60),
                Location = new Point(1580, 910),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderSize = 0;
            Controls.Add(saveButton);

            saveButton.Click += (sender, e) =>
            {
                SaveResultsToDatabase();
            };
        }
        private void SaveResultsToDatabase()
        {
            
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    int sessionId;

                    string insertSessionQuery = @"
                INSERT INTO dbo.InspectionSessions
                (ModelID, UserID, ShiftName, StartedAt, CompletedAt, StatusID)
                OUTPUT INSERTED.SessionID
                VALUES
                (@ModelID, @UserID, @ShiftName, @StartedAt, @CompletedAt, @StatusID);";

                    using (SqlCommand command = new SqlCommand(insertSessionQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@ModelID", modelId);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@ShiftName", GetCurrentShift());
                        command.Parameters.AddWithValue("@StartedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@CompletedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@StatusID", 3);

                        sessionId = Convert.ToInt32(command.ExecuteScalar());
                    }

                    int sampleId;

                    string insertSampleQuery = @"
                INSERT INTO dbo.Samples
                (SessionID, ModelID, SampleGroup, SampleNo)
                OUTPUT INSERTED.SampleID
                VALUES
                (@SessionID, @ModelID, @SampleGroup, @SampleNo);";

                    using (SqlCommand command = new SqlCommand(insertSampleQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@SessionID", sessionId);
                        command.Parameters.AddWithValue("@ModelID", modelId);
                        command.Parameters.AddWithValue("@SampleGroup", sampleGroup);
                        command.Parameters.AddWithValue("@SampleNo", sampleNo);

                        sampleId = Convert.ToInt32(command.ExecuteScalar());
                    }

                    foreach (ResultItem result in results)
                    {
                        string insertResultQuery = @"
                    INSERT INTO dbo.MeasurementResults
                    (SessionID, SampleID, CriteriaID, NumericValue, TextValue, StatusID, MeasuredAt)
                    VALUES
                    (@SessionID, @SampleID, @CriteriaID, @NumericValue, @TextValue, @StatusID, @MeasuredAt);";

                        using (SqlCommand command = new SqlCommand(insertResultQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@SessionID", sessionId);
                            command.Parameters.AddWithValue("@SampleID", sampleId);
                            command.Parameters.AddWithValue("@CriteriaID", result.CriteriaID);

                            command.Parameters.AddWithValue("@NumericValue",
                                result.NumericValue.HasValue ? result.NumericValue.Value : DBNull.Value);

                            command.Parameters.AddWithValue("@TextValue",
                                string.IsNullOrWhiteSpace(result.TextValue) ? DBNull.Value : result.TextValue);

                            command.Parameters.AddWithValue("@StatusID", result.StatusID);
                            command.Parameters.AddWithValue("@MeasuredAt", DateTime.Now);
                            

                            
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();

                    MessageBox.Show("Results saved successfully.");

                    LoginForm loginform=new LoginForm();
                    loginform.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Save failed:\n\n" + ex.Message);
                }
            }
        }
        private string GetCurrentShift()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (now >= new TimeSpan(8, 0, 0) && now < new TimeSpan(16, 0, 0))
                return "Morning Shift";

            if (now >= new TimeSpan(16, 0, 0) && now <= new TimeSpan(23, 59, 59))
                return "Night Shift";

            return "Out of Shift";
        }
    }
}