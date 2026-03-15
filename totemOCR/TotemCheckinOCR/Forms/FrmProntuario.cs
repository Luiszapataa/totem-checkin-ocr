using System;
using System.Windows.Forms;
using TotemCheckinOCR.Models;
using TotemCheckinOCR.Utils;

namespace TotemCheckinOCR.Forms
{
    public class FrmProntuario : Form
    {
        private readonly Agendamento agendamento;

        public FrmProntuario(Agendamento agendamento)
        {
            this.agendamento = agendamento;

            Text = "Prontuário do Paciente";
            Width = 600;
            Height = 450;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            int leftCol1 = 10;
            int leftCol2 = 120;
            int top = 10;
            int spacing = 30;

            void AddLabel(string titulo, string valor, bool multiLine = false, int height = 60)
            {
                var lblTitulo = new Label
                {
                    Text = titulo,
                    AutoSize = true,
                    Top = top + 4,
                    Left = leftCol1
                };
                Controls.Add(lblTitulo);

                if (multiLine)
                {
                    var txt = new TextBox
                    {
                        Top = top,
                        Left = leftCol2,
                        Width = 430,
                        Height = height,
                        Multiline = true,
                        ReadOnly = true,
                        Text = valor,
                        ScrollBars = ScrollBars.Vertical
                    };
                    Controls.Add(txt);
                    top += height + 10;
                }
                else
                {
                    var txt = new TextBox
                    {
                        Top = top,
                        Left = leftCol2,
                        Width = 430,
                        ReadOnly = true,
                        Text = valor
                    };
                    Controls.Add(txt);
                    top += spacing;
                }
            }

            AddLabel("Nome:", agendamento.NomePaciente);
            AddLabel("CPF:", CpfHelper.FormatCpf(agendamento.Cpf));
            AddLabel("Data/Hora:", agendamento.DataHora.ToString("dd/MM/yyyy HH:mm"));
            AddLabel("Médico:", agendamento.NomeMedico);
            AddLabel("CRM:", agendamento.Crm);
            AddLabel("Alergias:", agendamento.Alergias, multiLine: true);
            AddLabel("Medicamentos:", agendamento.Medicamentos, multiLine: true);
            AddLabel("Diagnóstico:", agendamento.Diagnostico, multiLine: true);
            AddLabel("Observações:", agendamento.Observacoes, multiLine: true);

            var btnFechar = new Button
            {
                Text = "Fechar",
                Width = 100,
                Top = top + 10,
                Left = Width / 2 - 70
            };
            btnFechar.Click += (s, e) => Close();
            Controls.Add(btnFechar);
        }
    }
}