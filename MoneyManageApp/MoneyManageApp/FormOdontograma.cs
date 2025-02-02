using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SQLite;
using static System.Windows.Forms.AxHost;

namespace MoneyManageApp
{
    public partial class FormOdontograma : Form
    {
        private Panel panelOdontograma;
        private Dictionary<int, ToothControl> dientes;
        private Label lblPaciente;
        private ComboBox cmbPaciente;
        private Button btnGuardar;
        private Button btnLimpiar;
        private TextBox txtEspecificaciones;
        private TextBox txtObservaciones;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Panel panelLeyenda;
        private string state;
        private Button btnOdontogramaKid;
        private Button btnFotos;

        public FormOdontograma()
        {
            InitializeComponent();
            CargarPacientes();
            InicializarDientes();
            cmbPaciente.SelectedIndexChanged += CmbPaciente_SelectedIndexChanged;
        }

        private void CmbPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaciente.SelectedIndex > 0)
            {
                var pacienteId = ((ComboBoxItem)cmbPaciente.SelectedItem).Value;
                CargarOdontograma(pacienteId);
            }
            else
            {
                BtnLimpiar_Click(null, null);
            }
        }

        private void CargarOdontograma(string pacienteId)
        {
            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(@"
                SELECT DientesEstado, Especificaciones, Observaciones 
                FROM Odontogramas 
                WHERE ClienteId = @ClienteId 
                ORDER BY FechaRegistro DESC LIMIT 1", connection))
                    {
                        command.Parameters.AddWithValue("@ClienteId", pacienteId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Cargar estado de los dientes
                                string dientesEstadoJson = reader["DientesEstado"].ToString();
                                var dientesEstado = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(dientesEstadoJson);

                                foreach (var diente in dientesEstado)
                                {
                                    if (dientes.ContainsKey(diente.Key))
                                    {
                                        dientes[diente.Key].ActualizarEstado(diente.Value);
                                    }
                                }

                                // Cargar especificaciones y observaciones
                                txtEspecificaciones.Text = reader["Especificaciones"].ToString();
                                txtObservaciones.Text = reader["Observaciones"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("No se encontró un odontograma registrado para este paciente.",
                                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Limpiar los campos
                                txtEspecificaciones.Clear();
                                txtObservaciones.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el odontograma: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Odontograma";
            this.Size = new Size(1300, 800); // Increased width to accommodate legend
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            panelOdontograma = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            Panel panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(243, 244, 246),
                AutoSize = true // Permitir que el panel se ajuste al contenido
            };

            lblPaciente = new Label
            {
                Text = "PACIENTE*",
                Location = new Point(10, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            txtBuscar = new TextBox
            {
                Location = new Point(80, 17),
                Width = 150,
                Text = "Buscar paciente..."
            };

            btnBuscar = new Button
            {
                Text = "🔍",
                Location = new Point(235, 15),
                Width = 30,
                Height = 30,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBuscar.Click += BtnBuscar_Click;

            cmbPaciente = new ComboBox
            {
                Location = new Point(270, 17),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(530, 15),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnGuardar.Click += BtnGuardar_Click;

            btnLimpiar = new Button
            {
                Text = "Limpiar",
                Location = new Point(640, 15),  // 530 + 100 + 10
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiar.Click += BtnLimpiar_Click;

            btnOdontogramaKid = new Button
            {
                Text = "Pediátrico",
                Location = new Point(750, 15),  // 640 + 100 + 10
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOdontogramaKid.Click += BtnOdontogramaKid_Click;

            // New Fotos button
            btnFotos = new Button
            {
                Text = "Fotos",
                Location = new Point(860, 15),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = true // Asegurarnos que sea visible
            };
            btnFotos.Click += BtnFotos_Click;


            // Create and setup legend panel
            panelLeyenda = new Panel
            {
                Dock = DockStyle.Right,
                Width = 150,
                BackColor = Color.FromArgb(249, 250, 251),
                Padding = new Padding(10)
            };

            CreateLegend();

            Panel panelNotas = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                Padding = new Padding(20)
            };

            Label lblEspecificaciones = new Label
            {
                Text = "CITA ACTUAL",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(20, 10)
            };

            txtEspecificaciones = new TextBox
            {
                Multiline = true,
                Location = new Point(20, 30),
                Size = new Size(450, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblObservaciones = new Label
            {
                Text = "PROXIMA CITA",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(490, 10)
            };

            txtObservaciones = new TextBox
            {
                Multiline = true,
                Location = new Point(490, 30),
                Size = new Size(450, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            panelNotas.Controls.AddRange(new Control[] {
                lblEspecificaciones, txtEspecificaciones,
                lblObservaciones, txtObservaciones
            });

            panelSuperior.Controls.AddRange(new Control[] {
        lblPaciente, txtBuscar, btnBuscar, cmbPaciente,
        btnGuardar, btnLimpiar, btnOdontogramaKid, btnFotos
    });

            this.Controls.AddRange(new Control[] {
            panelOdontograma,
            panelSuperior,
            panelNotas,
            panelLeyenda
            });
        }

        private void CreateLegend()
        {
            Label lblLeyenda = new Label
            {
                Text = "LEYENDA",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 10)
            };

            var legendItems = new Dictionary<string, Color>
            {
                { "Sano", Color.White },
                { "Caries", Color.Red },
                { "Obturado", Color.RoyalBlue },
                { "Ausente", Color.LightGray },
                { "Corona", Color.Gold },
                { "Puente", Color.Pink },
                { "Implante", Color.DarkGreen }
            };

            int yOffset = 40;
            foreach (var item in legendItems)
            {
                Panel colorBox = new Panel
                {
                    Location = new Point(10, yOffset),
                    Size = new Size(20, 20),
                    BackColor = item.Value,
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label label = new Label
                {
                    Text = item.Key,
                    Location = new Point(40, yOffset),
                    AutoSize = true
                };

                panelLeyenda.Controls.Add(colorBox);
                panelLeyenda.Controls.Add(label);

                yOffset += 30;
            }

            panelLeyenda.Controls.Add(lblLeyenda);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string searchTerm = txtBuscar.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchTerm)) return;

            foreach (ComboBoxItem item in cmbPaciente.Items)
            {
                if (item.Display.ToLower().Contains(searchTerm) ||
                    item.Value.ToLower().Contains(searchTerm))
                {
                    cmbPaciente.SelectedItem = item;
                    return;
                }
            }

            MessageBox.Show("No se encontró ningún paciente con ese criterio de búsqueda.",
                "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void BtnFotos_Click(object sender, EventArgs e)
        {
            FormFotos formFotos = new FormFotos();
            formFotos.Show(); // Muestra la ventana sin bloquear la actual
                              // Si quieres que sea modal (bloquee la actual), usa formFotos.ShowDialog();
        }


        private void CargarPacientes()
        {
            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();

                    // Modificamos la consulta para solo obtener pacientes mayores de 18 años
                    string query = @"
                SELECT CedulaRUC, NombreRazonSocial 
                FROM Clientes 
                WHERE (julianday('now') - julianday(AnioNacimiento)) / 365.25 >= 18
                ORDER BY NombreRazonSocial";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            cmbPaciente.Items.Clear();
                            cmbPaciente.Items.Add(new ComboBoxItem { Value = "", Display = "Seleccionar..." });

                            while (reader.Read())
                            {
                                cmbPaciente.Items.Add(new ComboBoxItem
                                {
                                    Value = reader["CedulaRUC"].ToString(),
                                    Display = reader["NombreRazonSocial"].ToString()
                                });
                            }
                        }
                    }
                }
                cmbPaciente.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la lista de pacientes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void InicializarDientes()
        {
            dientes = new Dictionary<int, ToothControl>();
            int toothWidth = 40;
            int toothHeight = 100;
            int spacing = 5;

            int[] dientesSuperior = { 18, 17, 16, 15, 14, 13, 12, 11, 21, 22, 23, 24, 25, 26, 27, 28 };
            int[] dientesInferior = { 48, 47, 46, 45, 44, 43, 42, 41, 31, 32, 33, 34, 35, 36, 37, 38 };

            int startY = 100;
            int centerX = (panelOdontograma.Width - (dientesSuperior.Length * (toothWidth + spacing))) / 2;

            foreach (int num in dientesSuperior)
            {
                var tooth = new ToothControl(num, true)
                {
                    Location = new Point(centerX + (Array.IndexOf(dientesSuperior, num) * (toothWidth + spacing)), startY),
                    Size = new Size(toothWidth, toothHeight)
                };
                dientes.Add(num, tooth);
                panelOdontograma.Controls.Add(tooth);
            }

            startY += toothHeight + 40;
            foreach (int num in dientesInferior)
            {
                var tooth = new ToothControl(num, false)
                {
                    Location = new Point(centerX + (Array.IndexOf(dientesInferior, num) * (toothWidth + spacing)), startY),
                    Size = new Size(toothWidth, toothHeight)
                };
                dientes.Add(num, tooth);
                panelOdontograma.Controls.Add(tooth);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbPaciente.SelectedIndex == 0)
            {
                MessageBox.Show("Por favor seleccione un paciente.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pacienteId = ((ComboBoxItem)cmbPaciente.SelectedItem).Value;
            var especificaciones = txtEspecificaciones.Text.Trim();
            var observaciones = txtObservaciones.Text.Trim();

            var dientesEstado = new Dictionary<int, string>();
            foreach (var diente in dientes)
            {
                dientesEstado[diente.Key] = diente.Value.Estado;
            }
            var dientesEstadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(dientesEstado);

            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(connection))
                    {
                        // Primero verificamos si existe un registro para este paciente
                        command.CommandText = "SELECT COUNT(*) FROM Odontogramas WHERE ClienteId = @ClienteId";
                        command.Parameters.AddWithValue("@ClienteId", pacienteId);
                        int existingRecords = Convert.ToInt32(command.ExecuteScalar());

                        command.Parameters.Clear();

                        if (existingRecords > 0)
                        {
                            // Si existe, actualizamos el registro
                            command.CommandText = @"
                        UPDATE Odontogramas 
                        SET FechaRegistro = @FechaRegistro,
                            Especificaciones = @Especificaciones,
                            Observaciones = @Observaciones,
                            DientesEstado = @DientesEstado
                        WHERE ClienteId = @ClienteId";
                        }
                        else
                        {
                            // Si no existe, insertamos un nuevo registro
                            command.CommandText = @"
                        INSERT INTO Odontogramas (ClienteId, FechaRegistro, Especificaciones, Observaciones, DientesEstado)
                        VALUES (@ClienteId, @FechaRegistro, @Especificaciones, @Observaciones, @DientesEstado)";
                        }

                        // Añadimos los parámetros
                        command.Parameters.AddWithValue("@ClienteId", pacienteId);
                        command.Parameters.AddWithValue("@FechaRegistro", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@Especificaciones", especificaciones);
                        command.Parameters.AddWithValue("@Observaciones", observaciones);
                        command.Parameters.AddWithValue("@DientesEstado", dientesEstadoJson);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Odontograma guardado exitosamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                BtnLimpiar_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el odontograma: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            foreach (ToothControl tooth in dientes.Values)
            {
                tooth.ResetState();
            }
            cmbPaciente.SelectedIndex = 0;
            txtEspecificaciones.Clear();
            txtObservaciones.Clear();
        }

        private void BtnOdontogramaKid_Click(object sender, EventArgs e)
        {
            KidsOdontogram form = new KidsOdontogram();
            form.ShowDialog();
        }
    }

    public class ToothControl : UserControl
    {
        private int number;
        private bool isUpper;
        private string state = "normal";
        private Color toothColor = Color.White;
        private Color borderColor = Color.Black;

        public string Estado => state;

        public ToothControl(int number, bool isUpper)
        {
            this.number = number;
            this.isUpper = isUpper;
            this.DoubleBuffered = true;
            this.Click += ToothControl_Click;
        }
        public void ActualizarEstado(string estado)
        {
            state = estado;
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Determinar colores basados en el estado
            switch (state.ToLower())
            {
                case "caries":
                    toothColor = Color.Red;
                    break;
                case "obturado":
                    toothColor = Color.RoyalBlue;
                    break;
                case "ausente":
                    toothColor = Color.LightGray;
                    break;
                case "corona":
                    toothColor = Color.Gold;
                    break;
                case "puente":
                    toothColor = Color.Pink;
                    break;
                case "implante":
                    toothColor = Color.Green;
                    break;
                default: // "sano"
                    toothColor = Color.White;
                    break;
            }

            // Calcular dimensiones
            int margin = 2;
            Rectangle toothRect = new Rectangle(
                margin,
                margin,
                Width - (margin * 2),
                Height - (margin * 2)
            );

            // Dibujar el contorno del diente
            using (var toothBrush = new SolidBrush(toothColor))
            using (var borderPen = new Pen(borderColor, 1.5f))
            {
                // Dibujar la corona
                int crownHeight = toothRect.Height / 2;
                Rectangle crownRect = new Rectangle(
                    toothRect.X,
                    isUpper ? toothRect.Y + crownHeight : toothRect.Y,
                    toothRect.Width,
                    crownHeight
                );

                // Dibujar la raíz
                Rectangle rootRect = new Rectangle(
                    toothRect.X + (toothRect.Width / 4),
                    isUpper ? toothRect.Y : toothRect.Y + crownHeight,
                    toothRect.Width / 2,
                    crownHeight
                );

                // Rellenar formas
                g.FillRectangle(toothBrush, crownRect);
                if (state.ToLower() != "ausente")
                {
                    g.FillRectangle(toothBrush, rootRect);
                }

                // Dibujar bordes
                g.DrawRectangle(borderPen, crownRect);
                if (state.ToLower() != "ausente")
                {
                    g.DrawRectangle(borderPen, rootRect);
                }

                // Dibujar número de diente
                using (var font = new Font("Arial", 8))
                using (var brush = new SolidBrush(Color.Black))
                {
                    var textPos = new PointF(
                        toothRect.X + (toothRect.Width - g.MeasureString(number.ToString(), font).Width) / 2,
                        isUpper ? toothRect.Y : toothRect.Bottom - 20
                    );
                    g.DrawString(number.ToString(), font, brush, textPos);
                }
            }
        }

        private void ToothControl_Click(object sender, EventArgs e)
        {
            using (FormEstadoDiente formEstado = new FormEstadoDiente(number))
            {
                if (formEstado.ShowDialog() == DialogResult.OK)
                {
                    state = formEstado.EstadoSeleccionado.ToLower();
                    this.Invalidate();
                }
            }
        }

        public void ResetState()
        {
            state = "normal";
            this.Invalidate();
        }

       
    }

    public class FormEstadoDiente : Form
    {
        private ComboBox cmbEstado;
        public string EstadoSeleccionado { get; private set; }

        public FormEstadoDiente(int numeroDiente)
        {
            this.Text = $"Estado del Diente {numeroDiente}";
            this.Size = new Size(300, 150);
            this.StartPosition = FormStartPosition.CenterParent;

            cmbEstado = new ComboBox
            {
                Location = new Point(20, 20),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbEstado.Items.AddRange(new string[] { "Sano", "Caries", "Obturado", "Ausente", "Corona","Puente","Implante" });
            cmbEstado.SelectedIndex = 0;

            var btnAceptar = new Button
            {
                Text = "Aceptar",
                DialogResult = DialogResult.OK,
                Location = new Point(85, 70),
                Width = 80
            };

            btnAceptar.Click += (s, e) => { EstadoSeleccionado = cmbEstado.SelectedItem.ToString(); };

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Location = new Point(180, 70),
                Width = 80
            };

            this.Controls.AddRange(new Control[] { cmbEstado, btnAceptar, btnCancelar });
        }
    }

    public class ComboBoxItem
    {
        public string Value { get; set; }
        public string Display { get; set; }
        public string Id { get; internal set; }

        // Constructor sin parámetros
        public ComboBoxItem() { }

        // Constructor con parámetros (si lo necesitas)
        public ComboBoxItem(string id, string display)
        {
            Value = id;
            Display = display;
        }

        public override string ToString() => Display;
    }

}
