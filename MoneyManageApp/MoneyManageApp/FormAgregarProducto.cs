using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormAgregarProducto : Form
    {
        private Label lblTitulo;
        private TextBox txtCodigo;
        private TextBox txtArticulo;
        private NumericUpDown nudStockInicial;
        private Button btnGuardar;
        private Button btnCancelar;

        public FormAgregarProducto()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Agregar Producto";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            lblTitulo = new Label
            {
                Text = "Agregar Nuevo Producto",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 40
            };

            Label lblCodigo = new Label
            {
                Text = "Código:",
                Location = new Point(30, 60),
                Width = 100
            };

            txtCodigo = new TextBox
            {
                Location = new Point(140, 60),
                Width = 200
            };

            Label lblArticulo = new Label
            {
                Text = "Artículo:",
                Location = new Point(30, 100),
                Width = 100
            };

            txtArticulo = new TextBox
            {
                Location = new Point(140, 100),
                Width = 200
            };

            Label lblStockInicial = new Label
            {
                Text = "Stock Inicial:",
                Location = new Point(30, 140),
                Width = 100
            };

            nudStockInicial = new NumericUpDown
            {
                Location = new Point(140, 140),
                Width = 200,
                Minimum = 0,
                Maximum = 100000,
                Value = 0
            };

            btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(140, 200),
                Width = 80
            };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(240, 200),
                Width = 80
            };
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblCodigo);
            this.Controls.Add(txtCodigo);
            this.Controls.Add(lblArticulo);
            this.Controls.Add(txtArticulo);
            this.Controls.Add(lblStockInicial);
            this.Controls.Add(nudStockInicial);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();
            string articulo = txtArticulo.Text.Trim();
            int stockInicial = (int)nudStockInicial.Value;

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(articulo))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO Productos (Codigo, Articulo, Entradas, Salidas, Stock) VALUES (@Codigo, @Articulo, @Entradas, @Salidas, @Stock)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Codigo", codigo);
                        command.Parameters.AddWithValue("@Articulo", articulo);
                        command.Parameters.AddWithValue("@Entradas", stockInicial);
                        command.Parameters.AddWithValue("@Salidas", 0);
                        command.Parameters.AddWithValue("@Stock", stockInicial);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
