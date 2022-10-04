using UnityEngine;

namespace Hathora {
    public abstract class ClientMessage {
        public virtual string ToJson() {
            return JsonUtility.ToJson(this);
        }
    }
}