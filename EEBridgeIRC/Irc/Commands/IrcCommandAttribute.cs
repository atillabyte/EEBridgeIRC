using System;

namespace EEBridgeIrc.Irc.Commands
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class IrcCommandAttribute : Attribute
    {
        public string CommandName { get; }
        public bool RequiresActivatedUser { get; set; }

        public IrcCommandAttribute(string commandName, bool requiresActivatedUser = true)
        {
            CommandName = commandName;
            RequiresActivatedUser = requiresActivatedUser;
        }
    }
}
