    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Drawing;
    using System.Windows.Forms;

    namespace MoneyManageApp
    {
        internal class FormCuentasPorCobrar : Form
        {
            private Label lblTitulo;
            private DataGridView dgvCuentasPorCobrar;
            private DataGridView dgvRecaudosDiarios;
            private Button btnRegistrarCobro;
            private Button btnRegistrarCuentaPorCobrar;

            public FormCuentasPorCobrar()
            {
                InitializeComponent();
                LoadData();
            }

            private void InitializeComponent()
            {
                this.Text = "Gestión de Cuentas por Cobrar";
                this.Size = new System.Drawing.Size(1000, 800);
                this.StartPosition = FormStartPosition.CenterScreen;

                // Título principal
                lblTitulo = new Label
                {
                    Text = "Gestión de Cuentas por Cobrar",
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    Height = 50
                };

                // Configuración principal de DataGridViews
                dgvCuentasPorCobrar = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    ReadOnly = true
                };

                dgvRecaudosDiarios = new DataGridView
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

                // Secciones de datos
                Panel cuentasPanel = CreateTableSection("Cuentas por Cobrar", dgvCuentasPorCobrar);
                Panel recaudosPanel = CreateTableSection("Recaudos Diarios", dgvRecaudosDiarios);

                // Botón para registrar cobros
                Panel buttonPanel = new Panel
                {
                    Height = 50,
                    Dock = DockStyle.Bottom
                };

                btnRegistrarCobro = new Button
                {
                    Text = "Registrar Cobro",
                    Width = 150,
                    Location = new Point(10, 10)
                };
                btnRegistrarCobro.Click += BtnRegistrarCobro_Click;

                btnRegistrarCuentaPorCobrar = new Button
                {
                    Text = "Registrar nueva cuenta",
                    Width = 150,
                    Location = new Point(170, 10)
                };
                btnRegistrarCuentaPorCobrar.Click += BtnRegistrarCuentaPorCobrar_Click;

                buttonPanel.Controls.AddRange( new Control[] {btnRegistrarCobro, btnRegistrarCuentaPorCobrar});

                // Agregar secciones al panel principal
                mainPanel.Controls.Add(recaudosPanel);
                mainPanel.Controls.Add(cuentasPanel);

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
                    dgvCuentasPorCobrar.Columns.Clear();
                    dgvRecaudosDiarios.Columns.Clear();

                    using (SQLiteConnection conn = Database.GetConnection())
                    {
                        conn.Open();

                        // Cargar datos de cuentas por cobrar
                        string queryCuentas = @"
                            SELECT Cliente, Articulo, ValorCredito, FechaInicial, SaldoCuenta, FechaFinal, Estado 
                            FROM CuentasPorCobrar";
                        using (SQLiteCommand cmd = new SQLiteCommand(queryCuentas, conn))
                        {
                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvCuentasPorCobrar.DataSource = dt;
                        }

                        // Cargar datos de recaudos diarios
                        string queryRecaudos = @"
                            SELECT RecaudoDiario, FechaCobro 
                            FROM RecaudosDiarios";
                        using (SQLiteCommand cmd = new SQLiteCommand(queryRecaudos, conn))
                        {
                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvRecaudosDiarios.DataSource = dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnRegistrarCobro_Click(object sender, EventArgs e)
            {
                // Crear una nueva instancia del formulario de agregar cuenta por cobrar
                FormAgregarCobro formAgregarCobro = new FormAgregarCobro();

                // Mostrar el formulario de agregar cuenta por cobrar
                formAgregarCobro.ShowDialog();  // Usar ShowDialog para abrirlo como modal (bloqueando la ventana principal)
                LoadData();
            }
            private void BtnRegistrarCuentaPorCobrar_Click(object sender, EventArgs e)
            {
                // Crear una nueva instancia del formulario de agregar cuenta por cobrar
                FormAgregarCuentaPorCobrar formAgregarCuenta = new FormAgregarCuentaPorCobrar();

                // Mostrar el formulario de agregar cuenta por cobrar
                formAgregarCuenta.ShowDialog();  // Usar ShowDialog para abrirlo como modal (bloqueando la ventana principal)
                LoadData();
            }

        }
    }
