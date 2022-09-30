using System;
using UnityEngine;
using Rect = Hathora.DataTypes.Rect;

[Serializable]
public class MapData {
    static int SIZE_FACTOR = 64;

    public int top;
    public int left;
    public int bottom;
    public int right;

    public Rect[] walls;

    public static MapData Parse(string jsonData) {
        MapData mapData = JsonUtility.FromJson<MapData>(jsonData);

        // Reduce everything by a factor of 64
        mapData.top /= SIZE_FACTOR;
        mapData.bottom /= SIZE_FACTOR;
        mapData.left /= SIZE_FACTOR;
        mapData.right /= SIZE_FACTOR;

        for(int i = 0; i < mapData.walls.Length; i++) {
            Rect wall = mapData.walls[i];
            wall.x /= SIZE_FACTOR;
            wall.y /= SIZE_FACTOR;
            wall.width /= SIZE_FACTOR;
            wall.height /= SIZE_FACTOR;
            mapData.walls[i] = wall;
        }

        return mapData;
    }
}
