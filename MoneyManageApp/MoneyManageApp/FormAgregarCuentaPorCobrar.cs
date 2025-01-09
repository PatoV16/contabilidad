using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace MoneyManageApp
{
    public partial class FormAgregarCuentaPorCobrar : Form
    {
        public FormAgregarCuentaPorCobrar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Agregar Cuenta por Cobrar";
            this.Size = new System.Drawing.Size(400, 350); // Aumentar el tamaño para ajustar todos los controles
            this.StartPosition = FormStartPosition.CenterScreen;

            // Campos de texto
            Label lblCliente = new Label { Text = "Cliente", Location = new System.Drawing.Point(20, 20) };
            ComboBox cmbCliente = new ComboBox { Location = new System.Drawing.Point(100, 20), Width = 250 };

            Label lblArticulo = new Label { Text = "Artículo", Location = new System.Drawing.Point(20, 60) };
            TextBox txtArticulo = new TextBox { Location = new System.Drawing.Point(100, 60), Width = 250 };

            Label lblValorCredito = new Label { Text = "Valor Crédito", Location = new System.Drawing.Point(20, 100) };
            TextBox txtValorCredito = new TextBox { Location = new System.Drawing.Point(100, 100), Width = 250 };

            Label lblFechaInicial = new Label { Text = "Fecha Inicial", Location = new System.Drawing.Point(20, 140) };
            DateTimePicker dtpFechaInicial = new DateTimePicker { Location = new System.Drawing.Point(100, 140), Width = 250 };

            Label lblSaldoCuenta = new Label { Text = "Saldo Cuenta", Location = new System.Drawing.Point(20, 180) };
            TextBox txtSaldoCuenta = new TextBox { Location = new System.Drawing.Point(100, 180), Width = 250 };

            Label lblFechaFinal = new Label { Text = "Fecha Final", Location = new System.Drawing.Point(20, 220) };
            DateTimePicker dtpFechaFinal = new DateTimePicker { Location = new System.Drawing.Point(100, 220), Width = 250 };

            Label lblEstado = new Label { Text = "Estado", Location = new System.Drawing.Point(20, 260) };
            ComboBox cmbEstado = new ComboBox { Location = new System.Drawing.Point(100, 260), Width = 250 };
            cmbEstado.Items.Add("Pendiente");
            cmbEstado.Items.Add("Pagada");

            // Botón para agregar cuenta
            Button btnAgregarCuenta = new Button
            {
                Text = "Agregar Cuenta",
                Location = new System.Drawing.Point(100, 300),
                Width = 250
            };
            btnAgregarCuenta.Click += (sender, e) => BtnAgregarCuenta_Click(sender, e, cmbCliente, txtArticulo, txtValorCredito, dtpFechaInicial, txtSaldoCuenta, dtpFechaFinal, cmbEstado);

            // Agregar los controles al formulario
            this.Controls.Add(lblCliente);
            this.Controls.Add(cmbCliente);
            this.Controls.Add(lblArticulo);
            this.Controls.Add(txtArticulo);
            this.Controls.Add(lblValorCredito);
            this.Controls.Add(txtValorCredito);
            this.Controls.Add(lblFechaInicial);
            this.Controls.Add(dtpFechaInicial);
            this.Controls.Add(lblSaldoCuenta);
            this.Controls.Add(txtSaldoCuenta);
            this.Controls.Add(lblFechaFinal);
            this.Controls.Add(dtpFechaFinal);
            this.Controls.Add(lblEstado);
            this.Controls.Add(cmbEstado);
            this.Controls.Add(btnAgregarCuenta);

            // Cargar los clientes al ComboBox
            LoadClientes(cmbCliente);
        }

        private void LoadClientes(ComboBox cmbCliente)
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT NombreRazonSocial FROM Clientes";  // Asegúrate de que 'Clientes' tiene el campo 'NombreCliente'
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        cmbCliente.Items.Clear();  // Limpiar antes de agregar nuevos elementos

                        while (reader.Read())
                        {
                            cmbCliente.Items.Add(reader["NombreRazonSocial"].ToString());  // Asegúrate de usar el nombre correcto de la columna
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregarCuenta_Click(object sender, EventArgs e, ComboBox cmbCliente, TextBox txtArticulo, TextBox txtValorCredito, DateTimePicker dtpFechaInicial, TextBox txtSaldoCuenta, DateTimePicker dtpFechaFinal, ComboBox cmbEstado)
        {
            try
            {
                // Validar campos
                if (cmbCliente.SelectedIndex == -1 || string.IsNullOrEmpty(txtArticulo.Text) ||
                    string.IsNullOrEmpty(txtValorCredito.Text) || string.IsNullOrEmpty(txtSaldoCuenta.Text) ||
                    cmbEstado.SelectedIndex == -1)
                {
                    MessageBox.Show("Todos los campos son obligatorios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Insertar la nueva cuenta por cobrar en la base de datos
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO CuentasPorCobrar (Cliente, Articulo, ValorCredito, FechaInicial, SaldoCuenta, FechaFinal, Estado)
                        VALUES (@Cliente, @Articulo, @ValorCredito, @FechaInicial, @SaldoCuenta, @FechaFinal, @Estado)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Cliente", cmbCliente.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@Articulo", txtArticulo.Text);
                        cmd.Parameters.AddWithValue("@ValorCredito", Convert.ToDecimal(txtValorCredito.Text));
                        cmd.Parameters.AddWithValue("@FechaInicial", dtpFechaInicial.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@SaldoCuenta", Convert.ToDecimal(txtSaldoCuenta.Text));
                        cmd.Parameters.AddWithValue("@FechaFinal", dtpFechaFinal.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Estado", cmbEstado.SelectedItem.ToString());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cuenta por cobrar registrada exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Cerrar el formulario después de agregar la cuenta
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar la cuenta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
