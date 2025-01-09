using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormAgregarIngreso : Form
    {
        private Label lblTitulo;
        private TextBox txtFecha;
        private TextBox txtConcepto;
        private TextBox txtMonto;
        private Button btnGuardar;
        private Button btnCancelar;

        public FormAgregarIngreso()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Configuración del formulario
            this.Text = "Agregar Ingreso";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Título del formulario
            lblTitulo = new Label
            {
                Text = "Registrar Nuevo Ingreso",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };

            // Campos para el ingreso de datos
            Label lblFecha = new Label
            {
                Text = "Fecha (YYYY-MM-DD):",
                Location = new Point(20, 70),
                Width = 150
            };
            txtFecha = new TextBox
            {
                Location = new Point(170, 70),
                Width = 200
            };

            Label lblConcepto = new Label
            {
                Text = "Concepto:",
                Location = new Point(20, 110),
                Width = 150
            };
            txtConcepto = new TextBox
            {
                Location = new Point(170, 110),
                Width = 200
            };

            Label lblMonto = new Label
            {
                Text = "Monto:",
                Location = new Point(20, 150),
                Width = 150
            };
            txtMonto = new TextBox
            {
                Location = new Point(170, 150),
                Width = 200
            };

            // Botones para guardar y cancelar
            btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(50, 200),
                Width = 100
            };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(200, 200),
                Width = 100
            };
            btnCancelar.Click += BtnCancelar_Click;

            // Agregar controles al formulario
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblFecha);
            this.Controls.Add(txtFecha);
            this.Controls.Add(lblConcepto);
            this.Controls.Add(txtConcepto);
            this.Controls.Add(lblMonto);
            this.Controls.Add(txtMonto);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validar los datos
            if (string.IsNullOrEmpty(txtFecha.Text) || string.IsNullOrEmpty(txtConcepto.Text) || string.IsNullOrEmpty(txtMonto.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!DateTime.TryParse(txtFecha.Text, out DateTime fecha))
            {
                MessageBox.Show("La fecha ingresada no es válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtMonto.Text, out decimal monto))
            {
                MessageBox.Show("El monto debe ser un número válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Guardar los datos en la base de datos
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    // Obtener el saldo actual acumulado
                    string querySaldo = @"
    SELECT IFNULL(Saldo, 0) 
    FROM IngresosEgresos 
    ORDER BY Fecha DESC, ROWID DESC 
    LIMIT 1";

                    SQLiteCommand cmdSaldo = new SQLiteCommand(querySaldo, conn);
                    decimal saldoActual = Convert.ToDecimal(cmdSaldo.ExecuteScalar());

                    // Insertar el nuevo ingreso
                    string queryIngreso = @"
                INSERT INTO IngresosEgresos (Fecha, Concepto, Ingresos, Egresos, Saldo)
                VALUES (@Fecha, @Concepto, @Ingresos, @Egresos, @Saldo)";

                    using (SQLiteCommand cmd = new SQLiteCommand(queryIngreso, conn))
                    {
                        cmd.Parameters.AddWithValue("@Fecha", fecha.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Concepto", txtConcepto.Text);
                        cmd.Parameters.AddWithValue("@Ingresos", monto);
                        cmd.Parameters.AddWithValue("@Egresos", 0); // Asumiendo que es solo un ingreso
                        cmd.Parameters.AddWithValue("@Saldo", saldoActual + monto); // Actualizamos el saldo

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Ingreso guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Cerrar el formulario después de guardar
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el ingreso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            // Cerrar el formulario sin guardar
            this.Close();
        }
    }
}
