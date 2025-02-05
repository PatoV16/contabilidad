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
        private TextBox txtSearchCuentas;
        private TextBox txtSearchRecaudos;
        private DataTable dtCuentas;
        private DataTable dtRecaudos;

        public FormCuentasPorCobrar()
        {
            InitializeComponent();
            LoadData();
            ConfigureDataGridViewColumns();
        }

        private void ConfigureDataGridViewColumns()
        {
            // Configurar títulos para las columnas de Cuentas por Cobrar
            dgvCuentasPorCobrar.Columns["Cliente"].HeaderText = "Paciente";
            dgvCuentasPorCobrar.Columns["Articulo"].HeaderText = "Tratamiento";
            dgvCuentasPorCobrar.Columns["ValorCredito"].HeaderText = "Valor Crédito";
            dgvCuentasPorCobrar.Columns["FechaInicial"].HeaderText = "Fecha Inicial";
            dgvCuentasPorCobrar.Columns["SaldoCuenta"].HeaderText = "Saldo Cuenta";
            dgvCuentasPorCobrar.Columns["FechaFinal"].HeaderText = "Fecha Final";
            dgvCuentasPorCobrar.Columns["Estado"].HeaderText = "Estado";

            // Hacer la columna SaldoCuenta editable
            dgvCuentasPorCobrar.Columns["SaldoCuenta"].ReadOnly = false;

            // Configurar títulos para las columnas de Recaudos Diarios
            dgvRecaudosDiarios.Columns["RecaudoDiario"].HeaderText = "Recaudo Diario";
            dgvRecaudosDiarios.Columns["FechaCobro"].HeaderText = "Fecha Cobro";
            dgvRecaudosDiarios.Columns["NombreCliente"].HeaderText = "Nombre Cliente";
            dgvRecaudosDiarios.Columns["Concepto"].HeaderText = "Concepto";

            // Agregar evento para guardar cambios en el saldo
            dgvCuentasPorCobrar.CellEndEdit += DgvCuentasPorCobrar_CellEndEdit;
        }

        private void DgvCuentasPorCobrar_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvCuentasPorCobrar.Columns["SaldoCuenta"].Index)
            {
                try
                {
                    // Obtener los valores de las columnas que identifican el registro
                    object clienteValue = dgvCuentasPorCobrar.Rows[e.RowIndex].Cells["Cliente"].Value;
                    object articuloValue = dgvCuentasPorCobrar.Rows[e.RowIndex].Cells["Articulo"].Value;
                    object fechaInicialValue = dgvCuentasPorCobrar.Rows[e.RowIndex].Cells["FechaInicial"].Value;
                    object saldoValue = dgvCuentasPorCobrar.Rows[e.RowIndex].Cells["SaldoCuenta"].Value;

                    // Validar que los valores no sean nulos
                    if (clienteValue == null || articuloValue == null || fechaInicialValue == null || saldoValue == null)
                    {
                        MessageBox.Show("Error: No se pueden obtener los valores necesarios para identificar el registro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Convertir valores
                    decimal nuevoSaldo;
                    if (!decimal.TryParse(saldoValue.ToString(), out nuevoSaldo))
                    {
                        MessageBox.Show("Error: El saldo ingresado no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Actualizar en la base de datos usando las columnas identificadoras
                    using (SQLiteConnection conn = Database.GetConnection())
                    {
                        conn.Open();
                        string updateQuery = @"
                    UPDATE CuentasPorCobrar 
                    SET SaldoCuenta = @Saldo 
                    WHERE Cliente = @Cliente 
                      AND Articulo = @Articulo 
                      AND FechaInicial = @FechaInicial";

                        using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Saldo", nuevoSaldo);
                            cmd.Parameters.AddWithValue("@Cliente", clienteValue.ToString());
                            cmd.Parameters.AddWithValue("@Articulo", articuloValue.ToString());
                            cmd.Parameters.AddWithValue("@FechaInicial", fechaInicialValue.ToString());
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Saldo actualizado correctamente", "Actualización Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar el saldo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Recargar los datos para deshacer el cambio
                    LoadData();
                }
            }
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
                ReadOnly = false
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
            Panel cuentasPanel = CreateTableSection("Cuentas por Cobrar", dgvCuentasPorCobrar, out txtSearchCuentas);
            Panel recaudosPanel = CreateTableSection("Recaudos Diarios", dgvRecaudosDiarios, out txtSearchRecaudos);

            // Configurar eventos de búsqueda
            txtSearchCuentas.TextChanged += TxtSearchCuentas_TextChanged;
            txtSearchRecaudos.TextChanged += TxtSearchRecaudos_TextChanged;

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

            buttonPanel.Controls.AddRange(new Control[] { btnRegistrarCobro, btnRegistrarCuentaPorCobrar });

            // Agregar secciones al panel principal
            mainPanel.Controls.Add(recaudosPanel);
            mainPanel.Controls.Add(cuentasPanel);

            // Agregar elementos al formulario
            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(lblTitulo);
        }

        private Panel CreateTableSection(string labelText, DataGridView dgv, out TextBox searchBox)
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

            // Panel de búsqueda
            Panel searchPanel = new Panel
            {
                Height = 30,
                Dock = DockStyle.Top
            };

            Label lblSearch = new Label
            {
                Text = "Buscar:",
                AutoSize = true,
                Location = new Point(5, 8)
            };

            searchBox = new TextBox
            {
                Width = 200,
                Location = new Point(60, 5)
            };

            searchPanel.Controls.Add(searchBox);
            searchPanel.Controls.Add(lblSearch);

            sectionPanel.Controls.Add(dgv);
            sectionPanel.Controls.Add(searchPanel);
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
                        dtCuentas = new DataTable();
                        adapter.Fill(dtCuentas);
                        dgvCuentasPorCobrar.DataSource = dtCuentas;
                    }

                    // Cargar datos de recaudos diarios
                    string queryRecaudos = @"
                        SELECT RecaudoDiario, FechaCobro, NombreCliente, Concepto 
                        FROM RecaudosDiarios";
                    using (SQLiteCommand cmd = new SQLiteCommand(queryRecaudos, conn))
                    {
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        dtRecaudos = new DataTable();
                        adapter.Fill(dtRecaudos);
                        dgvRecaudosDiarios.DataSource = dtRecaudos;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearchCuentas_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchCuentas.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ((DataTable)dgvCuentasPorCobrar.DataSource).DefaultView.RowFilter = "";
                return;
            }

            string filterExpression = string.Format(
                "Cliente LIKE '%{0}%' OR Articulo LIKE '%{0}%' OR Estado LIKE '%{0}%'",
                searchText.Replace("'", "''"));

            ((DataTable)dgvCuentasPorCobrar.DataSource).DefaultView.RowFilter = filterExpression;
        }

        private void TxtSearchRecaudos_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchRecaudos.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ((DataTable)dgvRecaudosDiarios.DataSource).DefaultView.RowFilter = "";
                return;
            }

            string filterExpression = string.Format(
                "NombreCliente LIKE '%{0}%' OR Concepto LIKE '%{0}%'",
                searchText.Replace("'", "''"));

            ((DataTable)dgvRecaudosDiarios.DataSource).DefaultView.RowFilter = filterExpression;
        }

        private void BtnRegistrarCobro_Click(object sender, EventArgs e)
        {
            FormAgregarCobro formAgregarCobro = new FormAgregarCobro();
            formAgregarCobro.ShowDialog();
            LoadData();
        }

        private void BtnRegistrarCuentaPorCobrar_Click(object sender, EventArgs e)
        {
            FormAgregarCuentaPorCobrar formAgregarCuenta = new FormAgregarCuentaPorCobrar();
            formAgregarCuenta.ShowDialog();
            LoadData();
        }
    }
}