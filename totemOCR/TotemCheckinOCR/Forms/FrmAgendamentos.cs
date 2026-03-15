using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using TotemCheckinOCR.Models;
using TotemCheckinOCR.Utils;

namespace TotemCheckinOCR.Forms
{
    public class FrmAgendamentos : Form
    {
        private Label lblHeader;
        private ListBox lstAgendamentos;
        private Button btnVerProntuario;
        private Button btnCheckin;
        private Button btnFechar;

        private readonly string nomePaciente;
        private readonly string cpfPaciente;
        private readonly List<Agendamento> agendamentos;

        private static readonly Random _random = new Random();

        public FrmAgendamentos(string nome, string cpf, List<Agendamento> agendamentos)
        {
            this.nomePaciente = nome;
            this.cpfPaciente = cpf;
            this.agendamentos = agendamentos;

            Text = "Agendamentos do Paciente";
            Width = 700;
            Height = 450;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            lblHeader = new Label
            {
                Text = $"Paciente: {nomePaciente} | CPF: {FormatarCpf(cpfPaciente)}",
                AutoSize = true,
                Top = 10,
                Left = 10
            };

            lstAgendamentos = new ListBox
            {
                Top = lblHeader.Bottom + 10,
                Left = 10,
                Width = 660,
                Height = 280
            };

            foreach (var ag in agendamentos.OrderBy(a => a.DataHora))
                lstAgendamentos.Items.Add(ag);

            btnVerProntuario = new Button
            {
                Text = "Visualizar Prontuário",
                Top = lstAgendamentos.Bottom + 10,
                Left = 10,
                Width = 160
            };
            btnVerProntuario.Click += BtnVerProntuario_Click;

            btnCheckin = new Button
            {
                Text = "Check-in",
                Top = lstAgendamentos.Bottom + 10,
                Left = btnVerProntuario.Right + 10,
                Width = 100
            };
            btnCheckin.Click += BtnCheckin_Click;

            btnFechar = new Button
            {
                Text = "Fechar",
                Top = lstAgendamentos.Bottom + 10,
                Left = btnCheckin.Right + 10,
                Width = 100
            };
            btnFechar.Click += (s, e) => Close();

            Controls.Add(lblHeader);
            Controls.Add(lstAgendamentos);
            Controls.Add(btnVerProntuario);
            Controls.Add(btnCheckin);
            Controls.Add(btnFechar);
        }

        private void BtnVerProntuario_Click(object? sender, EventArgs e)
        {
            if (lstAgendamentos.SelectedItem is not Agendamento selecionado)
            {
                MessageBox.Show("Selecione um agendamento.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var frm = new FrmProntuario(selecionado))
                frm.ShowDialog(this);
        }

        private void BtnCheckin_Click(object? sender, EventArgs e)
        {
            if (lstAgendamentos.SelectedItem is not Agendamento)
            {
                MessageBox.Show("Selecione um agendamento.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string senha = $"A{_random.Next(1, 1000):000}";

            ImprimirSenha(senha);

            MessageBox.Show($"Check-in confirmado!\n\nSua senha: {senha}",
                            "Check-in",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

            Application.Exit();
        }
        private void ImprimirSenha(string senha)
        {
            string titulo = "CLÍNICA DEV </>";
            string subtitulo = "Totem de Check-in";
            string linhaSeparadora = "--------------------------------";

            PrintDocument pd = new PrintDocument();

            pd.PrintPage += (sender, e) =>
            {
                float y = 10;
                float left = 5;
                float line = 20;

                Font fontTitulo = new Font("Consolas", 12, FontStyle.Bold);
                Font fontNormal = new Font("Consolas", 9);
                Font fontSecao = new Font("Consolas", 10, FontStyle.Bold);
                Font fontSenha = new Font("Consolas", 22, FontStyle.Bold);

                StringFormat center = new StringFormat { Alignment = StringAlignment.Center };

                e.Graphics.DrawString(titulo, fontTitulo, Brushes.Black,
                    e.PageBounds.Width / 2, y, center);
                y += line;

                e.Graphics.DrawString(subtitulo, fontNormal, Brushes.Black,
                    e.PageBounds.Width / 2, y, center);
                y += line;

                e.Graphics.DrawString(linhaSeparadora, fontNormal, Brushes.Black, left, y);
                y += line;

                e.Graphics.DrawString("Paciente:", fontSecao, Brushes.Black, left, y);
                y += line;

                string nome = nomePaciente.Length > 32
                                ? nomePaciente.Substring(0, 32)
                                : nomePaciente;

                e.Graphics.DrawString(nome, fontNormal, Brushes.Black, left, y);
                y += line;

                e.Graphics.DrawString(linhaSeparadora, fontNormal, Brushes.Black, left, y);
                y += line;

                e.Graphics.DrawString("SENHA:", fontSecao, Brushes.Black,
                    e.PageBounds.Width / 2, y, center);
                y += line;

                e.Graphics.DrawString(senha, fontSenha, Brushes.Black,
                    e.PageBounds.Width / 2, y, center);
                y += 45;

                e.Graphics.DrawString(linhaSeparadora, fontNormal, Brushes.Black, left, y);
                y += line;

                string data = "Data: " + DateTime.Now.ToString("dd/MM/yyyy");
                e.Graphics.DrawString(data, fontNormal, Brushes.Black,
                    e.PageBounds.Width / 2, y, center);
                y += line;

                string hora = "Hora: " + DateTime.Now.ToString("HH:mm");
                e.Graphics.DrawString(hora, fontNormal, Brushes.Black,
                    e.PageBounds.Width / 2, y, center);
                y += line;

                string msgFinal = "Aguarde ser chamado.";
                e.Graphics.DrawString(msgFinal, fontNormal, Brushes.Black,
                    e.PageBounds.Width / 2, y, center);
            };

            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao enviar para a impressora: " + ex.Message,
                                "Erro de impressão",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private static string FormatarCpf(string cpfDigits)
        {
            return CpfHelper.FormatCpf(cpfDigits);
        }
    }
}
