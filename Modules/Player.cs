using System.ComponentModel.DataAnnotations;

namespace MasterCardServer{
    public class Player{
        
        [Key]
        public string SockID{get;set;}
        public string playerName{get;set;}
        public int roomID{get;set;}
        
    }
}