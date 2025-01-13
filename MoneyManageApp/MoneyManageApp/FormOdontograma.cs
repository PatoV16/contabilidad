using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MoneyManageApp
{
    public partial class FormOdontograma : Form
    {
        private Panel panelOdontograma;
        private Dictionary<int, Button> dientes;
        private Label lblPaciente;
        private TextBox txtPaciente;
        private Button btnGuardar;
        private Button btnLimpiar;

        public FormOdontograma()
        {
            InitializeComponent();
            InicializarDientes();
        }

        private void InitializeComponent()
        {
            this.Text = "Odontograma";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Panel principal para el odontograma
            panelOdontograma = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Panel superior para datos del paciente
            Panel panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            lblPaciente = new Label
            {
                Text = "Paciente:",
                Location = new Point(10, 20),
                AutoSize = true
            };

            txtPaciente = new TextBox
            {
                Location = new Point(80, 17),
                Width = 300
            };

            btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(400, 15),
                Width = 100,
                Height = 30
            };
            btnGuardar.Click += BtnGuardar_Click;

            btnLimpiar = new Button
            {
                Text = "Limpiar",
                Location = new Point(520, 15),
                Width = 100,
                Height = 30
            };
            btnLimpiar.Click += BtnLimpiar_Click;

            panelSuperior.Controls.AddRange(new Control[] { lblPaciente, txtPaciente, btnGuardar, btnLimpiar });

            this.Controls.Add(panelOdontograma);
            this.Controls.Add(panelSuperior);
        }

        private void InicializarDientes()
        {
            dientes = new Dictionary<int, Button>();
            int buttonSize = 40;
            int spacing = 5;

            // Definir las filas de dientes según el odontograma
            int[][] filaDientes = new int[][]
            {
                new int[] { 18, 17, 16, 15, 14, 13, 12, 11, 21, 22, 23, 24, 25, 26, 27, 28 }, // Superior
                new int[] { 55, 54, 53, 52, 51, 61, 62, 63, 64, 65 }, // Superior temporal
                new int[] { 85, 84, 83, 82, 81, 71, 72, 73, 74, 75 }, // Inferior temporal
                new int[] { 48, 47, 46, 45, 44, 43, 42, 41, 31, 32, 33, 34, 35, 36, 37, 38 }  // Inferior
            };

            int startY = 100;

            for (int fila = 0; fila < filaDientes.Length; fila++)
            {
                int startX = (this.ClientSize.Width - (filaDientes[fila].Length * (buttonSize + spacing))) / 2;

                for (int i = 0; i < filaDientes[fila].Length; i++)
                {
                    int numeroDiente = filaDientes[fila][i];
                    Button btnDiente = new Button
                    {
                        Text = numeroDiente.ToString(),
                        Size = new Size(buttonSize, buttonSize),
                        Location = new Point(startX + (i * (buttonSize + spacing)),
                                          startY + (fila * (buttonSize + 40))),
                        Tag = numeroDiente
                    };
                    btnDiente.Click += Diente_Click;

                    dientes.Add(numeroDiente, btnDiente);
                    panelOdontograma.Controls.Add(btnDiente);
                }
            }
        }

        private void Diente_Click(object sender, EventArgs e)
        {
            Button btnDiente = (Button)sender;
            int numeroDiente = (int)btnDiente.Tag;

            using (FormEstadoDiente formEstado = new FormEstadoDiente(numeroDiente))
            {
                if (formEstado.ShowDialog() == DialogResult.OK)
                {
                    // Actualizar el estado visual del diente según la selección
                    ActualizarEstadoDiente(btnDiente, formEstado.EstadoSeleccionado);
                }
            }
        }

        private void ActualizarEstadoDiente(Button btnDiente, string estado)
        {
            // Aquí puedes definir diferentes colores o símbolos para cada estado
            switch (estado.ToLower())
            {
                case "caries":
                    btnDiente.BackColor = Color.Red;
                    break;
                case "obturado":
                    btnDiente.BackColor = Color.Blue;
                    break;
                case "ausente":
                    btnDiente.BackColor = Color.Gray;
                    break;
                case "sano":
                    btnDiente.BackColor = Color.Green;
                    break;
                default:
                    btnDiente.BackColor = SystemColors.Control;
                    break;
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Aquí implementar la lógica para guardar el estado del odontograma
            MessageBox.Show("Odontograma guardado correctamente", "Éxito",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            foreach (Button btnDiente in dientes.Values)
            {
                btnDiente.BackColor = SystemColors.Control;
            }
            txtPaciente.Clear();
        }
    }

    // Formulario para seleccionar el estado del diente
    public class FormEstadoDiente : Form
    {
        private ComboBox cmbEstado;
        private Button btnAceptar;
        private Button btnCancelar;
        public string EstadoSeleccionado { get; private set; }

        public FormEstadoDiente(int numeroDiente)
        {
            this.Text = $"Estado del Diente {numeroDiente}";
            this.Size = new Size(300, 150);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            cmbEstado = new ComboBox
            {
                Location = new Point(20, 20),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbEstado.Items.AddRange(new string[] { "Sano", "Caries", "Obturado", "Ausente" });
            cmbEstado.SelectedIndex = 0;

            btnAceptar = new Button
            {
                Text = "Aceptar",
                DialogResult = DialogResult.OK,
                Location = new Point(85, 70),
                Width = 80
            };

            btnCancelar = new Button
            {
                Text = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Location = new Point(180, 70),
                Width = 80
            };

            this.Controls.AddRange(new Control[] { cmbEstado, btnAceptar, btnCancelar });
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;

            btnAceptar.Click += (s, e) => EstadoSeleccionado = cmbEstado.SelectedItem.ToString();
        }
    }
}