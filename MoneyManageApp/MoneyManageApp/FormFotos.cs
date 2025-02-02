using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MoneyManageApp
{
    public partial class FormFotos : Form
    {
        private string dbConnection = "Data Source=database.db;Version=3;";
        private PictureBox currentPictureBox = null;
        private string selectedPatientId = null;
        private ComboBox cmbPacientes;
        private Button btnAgregarFoto;
        private FlowLayoutPanel flowLayoutPhotos;
        private Label lblPaciente;

        public FormFotos()
        {
            InitializeComponent();
            AsegurarTablaFotos(); // Añadir esta línea
            CargarPacientes();
        }

        private void InitializeComponent()
        {
            this.cmbPacientes = new ComboBox();
            this.btnAgregarFoto = new Button();
            this.flowLayoutPhotos = new FlowLayoutPanel();
            this.lblPaciente = new Label();

            // Form
            this.Text = "Gestión de Fotos de Pacientes";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Label
            this.lblPaciente.Text = "Seleccionar Paciente:";
            this.lblPaciente.Location = new Point(20, 20);
            this.lblPaciente.AutoSize = true;

            // ComboBox
            this.cmbPacientes.Location = new Point(20, 45);
            this.cmbPacientes.Size = new Size(300, 25);
            this.cmbPacientes.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPacientes.SelectedIndexChanged += new EventHandler(CmbPacientes_SelectedIndexChanged);

            // Button
            // Button
            this.btnAgregarFoto = new Button
            {
                Text = "Agregar Foto",
                Location = new Point(340, 45),
                Size = new Size(120, 25),
                Enabled = false, // Inicialmente deshabilitado
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnAgregarFoto.Click += new EventHandler(BtnAgregarFoto_Click);

            // FlowLayoutPanel for photos
            this.flowLayoutPhotos.Location = new Point(20, 90);
            this.flowLayoutPhotos.Size = new Size(740, 450);
            this.flowLayoutPhotos.AutoScroll = true;
            this.flowLayoutPhotos.BorderStyle = BorderStyle.FixedSingle;

            // Add controls to form
            this.Controls.Add(this.lblPaciente);
            this.Controls.Add(this.cmbPacientes);
            this.Controls.Add(this.btnAgregarFoto);
            this.Controls.Add(this.flowLayoutPhotos);
        }

        private void CargarPacientes()
        {
            try
            {
                cmbPacientes.Items.Clear();
                using (SQLiteConnection conn = new SQLiteConnection(dbConnection))
                {
                    conn.Open();
                    string query = "SELECT CedulaRUC, NombreRazonSocial FROM Clientes ORDER BY NombreRazonSocial";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            // Agregar item por defecto
                            cmbPacientes.Items.Add(new ComboBoxItem("", "Seleccione un paciente..."));

                            while (reader.Read())
                            {
                                string id = reader["CedulaRUC"].ToString();
                                string nombre = reader["NombreRazonSocial"].ToString();


                                var item = new ComboBoxItem(id, nombre);
                                cmbPacientes.Items.Add(item);
                            }
                        }
                    }
                }
                // Seleccionar el primer item (opción por defecto)
                if (cmbPacientes.Items.Count > 0)
                    cmbPacientes.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pacientes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbPacientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbPacientes.SelectedItem != null)
                {
                    var selected = (ComboBoxItem)cmbPacientes.SelectedItem;
                    selectedPatientId = selected.Id;


                    // Habilitar botón solo si hay un ID válido
                    btnAgregarFoto.Enabled = !string.IsNullOrEmpty(selectedPatientId);

                    if (!string.IsNullOrEmpty(selectedPatientId))
                    {
                        CargarFotos(selectedPatientId);
                    }
                }
                else
                {
                    btnAgregarFoto.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar paciente: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Asegúrate de que la clase ComboBoxItem esté definida así:
        public class ComboBoxItem
        {
            public string Id { get; private set; }
            public string Text { get; private set; }

            public ComboBoxItem(string id, string text)
            {
                Id = id;
                Text = text;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void CargarFotos(string cedulaRUC)
        {
            flowLayoutPhotos.Controls.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(dbConnection))
            {
                conn.Open();
                string query = "SELECT Id, Foto, FechaSubida FROM Fotos WHERE CedulaRUC = @cedula ORDER BY FechaSubida DESC";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedulaRUC);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            byte[] fotoBytes = (byte[])reader["Foto"];
                            using (MemoryStream ms = new MemoryStream(fotoBytes))
                            {
                                Image imagen = Image.FromStream(ms);
                                CrearPictureBox(imagen, reader["FechaSubida"].ToString());
                            }
                        }
                    }
                }
            }
        }

        private void CrearPictureBox(Image imagen, string fecha)
        {
            // Container panel for picture and label
            Panel containerPanel = new Panel();
            containerPanel.Size = new Size(200, 220);
            containerPanel.Margin = new Padding(5);

            // PictureBox for the image
            PictureBox pb = new PictureBox();
            pb.Size = new Size(190, 190);
            pb.Location = new Point(5, 5);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = imagen;
            pb.Click += PictureBox_Click;

            // Label for the date
            Label lblFecha = new Label();
            lblFecha.Text = fecha;
            lblFecha.AutoSize = true;
            lblFecha.Location = new Point(5, 200);

            containerPanel.Controls.Add(pb);
            containerPanel.Controls.Add(lblFecha);
            flowLayoutPhotos.Controls.Add(containerPanel);
        }

        private void BtnAgregarFoto_Click(object sender, EventArgs e)
        {
            // Primero verificamos que tenemos un paciente seleccionado
            if (selectedPatientId == null || string.IsNullOrEmpty(selectedPatientId))
            {
                MessageBox.Show("Por favor, seleccione un paciente primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Verificamos que el archivo existe
                        if (!File.Exists(ofd.FileName))
                        {
                            MessageBox.Show("El archivo seleccionado no existe.");
                            return;
                        }

                        byte[] imageBytes = File.ReadAllBytes(ofd.FileName);

                        // Verificamos que tenemos datos de imagen
                        if (imageBytes == null || imageBytes.Length == 0)
                        {
                            MessageBox.Show("No se pudo leer la imagen seleccionada.");
                            return;
                        }

                        // Verificación adicional del ID del paciente antes de guardar
                        Console.WriteLine($"CedulaRUC a guardar: {selectedPatientId}"); // Para debugging

                        GuardarFoto(selectedPatientId, imageBytes);
                        CargarFotos(selectedPatientId);

                        MessageBox.Show("Imagen guardada exitosamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error detallado: {ex.Message}\nStack Trace: {ex.StackTrace}",
                            "Error al guardar la foto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void GuardarFoto(string cedulaRUC, byte[] fotoBytes)
        {
            // Verificación adicional
            if (string.IsNullOrEmpty(cedulaRUC))
            {
                throw new ArgumentException("La cédula/RUC no puede estar vacía.");
            }

            if (fotoBytes == null || fotoBytes.Length == 0)
            {
                throw new ArgumentException("Los datos de la imagen no pueden estar vacíos.");
            }

            using (SQLiteConnection conn = new SQLiteConnection(dbConnection))
            {
                conn.Open();
                string query = @"INSERT INTO Fotos (CedulaRUC, Foto, FechaSubida) 
                        VALUES (@cedula, @foto, @fecha)";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    // Añadimos los parámetros de manera más explícita
                    var paramCedula = new SQLiteParameter("@cedula", System.Data.DbType.String)
                    {
                        Value = cedulaRUC
                    };
                    cmd.Parameters.Add(paramCedula);

                    var paramFoto = new SQLiteParameter("@foto", System.Data.DbType.Binary)
                    {
                        Value = fotoBytes
                    };
                    cmd.Parameters.Add(paramFoto);

                    var paramFecha = new SQLiteParameter("@fecha", System.Data.DbType.String)
                    {
                        Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    cmd.Parameters.Add(paramFecha);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clickedPb = (PictureBox)sender;
            Form imageForm = new Form();
            PictureBox fullPb = new PictureBox();

            imageForm.Size = new Size(800, 600);
            imageForm.StartPosition = FormStartPosition.CenterScreen;

            fullPb.Dock = DockStyle.Fill;
            fullPb.SizeMode = PictureBoxSizeMode.Zoom;
            fullPb.Image = clickedPb.Image;

            imageForm.Controls.Add(fullPb);
            imageForm.ShowDialog();
        }
        // Añade este método y llámalo en el constructor del formulario
        private void AsegurarTablaFotos()
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbConnection))
            {
                conn.Open();
                string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Fotos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CedulaRUC TEXT NOT NULL,
                Foto BLOB NOT NULL,
                FechaSubida TEXT NOT NULL,
                FOREIGN KEY(CedulaRUC) REFERENCES Clientes(CedulaRUC)
            )";
                using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    // Helper class for ComboBox items
    public class FotoComboBoxItem  // Nombre diferente
    {
        public string Id { get; set; }
        public string Text { get; set; }

        public FotoComboBoxItem(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}