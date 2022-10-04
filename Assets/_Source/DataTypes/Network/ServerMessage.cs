using System;

using DataTypes.Game;

namespace DataTypes.Network {

    [Serializable]
    public class ServerMessage {
        public ServerMessageType type;
        public GameState state;
        public int ts;
    }
}
