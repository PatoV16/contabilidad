using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormAgregarSalida : Form
    {
        private Label lblCodigo;
        private Label lblArticulo;
        private Label lblCantidad;
        private ComboBox cmbCodigo;
        private ComboBox cmbArticulo;
        private NumericUpDown nudCantidad;
        private Button btnGuardar;
        private Button btnCancelar;

        public FormAgregarSalida()
        {
            InitializeComponent();
            LoadComboBoxData();
        }

        private void InitializeComponent()
        {
            this.Text = "Agregar Salida";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            lblCodigo = new Label
            {
                Text = "Código del Producto:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            cmbCodigo = new ComboBox
            {
                Location = new Point(150, 20),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCodigo.SelectedIndexChanged += CmbCodigo_SelectedIndexChanged;

            lblArticulo = new Label
            {
                Text = "Artículo:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            cmbArticulo = new ComboBox
            {
                Location = new Point(150, 60),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbArticulo.SelectedIndexChanged += CmbArticulo_SelectedIndexChanged;

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
                Maximum = 10000
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
            this.Controls.Add(cmbCodigo);
            this.Controls.Add(lblArticulo);
            this.Controls.Add(cmbArticulo);
            this.Controls.Add(lblCantidad);
            this.Controls.Add(nudCantidad);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }

        private void LoadComboBoxData()
        {
            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT Codigo, Articulo FROM Productos";
                    using (var cmd = new SQLiteCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbCodigo.Items.Add(reader["Codigo"].ToString());
                            cmbArticulo.Items.Add(reader["Articulo"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbCodigo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCodigo.SelectedItem == null)
                return;

            string codigo = cmbCodigo.SelectedItem.ToString();
            UpdateArticuloFromCodigo(codigo);
        }

        private void CmbArticulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbArticulo.SelectedItem == null)
                return;

            string articulo = cmbArticulo.SelectedItem.ToString();
            UpdateCodigoFromArticulo(articulo);
        }

        private void UpdateArticuloFromCodigo(string codigo)
        {
            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT Articulo FROM Productos WHERE Codigo = @Codigo";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                cmbArticulo.SelectedItem = reader["Articulo"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al sincronizar el artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCodigoFromArticulo(string articulo)
        {
            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT Codigo FROM Productos WHERE Articulo = @Articulo";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Articulo", articulo);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                cmbCodigo.SelectedItem = reader["Codigo"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al sincronizar el código: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string codigo = cmbCodigo.SelectedItem?.ToString();
            string articulo = cmbArticulo.SelectedItem?.ToString();
            int cantidad = (int)nudCantidad.Value;

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(articulo))
            {
                MessageBox.Show("Por favor, seleccione un código y un artículo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();

                    string queryStock = "SELECT Stock FROM Productos WHERE Codigo = @Codigo";
                    using (var cmd = new SQLiteCommand(queryStock, connection))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        object stockResult = cmd.ExecuteScalar();

                        if (stockResult == null || Convert.ToInt32(stockResult) < cantidad)
                        {
                            MessageBox.Show("Stock insuficiente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    string querySalida = @"INSERT INTO Salidas (Codigo, Articulo, Fecha, Cantidad) 
                                           VALUES (@Codigo, @Articulo, @Fecha, @Cantidad)";
                    using (var cmd = new SQLiteCommand(querySalida, connection))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        cmd.Parameters.AddWithValue("@Articulo", articulo);
                        cmd.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmd.ExecuteNonQuery();
                    }

                    string queryUpdateStock = "UPDATE Productos SET Stock = Stock - @Cantidad WHERE Codigo = @Codigo";
                    using (var cmd = new SQLiteCommand(queryUpdateStock, connection))
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
