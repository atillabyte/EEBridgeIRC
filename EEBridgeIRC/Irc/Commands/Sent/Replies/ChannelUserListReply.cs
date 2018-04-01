using System.Text;

namespace EEBridgeIrc.Irc.Commands.Sent.Replies
{
    public class ChannelUserListReply : ISentCommand
    {
        private const int ResponseCode = 353;

        public string SenderAddress { get; set; }
        public string RecipientNickName { get; set; }
        public string ChannelName { get; set; }
        public string[] Users { get; set; }

        public string FormFullResponseString()
        {
            var response = new StringBuilder();
            response.AppendFormat(":{0} {1} {2} @ #{3} :{2}",
                SenderAddress, ResponseCode, RecipientNickName, ChannelName);

            foreach (var user in Users)
            {
                response.Append(" +");
                response.Append(user);
            }

            return response.ToString();
        }
    }
}
