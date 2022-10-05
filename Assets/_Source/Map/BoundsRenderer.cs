using UnityEngine;
using UnityEngine.Tilemaps;

using DataTypes;

public class BoundsRenderer : MonoBehaviour {

    // Config
    //

    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    Tile leftWall;

    [SerializeField]
    Tile rightWall;

    [SerializeField]
    Tile topWall;

    [SerializeField]
    Tile bottomWall;

    [SerializeField]
    Tile topLeftWall;

    [SerializeField]
    Tile topRightWall;

    [SerializeField]
    Tile bottomLeftWall;

    [SerializeField]
    Tile bottomRightWall;

    //
    // \Config

    public void Render(int top, int left, int bottom, int right) {

        int width = Mathf.Abs(right - left);
        int height = Mathf.Abs(top - bottom);

        // Account for double-counting
        int numBoundsTiles = 2 * width + 2 * height + 4;

        Vector3Int[] positions = new Vector3Int[numBoundsTiles];
        Tile[] floorTiles = new Tile[positions.Length];

        // Top Left
        positions[0] = new(left, top, 0);
        floorTiles[0] = topLeftWall;

        // Top Right
        positions[1] = new(right, top, 0);
        floorTiles[1] = topRightWall;

        // Bottom Left
        positions[2] = new(left, bottom, 0);
        floorTiles[2] = bottomLeftWall;

        // Bottom Right
        positions[3] = new(right, bottom, 0);
        floorTiles[3] = bottomRightWall;

        int index = 4;

        // Top and Bottom Edges
        for (int x = (left + 1); x < right; x++) {

            positions[index] = new(x, top, 0);
            floorTiles[index] = topWall;
            index++;

            positions[index] = new(x, bottom, 0);
            floorTiles[index] = bottomWall;
            index++;
        }

        // Left and Right Edges
        for (int y = (bottom + 1); y < top; y++) {

            positions[index] = new(left, y, 0);
            floorTiles[index] = leftWall;
            index++;

            positions[index] = new(right, y, 0);
            floorTiles[index] = rightWall;
            index++;
        }

        tilemap.SetTiles(positions, floorTiles);
    }
}
