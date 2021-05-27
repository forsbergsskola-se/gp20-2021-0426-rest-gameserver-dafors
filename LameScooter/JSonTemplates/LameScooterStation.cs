using MongoDB.Bson.Serialization.Attributes;

namespace LameScooter.JSonTemplates {
    
    [BsonIgnoreExtraElements]
    public class LameScooterStation {
        public LameScooterStation(string name, int bikesAvailable) {
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