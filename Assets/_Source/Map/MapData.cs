using System;
using UnityEngine;

using DataTypes.Game;

[Serializable]
public class TileDimensions {
    public int width;
    public int height;
}

[Serializable]
public class MapData {
    public int top;
    public int left;
    public int bottom;
    public int right;

    public TileDimensions tileDimensions;

    public ObstacleData[] walls;

    [NonSerialized]
    public static float HEIGHT_MULTIPLIER = 1.037837f;

    [NonSerialized]
    public static float WIDTH_MULTIPLIER  = 1.024f;

    [NonSerialized]
    public static float TILE_SIZE = 64.0f;

    static int convertX(int val) {
        return (int)Mathf.Round(val * WIDTH_MULTIPLIER / TILE_SIZE);
    }

    static float convertXf(int val) {
        return val * WIDTH_MULTIPLIER / TILE_SIZE;
    }

    static int convertY(int val) {
        return (int)Mathf.Round(val * HEIGHT_MULTIPLIER / TILE_SIZE);
    }

    static float convertYf(int val) {
        return val * HEIGHT_MULTIPLIER / TILE_SIZE;
    }

    public static MapData Parse(string jsonData) {
        MapData mapData = JsonUtility.FromJson<MapData>(jsonData);

        float tileWidth  = mapData.tileDimensions.width;
        float tileHeight = mapData.tileDimensions.height;

        float intendedHeight = Mathf.Abs(mapData.bottom - mapData.top);
        float intendedWidth  = Mathf.Abs(mapData.right - mapData.left);

        // Debug.Log(string.Format("WIDTH: TILE:{0}, INTENDED:{1}", (tileWidth  * TILE_SIZE), intendedWidth));
        // Debug.Log(string.Format("HEIGHT: TILE:{0}, INTENDED:{1}", (tileHeight * TILE_SIZE), intendedHeight));

        WIDTH_MULTIPLIER  = (tileWidth  * TILE_SIZE) / intendedWidth;
        HEIGHT_MULTIPLIER = (tileHeight * TILE_SIZE) / intendedHeight;

        // Debug.Log(string.Format("MULT: W:{0} H:{1}", WIDTH_MULTIPLIER, HEIGHT_MULTIPLIER));

        // Debug.Log(string.Format("[RAW] T:{0}, B:{1}, L:{2}, R:{3}", mapData.top, mapData.bottom, mapData.left, mapData.right));

        mapData.top    = convertY(mapData.top);
        mapData.bottom = convertY(mapData.bottom);
        mapData.left   = convertX(mapData.left);
        mapData.right  = convertX(mapData.right);

        mapData.top    *= -1;
        mapData.bottom *= -1;

        Debug.Log(string.Format("T:{0}, B:{1}, L:{2}, R:{3}", mapData.top, mapData.bottom, mapData.left, mapData.right));
        Debug.Log("---------");

        if (mapData.walls.Length > 0) {
            // Figure out the tile size of the wall
            //
            ObstacleData _wall = mapData.walls[0];
            int WALL_SIZE = (_wall.height < _wall.width) ? _wall.height : _wall.width;

            for(int i = 0; i < mapData.walls.Length; i++) {
                ObstacleData wall = mapData.walls[i];

                wall.x = (wall.x / WALL_SIZE);
                wall.width  /= WALL_SIZE;
                wall.height /= WALL_SIZE;

                // Account for the fact that Phaser/Server Y-axis
                // is inverted compared to Unity's
                //
                wall.y = (wall.y / WALL_SIZE * -1) - wall.height;

                mapData.walls[i] = wall;
            }
            // Debug.Log("---------");
        }

        return mapData;
    }
}
