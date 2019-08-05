// Based on the sample at https://github.com/Azure/azure-event-hubs/tree/master/samples/DotNet/Microsoft.Azure.EventHubs/SampleSender

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using pelazem.rndgen;
using pelazem.util;

namespace Sender
{
	class Program
	{
		private static string _event_hub_conn_string = string.Empty;

		private static volatile bool _keepRunning = true;

		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				_event_hub_conn_string = args[0];
			}
			else
			{
				_event_hub_conn_string = Environment.GetEnvironmentVariable("EventHubConnString");
			}

			if (string.IsNullOrWhiteSpace(_event_hub_conn_string))
			{
				Console.WriteLine("Please provide your Event Hub's connection string either as a command-line parameter, or as an environment variable.");
				Console.WriteLine("If providing as a command-line parameter, please enclose the Event Hub connection string in double quotes.");
				Console.WriteLine("If providing as an environment variable, please name the environment variable 'EventHubConnString'.");

				return;
			}

			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
				e.Cancel = true;
				_keepRunning = false;
			};

			Console.WriteLine("Running. Press Ctrl-C to end.");

			Run().Wait();

			Console.WriteLine("Exiting.");
		}

		private static async Task Run()
		{
			TripMessageGenerator generator = new TripMessageGenerator();

			// Creates an EventHubsConnectionStringBuilder object from a the connection string, and sets the EntityPath.
			// Typically the connection string should have the Entity Path in it, but for the sake of this simple scenario
			// we are using the connection string from the namespace.
			var connectionStringBuilder = new EventHubsConnectionStringBuilder(_event_hub_conn_string);

			EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

			while(_keepRunning)
			{
				string message = generator.GetMessage();

				try
				{
					Console.WriteLine(message);
					Console.WriteLine();

					await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
				}
				catch (Exception ex)
				{
					Console.WriteLine();
					Console.WriteLine("ERROR! " + message);
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.StackTrace);
					Console.WriteLine();
				}

				await Task.Delay(Converter.GetInt32(RandomGenerator.Numeric.GetUniform(250, 2500)));
			}

			await eventHubClient.CloseAsync();
		}
	}
}
