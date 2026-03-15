# \# Totem de Check-In com OCR

# 

# Sistema de autoatendimento offline para clínicas desenvolvido em C# / .NET.

# O paciente chega ao totem, posiciona o documento na câmera, o sistema lê

# o CPF automaticamente via OCR, valida o agendamento e imprime a senha térmica.

# Sem recepcionista, sem fila, sem digitação.

# 

# \## Como funciona

# 

# 1\. A webcam captura a imagem do documento (RG ou CNH)

# 2\. O Tesseract OCR extrai o CPF automaticamente

# 3\. O sistema busca os agendamentos do paciente nos dados locais

# 4\. O paciente seleciona o agendamento e confirma o check-in

# 5\. A senha é gerada (formato A000) e impressa na impressora térmica de 58mm

# 6\. O sistema encerra automaticamente

# 

# \## Tecnologias

# 

# \- C# / Windows Forms / .NET

# \- Tesseract OCR

# \- Impressora térmica 58mm (protocolo ESC/POS)

# \- Arquitetura modular em camadas (OCR, Impressão, Dados, Interface)

# 

# \## Funcionalidades

# 

# \- Captura ao vivo via webcam com congelamento de frame

# \- Extração automática de CPF por OCR (tempo máximo: 3 segundos)

# \- Edição manual do CPF como fallback

# \- Modo demonstração para testes sem câmera

# \- Validação completa do CPF (formato + dígitos verificadores)

# \- Listagem de agendamentos ordenados por data e hora

# \- Visualização de prontuário (somente leitura)

# \- Geração de senha randômica (A000)

# \- Impressão térmica com layout otimizado para bobina de 58mm

# \- Operação 100% offline

# 

# \## Requisitos

# 

# \- Windows 10 ou superior

# \- .NET Framework instalado

# \- Webcam conectada

# \- Impressora térmica de 58mm (opcional para testes)

# 

# \## Desenvolvido por

# 

# Pedro Henrique Passos Souza e Luis Felipe Alves Zapata

# FUNEPE — Engenharia de Software, 2025

