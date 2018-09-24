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
            var tosock = WebSocketConnectionManager.GetSocketById(msg.toSockID);
            if(tosock is null){
                var systemmessage = new Message
                {
                    fromSockID = msg.fromSockID,
                    toSockID = msg.toSockID,
                    messageType = Message.type.server,
                    message = "Target is not Online"
                };
                string msgstr = JsonConvert.SerializeObject(systemmessage);
                await SendMessageAsync(socketId, msgstr); 
            }
            else if(tosock.State != WebSocketState.Connecting &&  tosock.State != WebSocketState.Open){
                var systemmessage = new Message
                {
                    fromSockID = msg.fromSockID,
                    toSockID = msg.toSockID,
                    messageType = Message.type.server,
                    message = "Target is not Online"
                };
                using (var context = Methods.getDb())
                { 
                    var room = context.Rooms.Where(x=>x.SocketID==msg.toSockID);
                    if(room.Any())
                    { 
                         context.Rooms.RemoveRange(room);
                    } 
                    var sockid = context.Players.Where(x=>x.SockID==msg.toSockID);
                    if(sockid.Any())
                    {
                        var players= context.Players.Where(x=>x.roomID == sockid.Single().roomID && x.SockID!=msg.toSockID); 
                        foreach(var p in players){
                             Message mesg = new Message
                            {
                                fromSockID = "",
                                toSockID = p.SockID,
                                messageType = Message.type.server,
                                message = string.Format("Player [{0}] has disconnected",sockid.Single().playerName)
                            };
                            string msgstr = JsonConvert.SerializeObject(mesg);
                            await SendMessageAsync(p.SockID, msgstr); 
                        } 
                        context.Players.RemoveRange(sockid);
                    } 
                    await context.SaveChangesAsync();
                }
                await SendMessageAsync(msg.fromSockID,JsonConvert.SerializeObject(systemmessage));
            }else{
                await SendMessageAsync(msg.toSockID,message);
            }
        }
    }
}