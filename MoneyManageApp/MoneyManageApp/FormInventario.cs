using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManageApp
{
    using System;
    using System.Data.SQLite;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;

    namespace MoneyManageApp
    {
        internal class FormInventario : Form
        {
            private Label lblTitulo;
            private DataGridView dgvProductos;
            private DataGridView dgvEntradas;
            private DataGridView dgvSalidas;
            private Button btnAgregarEntrada;
            private Button btnAgregarSalida;
            private Button btnAgregarProducto;

            public FormInventario()
            {
                InitializeComponent();
                LoadData();
            }

            private void InitializeComponent()
            {
                this.Text = "Gestión de Inventario";
                this.Size = new System.Drawing.Size(1000, 800);
                this.StartPosition = FormStartPosition.CenterScreen;

                lblTitulo = new Label
                {
                    Text = "Gestión de Inventario",
                    Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    Height = 50
                };

                // Crear los DataGridView de clase
                dgvProductos = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    ReadOnly = true
                };

                dgvEntradas = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    ReadOnly = true
                };

                dgvSalidas = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    ReadOnly = true
                };

                // Crear panel principal con scroll
                Panel mainPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true
                };

                // Contenedor para lista de productos
                Panel productosPanel = CreateTableSection("Lista de Productos", dgvProductos);

                // Contenedor para entradas
                Panel entradasPanel = CreateTableSection("Registro de Entradas", dgvEntradas);

                // Contenedor para salidas
                Panel salidasPanel = CreateTableSection("Registro de Salidas", dgvSalidas);

                // Panel de botones
                Panel buttonPanel = new Panel
                {
                    Height = 50,
                    Dock = DockStyle.Bottom
                };

                btnAgregarProducto = new Button
                {
                    Text = "Agregar Producto",
                    Width = 120,
                    Location = new Point(280, 10)
                };
                btnAgregarProducto.Click += BtnAgregarProducto_Click;

                btnAgregarEntrada = new Button
                {
                    Text = "Agregar Entrada",
                    Width = 120,
                    Location = new Point(10, 10)
                };
                btnAgregarEntrada.Click += BtnAgregarEntrada_Click;

                btnAgregarSalida = new Button
                {
                    Text = "Agregar Salida",
                    Width = 120,
                    Location = new Point(140, 10)
                };
                btnAgregarSalida.Click += BtnAgregarSalida_Click;

                buttonPanel.Controls.AddRange(new Control[] { btnAgregarEntrada, btnAgregarSalida, btnAgregarProducto });


                // Agregar secciones al panel principal
                mainPanel.Controls.Add(salidasPanel);
                mainPanel.Controls.Add(entradasPanel);
                mainPanel.Controls.Add(productosPanel);

                // Agregar elementos al formulario
                this.Controls.Add(mainPanel);
                this.Controls.Add(buttonPanel);
                this.Controls.Add(lblTitulo);
            }

            private Panel CreateTableSection(string labelText, DataGridView dgv)
            {
                Panel sectionPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 260
                };

                Label lblSection = new Label
                {
                    Text = labelText,
                    Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold),
                    Height = 30,
                    Dock = DockStyle.Top
                };

                sectionPanel.Controls.Add(dgv);
                sectionPanel.Controls.Add(lblSection);

                return sectionPanel;
            }



            private void LoadData()
            {
                try
                {
                    dgvProductos.Columns.Clear();
                    dgvEntradas.Columns.Clear();
                    dgvSalidas.Columns.Clear();

                    using (SQLiteConnection conn = Database.GetConnection())
                    {
                        conn.Open();

                        // Cargar datos de productos
                        string queryProductos = "SELECT Codigo, Articulo, Entradas, Salidas, Stock FROM Productos";
                        using (SQLiteCommand cmd = new SQLiteCommand(queryProductos, conn))
                        {
                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvProductos.DataSource = dt;
                        }

                        // Cargar datos de entradas
                        string queryEntradas = "SELECT Codigo, Articulo, Fecha, Cantidad FROM Entradas";
                        using (SQLiteCommand cmd = new SQLiteCommand(queryEntradas, conn))
                        {
                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvEntradas.DataSource = dt;
                        }

                        // Cargar datos de salidas
                        string querySalidas = "SELECT Codigo, Articulo, Fecha, Cantidad FROM Salidas";
                        using (SQLiteCommand cmd = new SQLiteCommand(querySalidas, conn))
                        {
                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvSalidas.DataSource = dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            private void BtnAgregarProducto_Click(object sender, EventArgs e)
            {
                FormAgregarProducto formAgregarProducto = new FormAgregarProducto();
                formAgregarProducto.ShowDialog();

                // Recargar datos después de agregar un nuevo producto
                LoadData();
            }

            private void BtnAgregarEntrada_Click(object sender, EventArgs e)
            {
                // Crear una instancia del formulario de agregar entrada
                FormAgregarEntrada formAgregarEntrada = new FormAgregarEntrada();

                // Mostrar el formulario de forma modal
                formAgregarEntrada.ShowDialog();

                // Recargar datos después de cerrar el formulario (opcional)
                LoadData();
            }


            private void BtnAgregarSalida_Click(object sender, EventArgs e)
            {
                // Implementar lógica para agregar una nueva salida
                FormAgregarSalida formAgregarSalida = new FormAgregarSalida();
                formAgregarSalida.ShowDialog();

                // Recargar los datos después de registrar la salida
                LoadData();
            }
        }
    }

}
