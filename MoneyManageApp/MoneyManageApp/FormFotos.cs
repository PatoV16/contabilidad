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
            this.btnAgregarFoto.Text = "Agregar Foto";
            this.btnAgregarFoto.Location = new Point(340, 45);
            this.btnAgregarFoto.Size = new Size(120, 25);
            this.btnAgregarFoto.Click += new EventHandler(BtnAgregarFoto_Click);
            this.btnAgregarFoto.Enabled = false;

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
            cmbPacientes.Items.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(dbConnection))
            {
                conn.Open();
                string query = "SELECT CedulaRUC, NombreRazonSocial FROM Clientes ORDER BY NombreRazonSocial";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader["CedulaRUC"].ToString();
                            string nombre = reader["NombreRazonSocial"].ToString();
                            var item = new ComboBoxItem(id, $"{nombre} - {id}");
                            cmbPacientes.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void CmbPacientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPacientes.SelectedItem != null)
            {
                var selected = (ComboBoxItem)cmbPacientes.SelectedItem;
                selectedPatientId = selected.Id;
                btnAgregarFoto.Enabled = true;
                CargarFotos(selectedPatientId);
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
            if (selectedPatientId == null) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] imageBytes = File.ReadAllBytes(ofd.FileName);
                        GuardarFoto(selectedPatientId, imageBytes);
                        CargarFotos(selectedPatientId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cargar la imagen: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void GuardarFoto(string cedulaRUC, byte[] fotoBytes)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbConnection))
            {
                conn.Open();
                string query = @"INSERT INTO Fotos (CedulaRUC, Foto, FechaSubida) 
                               VALUES (@cedula, @foto, @fecha)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedulaRUC);
                    cmd.Parameters.AddWithValue("@foto", fotoBytes);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd"));
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
    }

    // Helper class for ComboBox items
    public class ComboBoxItem
    {
        public string Id { get; set; }
        public string Text { get; set; }

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
}