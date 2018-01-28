﻿using CommandLine;
using COTS.GameServer.CommandLineArgumentsParsing;
using COTS.GameServer.Lua;
using System;
using COTS.GameServer.Network;
using Microsoft.Extensions.DependencyInjection;
using COTS.Infra.CrossCutting.Ioc;
using NetworkMessage = COTS.Infra.CrossCutting.Network.NetworkMessage;

namespace COTS.GameServer {

    public sealed class Program
    {

        private static ServiceProvider _serviceProvider;

        private static void Main(string[] args) {
            
            var serviceCollection = new ServiceCollection();
            BootStrapper.ConfigureGlobalServices(serviceCollection);
            ConfigureLocalServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            
            var parser = Parser.Default;
            var parseAttempt = parser.ParseArguments<CommandLineArguments>(args: args);

            if (parseAttempt is Parsed<CommandLineArguments> successfullyParsed) {
                RunWithSucessfullyParsedCommandLineArguments(successfullyParsed.Value);
            } else if (parseAttempt is NotParsed<CommandLineArguments> failedAttempt) {
                ReportCommandLineParsingError(failedAttempt);
            } else {
                throw new InvalidOperationException("Fo reals? This line should never be reached.");
            }

            //var original = "testiculos";
            //var encoded = NetworkMessage.Encode(original);
            //Console.WriteLine(encoded.Length);
            //var decoded = NetworkMessage.Decode(encoded);
            //Console.WriteLine(original == decoded);

            Console.ReadLine();
        }

        private static void RunWithSucessfullyParsedCommandLineArguments(CommandLineArguments commandLineArguments)
        {
            _serviceProvider.GetService<LuaManager>().Run();
            //_serviceProvider.GetService<ProtocolLogin>().StartListening();
            //_serviceProvider.GetService<ProtocolGame>().StartListening();

            //var clientConnectionManager = commandLineArguments.GetClientConnectionManager();
            //Task.Run(() => clientConnectionManager.StartListening());
            NetworkManager nm = new NetworkManager();
            nm.Start();
            Console.WriteLine("NON BLOCKING");
            //System.Threading.Thread.Sleep(3000);
            //nm.Close();
        }

        private static void ReportCommandLineParsingError(NotParsed<CommandLineArguments> failedAttempt) {
            throw new NotImplementedException();
        }

        public static void ConfigureLocalServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<LuaManager>();
            //serviceCollection.AddSin<ConnectionManager>();
            //serviceCollection.AddTransient<ProtocolLogin>();
            //serviceCollection.AddTransient<ProtocolGame>();
        }
    }
}