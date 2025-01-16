using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace MoneyManageApp
{
    public partial class FormCitas : Form
    {
        private DataGridView dgvCitas;
        private ComboBox cmbEstadoFiltro;
        private Button btnRefresh;
        private Button btnNuevaCita;
        private Panel panelBotones;
        private Panel panelGrid;

        public FormCitas()
        {
            CreateControls();
            InitializeComponent();
        }

        private void CreateControls()
        {
            // DataGridView para mostrar citas
            dgvCitas = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = false, // Permitir edición
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = true,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false,
                BackgroundColor = System.Drawing.SystemColors.Window,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };
            dgvCitas.CellValueChanged += DgvCitas_CellValueChanged;

            // ComboBox para filtrar por estado
            cmbEstadoFiltro = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 150,
                Location = new System.Drawing.Point(500, 10)
            };
            cmbEstadoFiltro.Items.AddRange(new string[] { "Todos", "Pendiente", "Realizada" });
            cmbEstadoFiltro.SelectedIndex = 0;
            cmbEstadoFiltro.SelectedIndexChanged += CmbEstadoFiltro_SelectedIndexChanged;

            // Botón de refrescar
            btnRefresh = new Button
            {
                Text = "Refrescar",
                Size = new System.Drawing.Size(100, 30), // Ajusta el tamaño del botón
                Location = new System.Drawing.Point(10, 10) // Posición en el panel
            };
            btnRefresh.Click += BtnRefresh_Click;

            // Botón para nueva cita
            btnNuevaCita = new Button
            {
                Text = "Nueva Cita",
                Size = new System.Drawing.Size(100, 30), // Igual tamaño que el botón anterior
                Location = new System.Drawing.Point(120, 10) // Posición a la derecha del primer botón
            };
            btnNuevaCita.Click += BtnNuevaCita_Click;

            // Panel de botones
            panelBotones = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = System.Drawing.Color.WhiteSmoke
            };
            panelBotones.Controls.Add(cmbEstadoFiltro);
            panelBotones.Controls.Add(btnRefresh);
            panelBotones.Controls.Add(btnNuevaCita);

            // Panel del grid
            panelGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            panelGrid.Controls.Add(dgvCitas);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Control de Citas";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Controls.Add(panelGrid);
            this.Controls.Add(panelBotones);

            this.ResumeLayout(false);

            this.Load += FormCitas_Load;
        }

        private void ConfigureColumns()
        {
            if (dgvCitas?.Columns == null || dgvCitas.Columns.Count == 0)
                return;

            foreach (DataGridViewColumn column in dgvCitas.Columns)
            {
                if (column == null) continue;

                switch (column.Name)
                {
                    case "Id":
                        column.Width = 50;
                        column.HeaderText = "ID";
                        column.ReadOnly = true;
                        break;
                    case "Cliente":
                        column.Width = 200;
                        column.HeaderText = "Cliente";
                        column.ReadOnly = true;
                        break;
                    case "Fecha":
                        column.Width = 150;
                        column.HeaderText = "Fecha";
                        column.ReadOnly = true;
                        break;
                    case "Estado":
                        column.Width = 100;
                        column.HeaderText = "Estado";
                        column.ReadOnly = false; // Permitir edición
                        break;
                    case "Concepto":
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        column.HeaderText = "Concepto";
                        column.ReadOnly = true;
                        break;
                }
            }
        }

        private void LoadCitas()
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    if (conn == null)
                    {
                        throw new Exception("La conexión a la base de datos no está configurada.");
                    }

                    conn.Open();

                    string estadoFiltro = cmbEstadoFiltro.SelectedItem.ToString();
                    string query = @"
                        SELECT 
                            Id, 
                            Cliente, 
                            datetime(Fecha) as Fecha, 
                            Estado, 
                            Concepto 
                        FROM Citas";

                    if (estadoFiltro != "Todos")
                    {
                        query += " WHERE Estado = @Estado";
                    }

                    query += " ORDER BY datetime(Fecha) DESC;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (estadoFiltro != "Todos")
                        {
                            cmd.Parameters.AddWithValue("@Estado", estadoFiltro);
                        }

                        using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            dgvCitas.DataSource = null;
                            dgvCitas.DataSource = dt;

                            ConfigureColumns();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las citas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbEstadoFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCitas();
        }

        private void DgvCitas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCitas.Columns[e.ColumnIndex].Name == "Estado")
            {
                var id = dgvCitas.Rows[e.RowIndex].Cells["Id"].Value;
                var nuevoEstado = dgvCitas.Rows[e.RowIndex].Cells["Estado"].Value;

                try
                {
                    using (SQLiteConnection conn = Database.GetConnection())
                    {
                        conn.Open();
                        string query = "UPDATE Citas SET Estado = @Estado WHERE Id = @Id";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Estado actualizado correctamente.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar el estado: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadCitas();
        }

        private void BtnNuevaCita_Click(object sender, EventArgs e)
        {
            using (FormRegistrarCita formRegistrarCita = new FormRegistrarCita())
            {
                if (formRegistrarCita.ShowDialog() == DialogResult.OK)
                {
                    LoadCitas();
                }
            }
        }

        private void FormCitas_Load(object sender, EventArgs e)
        {
            LoadCitas();
        }
    }
}
