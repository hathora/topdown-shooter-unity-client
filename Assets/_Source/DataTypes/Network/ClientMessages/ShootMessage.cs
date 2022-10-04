using Hathora;

namespace DataTypes.Network.ClientMessages {
    public class ShootMessage : ClientMessage {
        public ClientMessageType type;

        public ShootMessage() {
            this.type = ClientMessageType.Shoot;
        }
    }
}