using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

// Classe para representar um compromisso
class Compromisso
{
    public DateTimeOffset DataHora { get; set; }
    public string Descricao { get; set; }
}

class Program
{
    // Lista para armazenar a agenda
    static List<Compromisso> agenda = new List<Compromisso>();

    static void Main(string[] args)
    {
        bool executando = true;

        while (executando)
        {
            Console.WriteLine("\n=== AGENDA COM FUSO HORÁRIO ===");
            Console.WriteLine("1. Adicionar Compromisso");
            Console.WriteLine("2. Ver compromissos de HOJE (informando o fuso)");
            Console.WriteLine("3. Ver compromissos por DATA (informando o fuso)");
            Console.WriteLine("4. Sair");
            Console.Write("Escolha uma opção: ");

            string opcao = Console.ReadLine();

            try
            {
                switch (opcao)
                {
                    case "1":
                        AdicionarCompromisso();
                        break;
                    case "2":
                        ExibirCompromissosHoje();
                        break;
                    case "3":
                        ExibirCompromissosPorData();
                        break;
                    case "4":
                        executando = false;
                        Console.WriteLine("Encerrando a agenda...");
                        break;
                    default:
                        Console.WriteLine("Opção inválida! Tente novamente.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("\nERRO: Formato inválido! Verifique como você digitou a data.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERRO: {ex.Message}");
            }
        }
    }

    static void AdicionarCompromisso()
    {
        Console.WriteLine("\n--- Adicionar Compromisso ---");
        Console.WriteLine("Formato esperado: dd/MM/yyyy HH:mm (Ex: 25/12/2023 15:00)");

        Console.WriteLine("Digite a data e hora do seu compromisso: ");
        var padraoBR = new CultureInfo("pt-BR");
        var dataHora = DateTimeOffset.Parse(Console.ReadLine(), padraoBR);

        Console.WriteLine("Digite a descrição do seu compromisso");
        var descricao = Console.ReadLine();

        // CORREÇÃO: Usando a formatação do próprio C# para garantir que os zeros apareçam (ex: 12:00 em vez de 12:0)
        Console.WriteLine($"Compromisso agendado:\n" +
            $"{dataHora:dd/MM/yyyy} às {dataHora:HH:mm} - {descricao}");

        // Salvando na lista
        agenda.Add(new Compromisso { DataHora = dataHora, Descricao = descricao });
    }

    static void ExibirCompromissosHoje()
    {
        Console.WriteLine("\n--- Compromissos de Hoje ---");
        Console.Write("Informe o fuso horário desejado (Ex: -03:00, +01:00, 00:00): ");

        // CORREÇÃO: Removemos o sinal de "+" se o usuário digitar, pois o TimeSpan não aceita "+"
        string inputFuso = Console.ReadLine().Replace("+", "");
        TimeSpan fuso = TimeSpan.Parse(inputFuso);

        // Descobre que dia é "hoje" no fuso horário informado
        DateTimeOffset hojeNoFuso = DateTimeOffset.UtcNow.ToOffset(fuso);

        Console.WriteLine($"\nMostrando compromissos para HOJE ({hojeNoFuso:dd/MM/yyyy}) no fuso {fuso}:");
        FiltrarEExibirPorData(hojeNoFuso.Date, fuso);
    }

    static void ExibirCompromissosPorData()
    {
        Console.WriteLine("\n--- Compromissos por Data ---");
        Console.Write("Informe a data desejada (dd/MM/yyyy): ");
        var padraoBR = new CultureInfo("pt-BR");
        DateTime dataDesejada = DateTime.Parse(Console.ReadLine(), padraoBR);

        Console.Write("Informe o fuso horário desejado (Ex: -03:00, +01:00, 00:00): ");

        // CORREÇÃO: Removemos o sinal de "+" aqui também
        string inputFuso = Console.ReadLine().Replace("+", "");
        TimeSpan fuso = TimeSpan.Parse(inputFuso);

        Console.WriteLine($"\nMostrando compromissos para a data {dataDesejada:dd/MM/yyyy} no fuso {fuso}:");
        FiltrarEExibirPorData(dataDesejada.Date, fuso);
    }

    static void FiltrarEExibirPorData(DateTime dataAlvo, TimeSpan fuso)
    {
        var compromissosFiltrados = agenda
            .Select(c => new
            {
                Original = c,
                DataConvertida = c.DataHora.ToOffset(fuso)
            })
            .Where(c => c.DataConvertida.Date == dataAlvo)
            .OrderBy(c => c.DataConvertida)
            .ToList();

        if (compromissosFiltrados.Count == 0)
        {
            Console.WriteLine("Nenhum compromisso encontrado para este dia neste fuso horário.");
            return;
        }

        foreach (var item in compromissosFiltrados)
        {
            Console.WriteLine($"- {item.DataConvertida:HH:mm} : {item.Original.Descricao}");
        }
    }
}