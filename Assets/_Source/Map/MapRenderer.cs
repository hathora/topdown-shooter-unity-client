using UnityEngine;
using DataTypes;

public class MapRenderer : MonoBehaviour {

    // Config
    //

    [SerializeField]
    FloorRenderer floor;

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

        obstacles.Render(mapData.walls);
    }
}
