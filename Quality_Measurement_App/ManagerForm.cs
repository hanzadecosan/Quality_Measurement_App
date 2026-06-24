using System;
using System.Drawing;
using System.Windows.Forms;

namespace Quality_Measurement_App
{
    public partial class ManagerForm : Form
    {
        public ManagerForm()
        {
            InitializeComponent();
            BuildManagerScreen();
        }

        private void BuildManagerScreen()
        {
            Controls.Clear();

            Text = "Yönetim Paneli";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            

            Label titleLabel = new Label 
            {
                Text = "YÖNETİM PANELİ",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 39, 51),
                Size = new Size(620, 70),
                Location = new Point(650, 130),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(titleLabel);

            Label subtitleLabel = new Label
            {
                Text = "İlk/Son numune kontrolleri sistemi yönetim sayfası",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(120, 130, 140),
                Size = new Size(620, 35),
                Location = new Point(650, 205),
                TextAlign = ContentAlignment.MiddleCenter
            };
          Controls.Add(subtitleLabel);

            Button addModelButton = CreateMenuButton("Yeni model ekle", 330);
            Button addUserButton = CreateMenuButton("Yeni kullanıcı ekle", 420);
            Button addCriteriaButton = CreateMenuButton("Yeni ölçüm kriteri ekle", 510);
            Button manageCriteriaButton = CreateMenuButton("Mevcut kriterleri düzenle", 600);
            Button viewRecordsButton = CreateMenuButton("Ölçüm Kayıtlarını Görüntüle", 710);
            Button backButton = CreateMenuButton("Başa dön", 810);

            Button exitButton = new Button
            {
                Text = "Çıkış",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(500, 65),
                BackColor = Color.FromArgb(173, 216, 230),
                Location = new Point(1250, 810),
                ForeColor = Color.FromArgb(30, 30, 30),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            exitButton.FlatAppearance.BorderSize = 0;

            


            backButton.BackColor = Color.FromArgb(52, 58, 64);

            Controls.Add(addModelButton);
            Controls.Add(addUserButton);
            Controls.Add(addCriteriaButton);
            Controls.Add(manageCriteriaButton);
            Controls.Add(viewRecordsButton);
            Controls.Add(backButton);
            Controls.Add(exitButton);

            addModelButton.Click += (sender, e) =>
            {
                AddModelForm addModelForm = new AddModelForm();
                addModelForm.Show();
                this.Hide();
            };
            addUserButton.Click += (sender, e) =>
            {
                AddUserForm addUserForm = new AddUserForm();
                addUserForm.Show();
                this.Hide();
            };

            addCriteriaButton.Click += (sender, e) =>
            {
                
                    AddCriteriaForm addCriteriaForm = new AddCriteriaForm();
                    addCriteriaForm.Show();
                    Hide();
                
            };

            viewRecordsButton.Click += (sender, e) =>
            {
                
                    RecordsForm recordsForm = new RecordsForm();
                    recordsForm.Show();
                    Hide();
                
            };
            manageCriteriaButton.Click += (sender, e) =>
            {
                ManageCriteriaForm form = new ManageCriteriaForm();
                form.Show();
                Hide();
            };

            backButton.Click += (sender, e) =>
            {
                LoginForm loginform=new LoginForm();
                loginform.Show();
                Close();
            };
            exitButton.Click += (s, e) =>
            {
                DialogResult result = MessageBox.Show(
                    "Uygulamayı kapatmak istediğinize emin misiniz?",
                    "Exit Application",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };

        }

        private Button CreateMenuButton(string text, int y)
        {
            Button button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Size = new Size(500, 65),
                Location = new Point(710, y),
                BackColor = Color.FromArgb(31, 87, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }
    }
}