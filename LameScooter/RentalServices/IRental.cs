using System.Threading.Tasks;

namespace LameScooter.RentalServices {
    public interface IRental {
        public int GetScooterCountInStation(string nameOfStation);
        public void Init();
    }

    public interface IRentalAsync : IRental {
        Task InitAsync();
    }
}