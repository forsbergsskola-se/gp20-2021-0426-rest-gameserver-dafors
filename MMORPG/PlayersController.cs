using System;
using System.Threading.Tasks;

/*
The first responsibility of the controller is to define the routes for the API. 
Define the routes using attributes. There is [HttpPost], [HttpPut] and a few more.
The second responsibility is to handle the business logic. 
Business logic is a term for the core of your application. 
Everything that creates transactions that change your data / model. 
This can include things such as generating IDs when creating a player, 
and deciding which properties to change when modifying a player.
 */

namespace MMORPG {
    public class PlayersController {
        private IRepository repository;
        
        public PlayersController(IRepository repo) {
            this.repository = repo;
        }
        public Task<Player> Get(Guid id) {
            throw new NotImplementedException();
        }

        public Task<Player[]> GetAll() {
            throw new NotImplementedException();
        }

        public Task<Player> Create(NewPlayer player) {
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