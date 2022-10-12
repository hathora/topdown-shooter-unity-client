using Hathora;

namespace DataTypes.Network.ClientMessages {
    public class SetAngleMessage : ClientMessage {
        public ClientMessageType type;
        public float angle;

        public SetAngleMessage(float angle) {
            this.type = ClientMessageType.SetAngle;
            this.angle = angle;
        }
    }
}