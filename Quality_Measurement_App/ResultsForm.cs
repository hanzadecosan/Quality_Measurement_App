using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class ResultsForm : Form
    {
        private string userName;
        private string modelName;
        private List<ResultItem> results;

        public ResultsForm(string userName, string modelName, List<ResultItem> results)
        {
            InitializeComponent();

            this.userName = userName;
            this.modelName = modelName;
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

            Button backButton = new Button
            {
                Text = "Back to Start",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(180, 44),
                Location = new Point(660, 485),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            backButton.FlatAppearance.BorderSize = 0;
            Controls.Add(backButton);

            saveButton.Click += (sender, e) =>
            {
                MessageBox.Show("Save operation will be added here.");
            };

            backButton.Click += (sender, e) =>
            {
                StartForm startForm = new StartForm();
                startForm.Show();
                this.Close();
            };
        }
    }
}