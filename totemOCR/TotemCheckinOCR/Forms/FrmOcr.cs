using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using TotemCheckinOCR.Services;
using TotemCheckinOCR.Utils;
using TotemCheckinOCR.Models;

namespace TotemCheckinOCR.Forms
{
    public class FrmOcr : Form
    {
        private PictureBox picCamera;
        private PictureBox picCaptured;
        private Button btnStartCamera;
        private Button btnStopCamera;
        private Button btnCapture;
        private Button btnRecapture;
        private Button btnConfirmar;
        private TextBox txtNome;
        private MaskedTextBox mtxtCpf;
        private CheckBox chkModoDemo;
        private Label lblDemo;
        private Label lblStatus;
        private FilterInfoCollection? videoDevices;
        private VideoCaptureDevice? videoSource;
        private Bitmap? currentFrame;
        private Bitmap? capturedImage;

        private readonly OcrReader ocrReader;
        private readonly AgendamentoRepository agendamentoRepository;

        public FrmOcr()
        {
            Text = "Totem Check-in OCR";
            Width = 1100;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            ocrReader = new OcrReader();
            agendamentoRepository = new AgendamentoRepository();

            InicializarComponentes();
            Load += FrmOcr_Load;
            FormClosing += FrmOcr_FormClosing;
        }

        private void InicializarComponentes()
        {
            var lblCamera = new Label
            {
                Text = "Câmera (ao vivo)",
                AutoSize = true,
                Top = 10,
                Left = 10
            };

            picCamera = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Top = lblCamera.Bottom + 5,
                Left = 10,
                Width = 520,
                Height = 360,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            var lblCapturada = new Label
            {
                Text = "Imagem capturada",
                AutoSize = true,
                Top = 10,
                Left = picCamera.Right + 10
            };

            picCaptured = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Top = lblCapturada.Bottom + 5,
                Left = picCamera.Right + 10,
                Width = 520,
                Height = 360,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            btnStartCamera = new Button
            {
                Text = "Iniciar Câmera",
                Top = picCamera.Bottom + 10,
                Left = 10,
                Width = 120
            };
            btnStartCamera.Click += BtnStartCamera_Click;

            btnStopCamera = new Button
            {
                Text = "Parar Câmera",
                Top = picCamera.Bottom + 10,
                Left = btnStartCamera.Right + 10,
                Width = 120
            };
            btnStopCamera.Click += BtnStopCamera_Click;

            btnCapture = new Button
            {
                Text = "Capturar",
                Top = picCamera.Bottom + 10,
                Left = btnStopCamera.Right + 10,
                Width = 120
            };
            btnCapture.Click += BtnCapture_Click;

            btnRecapture = new Button
            {
                Text = "Recapturar",
                Top = picCamera.Bottom + 10,
                Left = btnCapture.Right + 10,
                Width = 120,
                Enabled = false
            };
            btnRecapture.Click += BtnRecapture_Click;

            var lblNome = new Label
            {
                Text = "Nome:",
                AutoSize = true,
                Top = btnStartCamera.Bottom + 20,
                Left = 10
            };

            txtNome = new TextBox
            {
                Top = lblNome.Top - 3,
                Left = lblNome.Right + 5,
                Width = 400
            };

            var lblCpf = new Label
            {
                Text = "CPF:",
                AutoSize = true,
                Top = lblNome.Bottom + 15,
                Left = 10
            };

            mtxtCpf = new MaskedTextBox("000.000.000-00")
            {
                Top = lblCpf.Top - 3,
                Left = lblCpf.Right + 5,
                Width = 150
            };
            mtxtCpf.TextChanged += MtxtCpf_TextChanged;

            chkModoDemo = new CheckBox
            {
                Text = "Modo demonstração",
                Top = lblCpf.Bottom + 10,
                Left = 10,
                AutoSize = true
            };
            chkModoDemo.CheckedChanged += ChkModoDemo_CheckedChanged;

            lblDemo = new Label
            {
                Text = "Modo DEMO ativo: usando CPF de teste.",
                AutoSize = true,
                Top = chkModoDemo.Bottom + 5,
                Left = 10,
                ForeColor = System.Drawing.Color.DarkRed,
                Visible = false
            };

            btnConfirmar = new Button
            {
                Text = "Confirmar CPF e Buscar Agendamentos",
                Top = lblDemo.Bottom + 20,
                Left = 10,
                Width = 320,
                Height = 35,
                Enabled = false
            };
            btnConfirmar.Click += BtnConfirmar_Click;

            lblStatus = new Label
            {
                Text = "Status: aguardando captura.",
                AutoSize = true,
                Top = btnConfirmar.Bottom + 15,
                Left = 10,
                ForeColor = System.Drawing.Color.Navy
            };

            Controls.Add(lblCamera);
            Controls.Add(picCamera);
            Controls.Add(lblCapturada);
            Controls.Add(picCaptured);
            Controls.Add(btnStartCamera);
            Controls.Add(btnStopCamera);
            Controls.Add(btnCapture);
            Controls.Add(btnRecapture);
            Controls.Add(lblNome);
            Controls.Add(txtNome);
            Controls.Add(lblCpf);
            Controls.Add(mtxtCpf);
            Controls.Add(chkModoDemo);
            Controls.Add(lblDemo);
            Controls.Add(btnConfirmar);
            Controls.Add(lblStatus);
        }

        private void FrmOcr_Load(object? sender, EventArgs e)
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count > 0)
                {
                    StartCamera();
                }
                else
                {
                    AtualizarStatus("Nenhuma câmera encontrada. Você pode usar o modo demonstração.");
                }
            }
            catch (Exception ex)
            {
                AtualizarStatus("Erro ao inicializar câmera: " + ex.Message);
            }
        }

        private void FrmOcr_FormClosing(object? sender, FormClosingEventArgs e)
        {
            StopCamera();
            currentFrame?.Dispose();
            capturedImage?.Dispose();
        }

        private void StartCamera()
        {
            if (videoDevices == null || videoDevices.Count == 0)
            {
                AtualizarStatus("Nenhuma câmera disponível.");
                return;
            }

            if (videoSource != null && videoSource.IsRunning)
                return;

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
            AtualizarStatus("Câmera iniciada.");
        }

        private void StopCamera()
        {
            if (videoSource != null)
            {
                videoSource.NewFrame -= VideoSource_NewFrame;
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }
                videoSource = null;
            }
            AtualizarStatus("Câmera parada.");
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
                currentFrame?.Dispose();
                currentFrame = frame;
                if (picCamera.IsHandleCreated)
                {
                    picCamera.Invoke(new Action(() =>
                    {
                        picCamera.Image?.Dispose();
                        picCamera.Image = (Bitmap)frame.Clone();
                    }));
                }
            }
            catch
            {
                
            }
        }

        private void BtnStartCamera_Click(object? sender, EventArgs e)
        {
            StartCamera();
        }

        private void BtnStopCamera_Click(object? sender, EventArgs e)
        {
            StopCamera();
        }

        private void BtnCapture_Click(object? sender, EventArgs e)
        {
            if (chkModoDemo.Checked)
            {
                AplicarModoDemo();
                btnRecapture.Enabled = true;
                AtualizarStatus("Modo demo: CPF preenchido automaticamente.");
                return;
            }

            if (currentFrame == null)
            {
                MessageBox.Show("Nenhuma imagem da câmera disponível.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            capturedImage?.Dispose();
            capturedImage = (Bitmap)currentFrame.Clone();
            picCaptured.Image?.Dispose();
            picCaptured.Image = (Bitmap)capturedImage.Clone();

            try
            {
                AtualizarStatus("Processando OCR");
                var resultado = ocrReader.LerDados(capturedImage);

                if (!string.IsNullOrWhiteSpace(resultado.Nome))
                    txtNome.Text = resultado.Nome;

                if (!string.IsNullOrWhiteSpace(resultado.Cpf))
                {
                    string cpfFormatado = CpfHelper.FormatCpf(resultado.Cpf);
                    mtxtCpf.Text = cpfFormatado;
                }

                AtualizarStatus("OCR concluído. Revise os dados antes de confirmar.");
                btnRecapture.Enabled = true;
            }
            catch (Exception ex)
            {
                AtualizarStatus("Erro no OCR: " + ex.Message);
            }
        }

        private void BtnRecapture_Click(object? sender, EventArgs e)
        {
            capturedImage?.Dispose();
            capturedImage = null;
            picCaptured.Image?.Dispose();
            picCaptured.Image = null;
            AtualizarStatus("Imagem descartada. Capture novamente.");
        }

        private void MtxtCpf_TextChanged(object? sender, EventArgs e)
        {
            string digits = CpfHelper.OnlyDigits(mtxtCpf.Text);
            if (CpfHelper.IsValidCpf(digits))
            {
                btnConfirmar.Enabled = true;
                AtualizarStatus("CPF válido. Você pode confirmar.");
            }
            else
            {
                btnConfirmar.Enabled = false;
            }
        }

        private void ChkModoDemo_CheckedChanged(object? sender, EventArgs e)
        {
            if (chkModoDemo.Checked)
            {
                AplicarModoDemo();
                lblDemo.Visible = true;
            }
            else
            {
                lblDemo.Visible = false;
                txtNome.Clear();
                mtxtCpf.Clear();
                AtualizarStatus("Modo demo desativado. Use a câmera e o OCR.");
            }
        }

        private void AplicarModoDemo()
        {
            string cpfDemo = "54023506800";
            string cpfFormatado = CpfHelper.FormatCpf(cpfDemo);
            mtxtCpf.Text = cpfFormatado;

            var agendamentos = agendamentoRepository.BuscarPorCpf(cpfDemo);
            foreach (var ag in agendamentos)
            {
                txtNome.Text = ag.NomePaciente;
                break;
            }

            if (string.IsNullOrWhiteSpace(txtNome.Text))
                txtNome.Text = "PACIENTE DE DEMONSTRAÇÃO";

            AtualizarStatus("Modo demo: CPF de teste preenchido.");
        }

        private void BtnConfirmar_Click(object? sender, EventArgs e)
        {
            string cpfDigits = CpfHelper.OnlyDigits(mtxtCpf.Text);
            if (!CpfHelper.IsValidCpf(cpfDigits))
            {
                MessageBox.Show("CPF inválido. Corrija para continuar.", "CPF inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var agendamentos = agendamentoRepository.BuscarPorCpf(cpfDigits);

            if (agendamentos.Count == 0)
            {
                MessageBox.Show("Nenhum agendamento encontrado para este CPF.", "Sem agendamentos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AtualizarStatus("Nenhum agendamento encontrado.");
                return;
            }

            AtualizarStatus("Agendamentos encontrados. Abrindo tela de lista.");
            using (var frm = new FrmAgendamentos(txtNome.Text, cpfDigits, agendamentos))
            {
                frm.ShowDialog(this);
            }

            txtNome.Clear();
            mtxtCpf.Clear();
            capturedImage?.Dispose();
            capturedImage = null;
            picCaptured.Image?.Dispose();
            picCaptured.Image = null;
            AtualizarStatus("Fluxo concluído. Dados limpos da memória.");
        }

        private void AtualizarStatus(string mensagem)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => lblStatus.Text = "Status: " + mensagem));
            }
            else
            {
                lblStatus.Text = "Status: " + mensagem;
            }
        }
    }
}