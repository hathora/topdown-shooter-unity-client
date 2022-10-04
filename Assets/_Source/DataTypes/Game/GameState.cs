using System;

namespace DataTypes.Game {

    [Serializable]
    public class GameState {
        public PlayerData[] players;
        public BulletData[] bullets;
    }
}