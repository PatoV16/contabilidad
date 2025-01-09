using MoneyManageApp.MoneyManageApp;
using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormMenu : Form
    {
        private Label lblTitulo;
        private string nombreNegocio;

        public FormMenu()
        {
            // Cargar el nombre del negocio guardado o usar el valor por defecto
            CargarNombreNegocio();
            InitializeComponent();
        }

        private void CargarNombreNegocio()
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "CREATE TABLE IF NOT EXISTS ConfiguracionNegocio (Nombre TEXT);";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = "SELECT Nombre FROM ConfiguracionNegocio LIMIT 1;";
                    cmd = new SQLiteCommand(query, conn);
                    object result = cmd.ExecuteScalar();

                    nombreNegocio = result?.ToString() ?? "Nombre del Negocio";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el nombre del negocio: " + ex.Message);
                nombreNegocio = "Nombre del Negocio";
            }
        }

        private void GuardarNombreNegocio(string nuevoNombre)
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM ConfiguracionNegocio; INSERT INTO ConfiguracionNegocio (Nombre) VALUES (@nombre);";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", nuevoNombre);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el nombre del negocio: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.Text = $"Menú Principal - {nombreNegocio}";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.LightBlue;

            // Crear etiqueta para el título
            lblTitulo = new Label();
            lblTitulo.Text = nombreNegocio;
            lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Height = 60;
            lblTitulo.Click += LblTitulo_Click;
            this.Controls.Add(lblTitulo);

            // Botón para editar nombre (opcional - puedes usar solo el click en el título)
            Button btnEditarNombre = new Button();
            btnEditarNombre.Text = "Editar Nombre";
            btnEditarNombre.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnEditarNombre.Size = new System.Drawing.Size(100, 30);
            btnEditarNombre.Location = new System.Drawing.Point(680, 15);
            btnEditarNombre.Click += LblTitulo_Click;
            this.Controls.Add(btnEditarNombre);

            // Resto de los botones...
            Button btnIngresosEgresos = new Button();
            btnIngresosEgresos.Text = "Control de Ingresos y Egresos";
            btnIngresosEgresos.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            btnIngresosEgresos.Size = new System.Drawing.Size(250, 50);
            btnIngresosEgresos.Location = new System.Drawing.Point(275, 150);
            btnIngresosEgresos.Click += BtnIngresosEgresos_Click;
            this.Controls.Add(btnIngresosEgresos);

            Button btnCuentasPorCobrar = new Button();
            btnCuentasPorCobrar.Text = "Control de Cuentas por Cobrar";
            btnCuentasPorCobrar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            btnCuentasPorCobrar.Size = new System.Drawing.Size(250, 50);
            btnCuentasPorCobrar.Location = new System.Drawing.Point(275, 220);
            btnCuentasPorCobrar.Click += BtnCuentasPorCobrar_Click;
            this.Controls.Add(btnCuentasPorCobrar);

            Button btnInventario = new Button();
            btnInventario.Text = "Control de Inventario";
            btnInventario.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            btnInventario.Size = new System.Drawing.Size(250, 50);
            btnInventario.Location = new System.Drawing.Point(275, 290);
            btnInventario.Click += BtnInventario_Click;
            this.Controls.Add(btnInventario);

            Button btnRegistroClientes = new Button();
            btnRegistroClientes.Text = "Registro de Clientes";
            btnRegistroClientes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            btnRegistroClientes.Size = new System.Drawing.Size(250, 50);
            btnRegistroClientes.Location = new System.Drawing.Point(275, 360);
            btnRegistroClientes.Click += BtnRegistroClientes_Click;
            this.Controls.Add(btnRegistroClientes);

            Button btnBorrarBD = new Button();
            btnBorrarBD.Text = "Borrar Base de Datos";
            btnBorrarBD.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            btnBorrarBD.Size = new System.Drawing.Size(250, 50);
            btnBorrarBD.Location = new System.Drawing.Point(275, 430);
            btnBorrarBD.Click += BtnBorrarBD_Click;
            this.Controls.Add(btnBorrarBD);
        }

        private void LblTitulo_Click(object sender, EventArgs e)
        {
            using (Form inputForm = new Form())
            {
                inputForm.Text = "Editar Nombre del Negocio";
                inputForm.Size = new System.Drawing.Size(400, 150);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                TextBox txtNombre = new TextBox();
                txtNombre.Location = new System.Drawing.Point(20, 20);
                txtNombre.Size = new System.Drawing.Size(340, 25);
                txtNombre.Text = nombreNegocio;
                inputForm.Controls.Add(txtNombre);

                Button btnGuardar = new Button();
                btnGuardar.Text = "Guardar";
                btnGuardar.DialogResult = DialogResult.OK;
                btnGuardar.Location = new System.Drawing.Point(285, 60);
                inputForm.Controls.Add(btnGuardar);

                Button btnCancelar = new Button();
                btnCancelar.Text = "Cancelar";
                btnCancelar.DialogResult = DialogResult.Cancel;
                btnCancelar.Location = new System.Drawing.Point(190, 60);
                inputForm.Controls.Add(btnCancelar);

                inputForm.AcceptButton = btnGuardar;
                inputForm.CancelButton = btnCancelar;

                if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    nombreNegocio = txtNombre.Text.Trim();
                    lblTitulo.Text = nombreNegocio;
                    this.Text = $"Menú Principal - {nombreNegocio}";
                    GuardarNombreNegocio(nombreNegocio);
                }
            }
        }

        // Mantener el resto de los métodos existentes...
        private void BtnIngresosEgresos_Click(object sender, EventArgs e)
        {
            FormIngresosEgresos form = new FormIngresosEgresos();
            form.ShowDialog();
        }

        private void BtnCuentasPorCobrar_Click(object sender, EventArgs e)
        {
            FormCuentasPorCobrar form = new FormCuentasPorCobrar();
            form.ShowDialog();
        }

        private void BtnInventario_Click(object sender, EventArgs e)
        {
            FormInventario form = new FormInventario();
            form.ShowDialog();
        }

        private void BtnRegistroClientes_Click(object sender, EventArgs e)
        {
            FormRegistroClientes form = new FormRegistroClientes();
            form.ShowDialog();
        }

        private void BtnBorrarBD_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Estás seguro de que deseas borrar todos los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SQLiteConnection conn = Database.GetConnection())
                    {
                        conn.Open();
                        string queryBorrar = "DELETE FROM IngresosEgresos; DELETE FROM CuentasPorCobrar; DELETE FROM Inventario; DELETE FROM Clientes; DELETE FROM Productos; DELETE FROM Entradas; DELETE FROM Salidas; DELETE FROM RecaudosDiarios;";
                        SQLiteCommand cmd = new SQLiteCommand(queryBorrar, conn);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Los registros han sido borrados exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al borrar los registros: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}