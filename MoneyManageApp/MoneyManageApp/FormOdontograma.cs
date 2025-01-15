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
        private string state;

        public FormOdontograma()
        {
            InitializeComponent();
            CargarPacientes();
            InicializarDientes();
            cmbPaciente.SelectedIndexChanged += CmbPaciente_SelectedIndexChanged;


        }
        private void CmbPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaciente.SelectedIndex > 0) // Si hay un paciente seleccionado
            {
                var pacienteId = ((ComboBoxItem)cmbPaciente.SelectedItem).Value;
                CargarOdontograma(pacienteId);
            }
            else
            {
                // Limpiar el odontograma si no hay paciente seleccionado
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
                    using (var command = new SQLiteCommand("SELECT DientesEstado FROM Odontogramas WHERE ClienteId = @ClienteId ORDER BY FechaRegistro DESC LIMIT 1", connection))
                    {
                        command.Parameters.AddWithValue("@ClienteId", pacienteId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string dientesEstadoJson = reader["DientesEstado"].ToString();
                                var dientesEstado = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(dientesEstadoJson);

                                // Actualizar el estado de los dientes en la interfaz
                                // Dentro de CargarOdontograma
                                foreach (var diente in dientesEstado)
                                {
                                    if (dientes.ContainsKey(diente.Key))
                                    {
                                        dientes[diente.Key].ActualizarEstado(diente.Value);
                                    }
                                }

                            }
                            else
                            {
                                MessageBox.Show("No se encontró un odontograma registrado para este paciente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el odontograma: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       

        private void InitializeComponent()
        {
            this.Text = "Odontograma";
            this.Size = new Size(1000, 800);
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
                BackColor = Color.FromArgb(243, 244, 246)
            };

            lblPaciente = new Label
            {
                Text = "PACIENTE*",
                Location = new Point(10, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            cmbPaciente = new ComboBox
            {
                Location = new Point(80, 17),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(350, 15),
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
                Location = new Point(460, 15),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiar.Click += BtnLimpiar_Click;

            Panel panelNotas = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                Padding = new Padding(20)
            };

            Label lblEspecificaciones = new Label
            {
                Text = "ESPECIFICACIONES",
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
                Text = "OBSERVACIONES",
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
                lblPaciente, cmbPaciente,
                btnGuardar, btnLimpiar
            });

            this.Controls.AddRange(new Control[] { panelOdontograma, panelSuperior, panelNotas });
        }

        private void CargarPacientes()
        {
            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT CedulaRUC, NombreRazonSocial FROM Clientes ORDER BY NombreRazonSocial", connection))
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

            // Serializar el estado de los dientes
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
                        command.CommandText = @"
                        INSERT INTO Odontogramas (ClienteId, FechaRegistro, Especificaciones, Observaciones, DientesEstado)
                        VALUES (@ClienteId, @FechaRegistro, @Especificaciones, @Observaciones, @DientesEstado)";
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
                BtnLimpiar_Click(null, null); // Limpia el formulario después de guardar
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
                    toothColor = Color.Silver;
                    break;
                case "ausente":
                    toothColor = Color.LightGray;
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
            cmbEstado.Items.AddRange(new string[] { "Sano", "Caries", "Obturado", "Ausente" });
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
        public override string ToString() => Display;
    }

   
}
