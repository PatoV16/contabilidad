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

        lblTitulo = new Label
        {
            Text = "Registro de Clientes",
            Font = new System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            Height = 50
        };

        dgvClientes = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,  // Agregar esto
            MultiSelect = false,                                      // Y esto
            ReadOnly = true                                          // Y esto
        };

        Panel buttonPanel = new Panel
        {
            Height = 50,
            Dock = DockStyle.Bottom
        };

        btnAgregarCliente = new Button
        {
            Text = "Agregar Cliente",
            Width = 120,
            Location = new Point(10, 10)
        };
        btnAgregarCliente.Click += BtnAgregarCliente_Click;

        btnEditarCliente = new Button
        {
            Text = "Editar Cliente",
            Width = 120,
            Location = new Point(140, 10)
        };
        btnEditarCliente.Click += BtnEditarCliente_Click;

        btnEliminarCliente = new Button
        {
            Text = "Eliminar Cliente",
            Width = 120,
            Location = new Point(270, 10)
        };
        btnEliminarCliente.Click += BtnEliminarCliente_Click;

        buttonPanel.Controls.AddRange(new Control[] {
            btnAgregarCliente,
            btnEditarCliente,
            btnEliminarCliente
        });

        Panel mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };
        mainPanel.Controls.Add(dgvClientes);

        this.Controls.Add(mainPanel);
        this.Controls.Add(buttonPanel);
        this.Controls.Add(lblTitulo);
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
