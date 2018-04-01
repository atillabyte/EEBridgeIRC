using EEBridgeIrc.Irc.Commands.Sent;
using EEBridgeIrc.Irc.Commands.Sent.Errors;
using EEBridgeIrc.Irc.Commands.Sent.Replies;

namespace EEBridgeIrc.Irc.Commands.Received
{
    [IrcCommand("USER", false)]
    public class UserCommand : IReceivedCommand
    {
        public void ProcessCommand(string[] args, IrcClient client, IrcController controller)
        {
            if (args.Length < 4)
            {
                var replyCommand = new NeedMoreParamsError
                {
                    Command = "USER",
                    SenderAddress = Server.HostName
                };

                client.SendMessage(replyCommand.FormFullResponseString());
                return;
            }

            client.FullName = args[3];
            client.UserName = args[0];

            var connectUserId = Server.EEGuestClient.BigDB.Load("Usernames", client.UserName.ToLower()).GetString("owner", "");

            if (!connectUserId.StartsWith("simple")) {
                new WelcomeReply() {
                    Message = "Whoops! You are attempting to connect with a non-simple user, which is currently unsupported."
                }.SendMessageToClient(client);
                
                return;
            }

            client.UsernameOrEmail = connectUserId.Remove(0, "simple".Length);

            client.AttemptUserActivation();
            client.SetUserMask();
        }
    }
}
