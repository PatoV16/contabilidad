using System;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;

namespace MoneyManageApp
{
    public partial class FormAgregarCuentaPorCobrar : Form
    {
        // Field declarations
        private ComboBox cmbCliente;
        private TextBox txtArticulo;
        private TextBox txtValorCredito;
        private DateTimePicker dtpFechaInicial;
        private TextBox txtSaldoCuenta;
        private DateTimePicker dtpFechaFinal;
        private ComboBox cmbEstado;

        public FormAgregarCuentaPorCobrar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form configuration
            this.Text = "Agregar Cuenta por Cobrar";
            this.Size = new System.Drawing.Size(550, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.Padding = new Padding(20);

            // Create main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
            };

            // Style definitions
            var labelStyle = new Action<Label>((label) => {
                label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                label.ForeColor = Color.FromArgb(64, 64, 64);
                label.AutoSize = true;
            });

            var inputStyle = new Action<Control>((control) => {
                control.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
                control.Height = 30;
            });

            int yPos = 10;
            int spacing = 65;

            // Cliente
            Label lblCliente = new Label { Text = "Cliente", Location = new Point(0, yPos) };
            labelStyle(lblCliente);

            cmbCliente = new ComboBox
            {
                Location = new Point(0, yPos + 25),
                Width = mainPanel.Width - 40,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            inputStyle(cmbCliente);

            // Artículo
            yPos += spacing;
            Label lblArticulo = new Label { Text = "Artículo", Location = new Point(0, yPos) };
            labelStyle(lblArticulo);

            txtArticulo = new TextBox
            {
                Location = new Point(0, yPos + 25),
                Width = mainPanel.Width - 40,
                BorderStyle = BorderStyle.FixedSingle
            };
            inputStyle(txtArticulo);

            // Valor Crédito
            yPos += spacing;
            Label lblValorCredito = new Label { Text = "Valor Crédito", Location = new Point(0, yPos) };
            labelStyle(lblValorCredito);

            txtValorCredito = new TextBox
            {
                Location = new Point(0, yPos + 25),
                Width = mainPanel.Width - 40,
                BorderStyle = BorderStyle.FixedSingle
            };
            inputStyle(txtValorCredito);

            // Fecha Inicial
            yPos += spacing;
            Label lblFechaInicial = new Label { Text = "Fecha Inicial", Location = new Point(0, yPos) };
            labelStyle(lblFechaInicial);

            dtpFechaInicial = new DateTimePicker
            {
                Location = new Point(0, yPos + 25),
                Width = mainPanel.Width - 40,
                Format = DateTimePickerFormat.Short
            };
            inputStyle(dtpFechaInicial);

            // Saldo Cuenta
            yPos += spacing;
            Label lblSaldoCuenta = new Label { Text = "Saldo Cuenta", Location = new Point(0, yPos) };
            labelStyle(lblSaldoCuenta);

            txtSaldoCuenta = new TextBox
            {
                Location = new Point(0, yPos + 25),
                Width = mainPanel.Width - 40,
                BorderStyle = BorderStyle.FixedSingle
            };
            inputStyle(txtSaldoCuenta);

            // Fecha Final
            yPos += spacing;
            Label lblFechaFinal = new Label { Text = "Fecha Final", Location = new Point(0, yPos) };
            labelStyle(lblFechaFinal);

            dtpFechaFinal = new DateTimePicker
            {
                Location = new Point(0, yPos + 25),
                Width = mainPanel.Width - 40,
                Format = DateTimePickerFormat.Short
            };
            inputStyle(dtpFechaFinal);

            // Estado
            yPos += spacing;
            Label lblEstado = new Label { Text = "Estado", Location = new Point(0, yPos) };
            labelStyle(lblEstado);

            cmbEstado = new ComboBox
            {
                Location = new Point(0, yPos + 25),
                Width = mainPanel.Width - 40,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            inputStyle(cmbEstado);
            cmbEstado.Items.AddRange(new object[] { "Pendiente", "Pagada" });

            // Botón Agregar Cuenta
            yPos += spacing;
            Button btnAgregarCuenta = new Button
            {
                Text = "Agregar Cuenta",
                Location = new Point(0, yPos + 25),
                Width = mainPanel.Width - 40,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };

            // Add hover effect
            btnAgregarCuenta.MouseEnter += (s, e) => {
                btnAgregarCuenta.BackColor = Color.FromArgb(0, 102, 184);
            };
            btnAgregarCuenta.MouseLeave += (s, e) => {
                btnAgregarCuenta.BackColor = Color.FromArgb(0, 122, 204);
            };

            btnAgregarCuenta.Click += (sender, e) => BtnAgregarCuenta_Click(sender, e, cmbCliente, txtArticulo, txtValorCredito,
                                                                           dtpFechaInicial, txtSaldoCuenta, dtpFechaFinal, cmbEstado);

            // Add controls to main panel
            mainPanel.Controls.AddRange(new Control[] {
                lblCliente, cmbCliente,
                lblArticulo, txtArticulo,
                lblValorCredito, txtValorCredito,
                lblFechaInicial, dtpFechaInicial,
                lblSaldoCuenta, txtSaldoCuenta,
                lblFechaFinal, dtpFechaFinal,
                lblEstado, cmbEstado,
                btnAgregarCuenta
            });

            // Add main panel to form
            this.Controls.Add(mainPanel);

            // Load clients
            LoadClientes(cmbCliente);
        }

        private void LoadClientes(ComboBox cmbCliente)
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT NombreRazonSocial FROM Clientes";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        cmbCliente.Items.Clear();

                        while (reader.Read())
                        {
                            cmbCliente.Items.Add(reader["NombreRazonSocial"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregarCuenta_Click(object sender, EventArgs e, ComboBox cmbCliente, TextBox txtArticulo,
            TextBox txtValorCredito, DateTimePicker dtpFechaInicial, TextBox txtSaldoCuenta,
            DateTimePicker dtpFechaFinal, ComboBox cmbEstado)
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
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar la cuenta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}