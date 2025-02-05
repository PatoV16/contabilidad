using System.Data.SQLite;
using System.Windows.Forms;
using System;
using MoneyManageApp;
using System.Drawing;

public partial class FormAgregarCobro : Form
{
    ComboBox cmbCliente;  // Declarar el ComboBox como miembro de la clase
    TextBox txtConcepto;  // Declarar el TextBox para el concepto

    public FormAgregarCobro()
    {
        InitializeComponent();
        InitializeControls();  // Método para inicializar controles adicionales
        LoadClientes();  // Cargar los clientes después de la inicialización
    }

    private void InitializeComponent()
    {
        // Form configuration
        this.Text = "Registrar Cobro";
        this.Size = new System.Drawing.Size(500, 400);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Padding = new Padding(20);
        this.BackColor = Color.WhiteSmoke;
        this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // Create a main panel to hold controls
        Panel mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(20),
        };

        // Style for all labels
        var labelStyle = new Action<Label>((label) => {
            label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            label.ForeColor = Color.FromArgb(64, 64, 64);
            label.AutoSize = true;
        });

        // Style for all input controls
        var inputStyle = new Action<Control>((control) => {
            control.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            control.Height = 30;
        });

        // Cliente ComboBox
        Label lblCliente = new Label { Text = "Paciente", Location = new Point(0, 10) };
        labelStyle(lblCliente);

        cmbCliente = new ComboBox
        {
            Location = new Point(0, 35),
            Width = mainPanel.Width - 40,
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat
        };
        inputStyle(cmbCliente);

        // Monto TextBox
        Label lblMonto = new Label { Text = "Monto", Location = new Point(0, 75) };
        labelStyle(lblMonto);

        TextBox txtMonto = new TextBox
        {
            Location = new Point(0, 100),
            Width = mainPanel.Width - 40,
            BorderStyle = BorderStyle.FixedSingle
        };
        inputStyle(txtMonto);

        // Fecha de Cobro
        Label lblFechaCobro = new Label { Text = "Fecha de Cobro", Location = new Point(0, 140) };
        labelStyle(lblFechaCobro);

        DateTimePicker dtpFechaCobro = new DateTimePicker
        {
            Location = new Point(0, 165),
            Width = mainPanel.Width - 40,
            Format = DateTimePickerFormat.Short
        };
        inputStyle(dtpFechaCobro);

        // Concepto TextBox
        Label lblConcepto = new Label { Text = "Concepto", Location = new Point(0, 205) };
        labelStyle(lblConcepto);

        txtConcepto = new TextBox
        {
            Location = new Point(0, 230),
            Width = mainPanel.Width - 40,
            Height = 60,
            Multiline = true,
            BorderStyle = BorderStyle.FixedSingle,
            ScrollBars = ScrollBars.Vertical
        };

        // Register Button
        Button btnRegistrarCobro = new Button
        {
            Text = "Registrar Cobro",
            Location = new Point(0, 310),
            Width = mainPanel.Width - 40,
            Height = 40,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 204),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            Cursor = Cursors.Hand
        };

        btnRegistrarCobro.Click += (sender, e) => BtnRegistrarCobro_Click(sender, e, cmbCliente, txtMonto, dtpFechaCobro, txtConcepto);

        // Add hover effect
        btnRegistrarCobro.MouseEnter += (s, e) => {
            btnRegistrarCobro.BackColor = Color.FromArgb(0, 102, 184);
        };
        btnRegistrarCobro.MouseLeave += (s, e) => {
            btnRegistrarCobro.BackColor = Color.FromArgb(0, 122, 204);
        };

        // Add controls to main panel
        mainPanel.Controls.AddRange(new Control[] {
        lblCliente, cmbCliente,
        lblMonto, txtMonto,
        lblFechaCobro, dtpFechaCobro,
        lblConcepto, txtConcepto,
        btnRegistrarCobro
    });

        // Add main panel to form
        this.Controls.Add(mainPanel);
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
                    cmbCliente.Items.Clear();

                    while (reader.Read())
                    {
                        cmbCliente.Items.Add(reader["Cliente"].ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al cargar los pacientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRegistrarCobro_Click(object sender, EventArgs e, ComboBox cmbCliente, TextBox txtMonto, DateTimePicker dtpFechaCobro, TextBox txtConcepto)
    {
        try
        {
            // Validar campos
            if (cmbCliente.SelectedIndex == -1 || string.IsNullOrEmpty(txtMonto.Text) || string.IsNullOrEmpty(txtConcepto.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Convertir monto a decimal
            decimal monto = Convert.ToDecimal(txtMonto.Text);

            // Obtener cliente y saldo actual
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
            RegistrarCobro(clienteSeleccionado, monto, dtpFechaCobro.Value, txtConcepto.Text);

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

    private void RegistrarCobro(string cliente, decimal monto, DateTime fechaCobro, string concepto)
    {
        try
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = @"
                    INSERT INTO RecaudosDiarios (RecaudoDiario, FechaCobro, NombreCliente, Concepto)
                    VALUES (@RecaudoDiario, @FechaCobro, @NombreCliente, @Concepto)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RecaudoDiario", monto);
                    cmd.Parameters.AddWithValue("@FechaCobro", fechaCobro.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@NombreCliente", cliente);
                    cmd.Parameters.AddWithValue("@Concepto", concepto);
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
