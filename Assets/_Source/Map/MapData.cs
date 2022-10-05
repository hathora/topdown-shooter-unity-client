using System;
using UnityEngine;

using DataTypes.Game;

[Serializable]
public class MapData {
    static int SIZE_FACTOR = 64;

    public int top;
    public int left;
    public int bottom;
    public int right;

    public ObstacleData[] walls;

    [NonSerialized]
    static float HEIGHT_MULTIPLIER = 1.037837f;

    [NonSerialized]
    static float WIDTH_MULTIPLIER  = 1.024f;

    [NonSerialized]
    static float TILE_SIZE = 64.0f;

    static int convertX(int val) {
        return (int)Mathf.Round(val * WIDTH_MULTIPLIER / TILE_SIZE);
    }

    static int convertY(int val) {
        return (int)Mathf.Round(val * HEIGHT_MULTIPLIER / TILE_SIZE);
    }

    public static MapData Parse(string jsonData) {
        MapData mapData = JsonUtility.FromJson<MapData>(jsonData);

        Debug.Log(string.Format("[RAW] T:{0}, B:{1}, L:{2}, R:{3}", mapData.top, mapData.bottom, mapData.left, mapData.right));

        mapData.top    = convertY(mapData.top);
        mapData.bottom = convertY(mapData.bottom);
        mapData.left   = convertX(mapData.left);
        mapData.right  = convertX(mapData.right);

        mapData.top    *= -1;
        mapData.bottom *= -1;

        Debug.Log(string.Format("T:{0}, B:{1}, L:{2}, R:{3}", mapData.top, mapData.bottom, mapData.left, mapData.right));
        Debug.Log("---------");

        for(int i = 0; i < mapData.walls.Length; i++) {
            ObstacleData wall = mapData.walls[i];

            wall.x = convertX(wall.x);
            wall.y = convertY(wall.y);
            wall.width = convertX(wall.width);
            wall.height = convertY(wall.height);

            mapData.walls[i] = wall;
        }

        return mapData;
    }
}
