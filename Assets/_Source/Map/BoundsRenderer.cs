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

        Vector3Int[] positions = new Vector3Int[2 * width + 2 * height + 4];
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

        for (int x = left + 1; x < right; x++) {

            Vector3Int tPosition = new(x, top, 0);
            Vector3Int bPosition = new(x, bottom, 0);

            positions[index] = tPosition;
            floorTiles[index] = topWall;

            positions[index+1] = bPosition;
            floorTiles[index+1] = bottomWall;

            index += 2;
        }

        for (int y = bottom + 1; y < top; y++) {

            Vector3Int lPosition = new(left, y, 0);
            Vector3Int rPosition = new(right, y, 0);

            positions[index] = lPosition;
            floorTiles[index] = leftWall;

            positions[index + 1] = rPosition;
            floorTiles[index + 1] = rightWall;

            index += 2;
        }

        tilemap.SetTiles(positions, floorTiles);
    }
}
