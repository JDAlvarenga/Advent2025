using System.Reflection;
using Advent2025.Solutions;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Spectre.Console;



List<Type> solutionTypes = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t is { IsClass: true, IsPublic:true, Namespace: "Advent2025.Solutions" } && t.GetInterfaces().Contains(typeof(IDay)))
    .ToList();

AnsiConsole.MarkupLine("Advent of Code [yellow bold]2025[/]");
while (true)
{
    var action = PromptAction();
    if (action == Action.Exit) return;
    
    var selected = PromptSolutions(solutionTypes);
    if (selected.Count == 0) continue;
    
    switch (action)
    {
        case Action.ShowResults:
            
            var useSample = AnsiConsole.Prompt(
                new TextPrompt<bool>("Run with sample input?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(false)
                    .WithConverter(choice => choice ? "y" : "n"));
            
            
            var table = new Table()
                .Title("Results")
                .RoundedBorder();
            table.AddColumn(new TableColumn("Day").Centered());
            table.AddColumn(new TableColumn("Result").Centered());
            foreach (var solutionClass in selected)
            {
                var methods = solutionClass.GetMethods().Where(m => m.Name.StartsWith("Part"));
                var initiatedObject = Activator.CreateInstance(solutionClass); 
                foreach (var method in methods)
                {
                    try
                    {
                        var result = method.Invoke(initiatedObject, [useSample]);
                        table.AddRow($"{method.DeclaringType?.Name}/{method.Name}", result?.ToString()?? "null");
                    }
                    catch (Exception e)
                    {
                        AnsiConsole.WriteException(e);
                        table.AddRow($"{method.DeclaringType?.Name}/{method.Name}", "[red]Error[/]");
                    }
                }
            }
            
            AnsiConsole.Write(table);
                
            break;
        case Action.BenchMark:
            BenchmarkRunner.Run(selected.ToArray(), 
                DefaultConfig.Instance.
                    WithOptions(ConfigOptions.JoinSummary)
                    .AddDiagnoser(MemoryDiagnoser.Default)
                    .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Alphabetical)));
            break;
    }
}


static List<Type> PromptSolutions(List<Type> solutions)
{
    return AnsiConsole.Prompt(
        new MultiSelectionPrompt<Type>()
            .Title("Choose the solutions to run")
            .PageSize(7)
            .MoreChoicesText("[grey](Move up and down to reveal solutions)[/]")
            .InstructionsText(
                "[grey](Press [blue]<space>[/] to toggle a solution, " + 
                "[green]<enter>[/] to accept)[/]")
            .AddChoices(solutions));
}
static Action PromptAction()
{
    return AnsiConsole.Prompt(
        new SelectionPrompt<Action>()
            .Title("What do you want to do?")
            .AddChoices(Enum.GetValues<Action>()));
}
internal enum Action
{
    ShowResults, BenchMark, Exit
}