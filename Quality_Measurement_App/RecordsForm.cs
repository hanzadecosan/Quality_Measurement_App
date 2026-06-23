using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class RecordsForm : Form
    {
        private DataGridView sessionsGrid;
        private DataGridView detailsGrid;
        private DateTimePicker fromDatePicker;
        private DateTimePicker toDatePicker;

        public RecordsForm()
        {
            InitializeComponent();
            BuildRecordsScreen();
            LoadSessions();
        }

        private void BuildRecordsScreen()
        {
            Controls.Clear();

            Text = "Records Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            Label titleLabel = new Label
            {
                Text = "RECORDS DASHBOARD",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(800, 60),
                Location = new Point(100, 45)
            };
            Controls.Add(titleLabel);

            Label fromLabel = CreateLabel("From", 100, 125);
            fromDatePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 12),
                Format = DateTimePickerFormat.Short,
                Size = new Size(180, 35),
                Location = new Point(100, 160),
                Value = DateTime.Today
            };
            Controls.Add(fromDatePicker);

            Label toLabel = CreateLabel("To", 310, 125);
            toDatePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 12),
                Format = DateTimePickerFormat.Short,
                Size = new Size(180, 35),
                Location = new Point(310, 160),
                Value = DateTime.Today
            };
            Controls.Add(toDatePicker);

            Button loadButton = new Button
            {
                Text = "Load Records",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(190, 42),
                Location = new Point(520, 154),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            loadButton.FlatAppearance.BorderSize = 0;
            Controls.Add(loadButton);

            loadButton.Click += (sender, e) =>
            {
                LoadSessions();
            };

            Label sessionLabel = CreateLabel("Sessions", 100, 225);

            sessionsGrid = new DataGridView
            {
                Location = new Point(100, 265),
                Size = new Size(1720, 330),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            Controls.Add(sessionsGrid);

            sessionsGrid.SelectionChanged += (sender, e) =>
            {
                if (sessionsGrid.CurrentRow == null)
                    return;

                if (sessionsGrid.CurrentRow.Cells["SessionID"].Value == null)
                    return;

                int sessionId = Convert.ToInt32(sessionsGrid.CurrentRow.Cells["SessionID"].Value);
                LoadSessionDetails(sessionId);
            };

            Label detailLabel = CreateLabel("Selected Session Details", 100, 620);

            detailsGrid = new DataGridView
            {
                Location = new Point(100, 660),
                Size = new Size(1720, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(detailsGrid);

            Button backButton = new Button
            {
                Text = "Back",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Size = new Size(220, 55),
                Location = new Point(1600, 950),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            backButton.FlatAppearance.BorderSize = 0;
            Controls.Add(backButton);

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

        private void LoadSessions()
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            DateTime fromDate = fromDatePicker.Value.Date;
            DateTime toDate = toDatePicker.Value.Date.AddDays(1);

            string query = @"
                SELECT
                    ses.SessionID,
                    u.FullName AS Operator,
                    m.ModelName AS Model,
                    sam.SampleGroup,
                    sam.SampleNo,
                    ses.ShiftName,
                    ses.StartedAt,
                    ses.CompletedAt,
                    DATEDIFF(MINUTE, ses.StartedAt, ses.CompletedAt) AS DurationMinutes,
                    CASE
                        WHEN SUM(CASE WHEN st.StatusName = 'NOK' THEN 1 ELSE 0 END) > 0 THEN 'NOK'
                        ELSE 'OK'
                    END AS OverallStatus
                FROM dbo.InspectionSessions ses
                JOIN dbo.Users u ON ses.UserID = u.UserID
                JOIN dbo.Models m ON ses.ModelID = m.ModelID
                JOIN dbo.Samples sam ON ses.SessionID = sam.SessionID
                JOIN dbo.MeasurementResults mr ON ses.SessionID = mr.SessionID
                JOIN dbo.Status st ON mr.StatusID = st.StatusID
                WHERE ses.StartedAt >= @FromDate
                  AND ses.StartedAt < @ToDate
                GROUP BY
                    ses.SessionID,
                    u.FullName,
                    m.ModelName,
                    sam.SampleGroup,
                    sam.SampleNo,
                    ses.ShiftName,
                    ses.StartedAt,
                    ses.CompletedAt
                ORDER BY ses.SessionID DESC;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                DataTable table = new DataTable();

                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }

                sessionsGrid.DataSource = table;
            }

            detailsGrid.DataSource = null;
        }

        private void LoadSessionDetails(int sessionId)
        {
            string connectionString =
                "Server=localhost;Database=Quality_Measurement_DB;Trusted_Connection=True;TrustServerCertificate=True;";

            string query = @"
                SELECT
                    c.StepNo,
                    c.CriteriaName,
                    mr.NumericValue,
                    mr.TextValue,
                    st.StatusName AS Status,
                    mr.MeasuredAt
                FROM dbo.MeasurementResults mr
                JOIN dbo.InspectionCriteria c ON mr.CriteriaID = c.CriteriaID
                JOIN dbo.Status st ON mr.StatusID = st.StatusID
                WHERE mr.SessionID = @SessionID
                ORDER BY c.StepNo;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SessionID", sessionId);

                DataTable table = new DataTable();

                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }

                detailsGrid.DataSource = table;
            }
        }
    }
}