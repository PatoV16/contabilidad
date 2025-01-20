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
        private Label lblTitulo;
        private string passwordFile = "config.dat";

        // Usar los mismos colores del menú principal
        private Color primaryColor = Color.FromArgb(52, 152, 219);
        private Color buttonHoverColor = Color.FromArgb(41, 128, 185);

        public FormPassword()
        {
            InitializeComponent();
            CheckPasswordFile();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Text = "Ingreso al Sistema";

            // Título
            lblTitulo = new Label
            {
                Text = "Ingrese Contraseña",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = primaryColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(300, 50),
                Location = new Point(50, 50)
            };

            // Campo Contraseña
            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 30),
                Location = new Point(50, 130),
                PasswordChar = '•'
            };

            // Botón Ingresar
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

            // Eventos hover del botón
            btnIngresar.MouseEnter += (s, e) => {
                btnIngresar.BackColor = buttonHoverColor;
                btnIngresar.Cursor = Cursors.Hand;
            };
            btnIngresar.MouseLeave += (s, e) => {
                btnIngresar.BackColor = primaryColor;
            };

            // Agregar controles al formulario
            this.Controls.AddRange(new Control[] {
                lblTitulo,
                txtPassword,
                btnIngresar
            });

            // Permitir usar Enter para enviar
            this.AcceptButton = btnIngresar;
        }

        private void CheckPasswordFile()
        {
            if (!File.Exists(passwordFile))
            {
                using (var form = new Form())
                {
                    form.Size = new Size(400, 200);
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.Text = "Configurar Contraseña";
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MaximizeBox = false;
                    form.MinimizeBox = false;

                    var txtNewPass = new TextBox
                    {
                        Location = new Point(50, 30),
                        Size = new Size(280, 30),
                        PasswordChar = '•',
                        Font = new Font("Segoe UI", 12)
                    };

                    var lblInfo = new Label
                    {
                        Text = "Ingrese la contraseña para el sistema:",
                        Location = new Point(50, 10),
                        Size = new Size(280, 20)
                    };

                    var btnSave = new Button
                    {
                        Text = "Guardar",
                        Location = new Point(150, 80),
                        Size = new Size(100, 30),
                        DialogResult = DialogResult.OK
                    };

                    form.Controls.AddRange(new Control[] { lblInfo, txtNewPass, btnSave });
                    form.AcceptButton = btnSave;

                    if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtNewPass.Text))
                    {
                        SavePassword(txtNewPass.Text);
                        MessageBox.Show("Contraseña configurada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        SavePassword("admin123"); // Contraseña por defecto
                        MessageBox.Show("Se ha configurado la contraseña por defecto: admin123", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void SavePassword(string password)
        {
            string hashedPassword = HashPassword(password);
            File.WriteAllText(passwordFile, hashedPassword);
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

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Por favor, ingrese la contraseña.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
    }
}