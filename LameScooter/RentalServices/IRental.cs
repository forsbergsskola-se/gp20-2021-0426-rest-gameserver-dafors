﻿using System.Threading.Tasks;

namespace LameScooter.RentalServices {
    public interface IRental {
        public int GetScooterCountInStation(string nameOfStation);
        public void Init(string uri);
    }

    public interface IRentalAsync : IRental {
        Task InitAsync(string uri);
    }
}