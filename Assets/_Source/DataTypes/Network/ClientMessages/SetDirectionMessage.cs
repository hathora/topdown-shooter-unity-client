using System;
using Hathora;
using DataTypes.Game;

namespace DataTypes.Network.ClientMessages {
    [Serializable]
    public class SetDirectionMessage : ClientMessage {
        public ClientMessageType type;
        public Direction direction;

        public SetDirectionMessage(Direction direction) {
            this.type = ClientMessageType.SetDirection;
            this.direction = direction;
        }
    }
}