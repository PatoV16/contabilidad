using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoneyManageApp
{
    public class KidsOdontogram : Form
    {
        private Dictionary<int, ToothControlForKids> babyTeeth;

        public KidsOdontogram()
        {
            InitializeComponent();
            SetupBabyTeeth();
        }
        private void InitializeComponent()
        {
            // Configuración principal del formulario
            this.Text = "Odontograma Pediátrico";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Panel principal
            Panel panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Título
            Label lblTitulo = new Label
            {
                Text = "Odontograma Infantil",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Height = 40
            };

            // Panel de información del paciente
            Panel panelPaciente = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            Label lblNombre = new Label
            {
                Text = "Nombre:",
                Location = new Point(10, 20),
                AutoSize = true
            };

            TextBox txtNombrePaciente = new TextBox
            {
                Location = new Point(70, 15),
                Width = 300,
                Text = "Nombre del Paciente"
            };

            Label lblEdad = new Label
            {
                Text = "Fecha de Nacimiento:",
                Location = new Point(380, 20),
                AutoSize = true
            };

            DateTimePicker dtpNacimiento = new DateTimePicker
            {
                Location = new Point(530, 15),
                Width = 150,
                Format = DateTimePickerFormat.Short
            };

            // Leyenda de estados
            Panel panelLeyenda = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            Dictionary<string, Color> estadosLeyenda = new Dictionary<string, Color>
    {
        { "Sano", Color.White },
        { "Caries", Color.Red },
        { "Tratado", Color.Green },
        { "Sellante", Color.Blue },
        { "En Erupción", Color.Yellow }
    };

            int xLeyenda = 10;
            foreach (var estado in estadosLeyenda)
            {
                Panel colorBox = new Panel
                {
                    Location = new Point(xLeyenda, 20),
                    Size = new Size(20, 20),
                    BackColor = estado.Value,
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label lblEstado = new Label
                {
                    Text = estado.Key,
                    Location = new Point(xLeyenda + 25, 20),
                    AutoSize = true
                };

                panelLeyenda.Controls.Add(colorBox);
                panelLeyenda.Controls.Add(lblEstado);
                xLeyenda += 100;
            }

            // Agregar controles al panel de información del paciente
            panelPaciente.Controls.AddRange(new Control[]
            {
        lblNombre, txtNombrePaciente, lblEdad, dtpNacimiento
            });

            // Agregar controles al formulario
            this.Controls.AddRange(new Control[]
            {
        panelPrincipal,
        lblTitulo,
        panelPaciente,
        panelLeyenda
            });
        }


        private void SetupBabyTeeth()
        {
            babyTeeth = new Dictionary<int, ToothControlForKids>();
            int[] babyTeethNumbers = {
                // Top baby teeth
                51, 52, 53, 54, 55,
                // Bottom baby teeth
                81, 82, 83, 84, 85,
                // Opposite side baby teeth
                61, 62, 63, 64, 65,
                71, 72, 73, 74, 75
            };

            int toothWidth = 40;
            int toothHeight = 80;
            int spacing = 5;
            int startY = 100;
            int centerX = (this.ClientSize.Width - (babyTeethNumbers.Length * (toothWidth + spacing))) / 2;

            foreach (int toothNumber in babyTeethNumbers)
            {
                var tooth = new ToothControlForKids(toothNumber)
                {
                    Location = new Point(
                        centerX + (Array.IndexOf(babyTeethNumbers, toothNumber) * (toothWidth + spacing)),
                        startY
                    ),
                    Size = new Size(toothWidth, toothHeight)
                };
                babyTeeth.Add(toothNumber, tooth);
                this.Controls.Add(tooth);
            }
        }
    }

    public class ToothControlForKids : UserControl
    {
        private int toothNumber;
        private string toothState = "healthy";
        private Color toothColor = Color.White;

        public ToothControlForKids(int toothNumber)
        {
            this.toothNumber = toothNumber;
            this.Click += ToothControl_Click;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Draw tooth with color based on state
            using (var brush = new SolidBrush(toothColor))
            using (var pen = new Pen(Color.Black))
            {
                g.FillRectangle(brush, ClientRectangle);
                g.DrawRectangle(pen, ClientRectangle);

                // Show tooth number
                using (var font = new Font("Comic Sans MS", 10))
                {
                    g.DrawString(toothNumber.ToString(), font, Brushes.Black,
                        new PointF(Width / 2 - 10, Height - 20));
                }
            }
        }

        private void ToothControl_Click(object sender, EventArgs e)
        {
            using (var toothStatusForm = new ToothStatusFormForKids(toothNumber))
            {
                if (toothStatusForm.ShowDialog() == DialogResult.OK)
                {
                    toothState = toothStatusForm.SelectedStatus.ToLower();
                    UpdateToothColor();
                }
            }
        }

        private void UpdateToothColor()
        {
            switch (toothState)
            {
                case "cavity": toothColor = Color.Red; break;
                case "healthy": toothColor = Color.White; break;
                case "treated": toothColor = Color.Green; break;
                case "sealed": toothColor = Color.Blue; break;
                case "growing": toothColor = Color.Yellow; break;
                default: toothColor = Color.White; break;
            }
            Invalidate();
        }
    }

    public class ToothStatusFormForKids : Form
    {
        private ComboBox cmbStatus;
        public string SelectedStatus { get; private set; }

        public ToothStatusFormForKids(int toothNumber)
        {
            this.Text = $"Choose the Status for Tooth {toothNumber}";
            this.Size = new Size(300, 200);
            this.StartPosition = FormStartPosition.CenterParent;

            cmbStatus = new ComboBox
            {
                Location = new Point(20, 20),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new string[] {
                "Healthy", "Cavity", "Treated", "Sealed", "Growing"
            });
            cmbStatus.SelectedIndex = 0;

            var btnAccept = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(50, 100)
            };

            btnAccept.Click += (s, e) => {
                SelectedStatus = cmbStatus.SelectedItem.ToString();
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(150, 100)
            };

            this.Controls.AddRange(new Control[] { cmbStatus, btnAccept, btnCancel });
        }
    }
}
