using System;
using System.Threading.Tasks;
/*
    The purpose of the class is to persist and manipulate the Player objects in a text file. 
    One possible solution is to serialize the players as JSON to the text file. 
    The text file name should be game-dev.txt. 
    You can use, for example, File.ReadAllTextAsync and File.WriteAllTextAsync methods for the implementation.
 */

namespace MMORPG {
    public class FileRepository : IRepository {
        public Task<Player> Get(Guid id) {
            throw new NotImplementedException();
        }

        public Task<Player[]> GetAll() {
            throw new NotImplementedException();
        }

        public Task<Player> Create(Player player) {
            throw new NotImplementedException();
        }

        public Task<Player> Modify(Guid id, ModifiedPlayer player) {
            throw new NotImplementedException();
        }

        public Task<Player> Delete(Guid id) {
            throw new NotImplementedException();
        }
    }
}