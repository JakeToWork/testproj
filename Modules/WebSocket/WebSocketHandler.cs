using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static MasterCardServer.Methods;
namespace MasterCardServer
{
    public abstract class WebSocketHandler
    {
        protected WebSocketConnectionManager WebSocketConnectionManager { get; set; }
        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socket);
            var socketId = WebSocketConnectionManager.GetId(socket);
           
            await SendMessageAsync(socket, socketId);
            
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket); 
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));

             
            using (var context = getDb())
            { 
                var room = context.Rooms.Where(x=>x.SocketID==socketId);
                if(room.Any())
                { 
                     context.Rooms.RemoveRange(room);
                } 
                var sockid = context.Players.Where(x=>x.SockID==socketId);
                if(sockid.Any())
                {
                    var roomID = sockid.Single().roomID;
                    if(roomID != 0){ 
                        var players= context.Players.Where(x=>x.roomID == roomID && x.SockID!=socketId); 
                       
                        foreach(var p in players){
                            Message msg = new Message
                            {
                                fromSockID = "",
                                toSockID = p.SockID,
                                messageType = Message.type.server,
                                message = string.Format("Player [{0}] has disconnected",sockid.Single().playerName)
                            };
                            string msgstr = JsonConvert.SerializeObject(msg);
                            await SendMessageAsync(p.SockID, msgstr);
                        } 
                    }
                    context.Players.RemoveRange(sockid);
                } 
                await context.SaveChangesAsync();
            }
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if(socket.State != WebSocketState.Open)
                return; 
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.UTF8.GetBytes(message),
                                                                  offset: 0, 
                                                                  count: message.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);          
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach(var pair in WebSocketConnectionManager.GetAll())
            {
                if(pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}