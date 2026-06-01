using System;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private int columnStack = 2;
    [SerializeField] private int rowStack = 18;

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

}
