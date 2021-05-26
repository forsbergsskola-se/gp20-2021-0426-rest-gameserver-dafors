using System.Threading.Tasks;
using MongoDB.Driver;

namespace LameScooter.RentalServices {
    public class MongoDBLameScooterRental : IRental {
        private MongoClient client;
        public int GetScooterCountInStation(string nameOfStation) {
            throw new System.NotImplementedException();
        }

        public void Init(string uri ="") {
            InitAsync(uri).GetAwaiter().GetResult();
        }
        
        public async Task InitAsync(string uri = "") {
            client = new MongoClient("");
        }
    }
}