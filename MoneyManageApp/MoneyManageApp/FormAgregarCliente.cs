using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormAgregarCliente : Form
    {
        private TextBox txtCedulaRuc;
        private TextBox txtNombre;
        private TextBox txtDireccion;
        private TextBox txtCiudad;
        private TextBox txtTelefono;
        private TextBox txtCorreo;
        private Button btnGuardar;
        private Button btnCancelar;
        private DateTimePicker dtpNacimiento;

        public FormAgregarCliente()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Agregar Cliente";

            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 2,
                RowCount = 8,
                AutoSize = true
            };

            // Campos
            panel.Controls.Add(new Label { Text = "Cédula/RUC:" }, 0, 0);
            txtCedulaRuc = new TextBox { Width = 200 };
            panel.Controls.Add(txtCedulaRuc, 1, 0);

            panel.Controls.Add(new Label { Text = "Nombre/Razón Social:" }, 0, 1);
            txtNombre = new TextBox { Width = 200 };
            panel.Controls.Add(txtNombre, 1, 1);

            panel.Controls.Add(new Label { Text = "Dirección:" }, 0, 2);
            txtDireccion = new TextBox { Width = 200 };
            panel.Controls.Add(txtDireccion, 1, 2);

            panel.Controls.Add(new Label { Text = "Ciudad:" }, 0, 3);
            txtCiudad = new TextBox { Width = 200 };
            panel.Controls.Add(txtCiudad, 1, 3);

            panel.Controls.Add(new Label { Text = "Teléfono:" }, 0, 4);
            txtTelefono = new TextBox { Width = 200 };
            panel.Controls.Add(txtTelefono, 1, 4);

            panel.Controls.Add(new Label { Text = "Correo Electrónico:" }, 0, 5);
            txtCorreo = new TextBox { Width = 200 };
            panel.Controls.Add(txtCorreo, 1, 5);

            panel.Controls.Add(new Label { Text = "Fecha de Nacimiento:" }, 0, 6);
            dtpNacimiento = new DateTimePicker
            {
                Width = 200,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Now,
                Value = DateTime.Now.AddYears(-20)
            };
            panel.Controls.Add(dtpNacimiento, 1, 6);

            // Botones
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 40
            };

            btnGuardar = new Button { Text = "Guardar" };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };

            buttonPanel.Controls.AddRange(new Control[] { btnCancelar, btnGuardar });

            // Agregar al formulario
            this.Controls.Add(panel);
            this.Controls.Add(buttonPanel);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var cliente = GetClienteData();

                // Aquí llamamos al método que guarda el cliente en la base de datos
                GuardarClienteEnBaseDeDatos(cliente);

                MessageBox.Show("Cliente guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para obtener los datos del cliente
        public (string cedulaRuc, string nombre, string direccion, string ciudad, string telefono, string correo, DateTime fechaNacimiento) GetClienteData()
        {
            if (string.IsNullOrWhiteSpace(txtCedulaRuc.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
                throw new Exception("Cédula/RUC y Nombre/Razón Social son campos obligatorios.");

            return (
                txtCedulaRuc.Text,
                txtNombre.Text,
                txtDireccion.Text,
                txtCiudad.Text,
                txtTelefono.Text,
                txtCorreo.Text,
                dtpNacimiento.Value
            );
        }

        private void GuardarClienteEnBaseDeDatos((string cedulaRuc, string nombre, string direccion, string ciudad, string telefono, string correo, DateTime fechaNacimiento) cliente)
        {


            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();

                string insertQuery = @"
                    INSERT INTO Clientes (CedulaRUC, NombreRazonSocial, Direccion, Ciudad, Telefono, CorreoElectronico, AnioNacimiento)
                    VALUES (@CedulaRUC, @NombreRazonSocial, @Direccion, @Ciudad, @Telefono, @CorreoElectronico, @AnioNacimiento)";

                using (var command = new SQLiteCommand(insertQuery, conn))
                {
                    command.Parameters.AddWithValue("@CedulaRUC", cliente.cedulaRuc);
                    command.Parameters.AddWithValue("@NombreRazonSocial", cliente.nombre);
                    command.Parameters.AddWithValue("@Direccion", cliente.direccion);
                    command.Parameters.AddWithValue("@Ciudad", cliente.ciudad);
                    command.Parameters.AddWithValue("@Telefono", cliente.telefono);
                    command.Parameters.AddWithValue("@CorreoElectronico", cliente.correo);
                    command.Parameters.AddWithValue("@AnioNacimiento", cliente.fechaNacimiento.ToString("yyyy-MM-dd"));

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
