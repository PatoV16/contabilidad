using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace MoneyManageApp
{
    public partial class FormCitas : Form
    {
        private DataGridView dgvCitas;
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
            dgvCitas = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = true,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false,
                BackgroundColor = System.Drawing.SystemColors.Window,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };

            btnRefresh = new Button
            {
                Text = "Refrescar",
                Size = new System.Drawing.Size(100, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnRefresh.Click += BtnRefresh_Click;

            btnNuevaCita = new Button
            {
                Text = "Nueva Cita",
                Size = new System.Drawing.Size(100, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnNuevaCita.Click += BtnNuevaCita_Click;

            panelBotones = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = System.Drawing.Color.WhiteSmoke
            };

            panelGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Control de Citas";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            panelBotones.SuspendLayout();
            panelBotones.Controls.Add(btnNuevaCita);
            panelBotones.Controls.Add(btnRefresh);
            panelBotones.ResumeLayout(false);

            panelGrid.SuspendLayout();
            panelGrid.Controls.Add(dgvCitas);
            panelGrid.ResumeLayout(false);

            this.Controls.Add(panelGrid);
            this.Controls.Add(panelBotones);

            this.ResumeLayout(false);

            this.Load += FormCitas_Load;
        }

        private void ConfigureColumns()
        {
            if (dgvCitas?.Columns == null || dgvCitas.Columns.Count == 0)
                return;

            try
            {
                foreach (DataGridViewColumn column in dgvCitas.Columns)
                {
                    if (column == null) continue;

                    switch (column.Name)
                    {
                        case "Id":
                            column.Width = 50;
                            column.HeaderText = "ID";
                            break;
                        case "Cliente":
                            column.Width = 200;
                            column.HeaderText = "Cliente";
                            break;
                        case "Fecha":
                            column.Width = 150;
                            column.HeaderText = "Fecha";
                            break;
                        case "Estado":
                            column.Width = 100;
                            column.HeaderText = "Estado";
                            break;
                        case "Concepto":
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            column.HeaderText = "Concepto";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al configurar columnas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string query = @"
                        SELECT 
                            Id, 
                            Cliente, 
                            datetime(Fecha) as Fecha, 
                            Estado, 
                            Concepto 
                        FROM Citas 
                        ORDER BY datetime(Fecha) DESC;";

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvCitas.DataSource = null;
                        dgvCitas.DataSource = dt;

                        ConfigureColumns();

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("No hay citas registradas.", "Información",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
