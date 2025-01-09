using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormEditarCliente : Form
    {
        private Label lblCedulaRuc;
        private Label lblNombreRazonSocial;
        private Label lblDireccion;
        private Label lblCiudad;
        private Label lblTelefono;
        private Label lblCorreoElectronico;

        private TextBox txtCedulaRuc;
        private TextBox txtNombreRazonSocial;
        private TextBox txtDireccion;
        private TextBox txtCiudad;
        private TextBox txtTelefono;
        private TextBox txtCorreoElectronico;

        private Button btnGuardar;
        private Button btnCancelar;

        private string cedulaRucOriginal;

        public FormEditarCliente(string cedulaRuc)
        {
            cedulaRucOriginal = cedulaRuc;
            InitializeComponent();
            LoadClientData(cedulaRuc);
        }

        private void InitializeComponent()
        {
            this.Text = "Editar Cliente";
            this.Size = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            lblCedulaRuc = new Label { Text = "Cédula/RUC:", Location = new System.Drawing.Point(20, 20) };
            lblNombreRazonSocial = new Label { Text = "Nombre/Razón Social:", Location = new System.Drawing.Point(20, 60) };
            lblDireccion = new Label { Text = "Dirección:", Location = new System.Drawing.Point(20, 100) };
            lblCiudad = new Label { Text = "Ciudad:", Location = new System.Drawing.Point(20, 140) };
            lblTelefono = new Label { Text = "Teléfono:", Location = new System.Drawing.Point(20, 180) };
            lblCorreoElectronico = new Label { Text = "Correo Electrónico:", Location = new System.Drawing.Point(20, 220) };

            txtCedulaRuc = new TextBox { Location = new System.Drawing.Point(160, 20), Width = 200, ReadOnly = true };
            txtNombreRazonSocial = new TextBox { Location = new System.Drawing.Point(160, 60), Width = 200 };
            txtDireccion = new TextBox { Location = new System.Drawing.Point(160, 100), Width = 200 };
            txtCiudad = new TextBox { Location = new System.Drawing.Point(160, 140), Width = 200 };
            txtTelefono = new TextBox { Location = new System.Drawing.Point(160, 180), Width = 200 };
            txtCorreoElectronico = new TextBox { Location = new System.Drawing.Point(160, 220), Width = 200 };

            btnGuardar = new Button { Text = "Guardar", Location = new System.Drawing.Point(160, 260), Width = 80 };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button { Text = "Cancelar", Location = new System.Drawing.Point(250, 260), Width = 80 };
            btnCancelar.Click += BtnCancelar_Click;

            this.Controls.Add(lblCedulaRuc);
            this.Controls.Add(lblNombreRazonSocial);
            this.Controls.Add(lblDireccion);
            this.Controls.Add(lblCiudad);
            this.Controls.Add(lblTelefono);
            this.Controls.Add(lblCorreoElectronico);
            this.Controls.Add(txtCedulaRuc);
            this.Controls.Add(txtNombreRazonSocial);
            this.Controls.Add(txtDireccion);
            this.Controls.Add(txtCiudad);
            this.Controls.Add(txtTelefono);
            this.Controls.Add(txtCorreoElectronico);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }

        private void LoadClientData(string cedulaRuc)
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT CedulaRuc, NombreRazonSocial, Direccion, Ciudad, Telefono, CorreoElectronico FROM Clientes WHERE CedulaRuc = @CedulaRuc";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CedulaRuc", cedulaRuc);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtCedulaRuc.Text = reader["CedulaRuc"].ToString();
                                txtNombreRazonSocial.Text = reader["NombreRazonSocial"].ToString();
                                txtDireccion.Text = reader["Direccion"].ToString();
                                txtCiudad.Text = reader["Ciudad"].ToString();
                                txtTelefono.Text = reader["Telefono"].ToString();
                                txtCorreoElectronico.Text = reader["CorreoElectronico"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Cliente no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos del cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SQLiteConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Clientes SET NombreRazonSocial = @Nombre, Direccion = @Direccion, Ciudad = @Ciudad, Telefono = @Telefono, CorreoElectronico = @Correo WHERE CedulaRuc = @CedulaRuc";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CedulaRuc", cedulaRucOriginal);
                        cmd.Parameters.AddWithValue("@Nombre", txtNombreRazonSocial.Text);
                        cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);
                        cmd.Parameters.AddWithValue("@Ciudad", txtCiudad.Text);
                        cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@Correo", txtCorreoElectronico.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Cliente actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar los datos del cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
