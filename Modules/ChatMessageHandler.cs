using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks; 
using Newtonsoft.Json;
namespace MasterCardServer
{
    public class ChatMessageHandler : WebSocketHandler
    {
        public ChatMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {

        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

        
        }
        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message =  Encoding.UTF8.GetString(buffer, 0, result.Count) ;
            var msg = JsonConvert.DeserializeObject<Message>(message);

            await SendMessageAsync(msg.toSockID,msg.message);
        }
    }
}