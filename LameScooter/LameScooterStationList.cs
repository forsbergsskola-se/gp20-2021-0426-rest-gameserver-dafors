using System.Collections.Generic;
namespace LameScooter {
    public class LameScooterStationList {
        public string id { get; set; }
        public string name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public int bikesAvailable { get; set; }
        public int spacesAvailable { get; set; }
        public int capacity { get; set; }
        public bool allowDropoff { get; set; }
        public bool allowOverloading { get; set; }
        public bool isFloatingBike { get; set; }
        public bool isCarStation { get; set; }
        public string state { get; set; }
        public List<string> networks { get; set; }
        public bool realTimeData { get; set; }

        public override string ToString() {
            return name;
        }

        // public int id;
        // public string name;
        // public float x;
        // public float y;
        // public int bikesAvailable;
        // public int spacesAvailable;
        // public int capacity;
        // public bool allowDropoff;
        // public bool allowOverLoading;
        // public bool isFloatingBike;
        // public bool isCarStation;
        // public string state;
        // public string[] networks;
        // public bool realTimeData;
    }
}