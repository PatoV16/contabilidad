using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MoneyManageApp
{
    public class KidsOdontogram : Form
    {
        private Dictionary<int, ToothControl> babyTeeth;
        private ComboBox cmbPaciente;
        private TextBox txtEspecificaciones;
        private TextBox txtObservaciones;

        public KidsOdontogram()
        {
            InitializeComponent();
            SetupBabyTeeth();
            CargarPacientes();
        }
        private void CargarPacientes()
        {
            cmbPaciente.Items.Clear();
            cmbPaciente.Items.Add("Seleccionar Paciente");

            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(@"
                SELECT CedulaRUC, NombreRazonSocial 
                FROM Clientes 
                WHERE (julianday('now') - julianday(AnioNacimiento)) / 365.25 < 14", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string pacienteId = reader["CedulaRUC"].ToString();
                                string nombrePaciente = reader["NombreRazonSocial"].ToString();

                                cmbPaciente.Items.Add(new ComboBoxItem(nombrePaciente, pacienteId));
                            }
                        }
                    }
                }

                cmbPaciente.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pacientes: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Suscribirse al evento para cargar el odontograma cuando se seleccione un paciente
            cmbPaciente.SelectedIndexChanged += cmbPaciente_SelectedIndexChanged;
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
                                string dientesEstadoJson = reader["DientesEstado"]?.ToString();
                                if (!string.IsNullOrEmpty(dientesEstadoJson))
                                {
                                    var dientesEstado = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(dientesEstadoJson);
                                    foreach (var diente in dientesEstado)
                                    {
                                        if (babyTeeth.ContainsKey(diente.Key))
                                        {
                                            babyTeeth[diente.Key].ActualizarEstado(diente.Value);
                                        }
                                    }
                                }

                                // Cargar especificaciones y observaciones con manejo de null
                                txtEspecificaciones.Text = reader["Especificaciones"] != DBNull.Value ?
                                    reader["Especificaciones"].ToString() : string.Empty;

                                txtObservaciones.Text = reader["Observaciones"] != DBNull.Value ?
                                    reader["Observaciones"].ToString() : string.Empty;
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
            // Configuración de la ventana principal
            this.Text = "Odontograma Pediátrico";
            this.Size = new Size(900, 700);  // Tamaño de la ventana
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Crear panel principal
            Panel panelPrincipal = CrearPanelPrincipal();

            // Crear título
            Label lblTitulo = CrearTitulo();

            // Crear panel de información del paciente
            Panel panelPaciente = CrearPanelPaciente();

            // Crear panelNotas en la parte inferior del formulario
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


            // Crear leyenda de estados
            Panel panelLeyenda = CrearPanelLeyenda();

            // Agregar controles al formulario
            this.Controls.Add(panelPaciente);
            this.Controls.Add(panelPrincipal);
            this.Controls.Add(panelLeyenda);
            this.Controls.Add(panelNotas);  // Agregar panelNotas al formulario

            // Forzar actualización de la UI
            this.Refresh();
        }



        private Panel CrearPanelPrincipal()
        {
            return new Panel
            {
                Name = "panelPrincipal",
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
        }

        private Label CrearTitulo()
        {
            return new Label
            {
                Text = "Odontograma Infantil",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Height = 40
            };
        }

        private Panel CrearPanelPaciente()
        {
            Panel panelPaciente = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(10)
            };

            Label lblNombre = new Label
            {
                Text = "Nombre:",
                Location = new Point(10, 20),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            cmbPaciente = new ComboBox
            {
                Location = new Point(80, 15),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            FlowLayoutPanel panelBotones = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };

            Button btnGuardar = new Button
            {
                Text = "Guardar Odontograma",
                Width = 150
            };
            btnGuardar.Click += (sender, e) => GuardarOdontograma();

           ;

           

            panelBotones.Controls.AddRange(new Control[] { btnGuardar });

            panelPaciente.Controls.Add(lblNombre);
            panelPaciente.Controls.Add(cmbPaciente);
            panelPaciente.Controls.Add(panelBotones);

            return panelPaciente;
        }




        private Panel CrearPanelLeyenda()
        {
            Panel panelLeyenda = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            Dictionary<string, Color> estadosLeyenda = new Dictionary<string, Color>
    {
        { "Sano", Color.White },
        { "Caries", Color.Red },
        { "Tratado", Color.Green },
        { "Sellante", Color.Blue },
        { "En Erupción", Color.Yellow }
    };

            int xLeyenda = 10;
            foreach (var estado in estadosLeyenda)
            {
                Panel colorBox = new Panel
                {
                    Location = new Point(xLeyenda, 20),
                    Size = new Size(20, 20),
                    BackColor = estado.Value,
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label lblEstado = new Label
                {
                    Text = estado.Key,
                    Location = new Point(xLeyenda + 25, 20),
                    AutoSize = true
                };

                panelLeyenda.Controls.Add(colorBox);
                panelLeyenda.Controls.Add(lblEstado);
                xLeyenda += 100;
            }

            return panelLeyenda;
        }

        private void SetupBabyTeeth()
        {
            babyTeeth = new Dictionary<int, ToothControl>();

            // Números de dientes temporales según el sistema FDI organizados en dos filas
            int[][] babyTeethNumbers = {
        new int[] { 51, 52, 53, 54, 55, 61, 62, 63, 64, 65 }, // Superiores
        new int[] { 71, 72, 73, 74, 75, 81, 82, 83, 84, 85 }  // Inferiores
    };

            int toothWidth = 40;
            int toothHeight = 60;
            int spacing = 10;
            int startY = 100;

            // Calcular posición inicial (centrado)
            int centerX = (this.ClientSize.Width - (10 * (toothWidth + spacing))) / 2;

            // Panel principal donde se agregarán los controles
            var panel = Controls.Find("panelPrincipal", true).FirstOrDefault();
            if (panel == null) throw new Exception("No se encontró el panel principal.");

            for (int row = 0; row < babyTeethNumbers.Length; row++)
            {
                for (int col = 0; col < babyTeethNumbers[row].Length; col++)
                {
                    int toothNumber = babyTeethNumbers[row][col];

                    // Crear control para el diente
                    var tooth = new ToothControl(toothNumber, row == 0)
                    {
                        Location = new Point(
                            centerX + (col * (toothWidth + spacing)),
                            startY + (row * (toothHeight + spacing + 20))
                        ),
                        Size = new Size(toothWidth, toothHeight)
                    };

                    babyTeeth.Add(toothNumber, tooth);
                    panel.Controls.Add(tooth); // Agregar al panel
                }
            }
        }
        public class ToothControl : UserControl
        {
            private int number;
            private bool isUpper;
            private string estado = "sano";
            private Color colorDiente = Color.White;
            private Color colorBorde = Color.Black;
            private Color borderColor = Color.Black;

            public string Estado => estado;

            public ToothControl(int number, bool isUpper)
            {
                this.number = number;
                this.isUpper = isUpper;
                this.DoubleBuffered = true;
                this.Click += ToothControl_Click;
            }

            public void ActualizarEstado(string nuevoEstado)
            {
                estado = nuevoEstado.ToLower();
                ActualizarColor();
                Invalidate();
            }

            private void ActualizarColor()
            {
                switch (estado)
                {
                    case "caries": colorDiente = Color.Red; break;
                    case "tratado": colorDiente = Color.Green; break;
                    case "sellante": colorDiente = Color.Blue; break;
                    case "en erupción": colorDiente = Color.Yellow; break;
                    default: colorDiente = Color.White; break;
                }
            }

            private void ToothControl_Click(object sender, EventArgs e)
            {
                using (var formularioEstado = new FormularioEstadoDiente(number))
                {
                    if (formularioEstado.ShowDialog() == DialogResult.OK)
                    {
                        estado = formularioEstado.EstadoSeleccionado.ToLower();
                        ActualizarColor();
                        Invalidate();
                    }
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Determinar color basado en el estado actual
                Color colorDiente;
                switch (estado.ToLower())
                {
                    case "caries":
                        colorDiente = Color.Red;
                        break;
                    case "tratado":
                        colorDiente = Color.Green;
                        break;
                    case "sellante":
                        colorDiente = Color.Blue;
                        break;
                    case "en erupción":
                        colorDiente = Color.Yellow;
                        break;
                    default:
                        colorDiente = Color.White;
                        break;
                }

                // Calcular dimensiones del diente
                int margin = 4;
                Rectangle toothRect = new Rectangle(
                    margin,
                    margin,
                    Width - (margin * 2),
                    Height - (margin * 2)
                );

                // Dibujar el contorno del diente
                using (var toothBrush = new SolidBrush(colorDiente))
                using (var borderPen = new Pen(borderColor, 2.0f))
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
                        toothRect.X + (toothRect.Width / 3),
                        isUpper ? toothRect.Y : toothRect.Y + crownHeight,
                        toothRect.Width / 3,
                        crownHeight
                    );

                    // Rellenar formas
                    g.FillRectangle(toothBrush, crownRect);
                    if (estado.ToLower() != "ausente")
                    {
                        g.FillRectangle(toothBrush, rootRect);
                    }

                    // Dibujar bordes
                    g.DrawRectangle(borderPen, crownRect);
                    if (estado.ToLower() != "ausente")
                    {
                        g.DrawRectangle(borderPen, rootRect);
                    }

                    // Dibujar número de diente
                    using (var font = new Font("Arial", 10, FontStyle.Bold))
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


            private class FormularioEstadoDiente : Form
            {
                private ComboBox cmbEstado;
                public string EstadoSeleccionado { get; private set; }

                public FormularioEstadoDiente(int numeroDiente)
                {
                    // Configuración del formulario
                    this.Text = $"Estado del Diente {numeroDiente}";
                    this.Size = new Size(350, 180);
                    this.StartPosition = FormStartPosition.CenterParent;

                    // ComboBox para seleccionar estado
                    cmbEstado = new ComboBox
                    {
                        Location = new Point(20, 20),
                        Width = 300,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cmbEstado.Items.AddRange(new string[]
                    {
            "Sano",
            "Caries",
            "Tratado",
            "Sellante",
            "En Erupción"
                    });
                    cmbEstado.SelectedIndex = 0;

                    // Botón Guardar
                    Button btnGuardar = new Button
                    {
                        Text = "Guardar",
                        Location = new Point(50, 70),
                        Width = 100
                    };
                    btnGuardar.Click += (sender, e) =>
                    {
                        EstadoSeleccionado = cmbEstado.SelectedItem.ToString();
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    };

                    // Botón Cancelar
                    Button btnCancelar = new Button
                    {
                        Text = "Cancelar",
                        Location = new Point(200, 70),
                        Width = 100
                    };
                    btnCancelar.Click += (sender, e) =>
                    {
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                    };

                    // Agregar controles al formulario
                    this.Controls.Add(cmbEstado);
                    this.Controls.Add(btnGuardar);
                    this.Controls.Add(btnCancelar);
                }
            }

        }
      
        private void cmbPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaciente.SelectedIndex > 0) // Evita la opción "Seleccionar Paciente"
            {
                var pacienteSeleccionado = cmbPaciente.SelectedItem as ComboBoxItem;
                if (pacienteSeleccionado != null)
                {
                    CargarOdontograma(pacienteSeleccionado.Value);
                }
            }
        }

        
        public class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public ComboBoxItem(string text, string value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }
        private void GuardarOdontograma()
        {
            // Validate patient selection
            if (cmbPaciente.SelectedIndex <= 0)
            {
                MessageBox.Show("Seleccione un paciente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get selected patient ID
            var pacienteSeleccionado = cmbPaciente.SelectedItem as ComboBoxItem;
            if (pacienteSeleccionado == null)
            {
                MessageBox.Show("Paciente inválido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate that the fields are not empty
            string especificaciones = txtEspecificaciones.Text.Trim();
            string observaciones = txtObservaciones.Text.Trim();

            if (string.IsNullOrWhiteSpace(especificaciones) || string.IsNullOrWhiteSpace(observaciones))
            {
                MessageBox.Show("Debe llenar todos los campos de especificaciones y observaciones",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prepare tooth states dictionary
            var dientesEstado = babyTeeth.ToDictionary(
                tooth => tooth.Key,
                tooth => tooth.Value.Estado
            );

            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var command = new SQLiteCommand(connection))
                        {
                            // Check if an odontogram already exists for this patient
                            command.CommandText = @"
                        SELECT Id FROM Odontogramas 
                        WHERE ClienteId = @ClienteId";
                            command.Parameters.AddWithValue("@ClienteId", pacienteSeleccionado.Value);
                            var existingRecord = command.ExecuteScalar();

                            // Prepare JSON for tooth states
                            string dientesEstadoJson = JsonConvert.SerializeObject(dientesEstado);

                            if (existingRecord != null)
                            {
                                command.CommandText = @"
                            UPDATE Odontogramas 
                            SET FechaRegistro = @FechaRegistro, 
                                DientesEstado = @DientesEstado, 
                                Especificaciones = @Especificaciones,
                                Observaciones = @Observaciones 
                            WHERE ClienteId = @ClienteId";
                            }
                            else
                            {
                                command.CommandText = @"
                            INSERT INTO Odontogramas 
                            (ClienteId, FechaRegistro, DientesEstado, Especificaciones, Observaciones) 
                            VALUES (@ClienteId, @FechaRegistro, @DientesEstado, @Especificaciones, @Observaciones)";
                            }

                            command.Parameters.AddWithValue("@FechaRegistro", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.Parameters.AddWithValue("@DientesEstado", dientesEstadoJson);
                            command.Parameters.AddWithValue("@Especificaciones", especificaciones);
                            command.Parameters.AddWithValue("@Observaciones", observaciones);

                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }

                MessageBox.Show("Odontograma guardado exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar odontograma: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }   


    }
}
