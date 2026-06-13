using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileStack
{
    public Vector3 pos;
    public Quaternion rot;
    public String axis;
}

[CreateAssetMenu(fileName = "TileSettings", menuName = "Scriptable Objects/TileSettings")]
public class TileSettings : ScriptableObject
{
    public static Dictionary<String, float> general = new Dictionary<string, float>
    {
        ["total"] = 144,

        ["char"] = 36,
        ["circle"] = 36,
        ["stick"] = 36,

        ["dragon"] = 12,
        ["wind"] = 16,
        ["season"] = 4,
        ["flower"] = 4,

        ["columnStack"] = 18,
        ["rowStack"] = 2,

        ["axisSpacing"] = 1.10f,
        ["ySpacing"] = 1.04f,
        ["scale"] = 6.5f,
        ["maxRepeat"] = 4
    };

    public static TileStack[] boardSetting =
    {
        new TileStack
        {
            pos = new Vector3(-245, 40, 220),
            rot = Quaternion.Euler(new Vector3(0, 0, 90)),
            axis = "x"
        },

        new TileStack
        {
            pos = new Vector3(-277.5f, 40, 215),
            rot = Quaternion.Euler(new Vector3(0, 90, 90)),
            axis = "z"
        },

        new TileStack
        {
            pos = new Vector3(233, 40, 215),
            rot = Quaternion.Euler(new Vector3(0, 90, 90)),
            axis = "z"
        },

        new TileStack
        {
            pos = new Vector3(-245, 40, 660),
            rot = Quaternion.Euler(new Vector3(0, 0, 90)),
            axis = "x"
        }
    };
}
