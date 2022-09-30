using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

using Hathora.DataTypes;

public class FloorRenderer : MonoBehaviour {

    // Config
    //

    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    Tile[] tiles;

    //
    // \Config

    private Tile GetRandomTile() {
        return tiles[Random.Range(0, tiles.Length)];
    }

    public void Render(int top, int left, int bottom, int right) {

        int width = Mathf.Abs(right - left) + 1;
        int height = Mathf.Abs(top - bottom) + 1;

        Vector3Int[] positions = new Vector3Int[width * height];
        Tile[] floorTiles = new Tile[positions.Length];

        int index = 0;
        for (int x = left; x <= right; x++) {
            for (int y = bottom; y <= top; y++) {
                Vector3Int position = new(x, y, 0);

                positions[index] = position;
                floorTiles[index] = GetRandomTile();

                index++;
            }
        }

        tilemap.SetTiles(positions, floorTiles);
    }
}
