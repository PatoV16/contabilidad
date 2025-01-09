using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormIngresosEgresos : Form
    {
        private Label lblTitulo;
        private DataGridView dgvIngresosEgresos;
        private Button btnRegistrarIngresoEgreso;
        private Button btnRegistrarNuevoIngresoEgreso;

        public FormIngresosEgresos()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Ingresos y Egresos";
            this.Size = new System.Drawing.Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Título principal
            lblTitulo = new Label
            {
                Text = "Ingresos y Egresos",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };

            // Configuración del DataGridView para Ingresos y Egresos
            dgvIngresosEgresos = new DataGridView
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

            // Sección para el DataGridView
            Panel ingresosEgresosPanel = CreateTableSection("Ingresos y Egresos", dgvIngresosEgresos);

            // Botón para registrar ingresos/egresos
            Panel buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom
            };

            btnRegistrarIngresoEgreso = new Button
            {
                Text = "Registrar Nuevo Ingreso",
                Width = 200,
                Location = new Point(10, 10)
            };
            btnRegistrarIngresoEgreso.Click += BtnRegistrarIngreso_Click;

            btnRegistrarNuevoIngresoEgreso = new Button
            {
                Text = "Registrar Nueva Salida",
                Width = 200,
                Location = new Point(220, 10)
            };
            btnRegistrarNuevoIngresoEgreso.Click += BtnRegistrarNuevaSalida_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnRegistrarIngresoEgreso, btnRegistrarNuevoIngresoEgreso });

            // Agregar secciones al panel principal
            mainPanel.Controls.Add(ingresosEgresosPanel);

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
                Height = 300
            };

            Label lblSection = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
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
                dgvIngresosEgresos.Columns.Clear();

                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    // Cargar datos de ingresos y egresos
                    string queryIngresosEgresos = @"
                        SELECT Fecha, Concepto, Ingresos, Egresos, Saldo
                        FROM IngresosEgresos";
                    using (SQLiteCommand cmd = new SQLiteCommand(queryIngresosEgresos, conn))
                    {
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvIngresosEgresos.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegistrarIngreso_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario de agregar ingreso
            FormAgregarIngreso formAgregarIngreso = new FormAgregarIngreso();

            // Mostrar el formulario de agregar ingreso
            formAgregarIngreso.ShowDialog();  // Usar ShowDialog para abrirlo como modal (bloqueando la ventana principal)

            // Aquí puedes agregar código para actualizar la vista o realizar otras acciones, si es necesario
            LoadData();
        }

        private void BtnRegistrarNuevaSalida_Click(object sender, EventArgs e)
        {
            // Crear una nueva instancia del formulario de agregar egreso
            FormAgregarNuevoEgreso formAgregarEgreso = new FormAgregarNuevoEgreso();

            // Mostrar el formulario de agregar egreso
            formAgregarEgreso.ShowDialog();  // Usar ShowDialog para abrirlo como modal (bloqueando la ventana principal)

            // Aquí puedes agregar código para actualizar la vista o realizar otras acciones, si es necesario
            LoadData();
        }

    }
}
