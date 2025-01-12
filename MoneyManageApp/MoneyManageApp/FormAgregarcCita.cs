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
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ComboBox de clientes
            cboClientes = new ComboBox
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(340, 30)
            };
            this.Controls.Add(cboClientes);

            // DateTimePicker para la fecha
            dtpFecha = new DateTimePicker
            {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(340, 30)
            };
            this.Controls.Add(dtpFecha);

            // TextBox para el estado
            txtEstado = new TextBox
            {
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(340, 30)
            };
            this.Controls.Add(txtEstado);

            // TextBox para el concepto
            txtConcepto = new TextBox
            {
                Location = new System.Drawing.Point(20, 140),
                Size = new System.Drawing.Size(340, 30)
            };
            this.Controls.Add(txtConcepto);

            // Botón para guardar la cita
            btnGuardar = new Button
            {
                Text = "Guardar",
                Size = new System.Drawing.Size(100, 30),
                Location = new System.Drawing.Point(20, 180)
            };
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private void LoadClientes()
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Cliente FROM Clientes;";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cboClientes.Items.Add(reader["Cliente"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboClientes.Text) || string.IsNullOrWhiteSpace(txtEstado.Text) || string.IsNullOrWhiteSpace(txtConcepto.Text))
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
                    cmd.Parameters.AddWithValue("@estado", txtEstado.Text);
                    cmd.Parameters.AddWithValue("@concepto", txtConcepto.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cita registrada exitosamente.");
                    this.Close();  // Cerrar el formulario después de guardar
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la cita: " + ex.Message);
            }
        }
    }
}
