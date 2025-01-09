using System.Data.SQLite;
using System.Windows.Forms;
using System;
using MoneyManageApp;

public partial class FormAgregarCobro : Form
{
    ComboBox cmbCliente;  // Declarar el ComboBox como miembro de la clase

    public FormAgregarCobro()
    {
        InitializeComponent();
        InitializeControls();  // Método para inicializar controles adicionales
        LoadClientes();  // Cargar los clientes después de la inicialización
    }

    private void InitializeComponent()
    {
        this.Text = "Registrar Cobro";
        this.Size = new System.Drawing.Size(400, 300);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Cliente ComboBox
        Label lblCliente = new Label { Text = "Cliente", Location = new System.Drawing.Point(20, 20) };

        // Inicializar ComboBox y agregarlo al formulario
        cmbCliente = new ComboBox { Location = new System.Drawing.Point(100, 20), Width = 250 };

        // Monto del pago
        Label lblMonto = new Label { Text = "Monto", Location = new System.Drawing.Point(20, 60) };
        TextBox txtMonto = new TextBox { Location = new System.Drawing.Point(100, 60), Width = 250 };

        // Fecha de cobro
        Label lblFechaCobro = new Label { Text = "Fecha de Cobro", Location = new System.Drawing.Point(20, 100) };
        DateTimePicker dtpFechaCobro = new DateTimePicker { Location = new System.Drawing.Point(100, 100), Width = 250 };

        // Botón para registrar cobro
        Button btnRegistrarCobro = new Button
        {
            Text = "Registrar Cobro",
            Location = new System.Drawing.Point(100, 140),
            Width = 250
        };
        btnRegistrarCobro.Click += (sender, e) => BtnRegistrarCobro_Click(sender, e, cmbCliente, txtMonto, dtpFechaCobro);

        // Agregar controles al formulario
        this.Controls.Add(lblCliente);
        this.Controls.Add(cmbCliente);  // Agregar ComboBox al formulario
        this.Controls.Add(lblMonto);
        this.Controls.Add(txtMonto);
        this.Controls.Add(lblFechaCobro);
        this.Controls.Add(dtpFechaCobro);
        this.Controls.Add(btnRegistrarCobro);
    }

    private void InitializeControls()
    {
        // Puedes agregar aquí otras inicializaciones si es necesario.
    }

    private void LoadClientes()
    {
        try
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = "SELECT DISTINCT Cliente FROM CuentasPorCobrar WHERE SaldoCuenta > 0";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    cmbCliente.Items.Clear();  // Limpiar los elementos actuales antes de agregar nuevos

                    while (reader.Read())
                    {
                        cmbCliente.Items.Add(reader["Cliente"].ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al cargar los clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRegistrarCobro_Click(object sender, EventArgs e, ComboBox cmbCliente, TextBox txtMonto, DateTimePicker dtpFechaCobro)
    {
        try
        {
            // Validar campos
            if (cmbCliente.SelectedIndex == -1 || string.IsNullOrEmpty(txtMonto.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Convertir monto a decimal
            decimal monto = Convert.ToDecimal(txtMonto.Text);

            // Verificar si el cliente tiene saldo suficiente
            string clienteSeleccionado = cmbCliente.SelectedItem.ToString();
            decimal saldoActual = GetSaldoCuenta(clienteSeleccionado);

            if (monto > saldoActual)
            {
                MessageBox.Show("El monto a pagar no puede ser mayor al saldo disponible", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Actualizar el saldo de la cuenta del cliente
            ActualizarSaldoCuenta(clienteSeleccionado, saldoActual - monto);

            // Registrar el cobro en la base de datos
            RegistrarCobro(clienteSeleccionado, monto, dtpFechaCobro.Value);

            MessageBox.Show("Cobro registrado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close(); // Cerrar el formulario después de registrar el cobro
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al registrar el cobro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private decimal GetSaldoCuenta(string cliente)
    {
        decimal saldo = 0;
        try
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = "SELECT SaldoCuenta FROM CuentasPorCobrar WHERE Cliente = @Cliente";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Cliente", cliente);
                    saldo = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al obtener el saldo de la cuenta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return saldo;
    }

    private void ActualizarSaldoCuenta(string cliente, decimal nuevoSaldo)
    {
        try
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = "UPDATE CuentasPorCobrar SET SaldoCuenta = @NuevoSaldo WHERE Cliente = @Cliente";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NuevoSaldo", nuevoSaldo);
                    cmd.Parameters.AddWithValue("@Cliente", cliente);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al actualizar el saldo de la cuenta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RegistrarCobro(string cliente, decimal monto, DateTime fechaCobro)
    {
        try
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
                    INSERT INTO RecaudosDiarios (RecaudoDiario, FechaCobro)
                    VALUES (@RecaudoDiario, @FechaCobro)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RecaudoDiario", monto);
                    cmd.Parameters.AddWithValue("@FechaCobro", fechaCobro.ToString("yyyy-MM-dd"));

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al registrar el cobro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
