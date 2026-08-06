using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TileStack
{
    public Vector3 pos;
    public Quaternion rot;
    public string axis;
    public int direction;
}

[CreateAssetMenu(fileName = "TileSettings", menuName = "Scriptable Objects/TileSettings")]
public class TileSettings : ScriptableObject
{
    public static readonly Dictionary<string, float> general = new Dictionary<string, float>
    {
        ["Total"] = 144,

        ["Char"] = 36,
        ["Circle"] = 36,
        ["Stick"] = 36,

        ["Dragon"] = 12,
        ["Wind"] = 16,
        ["Season"] = 4,
        ["Flower"] = 4,

        ["ColumnStack"] = 18,
        ["RowStack"] = 2,

        ["AxisSpacing"] = 1.01f,
        ["YSpacing"] = 1.04f,
        ["Scale"] = 6.5f,
        ["MaxRepeat"] = 4
    };

    public static readonly TileStack[] DropSetting = new TileStack[]
    {
        new TileStack
        {
            pos = new Vector3(-183, 40, 300),
            rot = Quaternion.Euler(new Vector3(180, 180, 90)),
            axis = "x",
            direction = 1,
        },
        new TileStack
        {
            pos = new Vector3(57, 40, 335),
            rot = Quaternion.Euler(new Vector3(180, 90, 90)),
            axis = "z",
            direction = 1,
        },
        new TileStack
        {
            pos = new Vector3(20, 40, 500),
            rot = Quaternion.Euler(new Vector3(180, 0, 90)),
            axis = "x",
            direction = -1,
        },
        new TileStack
        {
            pos = new Vector3(-145, 40, 490),
            rot = Quaternion.Euler(new Vector3(180, 270, 90)),
            axis = "z",
            direction = -1,
        }
    };

    public static readonly TileStack[] BoardSetting =
    {
        new TileStack
        {
            pos = new Vector3(200.5f, 40, 220),
            rot = Quaternion.Euler(new Vector3(0, 0, 90)),
            axis = "x",
            direction = -1
        },

        new TileStack
        {
            pos = new Vector3(-277.5f, 40, 217),
            rot = Quaternion.Euler(new Vector3(0, 90, 90)),
            axis = "z",
            direction = 1
        },

        new TileStack
        {
            pos = new Vector3(-245, 40, 660),
            rot = Quaternion.Euler(new Vector3(0, 0, 90)),
            axis = "x",
            direction = 1
        },

        new TileStack
        {
            pos = new Vector3(233, 40, 663),
            rot = Quaternion.Euler(new Vector3(0, 90, 90)),
            axis = "z",
            direction = -1
        },
    };

    public static readonly TileStack[] OutsideHandSettings = new TileStack[]
    {
        
    };
}
