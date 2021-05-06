using System.Collections.Generic;
namespace LameScooter {
    public class LameScooterStationList {
        public LameScooterStationList(string name, int bikesAvailable) {
            this.name = name;
            this.bikesAvailable = bikesAvailable;
        }
        
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
    }
}