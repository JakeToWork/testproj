namespace MasterCardServer{

    public class Message{
        public enum type{
            player,
            server
        }
        public string fromSockID{get;set;}
        public string toSockID{get;set;}
        public type messageType{get;set;}
        public string message{get;set;}
    }
}