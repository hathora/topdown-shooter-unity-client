using System;
using UnityEngine;

using DataTypes.Game;

[Serializable]
public class MapData {
    public int tileSize;

    public int top;
    public int left;
    public int bottom;
    public int right;

    public ObstacleData[] walls;

    [NonSerialized]
    public static int TILE_SIZE;

    public static MapData Parse(string jsonData) {
        MapData mapData = JsonUtility.FromJson<MapData>(jsonData);

        TILE_SIZE = mapData.tileSize;

        // Account for Phaser's inverted Y-axis
        mapData.top    *= -1;
        mapData.bottom *= -1;

        // Debug.Log(string.Format("T:{0}, B:{1}, L:{2}, R:{3}", mapData.top, mapData.bottom, mapData.left, mapData.right));
        // Debug.Log("---------");

        if (mapData.walls.Length > 0) {
            ObstacleData _wall = mapData.walls[0];

            // Account for Phaser's inverted Y-axis
            for(int i = 0; i < mapData.walls.Length; i++) {
                ObstacleData wall = mapData.walls[i];

                wall.y = (wall.y * -1) - wall.height;

                mapData.walls[i] = wall;
            }
            // Debug.Log("---------");
        }

        return mapData;
    }
}
