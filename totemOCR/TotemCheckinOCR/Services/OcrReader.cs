using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tesseract;
using TotemCheckinOCR.Utils;

namespace TotemCheckinOCR.Services
{
    public class OcrReader
    {
        private readonly string tessdataPath;

        public OcrReader()
        {
            tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
        }

        public (string? Nome, string? Cpf) LerDados(Bitmap imagem)
        {
            if (!Directory.Exists(tessdataPath))
                throw new DirectoryNotFoundException($"Pasta tessdata não encontrada em: {tessdataPath}");

            using var engine = new TesseractEngine(tessdataPath, "por", EngineMode.Default);

            engine.DefaultPageSegMode = PageSegMode.Auto;

            using var ms = new MemoryStream();
            imagem.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;

            using var pix = Pix.LoadFromMemory(ms.ToArray());
            using var page = engine.Process(pix);

            string text = page.GetText() ?? string.Empty;

            string? cpf = ExtrairCpf(text);
            string? nome = ExtrairNome(text);

            return (nome, cpf);
        }

        private static string? ExtrairCpf(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            
            var linhas = text //procura o CPF qubrando a linha
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToList();

            foreach (var linha in linhas) 
            {
                string upper = linha.ToUpperInvariant();
                if (upper.Contains("CPF"))
                {
                    string digitsInLine = new string(linha.Where(char.IsDigit).ToArray());
                    if (digitsInLine.Length >= 11)
                    {
                        for (int i = 0; i <= digitsInLine.Length - 11; i++)//tenta encontrar uyn cpf valido

                        {
                            string cand = digitsInLine.Substring(i, 11);
                            if (CpfHelper.IsValidCpf(cand))
                                return cand;
                        }
                        return digitsInLine.Substring(0, 11);
                    }
                }
            }
            string allDigits = new string(text.Where(char.IsDigit).ToArray());
            if (allDigits.Length < 11)
                return null;

            for (int i = 0; i <= allDigits.Length - 11; i++)
            {
                string cand = allDigits.Substring(i, 11);
                if (CpfHelper.IsValidCpf(cand))
                    return cand;
            }

            return allDigits.Length >= 11 ? allDigits.Substring(0, 11) : null;//retoorns primeiros 11 digitos
        }

        private static string? ExtrairNome(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var linhas = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var linha in linhas)
            {
                string trimmed = linha.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                int digitCount = trimmed.Count(char.IsDigit);
                if (digitCount > 3)
                    continue;

                var palavras = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (palavras.Length >= 2)
                {
                    return trimmed.ToUpperInvariant();
                }
            }

            return null;
        }
    }
}
