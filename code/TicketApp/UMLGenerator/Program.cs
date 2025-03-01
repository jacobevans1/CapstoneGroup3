using System.Reflection;

namespace UMLGenerator
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// Path to your TicketAppWeb project DLL (adjust as needed)
			string ticketAppWebPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "TicketAppWeb");
			string outputPath = Path.Combine(ticketAppWebPath, "ClassDiagram.puml");

			// Load the TicketAppWeb assembly
			var dllPath = Path.Combine(ticketAppWebPath, "bin", "Debug", "net8.0", "TicketAppWeb.dll");
			if (!File.Exists(dllPath))
			{
				Console.WriteLine($"Error: Could not find TicketAppWeb.dll at {dllPath}");
				return;
			}

			var assembly = Assembly.LoadFrom(dllPath);

			using (StreamWriter writer = new StreamWriter(outputPath))
			{
				writer.WriteLine("@startuml");

				// Get all classes in the TicketAppWeb namespace (including sub-namespaces like Models, Controllers, ViewModels)
				var types = assembly.GetTypes()
					.Where(t => t.IsClass && t.Namespace != null && t.Namespace.StartsWith("TicketAppWeb"));

				foreach (var type in types)
				{
					writer.WriteLine($"class {type.Name} {{");

					// Add properties (fields)
					foreach (var prop in type.GetProperties())
					{
						writer.WriteLine($"+ {prop.PropertyType.Name} {prop.Name}");
					}

					// Add methods
					foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
					{
						if (!method.IsSpecialName) // Skip property getters/setters
						{
							var parameters = string.Join(", ", method.GetParameters()
								.Select(p => $"{p.ParameterType.Name} {p.Name}"));
							writer.WriteLine($"+ {method.ReturnType.Name} {method.Name}({parameters})");
						}
					}

					writer.WriteLine("}");
				}

				// Draw relationships between Controllers and Models/Views
				foreach (var type in types)
				{
					if (type.Name.Contains("Controller") || type.Name.Contains("ViewModel") || type.Name.Contains("Model"))
					{
						foreach (var prop in type.GetProperties())
						{
							if (types.Contains(prop.PropertyType))
							{
								writer.WriteLine($"{type.Name} --> {prop.PropertyType.Name}");
							}
						}
					}
				}

				writer.WriteLine("@enduml");
			}

			Console.WriteLine($"Class diagram generated at: {outputPath}");
		}
	}
}
