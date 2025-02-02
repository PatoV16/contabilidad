using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
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
        private Button btnProximasCitas;
        private MonthCalendar monthCalendar;
        private Panel panelCalendario;

        public FormCitas()
        {
            CreateControls();
            InitializeComponent();
        }

        private void CreateControls()
        {
            // DataGridView for appointments
            dgvCitas = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                AllowUserToAddRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = true,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowTemplate = { Height = 40 } // Increase row height
            };
            dgvCitas.CellValueChanged += DgvCitas_CellValueChanged;
            dgvCitas.CurrentCellDirtyStateChanged += DgvCitas_CurrentCellDirtyStateChanged;

            // Appointment status filter
            cmbEstadoFiltro = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 150,
                Location = new Point(500, 10),
                Font = new Font("Segoe UI", 12)
            };
            cmbEstadoFiltro.Items.AddRange(new string[] { "Pendiente", "Realizado" });
            cmbEstadoFiltro.SelectedIndex = 0;
            cmbEstadoFiltro.SelectedIndexChanged += CmbEstadoFiltro_SelectedIndexChanged;

            // Refresh button
            btnRefresh = new Button
            {
                Text = "Actualizar",
                Size = new Size(100, 40),
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            btnRefresh.Click += BtnRefresh_Click;

            // New appointment button
            btnNuevaCita = new Button
            {
                Text = "Nueva Cita",
                Size = new Size(150, 40),
                Location = new Point(120, 10),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            btnNuevaCita.Click += BtnNuevaCita_Click;

            // Buttons panel
            panelBotones = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            panelBotones.Controls.Add(cmbEstadoFiltro);
            panelBotones.Controls.Add(btnRefresh);
            panelBotones.Controls.Add(btnNuevaCita);

            // Grid panel
            panelGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            panelGrid.Controls.Add(dgvCitas);

            // Próximas citas button
            btnProximasCitas = new Button
            {
                Text = "Próxima cita",
                Size = new Size(180, 40),
                Location = new Point(280, 10),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(231, 76, 60),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            btnProximasCitas.Click += BtnProximasCitas_Click;

            panelBotones.Controls.Add(btnProximasCitas); // Añadir al panel de botones

            // MonthCalendar for appointments
            monthCalendar = new MonthCalendar
            {
                Dock = DockStyle.Fill,
                MaxSelectionCount = 1, // Permitir selección de una sola fecha
                BackColor = Color.White,
                ForeColor = Color.Black,
                TitleBackColor = Color.FromArgb(52, 152, 219),
                TitleForeColor = Color.White,
                TrailingForeColor = Color.Gray,
                CalendarDimensions = new Size(2, 1) // Mostrar dos meses
            };
            monthCalendar.DateSelected += MonthCalendar_DateSelected;

            // Calendar panel
            panelCalendario = new Panel
            {
                Dock = DockStyle.Left,
                Width = 400,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(245, 245, 245)
            };
            panelCalendario.Controls.Add(monthCalendar);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Calendario de citas dentales";
            this.Size = new Size(1200, 600); // Ajustar el tamaño del formulario
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);

            this.Controls.Add(panelGrid);
            this.Controls.Add(panelBotones);
            this.Controls.Add(panelCalendario); // Añadir el panel del calendario

            this.ResumeLayout(false);

            this.Load += FormCitas_Load;
        }

        private void ConfigureColumns()
        {
            if (dgvCitas.Columns.Count == 0)
            {
                dgvCitas.Columns.Add("Id", "ID");
                dgvCitas.Columns.Add("Cliente", "Paciente");
                dgvCitas.Columns.Add("Fecha", "Fecha");
                dgvCitas.Columns.Add("Hora", "Hora");
                dgvCitas.Columns.Add("Concepto", "Concepto");

                // Crear y agregar la columna ComboBox para el estado
                var comboBoxColumn = new DataGridViewComboBoxColumn
                {
                    Name = "Estado",
                    HeaderText = "Estado",
                    DataSource = new List<string> { "Pendiente", "Realizado" }, // Opciones del ComboBox
                    Width = 150,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                    FlatStyle = FlatStyle.Flat
                };
                dgvCitas.Columns.Add(comboBoxColumn);
            }
        }

        private void LoadCitas(bool showUpcoming = false, DateTime? fecha = null)
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    string estadoFiltro = cmbEstadoFiltro.SelectedItem.ToString();
                    string query = @"
                        SELECT 
                            Id, 
                            Cliente, 
                            datetime(Fecha) as Fecha, 
                            Hora,
                            Estado, 
                            Concepto 
                        FROM Citas";

                    if (estadoFiltro != "All")
                    {
                        query += " WHERE Estado = @Estado";
                    }

                    if (fecha.HasValue)
                    {
                        query += (estadoFiltro == "All" ? " WHERE" : " AND") + " date(Fecha) = @Fecha";
                    }

                    if (showUpcoming)
                    {
                        // Filtrar citas con fecha y hora mayor al momento actual
                        query += (estadoFiltro == "All" && !fecha.HasValue ? " WHERE" : " AND") + " datetime(Fecha || ' ' || Hora) > datetime('now')";
                    }

                    query += " ORDER BY datetime(Fecha || ' ' || Hora) ASC;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (estadoFiltro != "All")
                        {
                            cmd.Parameters.AddWithValue("@Estado", estadoFiltro);
                        }

                        if (fecha.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@Fecha", fecha.Value.ToString("yyyy-MM-dd"));
                        }

                        using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            dgvCitas.DataSource = null; // Desvincula la fuente de datos
                            dgvCitas.DataSource = dt;

                            ConfigureColumns();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading appointments: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento del botón "Próximas Citas"
        private void BtnProximasCitas_Click(object sender, EventArgs e)
        {
            LoadCitas(showUpcoming: true); // Cargar citas próximas
        }

        private void CmbEstadoFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCitas();
        }

        private void DgvCitas_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvCitas.IsCurrentCellDirty && dgvCitas.CurrentCell.ColumnIndex == dgvCitas.Columns["Estado"].Index)
            {
                dgvCitas.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvCitas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCitas.Columns[e.ColumnIndex].Name == "Estado")
            {
                try
                {
                    var id = dgvCitas.Rows[e.RowIndex].Cells["Id"].Value;
                    var nuevoEstado = dgvCitas.Rows[e.RowIndex].Cells["Estado"].Value;

                    using (SQLiteConnection conn = Database.GetConnection())
                    {
                        conn.Open();
                        string query = "UPDATE Citas SET Estado = @Estado WHERE Id = @Id";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Estado", nuevoEstado ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Estado actualizado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar el estado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // No cargar citas al inicio, esperar a que el usuario seleccione una fecha
            // LoadCitas();
        }

        private void MonthCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            DateTime fechaSeleccionada = e.Start;
            LoadCitas(fecha: fechaSeleccionada); // Filtrar citas por fecha seleccionada
        }
    }
}