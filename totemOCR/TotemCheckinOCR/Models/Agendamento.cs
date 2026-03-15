using System;

namespace TotemCheckinOCR.Models
{
    public class Agendamento
    {
        public string Cpf { get; set; } = string.Empty;
        public string NomePaciente { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public string NomeMedico { get; set; } = string.Empty;
        public string Crm { get; set; } = string.Empty;
        public string Alergias { get; set; } = string.Empty;
        public string Medicamentos { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{DataHora:dd/MM/yyyy HH:mm} - Dr(a). {NomeMedico} - {Diagnostico}";
        }
    }
}