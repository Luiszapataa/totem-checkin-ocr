using System;
using System.Linq;


//logica do cpf

namespace TotemCheckinOCR.Utils
{
    public static class CpfHelper
    {
        public static string OnlyDigits(string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        public static string FormatCpf(string cpfDigits)
        {
            cpfDigits = OnlyDigits(cpfDigits);
            if (cpfDigits.Length != 11)
                return cpfDigits;

            string s = cpfDigits;
            s = s.Insert(3, ".");
            s = s.Insert(7, ".");
            s = s.Insert(11, "-");
            return s;
        }

        public static bool IsValidCpf(string cpf)
        {
            cpf = OnlyDigits(cpf);

            if (cpf.Length != 11)
                return false;

            if (new string(cpf[0], cpf.Length) == cpf)
                return false;

            int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * mult1[i];

            int remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            string digit = remainder.ToString();
            tempCpf = tempCpf + digit;
            sum = 0;

            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * mult2[i];

            remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            digit = digit + remainder.ToString();
            return cpf.EndsWith(digit);
        }
    }
}