using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoneyManageApp
{
    internal class FormAgregarCliente : Form
    {
        private TextBox txtCedulaRuc;
        private TextBox txtNombre;
        private TextBox txtDireccion;
        private TextBox txtCiudad;
        private TextBox txtTelefono;
        private TextBox txtCorreo;
        private Button btnGuardar;
        private Button btnCancelar;

        public FormAgregarCliente()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Agregar Cliente";

            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 2,
                RowCount = 7
            };

            // Campos
            panel.Controls.Add(new Label { Text = "Cédula/RUC:" }, 0, 0);
            txtCedulaRuc = new TextBox { Width = 200 };
            panel.Controls.Add(txtCedulaRuc, 1, 0);

            panel.Controls.Add(new Label { Text = "Nombre:" }, 0, 1);
            txtNombre = new TextBox { Width = 200 };
            panel.Controls.Add(txtNombre, 1, 1);

            panel.Controls.Add(new Label { Text = "Dirección:" }, 0, 2);
            txtDireccion = new TextBox { Width = 200 };
            panel.Controls.Add(txtDireccion, 1, 2);

            panel.Controls.Add(new Label { Text = "Ciudad:" }, 0, 3);
            txtCiudad = new TextBox { Width = 200 };
            panel.Controls.Add(txtCiudad, 1, 3);

            panel.Controls.Add(new Label { Text = "Teléfono:" }, 0, 4);
            txtTelefono = new TextBox { Width = 200 };
            panel.Controls.Add(txtTelefono, 1, 4);

            panel.Controls.Add(new Label { Text = "Correo:" }, 0, 5);
            txtCorreo = new TextBox { Width = 200 };
            panel.Controls.Add(txtCorreo, 1, 5);

            // Botones
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 40
            };

            btnGuardar = new Button { Text = "Guardar", DialogResult = DialogResult.OK };
            btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };
            buttonPanel.Controls.AddRange(new Control[] { btnCancelar, btnGuardar });

            this.Controls.AddRange(new Control[] { panel, buttonPanel });
        }

        // Método para obtener los datos del cliente
        public (string cedulaRuc, string nombre, string direccion, string ciudad, string telefono, string correo) GetClienteData()
        {
            return (
                txtCedulaRuc.Text,
                txtNombre.Text,
                txtDireccion.Text,
                txtCiudad.Text,
                txtTelefono.Text,
                txtCorreo.Text
            );
        }
    }
}