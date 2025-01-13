using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace MoneyManageApp
{
    public partial class FormRegistrarCita : Form
    {
        private ComboBox cboClientes;
        private DateTimePicker dtpFecha;
        private TextBox txtEstado;
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
            this.Size = new System.Drawing.Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Etiqueta para el cliente
            Label lblCliente = new Label
            {
                Text = "Cliente:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(100, 20)
            };
            this.Controls.Add(lblCliente);

            // ComboBox de clientes
            cboClientes = new ComboBox
            {
                Location = new System.Drawing.Point(20, 40),
                Size = new System.Drawing.Size(340, 30)
            };
            this.Controls.Add(cboClientes);

            // Etiqueta para la fecha
            Label lblFecha = new Label
            {
                Text = "Fecha:",
                Location = new System.Drawing.Point(20, 80),
                Size = new System.Drawing.Size(100, 20)
            };
            this.Controls.Add(lblFecha);

            // DateTimePicker para la fecha
            dtpFecha = new DateTimePicker
            {
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(340, 30)
            };
            this.Controls.Add(dtpFecha);

            // Etiqueta para el estado
            Label lblEstado = new Label
            {
                Text = "Estado:",
                Location = new System.Drawing.Point(20, 140),
                Size = new System.Drawing.Size(100, 20)
            };
            this.Controls.Add(lblEstado);

            // ComboBox para el estado
            ComboBox cboEstado = new ComboBox
            {
                Location = new System.Drawing.Point(20, 160),
                Size = new System.Drawing.Size(340, 30),
                DropDownStyle = ComboBoxStyle.DropDownList // Evita que el usuario escriba valores personalizados
            };
            cboEstado.Items.AddRange(new string[] { "Pendiente", "Realizado" }); // Opciones predefinidas
            this.Controls.Add(cboEstado);

            // Etiqueta para el concepto
            Label lblConcepto = new Label
            {
                Text = "Concepto:",
                Location = new System.Drawing.Point(20, 200),
                Size = new System.Drawing.Size(100, 20)
            };
            this.Controls.Add(lblConcepto);

            // TextBox para el concepto
            txtConcepto = new TextBox
            {
                Location = new System.Drawing.Point(20, 220),
                Size = new System.Drawing.Size(340, 30)
            };
            this.Controls.Add(txtConcepto);

            // Botón para guardar la cita
            btnGuardar = new Button
            {
                Text = "Guardar",
                Size = new System.Drawing.Size(100, 30),
                Location = new System.Drawing.Point(20, 260)
            };
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
                    string query = "SELECT NombreRazonSocial FROM Clientes;"; // Columna correcta
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cboClientes.Items.Add(reader["NombreRazonSocial"].ToString()); // Cambiar "Cliente" por "NombreRazonSocial"
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message);
            }
        }


        private void BtnGuardar_Click(object sender, EventArgs e, string estadoSeleccionado)
        {
            if (string.IsNullOrWhiteSpace(cboClientes.Text) || string.IsNullOrWhiteSpace(estadoSeleccionado) || string.IsNullOrWhiteSpace(txtConcepto.Text))
            {
                MessageBox.Show("Por favor complete todos los campos.");
                return;
            }

            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Citas (Cliente, Fecha, Estado, Concepto) VALUES (@cliente, @fecha, @estado, @concepto);";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@cliente", cboClientes.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@fecha", dtpFecha.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@estado", estadoSeleccionado);
                    cmd.Parameters.AddWithValue("@concepto", txtConcepto.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cita registrada exitosamente.");
                    this.Close(); // Cerrar el formulario después de guardar
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la cita: " + ex.Message);
            }
        }

    }
}
