using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormAgregarEntrada : Form
    {
        private Label lblCodigo;
        private Label lblArticulo;
        private Label lblCantidad;
        private TextBox txtCodigo;
        private TextBox txtArticulo;
        private NumericUpDown nudCantidad;
        private Button btnGuardar;
        private Button btnCancelar;

        public FormAgregarEntrada()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Agregar Entrada";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            lblCodigo = new Label
            {
                Text = "Código del Producto:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtCodigo = new TextBox
            {
                Location = new Point(150, 20),
                Width = 200
            };

            lblArticulo = new Label
            {
                Text = "Artículo:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            txtArticulo = new TextBox
            {
                Location = new Point(150, 60),
                Width = 200
            };

            lblCantidad = new Label
            {
                Text = "Cantidad:",
                Location = new Point(20, 100),
                AutoSize = true
            };

            nudCantidad = new NumericUpDown
            {
                Location = new Point(150, 100),
                Width = 200,
                Minimum = 1,
                Maximum = 1000000
            };

            btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(150, 160),
                Width = 80
            };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(240, 160),
                Width = 80
            };
            btnCancelar.Click += (sender, e) => this.Close();

            this.Controls.Add(lblCodigo);
            this.Controls.Add(txtCodigo);
            this.Controls.Add(lblArticulo);
            this.Controls.Add(txtArticulo);
            this.Controls.Add(lblCantidad);
            this.Controls.Add(nudCantidad);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();
            string articulo = txtArticulo.Text.Trim();
            int cantidad = (int)nudCantidad.Value;

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

                    // Verificar si el producto existe en la tabla Productos
                    string queryCheckProducto = "SELECT COUNT(*) FROM Productos WHERE Codigo = @Codigo";
                    using (var cmdCheck = new SQLiteCommand(queryCheckProducto, connection))
                    {
                        cmdCheck.Parameters.AddWithValue("@Codigo", codigo);
                        long count = (long)cmdCheck.ExecuteScalar();

                        if (count == 0)
                        {
                            MessageBox.Show("El producto no existe en el inventario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Insertar la entrada en la tabla Entradas
                    string queryInsertEntrada = @"INSERT INTO Entradas (Codigo, Articulo, Fecha, Cantidad) 
                                                    VALUES (@Codigo, @Articulo, @Fecha, @Cantidad)";
                    using (var cmdInsert = new SQLiteCommand(queryInsertEntrada, connection))
                    {
                        cmdInsert.Parameters.AddWithValue("@Codigo", codigo);
                        cmdInsert.Parameters.AddWithValue("@Articulo", articulo);
                        cmdInsert.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmdInsert.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmdInsert.ExecuteNonQuery();
                    }

                    // Actualizar el stock en la tabla Productos
                    string queryUpdateStock = "UPDATE Productos SET Entradas = Entradas + @Cantidad, Stock = Stock + @Cantidad WHERE Codigo = @Codigo";
                    using (var cmdUpdate = new SQLiteCommand(queryUpdateStock, connection))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmdUpdate.Parameters.AddWithValue("@Codigo", codigo);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Entrada registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la entrada: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
