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
    [SerializeField] private int totalTiles = 144;
    [SerializeField] private int seasonTiles = 4;
    [SerializeField] private int flowerTiles = 4;
    [SerializeField] private int wanTiles = 36;
    [SerializeField] private int tiaoTiles = 36;
    [SerializeField] private int tongTiles = 36;
    [SerializeField] private int sanYuanTiles = 12;
    [SerializeField] private int fengTiles = 16;

    [SerializeField] private int columnStack = 18;
    [SerializeField] private int rowStack = 2;

    [SerializeField] private float axisSpacing = 1.024f;
    [SerializeField] private float ySpacing = 1.04f;

    [SerializeField] private TileStack[] boardSetting =
    {
        new TileStack
        {
            pos = new Vector3(-245, 40, 220),
            rot = Quaternion.Euler(new Vector3(-90, 90, 0)),
            axis = "x"
        },

        new TileStack
        {
            pos = new Vector3(-277.5f, 40, 200),
            rot = Quaternion.Euler(new Vector3(-90, 0, 0)),
            axis = "z"
        },

        new TileStack
        {
            pos = new Vector3(275, 40, 200),
            rot = Quaternion.Euler(new Vector3(-90, 0, 0)),
            axis = "z"
        },

        new TileStack
        {
            pos = new Vector3(-245, 40, 660),
            rot = Quaternion.Euler(new Vector3(-90, -90, 0)),
            axis = "x"
        }
    };

    public int TotalTiles => totalTiles;
    public int SeasonTiles => seasonTiles;
    public int FlowerTiles => flowerTiles;
    public int WanTiles => wanTiles;
    public int TiaoTiles => tiaoTiles;
    public int TongTiles => tongTiles;
    public int SanYuanTiles => sanYuanTiles;
    public int FengTiles => fengTiles;
    public int ColumnStack => columnStack;
    public int RowStack => rowStack;

    public float AxisSpacing => axisSpacing;
    public float YSpacing => ySpacing;

    public TileStack[] BoardSetting => boardSetting;

}
