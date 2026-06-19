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
            Size = new Size(900, 600);
            BackColor = Color.FromArgb(245, 247, 250);

            Label titleLabel = new Label
            {
                Text = "MEASUREMENT RESULTS",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(500, 40),
                Location = new Point(35, 25)
            };
            Controls.Add(titleLabel);

            Label infoLabel = new Label
            {
                Text = $"Operator: {userName}   |   Model: {modelName}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(90, 100, 110),
                Size = new Size(700, 30),
                Location = new Point(38, 65)
            };
            Controls.Add(infoLabel);

            DataGridView resultsGrid = new DataGridView
            {
                Location = new Point(40, 115),
                Size = new Size(800, 340),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };

            resultsGrid.Columns.Add("StepNo", "Step");
            resultsGrid.Columns.Add("CriteriaName", "Criteria");
            resultsGrid.Columns.Add("EnteredValue", "Entered Value");
            resultsGrid.Columns.Add("Status", "Status");

            foreach (ResultItem result in results)
            {
                resultsGrid.Rows.Add(
                    result.StepNo,
                    result.CriteriaName,
                    result.EnteredValue,
                    result.Status
                );
            }

            Controls.Add(resultsGrid);

            Button saveButton = new Button
            {
                Text = "Save Results",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(180, 44),
                Location = new Point(460, 485),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            saveButton.FlatAppearance.BorderSize = 0;
            Controls.Add(saveButton);

            

            saveButton.Click += (sender, e) =>
            {
                SaveResultsToDatabase();
                StartForm startForm = new StartForm();
                startForm.Show();
                this.Close();
            };

        }
        private void SaveResultsToDatabase()
        {
            MessageBox.Show(
    $"UserID = {userId}\n" +
    $"ModelID = {modelId}\n" +
    $"ModelName = {modelName}"
);
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

                    StartForm startForm = new StartForm();
                    startForm.Show();
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