using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using EEBridgeIrc.Irc.Commands;

namespace EEBridgeIrc.Irc
{
    public class IrcCommandProcessor
    {
        private readonly IrcController _controller;
        private readonly Dictionary<string, IReceivedCommand> _availableCommands;
        private readonly Dictionary<IReceivedCommand, IrcCommandAttribute> _commandProperties;

        public IrcCommandProcessor(IrcController controller)
        {
            _controller = controller;
            _commandProperties = new Dictionary<IReceivedCommand, IrcCommandAttribute>();
            _availableCommands = FindAllAvailableCommands();
        }

        public void ProcessCommand(IrcClient client, string command)
        {
            var commandDetails = Parse(command);
            foreach (var pair in _availableCommands)
            {
                if (!pair.Key.Equals(commandDetails.Key, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (!client.UserActivated && _commandProperties[pair.Value].RequiresActivatedUser)
                    continue;

                pair.Value.ProcessCommand(commandDetails.Value, client, _controller);
                return;
            }
        }

        private KeyValuePair<string, string[]> Parse(string rawCommand)
        {
            var arguments = new List<string>();
            if (string.IsNullOrWhiteSpace(rawCommand))
                return new KeyValuePair<string, string[]>();

            var commandParts = rawCommand.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            
            // If the first part has a colon, than its an identifier and we can ignore it
            var partIndex = 0;
            if (commandParts[partIndex].StartsWith(":"))
                partIndex++;

            // Make sure we have any valid arguments left
            if (partIndex >= commandParts.Length)
                return new KeyValuePair<string, string[]>();

            // Extract the command name
            var commandName = commandParts[partIndex];
            partIndex++;

            // Gather the rest of the arguments
            while (partIndex < commandParts.Length)
            {
                // If this argument starts with a colon, it means it's the last argument provided
                //   and it should be combined with all arguments afterwards
                if (commandParts[partIndex].StartsWith(":"))
                {
                    var builder = new StringBuilder(commandParts[partIndex].Substring(1));
                    partIndex++;

                    while (partIndex < commandParts.Length)
                    {
                        builder.Append(" ");
                        builder.Append(commandParts[partIndex]);
                        partIndex++;
                    }

                    arguments.Add(builder.ToString());
                }
                else
                {
                    arguments.Add(commandParts[partIndex]);
                }

                partIndex++;
            }

            return new KeyValuePair<string, string[]>(commandName, arguments.ToArray());
        }

        private Dictionary<string, IReceivedCommand> FindAllAvailableCommands()
        {
            var commandMap = new Dictionary<string, IReceivedCommand>();
            var commands = Assembly.GetExecutingAssembly()
                                   .GetTypes()
                                   .Where(x => typeof(IReceivedCommand).IsAssignableFrom(x) && !x.IsInterface)
                                   .ToArray();

            foreach (var cmd in commands)
            {
                var commandAttribute = cmd.GetCustomAttributes(false)
                                          .OfType<IrcCommandAttribute>()
                                          .FirstOrDefault();

                if (commandAttribute == null)
                    continue;

                if (commandMap.ContainsKey(commandAttribute.CommandName))
                    throw new InvalidOperationException("Multiple received commands contain the command name: " +
                                                        commandAttribute.CommandName);

                var instance = Activator.CreateInstance(cmd) as IReceivedCommand;

                if (instance == null) {
                    throw new InvalidOperationException(
                       string.Format("Type {0} is not a IReceivedCommand", cmd.Name));
                }

                commandMap.Add(commandAttribute.CommandName, instance);
                _commandProperties.Add(instance, commandAttribute);
            }

            return commandMap;
        }
    }
}
