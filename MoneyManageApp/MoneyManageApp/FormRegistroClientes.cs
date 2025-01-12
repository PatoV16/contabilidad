using System.Data.SQLite;
using System.Data;
using System.Windows.Forms;
using System;
using MoneyManageApp;
using System.Drawing;

internal class FormRegistroClientes : Form
{
    private Label lblTitulo;
    private DataGridView dgvClientes;
    private Button btnAgregarCliente;
    private Button btnEditarCliente;
    private Button btnEliminarCliente;
    private TextBox txtBuscar;
    private ComboBox cmbCriterioBusqueda;

    public FormRegistroClientes()
    {
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        this.Text = "Registro de Clientes";
        this.Size = new System.Drawing.Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.WhiteSmoke;

        // Título
        lblTitulo = new Label
        {
            Text = "Registro de Clientes",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            Height = 50,
            ForeColor = Color.FromArgb(64, 64, 64)
        };

        // Panel de búsqueda
        Panel searchPanel = new Panel
        {
            Height = 60,
            Dock = DockStyle.Top,
            Padding = new Padding(10),
            BackColor = Color.White
        };

        // ComboBox para criterio de búsqueda
        cmbCriterioBusqueda = new ComboBox
        {
            Width = 150,
            Location = new Point(10, 15),
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f)
        };
        cmbCriterioBusqueda.Items.AddRange(new string[] {
            "Cédula/RUC",
            "Nombre/Razón Social",
            "Ciudad",
            "Teléfono"
        });
        cmbCriterioBusqueda.SelectedIndex = 0;

        // TextBox de búsqueda
        txtBuscar = new TextBox
        {
            Width = 300,
            Location = new Point(170, 15),
            Font = new Font("Segoe UI", 9.5f),
            BorderStyle = BorderStyle.FixedSingle
        };
        txtBuscar.TextChanged += TxtBuscar_TextChanged;

        searchPanel.Controls.AddRange(new Control[] { cmbCriterioBusqueda, txtBuscar });

        // DataGridView
        dgvClientes = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            AllowUserToResizeRows = false
        };

        // Estilo del DataGridView
        dgvClientes.DefaultCellStyle.Font = new Font("Segoe UI", 9);
        dgvClientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        dgvClientes.EnableHeadersVisualStyles = false;
        dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
        dgvClientes.ColumnHeadersHeight = 32;

        // Panel de botones
        Panel buttonPanel = new Panel
        {
            Height = 60,
            Dock = DockStyle.Bottom,
            Padding = new Padding(10),
            BackColor = Color.White
        };

        // Estilo común para botones
        var buttonStyle = new Action<Button>((btn) => {
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = new Font("Segoe UI", 9);
            btn.Height = 35;
            btn.Cursor = Cursors.Hand;
        });

        btnAgregarCliente = new Button
        {
            Text = "Agregar Cliente",
            Width = 120,
            Location = new Point(10, 12),
            BackColor = Color.FromArgb(0, 122, 204),
            ForeColor = Color.White
        };
        buttonStyle(btnAgregarCliente);
        btnAgregarCliente.Click += BtnAgregarCliente_Click;

        btnEditarCliente = new Button
        {
            Text = "Editar Cliente",
            Width = 120,
            Location = new Point(140, 12),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(0, 122, 204)
        };
        buttonStyle(btnEditarCliente);
        btnEditarCliente.Click += BtnEditarCliente_Click;

        btnEliminarCliente = new Button
        {
            Text = "Eliminar Cliente",
            Width = 120,
            Location = new Point(270, 12),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(0, 122, 204)
        };
        buttonStyle(btnEliminarCliente);
        btnEliminarCliente.Click += BtnEliminarCliente_Click;

        buttonPanel.Controls.AddRange(new Control[] {
            btnAgregarCliente,
            btnEditarCliente,
            btnEliminarCliente
        });

        // Panel principal
        Panel mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            BackColor = Color.White
        };
        mainPanel.Controls.Add(dgvClientes);

        // Agregar controles al formulario
        this.Controls.Add(mainPanel);
        this.Controls.Add(buttonPanel);
        this.Controls.Add(searchPanel);
        this.Controls.Add(lblTitulo);
    }

    private void TxtBuscar_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string searchText = txtBuscar.Text.Trim();
            string searchCriteria = cmbCriterioBusqueda.SelectedItem.ToString();

            string columnName;

            if (searchCriteria == "Cédula/RUC")
            {
                columnName = "CedulaRuc";
            }
            else if (searchCriteria == "Nombre/Razón Social")
            {
                columnName = "NombreRazonSocial";
            }
            else if (searchCriteria == "Ciudad")
            {
                columnName = "Ciudad";
            }
            else if (searchCriteria == "Teléfono")
            {
                columnName = "Telefono";
            }
            else
            {
                columnName = "NombreRazonSocial";
            }


            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = $@"
                    SELECT CedulaRuc, NombreRazonSocial, Direccion, Ciudad, Telefono, CorreoElectronico 
                    FROM Clientes 
                    WHERE {columnName} LIKE @SearchTerm
                ";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchText}%");
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvClientes.DataSource = dt;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al buscar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAgregarCliente_Click(object sender, EventArgs e)
    {
        // Crear el formulario de agregar cliente
        FormAgregarCliente formAgregarCliente = new FormAgregarCliente();

        // Mostrar el formulario de agregar cliente como modal
        if (formAgregarCliente.ShowDialog() == DialogResult.OK)
        {
            // Obtener los datos del cliente desde el formulario
            var clienteData = formAgregarCliente.GetClienteData();

            // Llamar a la función para agregar el cliente a la base de datos
            AddClienteToDatabase(clienteData.cedulaRuc, clienteData.nombre, clienteData.direccion,
                                 clienteData.ciudad, clienteData.telefono, clienteData.correo);

            // Recargar los datos en el DataGridView
            LoadData();
        }
    }

    private void BtnEditarCliente_Click(object sender, EventArgs e)
    {
        if (dgvClientes.CurrentRow == null)
        {
            MessageBox.Show("Por favor seleccione un cliente para editar.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string cedulaRuc = dgvClientes.CurrentRow.Cells["CedulaRuc"].Value?.ToString();
        if (string.IsNullOrEmpty(cedulaRuc))
        {
            MessageBox.Show("Error al obtener la información del cliente.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using (var formEditarCliente = new FormEditarCliente(cedulaRuc))
        {
            if (formEditarCliente.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }
    }



    private void BtnEliminarCliente_Click(object sender, EventArgs e)
    {
        // Aquí implementas la lógica para eliminar un cliente
        if (dgvClientes.SelectedRows.Count > 0)
        {
            string cedulaRuc = dgvClientes.SelectedRows[0].Cells["CedulaRuc"].Value.ToString();
            DeleteClienteFromDatabase(cedulaRuc);
            LoadData();
        }
    }

    private void LoadData()
    {
        try
        {
            // Limpiar las columnas predefinidas antes de cargar los nuevos datos
            dgvClientes.Columns.Clear();

    

            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = "SELECT CedulaRuc, NombreRazonSocial, Direccion, Ciudad, Telefono, CorreoElectronico FROM Clientes";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Asignar los datos al DataGridView
                    dgvClientes.DataSource = dt;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AddClienteToDatabase(string cedulaRuc, string nombre, string direccion, string ciudad, string telefono, string correo)
    {
        try
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Clientes (CedulaRuc, NombreRazonSocial, Direccion, Ciudad, Telefono, CorreoElectronico) VALUES (@CedulaRuc, @Nombre, @Direccion, @Ciudad, @Telefono, @Correo)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CedulaRuc", cedulaRuc);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Direccion", direccion);
                    cmd.Parameters.AddWithValue("@Ciudad", ciudad);
                    cmd.Parameters.AddWithValue("@Telefono", telefono);
                    cmd.Parameters.AddWithValue("@Correo", correo);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al agregar cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteClienteFromDatabase(string cedulaRuc)
    {
        try
        {
            using (SQLiteConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Clientes WHERE CedulaRuc = @CedulaRuc";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CedulaRuc", cedulaRuc);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al eliminar cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
