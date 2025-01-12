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
        private ComboBox cmbCodigo;
        private ComboBox cmbArticulo;
        private NumericUpDown nudCantidad;
        private Button btnGuardar;
        private Button btnCancelar;

        public FormAgregarEntrada()
        {
            InitializeComponent();
            LoadComboBoxData();
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

                    // Cargar códigos de productos en el ComboBox
                    string queryCodigos = "SELECT Codigo, Articulo FROM Productos";
                    using (var cmd = new SQLiteCommand(queryCodigos, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string codigo = reader["Codigo"].ToString();
                            string articulo = reader["Articulo"].ToString();

                            cmbCodigo.Items.Add(codigo);
                            cmbArticulo.Items.Add(articulo);
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

            string selectedCodigo = cmbCodigo.SelectedItem.ToString();

            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT Articulo FROM Productos WHERE Codigo = @Codigo";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", selectedCodigo);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string articulo = reader["Articulo"].ToString();
                                cmbArticulo.SelectedItem = articulo;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbArticulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbArticulo.SelectedItem == null)
                return;

            string selectedArticulo = cmbArticulo.SelectedItem.ToString();

            try
            {
                using (var connection = Database.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT Codigo FROM Productos WHERE Articulo = @Articulo";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Articulo", selectedArticulo);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string codigo = reader["Codigo"].ToString();
                                cmbCodigo.SelectedItem = codigo;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el código: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    MessageBox.Show("Entrada registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la entrada: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
