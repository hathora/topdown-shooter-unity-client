using UnityEngine;
using UnityEngine.Tilemaps;

using DataTypes.Game;

public class ObstaclesRenderer : MonoBehaviour {

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

    private void RenderObstacle(int x, int y, int width, int height) {

        // Debug.Log(string.Format("WALL: ({0}, {1}): W:{2}, H:{3}", x, y, width, height));

        for (int i = x; i <= (x + width - 1); i++) {
            for (int j = y; j <= (y + height - 1); j++) {
                Vector3Int position = new(i, j, 0);
                tilemap.SetTile(position, GetRandomTile());
            }
        }
    }

    public void Render(ObstacleData[] obstacles) {

        foreach(ObstacleData obstacle in obstacles) {
            int x = obstacle.x;
            int y = obstacle.y;
            int width = obstacle.width;
            int height = obstacle.height;

            RenderObstacle(x, y, width, height);
        }
    }
}
