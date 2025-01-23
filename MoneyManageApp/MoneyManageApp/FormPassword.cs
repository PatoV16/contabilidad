using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MoneyManageApp
{
    public class FormPassword : Form
    {
        private TextBox txtPassword;
        private Button btnIngresar;
        private Button btnRecuperarContrasena;
        private Label lblTitulo;
        private string passwordFile = "config.dat";
        private string securityFile = "security.dat";

        private Color primaryColor = Color.FromArgb(52, 152, 219);
        private Color buttonHoverColor = Color.FromArgb(41, 128, 185);

        public FormPassword()
        {
            InitializeComponent();
            CheckPasswordFile();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Text = "Ingreso al Sistema";

            lblTitulo = new Label
            {
                Text = "Ingrese Contraseña",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = primaryColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(300, 50),
                Location = new Point(50, 30)
            };

            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 30),
                Location = new Point(50, 120),
                PasswordChar = '•'
            };

            btnIngresar = new Button
            {
                Text = "Ingresar",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(300, 40),
                Location = new Point(50, 180),
                FlatStyle = FlatStyle.Flat,
                BackColor = primaryColor,
                ForeColor = Color.White
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Click += BtnIngresar_Click;

            btnRecuperarContrasena = new Button
            {
                Text = "Recuperar Contraseña",
                Font = new Font("Segoe UI", 10),
                Size = new Size(300, 30),
                Location = new Point(50, 240),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White
            };
            btnRecuperarContrasena.FlatAppearance.BorderSize = 0;
            btnRecuperarContrasena.Click += BtnRecuperarContrasena_Click;

            btnIngresar.MouseEnter += (s, e) => btnIngresar.BackColor = buttonHoverColor;
            btnIngresar.MouseLeave += (s, e) => btnIngresar.BackColor = primaryColor;

            this.Controls.AddRange(new Control[] { lblTitulo, txtPassword, btnIngresar, btnRecuperarContrasena });
            this.AcceptButton = btnIngresar;
        }

        private void CheckPasswordFile()
        {
            if (!File.Exists(passwordFile) || !File.Exists(securityFile))
            {
                using (var form = new Form())
                {
                    form.Size = new Size(400, 300);
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.Text = "Configurar Contraseña y Seguridad";

                    var txtNewPass = new TextBox
                    {
                        Location = new Point(50, 30),
                        Size = new Size(280, 30),
                        PasswordChar = '•',
                        Font = new Font("Segoe UI", 12)
                    };

                    var lblPass = new Label
                    {
                        Text = "Ingrese la contraseña:",
                        Location = new Point(50, 10),
                        Size = new Size(280, 20)
                    };

                    var txtQuestion = new TextBox
                    {
                        Location = new Point(50, 100),
                        Size = new Size(280, 30),
                        Font = new Font("Segoe UI", 12)
                    };

                    var lblQuestion = new Label
                    {
                        Text = "Pregunta de seguridad:",
                        Location = new Point(50, 80),
                        Size = new Size(280, 20)
                    };

                    var txtAnswer = new TextBox
                    {
                        Location = new Point(50, 170),
                        Size = new Size(280, 30),
                        Font = new Font("Segoe UI", 12)
                    };

                    var lblAnswer = new Label
                    {
                        Text = "Respuesta a la pregunta:",
                        Location = new Point(50, 150),
                        Size = new Size(280, 20)
                    };

                    var btnSave = new Button
                    {
                        Text = "Guardar",
                        Location = new Point(150, 220),
                        Size = new Size(100, 30),
                        DialogResult = DialogResult.OK
                    };

                    form.Controls.AddRange(new Control[] { lblPass, txtNewPass, lblQuestion, txtQuestion, lblAnswer, txtAnswer, btnSave });
                    form.AcceptButton = btnSave;

                    if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtNewPass.Text)
                        && !string.IsNullOrWhiteSpace(txtQuestion.Text) && !string.IsNullOrWhiteSpace(txtAnswer.Text))
                    {
                        SavePassword(txtNewPass.Text);
                        SaveSecurityQuestion(txtQuestion.Text, txtAnswer.Text);
                        MessageBox.Show("Configuración completada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Debe configurar la contraseña y las preguntas de seguridad.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Environment.Exit(0);
                    }
                }
            }
        }

        private void SavePassword(string password)
        {
            string hashedPassword = HashPassword(password);
            File.WriteAllText(passwordFile, hashedPassword);
        }

        private void SaveSecurityQuestion(string question, string answer)
        {
            string hashedAnswer = HashPassword(answer);
            File.WriteAllText(securityFile, $"{question}|{hashedAnswer}");
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            string storedHash = File.ReadAllText(passwordFile);
            string inputHash = HashPassword(txtPassword.Text);

            if (inputHash == storedHash)
            {
                this.Hide();
                var menuForm = new FormMenu();
                menuForm.FormClosed += (s, args) => this.Close();
                menuForm.Show();
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void BtnRecuperarContrasena_Click(object sender, EventArgs e)
        {
            string[] securityData = File.ReadAllText(securityFile).Split('|');
            string question = securityData[0];
            string correctAnswerHash = securityData[1];

            using (var form = new Form())
            {
                form.Size = new Size(400, 200);
                form.StartPosition = FormStartPosition.CenterParent;
                form.Text = "Recuperación de Contraseña";

                var lblQuestion = new Label
                {
                    Text = question,
                    Location = new Point(50, 30),
                    Size = new Size(300, 20)
                };

                var txtAnswer = new TextBox
                {
                    Location = new Point(50, 60),
                    Size = new Size(300, 30),
                    Font = new Font("Segoe UI", 12)
                };

                var btnSubmit = new Button
                {
                    Text = "Verificar",
                    Location = new Point(150, 110),
                    Size = new Size(100, 30),
                    DialogResult = DialogResult.OK
                };

                form.Controls.AddRange(new Control[] { lblQuestion, txtAnswer, btnSubmit });
                form.AcceptButton = btnSubmit;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string inputHash = HashPassword(txtAnswer.Text);
                    if (inputHash == correctAnswerHash)
                    {
                        MessageBox.Show("Respuesta correcta. Por favor configure una nueva contraseña.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CheckPasswordFile();
                    }
                    else
                    {
                        MessageBox.Show("Respuesta incorrecta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
