using System;

namespace DataTypes.Game {

    [Serializable]
    public class PlayerData {
        public string id;
        public Position position;
        public int aimAngle;
    }
}