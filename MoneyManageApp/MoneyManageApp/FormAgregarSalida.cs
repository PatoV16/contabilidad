using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormAgregarSalida : Form
    {
        private TextBox txtCodigo;
        private TextBox txtArticulo;
        private NumericUpDown numCantidad;
        private Button btnGuardar;
        private Button btnCancelar;

        public FormAgregarSalida()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Agregar Salida";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblCodigo = new Label
            {
                Text = "Código:",
                Location = new Point(30, 30),
                AutoSize = true
            };

            txtCodigo = new TextBox
            {
                Location = new Point(150, 30),
                Width = 200
            };

            Label lblArticulo = new Label
            {
                Text = "Artículo:",
                Location = new Point(30, 80),
                AutoSize = true
            };

            txtArticulo = new TextBox
            {
                Location = new Point(150, 80),
                Width = 200
            };

            Label lblCantidad = new Label
            {
                Text = "Cantidad:",
                Location = new Point(30, 130),
                AutoSize = true
            };

            numCantidad = new NumericUpDown
            {
                Location = new Point(150, 130),
                Width = 200,
                Minimum = 1,
                Maximum = 10000
            };

            btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(150, 200),
                Width = 80
            };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(250, 200),
                Width = 80
            };
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.Add(lblCodigo);
            this.Controls.Add(txtCodigo);
            this.Controls.Add(lblArticulo);
            this.Controls.Add(txtArticulo);
            this.Controls.Add(lblCantidad);
            this.Controls.Add(numCantidad);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();
            string articulo = txtArticulo.Text.Trim();
            int cantidad = (int)numCantidad.Value;

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(articulo))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    // Verificar si el producto existe y tiene suficiente stock
                    string queryStock = "SELECT Stock FROM Productos WHERE Codigo = @Codigo";
                    using (SQLiteCommand cmd = new SQLiteCommand(queryStock, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        object result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show("El producto no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int stockActual = Convert.ToInt32(result);

                        if (stockActual < cantidad)
                        {
                            MessageBox.Show("No hay suficiente stock disponible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Registrar la salida en la tabla `Salidas`
                    string queryInsertSalida = @"
                        INSERT INTO Salidas (Codigo, Articulo, Fecha, Cantidad)
                        VALUES (@Codigo, @Articulo, @Fecha, @Cantidad)";
                    using (SQLiteCommand cmd = new SQLiteCommand(queryInsertSalida, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        cmd.Parameters.AddWithValue("@Articulo", articulo);
                        cmd.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmd.ExecuteNonQuery();
                    }

                    // Actualizar el stock en la tabla `Productos`
                    string queryUpdateStock = "UPDATE Productos SET Stock = Stock - @Cantidad, Salidas = Salidas + @Cantidad WHERE Codigo = @Codigo";
                    using (SQLiteCommand cmd = new SQLiteCommand(queryUpdateStock, conn))
                    {
                        cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Salida registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la salida: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
