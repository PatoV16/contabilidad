using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace MoneyManageApp
{
    public partial class FormRegistrarCita : Form
    {
        private ComboBox cboClientes;
        private DateTimePicker dtpFecha;
        private DateTimePicker dtpHora;
        private ComboBox cboEstado;
        private TextBox txtConcepto;
        private Button btnGuardar;

        public FormRegistrarCita()
        {
            InitializeComponent();
            LoadClientes();
        }

        private void InitializeComponent()
        {
            this.Text = "Registrar Cita";
            this.Size = new Size(420, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Título del formulario
            Label lblTitulo = new Label
            {
                Text = "Registrar Nueva Cita",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitulo);

            // Etiqueta para el cliente
            Label lblCliente = new Label
            {
                Text = "Paciente:",
                Location = new Point(20, 70),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            this.Controls.Add(lblCliente);

            // ComboBox de clientes
            cboClientes = new ComboBox
            {
                Location = new Point(20, 95),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cboClientes);

            // Etiqueta para la fecha
            Label lblFecha = new Label
            {
                Text = "Fecha:",
                Location = new Point(20, 140),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            this.Controls.Add(lblFecha);

            // DateTimePicker para la fecha
            dtpFecha = new DateTimePicker
            {
                Location = new Point(20, 165),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };
            this.Controls.Add(dtpFecha);

            // Etiqueta para la hora
            Label lblHora = new Label
            {
                Text = "Hora:",
                Location = new Point(20, 210),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            this.Controls.Add(lblHora);

            // DateTimePicker para la hora
            dtpHora = new DateTimePicker
            {
                Location = new Point(20, 235),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true
            };
            this.Controls.Add(dtpHora);

            // Etiqueta para el estado
            Label lblEstado = new Label
            {
                Text = "Estado:",
                Location = new Point(20, 280),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            this.Controls.Add(lblEstado);

            // ComboBox para el estado
            cboEstado = new ComboBox
            {
                Location = new Point(20, 305),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboEstado.Items.AddRange(new string[] { "Pendiente", "Realizado" });
            this.Controls.Add(cboEstado);

            // Etiqueta para el concepto
            Label lblConcepto = new Label
            {
                Text = "Concepto:",
                Location = new Point(20, 350),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            this.Controls.Add(lblConcepto);

            // TextBox para el concepto
            txtConcepto = new TextBox
            {
                Location = new Point(20, 375),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtConcepto);

            // Botón para guardar la cita
            btnGuardar = new Button
            {
                Text = "Guardar Cita",
                Size = new Size(150, 40),
                Location = new Point(20, 430),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(39, 174, 96),
                FlatStyle = FlatStyle.Flat
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += (sender, e) => BtnGuardar_Click(sender, e, cboEstado.SelectedItem?.ToString());
            this.Controls.Add(btnGuardar);
        }

        private void LoadClientes()
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT NombreRazonSocial FROM Clientes;";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cboClientes.Items.Add(reader["NombreRazonSocial"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e, string estadoSeleccionado)
        {
            if (string.IsNullOrWhiteSpace(cboClientes.Text) || string.IsNullOrWhiteSpace(estadoSeleccionado) || string.IsNullOrWhiteSpace(txtConcepto.Text))
            {
                MessageBox.Show("Por favor complete todos los campos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Citas (Cliente, Fecha, Hora, Estado, Concepto) VALUES (@cliente, @fecha, @hora, @estado, @concepto);";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@cliente", cboClientes.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@fecha", dtpFecha.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@hora", dtpHora.Value.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@estado", estadoSeleccionado);
                    cmd.Parameters.AddWithValue("@concepto", txtConcepto.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cita registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la cita: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
