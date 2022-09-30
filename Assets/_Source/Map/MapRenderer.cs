using UnityEngine;
using Hathora.DataTypes;

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
        int top = mapData.top;
        int bottom = mapData.bottom;
        int left = mapData.left;
        int right = mapData.right;

        // Make sure top > bottom and right > left
        //
        if (bottom > top) {
            top = mapData.bottom;
            bottom = mapData.top;
        }
        if (left > right) {
            right = mapData.left;
            left = mapData.right;
        }

        floor.Render(top, left, bottom, right);
        bounds.Render(top, left, bottom, right);

        obstacles.Render(mapData.walls);
    }
}
