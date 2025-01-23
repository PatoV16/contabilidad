using System;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormMenu : Form
    {
        private Label lblTitulo;
        private string nombreNegocio;
        private Panel mainContainer;
        private FlowLayoutPanel buttonContainer;

        // Colores temáticos dentales
        private Color primaryColor = Color.FromArgb(41, 171, 226);      // Azul dental
        private Color secondaryColor = Color.FromArgb(245, 247, 250);   // Gris muy claro
        private Color buttonHoverColor = Color.FromArgb(0, 150, 199);   // Azul oscuro
        private Color accentColor = Color.FromArgb(255, 255, 255);      // Blanco
        private Color textColor = Color.FromArgb(70, 70, 70);           // Gris oscuro

        public FormMenu()
        {
            CargarNombreNegocio();
            InitializeComponent();
        }

        private void CargarNombreNegocio()
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "CREATE TABLE IF NOT EXISTS ConfiguracionNegocio (Nombre TEXT);";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = "SELECT Nombre FROM ConfiguracionNegocio LIMIT 1;";
                    cmd = new SQLiteCommand(query, conn);
                    object result = cmd.ExecuteScalar();

                    nombreNegocio = result?.ToString() ?? "Nombre del Negocio";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el nombre del negocio: " + ex.Message);
                nombreNegocio = "Nombre del Negocio";
            }
        }

        private void GuardarNombreNegocio(string nuevoNombre)
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM ConfiguracionNegocio; INSERT INTO ConfiguracionNegocio (Nombre) VALUES (@nombre);";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", nuevoNombre);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el nombre del negocio: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.Text = $"Consultorio Dental - {nombreNegocio}";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = secondaryColor;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Panel principal
            mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            // Panel superior
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = primaryColor
            };

            // Título principal
            lblTitulo = new Label
            {
                Text = "🦷 " + nombreNegocio,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = accentColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };
            lblTitulo.Click += LblTitulo_Click;

            // Subtítulo
            Label lblSubtitulo = new Label
            {
                Text = "Sistema de Gestión Odontológica",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = accentColor,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Bottom,
                Height = 30
            };

            headerPanel.Controls.Add(lblTitulo);
            headerPanel.Controls.Add(lblSubtitulo);

            // Contenedor de botones
            buttonContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = accentColor
            };

            // Crear botones del menú
            var btnIngresosEgresos = CreateMenuButton("💰 Control de\nIngresos y Egresos", BtnIngresosEgresos_Click);
            var btnCuentasPorCobrar = CreateMenuButton("📊 Control de\nCuentas por Cobrar", BtnCuentasPorCobrar_Click);
            var btnInventario = CreateMenuButton("📦 Control de\nInventario", BtnInventario_Click);
            var btnRegistroClientes = CreateMenuButton("👥 Registro de\nPacientes", BtnRegistroClientes_Click);
            var btnCitas = CreateMenuButton("📅 Control de\nCitas", BtnCitas_Click);
            var btnOdontograma = CreateMenuButton("🦷 Control de\nOdontograma", BtnOdontograma_Click);
            var btnBorrarBD = CreateMenuButton("🗑️ Borrar Base\nde Datos", BtnBorrarBD_Click);

            buttonContainer.Controls.AddRange(new Control[] {
                btnIngresosEgresos, btnCuentasPorCobrar, btnInventario,
                btnRegistroClientes, btnCitas, btnOdontograma, btnBorrarBD
            });

            mainContainer.Controls.Add(buttonContainer);
            mainContainer.Controls.Add(headerPanel);
            this.Controls.Add(mainContainer);
        }


        private Button CreateMenuButton(string text, EventHandler clickHandler)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(200, 100),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = primaryColor,
                ForeColor = Color.White,
                Margin = new Padding(10),
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = true
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.Click += clickHandler;

            // Eventos para efectos hover
            btn.MouseEnter += (s, e) => {
                btn.BackColor = buttonHoverColor;
                btn.Cursor = Cursors.Hand;
            };
            btn.MouseLeave += (s, e) => {
                btn.BackColor = primaryColor;
            };

            return btn;
        }

        private Button CreateStyledButton(string text, int width, int height)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(width, height),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = primaryColor,
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderColor = primaryColor;
            btn.FlatAppearance.BorderSize = 1;

            btn.MouseEnter += (s, e) => {
                btn.BackColor = primaryColor;
                btn.ForeColor = Color.White;
            };
            btn.MouseLeave += (s, e) => {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = primaryColor;
            };

            return btn;
        }

        private void LblTitulo_Click(object sender, EventArgs e)
        {
            using (Form inputForm = new Form())
            {
                inputForm.Text = "Editar Nombre del Negocio";
                inputForm.Size = new System.Drawing.Size(400, 150);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.BackColor = Color.White;

                TextBox txtNombre = new TextBox
                {
                    Location = new Point(20, 20),
                    Size = new Size(340, 25),
                    Text = nombreNegocio,
                    Font = new Font("Segoe UI", 10)
                };

                Button btnGuardar = CreateStyledButton("Guardar", 80, 30);
                btnGuardar.DialogResult = DialogResult.OK;
                btnGuardar.Location = new Point(280, 60);

                Button btnCancelar = CreateStyledButton("Cancelar", 80, 30);
                btnCancelar.DialogResult = DialogResult.Cancel;
                btnCancelar.Location = new Point(190, 60);

                inputForm.Controls.AddRange(new Control[] { txtNombre, btnGuardar, btnCancelar });
                inputForm.AcceptButton = btnGuardar;
                inputForm.CancelButton = btnCancelar;

                if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    nombreNegocio = txtNombre.Text.Trim();
                    lblTitulo.Text = nombreNegocio;
                    this.Text = $"Menú Principal - {nombreNegocio}";
                    GuardarNombreNegocio(nombreNegocio);
                }
            }
        }

        private void BtnIngresosEgresos_Click(object sender, EventArgs e)
        {
            FormIngresosEgresos form = new FormIngresosEgresos();
            form.ShowDialog();
        }

        private void BtnCuentasPorCobrar_Click(object sender, EventArgs e)
        {
            FormCuentasPorCobrar form = new FormCuentasPorCobrar();
            form.ShowDialog();
        }

        private void BtnInventario_Click(object sender, EventArgs e)
        {
            FormInventario form = new FormInventario();
            form.ShowDialog();
        }

        private void BtnRegistroClientes_Click(object sender, EventArgs e)
        {
            FormRegistroClientes form = new FormRegistroClientes();
            form.ShowDialog();
        }

        private void BtnCitas_Click(object sender, EventArgs e)
        {
            FormCitas form = new FormCitas();
            form.ShowDialog();
        }

        private void BtnBorrarBD_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "¿Estás seguro de que deseas borrar todos los registros?\nEsta acción no se puede deshacer.",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SQLiteConnection conn = Database.GetConnection())
                    {
                        conn.Open();
                        string queryBorrar = @"
                            DELETE FROM IngresosEgresos; 
                            DELETE FROM CuentasPorCobrar; 
                            DELETE FROM Clientes; 
                            DELETE FROM Productos; 
                            DELETE FROM Entradas; 
                            DELETE FROM Salidas; 
                            DELETE FROM RecaudosDiarios;
                            DELETE FROM Odontogramas;
                            DELETE FROM Citas";
                            
                        SQLiteCommand cmd = new SQLiteCommand(queryBorrar, conn);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show(
                            "Los registros han sido borrados exitosamente.",
                            "Éxito",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al borrar los registros: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void BtnOdontograma_Click(object sender, EventArgs e)
        {
            FormOdontograma form = new FormOdontograma();
            form.ShowDialog();
        }
    }
}
