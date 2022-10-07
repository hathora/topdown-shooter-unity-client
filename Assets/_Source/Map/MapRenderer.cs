using UnityEngine;
using DataTypes;

public class MapRenderer : MonoBehaviour {

    // Config
    //

    [SerializeField]
    FloorRenderer floor;

    [SerializeField]
    BoundsRenderer bounds;

    [SerializeField]
    ObstaclesRenderer obstacles;

    //
    // \Config

    public void Render(MapData mapData) {
        int bottom = mapData.bottom;
        int left   = mapData.left;

        int topBound = mapData.top;
        int rightBound = mapData.right;

        int top   = topBound - 1;
        int right = rightBound - 1;

        // Drawn inclusive
        floor.Render(top, left, bottom, right);

        // Bounds go around the floor, so expand each edge by 1
        // bounds.Render((top + 1), (left - 1), (bottom - 1), (right + 1));

        obstacles.Render(mapData.walls);
    }
}
