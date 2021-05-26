namespace LameScooter.JSonTemplates {
    public class LameScooterStationList {
        public LameScooterStationList(string name, int bikesAvailable) {
            this.name = name;
            this.bikesAvailable = bikesAvailable;
        }
        public string name { get; set; }
        public int bikesAvailable { get; set; }
        public override string ToString() {
            return name;
        }
    }
}