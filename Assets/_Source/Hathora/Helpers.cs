using UnityEngine;

namespace Hathora {
    public static class Helpers {

        public static void LogPoint(Vector3 point, string prefix = "") {
            Debug.Log(string.Format("{0}({1}, {2}, {3})", prefix, point.x, point.y, point.z));
        }
    }
}