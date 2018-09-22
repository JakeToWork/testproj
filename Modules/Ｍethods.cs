using Microsoft.EntityFrameworkCore;

namespace MasterCardServer{
    public static class Methods{
        public static  DbContextOptions<myDB> DbOption (){
            var options= new DbContextOptionsBuilder<myDB>()
                .UseInMemoryDatabase(databaseName: "Rooms")
                .Options;
            return options;
        }
        public static myDB getDb(){
            return  new myDB(Methods.DbOption());
        }
        //public static readonly RoomContext _roomContext; 
    }
}