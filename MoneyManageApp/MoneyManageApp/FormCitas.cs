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
        private Button btnNuevaCita;  // Nuevo botón para registrar cita

        public FormCitas()
        {
            InitializeComponent();
            LoadCitas();
        }

        private void InitializeComponent()
        {
            this.Text = "Control de Citas";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Crear DataGridView
            dgvCitas = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            this.Controls.Add(dgvCitas);

            // Botón para refrescar la lista
            btnRefresh = new Button
            {
                Text = "Refrescar",
                Size = new System.Drawing.Size(100, 30),
                Location = new System.Drawing.Point(680, 10)
            };
            btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(btnRefresh);

            // Botón para agregar nueva cita
            btnNuevaCita = new Button
            {
                Text = "Nueva Cita",
                Size = new System.Drawing.Size(100, 30),
                Location = new System.Drawing.Point(560, 10)  // Puedes ajustar la posición según desees
            };
            btnNuevaCita.Click += BtnNuevaCita_Click;
            this.Controls.Add(btnNuevaCita);
        }

        private void LoadCitas()
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, Cliente, Fecha, Estado, Concepto FROM Citas;";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvCitas.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las citas: " + ex.Message);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadCitas();
        }

        // Método para abrir el formulario de nueva cita
        private void BtnNuevaCita_Click(object sender, EventArgs e)
        {
            FormRegistrarCita formRegistrarCita = new FormRegistrarCita();
            formRegistrarCita.ShowDialog();  // Mostrar el formulario de nueva cita
        }
    }
}
