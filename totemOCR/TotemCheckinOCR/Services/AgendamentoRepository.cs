using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using TotemCheckinOCR.Models;
using TotemCheckinOCR.Utils;

namespace TotemCheckinOCR.Services
{
    public class AgendamentoRepository
    {
        private readonly List<Agendamento> agendamentos;

        public AgendamentoRepository()
        {
            agendamentos = CarregarDeRecurso();
        }

        private List<Agendamento> CarregarDeRecurso()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "TotemCheckinOCR.Data.agendamentos.json";

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new InvalidOperationException($"Recurso embutido não encontrado: {resourceName}");

            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var lista = JsonSerializer.Deserialize<List<Agendamento>>(json, options);
            return lista ?? new List<Agendamento>();
        }

        public List<Agendamento> BuscarPorCpf(string cpfDigits)
        {
            cpfDigits = CpfHelper.OnlyDigits(cpfDigits);
            var resultado = new List<Agendamento>();

            foreach (var ag in agendamentos)
            {
                if (CpfHelper.OnlyDigits(ag.Cpf) == cpfDigits)
                {
                    resultado.Add(ag);
                }
            }

            return resultado;
        }
    }
}