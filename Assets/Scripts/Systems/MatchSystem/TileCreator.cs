using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public struct Tile
{
    public int id;
    public int? number;
    public string suit;
    public string name;
}

class TileTracker
{
    public static Dictionary<String, Dictionary<int, int>> Normal = new Dictionary<string, Dictionary<int, int>>
    {
        ["Wan"] = new Dictionary<int, int>{},
        ["Tong"] = new Dictionary<int, int>{},
        ["Tiao"] = new Dictionary<int, int>{}
    };
    public static Dictionary<String, Dictionary<String, int>> Special = new Dictionary<string, Dictionary<string, int>>
    {
        ["Dragon"] = new Dictionary<string, int>{},
        ["Wind"] = new Dictionary<string, int>{},
        ["Flower"] = new Dictionary<string, int>{},
        ["Season"] = new Dictionary<string, int>{}
    };
    public static int total = 0;
}

public class TileCreator : MonoBehaviour
{

    [SerializeField] private TileSettings tileSettings;
    [SerializeField] private GameObject blankTile;

    private Vector3 SetTilePosX(Transform tileTransform, Vector3 tileBounds, int column, int row)
    {
        return new Vector3(tileTransform.position.x + (column-1)*tileBounds.x*tileSettings.AxisSpacing, tileTransform.position.y + (row -1)*tileBounds.y*tileSettings.YSpacing, tileTransform.position.z);
    }

    private Vector3 SetTilePosZ(Transform tileTransform, Vector3 tileBounds, int column, int row)
    {
        return new Vector3(tileTransform.position.x, tileTransform.position.y + (row -1)*tileBounds.y*tileSettings.YSpacing, tileTransform.position.z + (column-1)*tileBounds.z*tileSettings.AxisSpacing);
    }

    private Vector3 SetTilePos(GameObject tile, int column, int row, String axis)
    {
        if (axis == "x")
        {
            return SetTilePosX(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row);
        }
        return SetTilePosZ(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row);
    }

    private String assignSuit()
    {
        Dictionary<String, int> suitSums = new Dictionary<string, int>
        {
            ["Char"] = tileSettings.WanTiles - TileTracker.Normal["Wan"].Sum(pair => pair.Value),
            ["Circle"] = tileSettings.TongTiles - TileTracker.Normal["Tong"].Sum(pair => pair.Value),
            ["Stick"] = tileSettings.TiaoTiles - TileTracker.Normal["Tiao"].Sum(pair => pair.Value),

            ["Dragon"] = tileSettings.SanYuanTiles - TileTracker.Special["Dragon"].Sum(pair => pair.Value),
            ["Wind"] = tileSettings.FengTiles - TileTracker.Special["Wind"].Sum(pair => pair.Value),
            ["Flower"] = tileSettings.FlowerTiles - TileTracker.Special["Flower"].Sum(pair => pair.Value),
            ["Season"] = tileSettings.SeasonTiles - TileTracker.Special["Season"].Sum(pair => pair.Value)
        };
        int leftover = tileSettings.TotalTiles - TileTracker.total;
        int n = UnityEngine.Random.Range(1, leftover);

        foreach (var pair in suitSums)
        {
            n -= pair.Value;
            if (n < 0) { return pair.Key; }
        }

        return null;
    }

    /*private Tile assignNormalTile(String suit)
    {
        Dictionary<String, String> tilePrefixes = new Dictionary<string, string>
        {
          ["Char"] = "M",
          ["Circle"] = "C",
          ["Stick"] = "S"  
        };


    }*/

    private void CreateTileStack(int stackNum)
    {
        for (int row = 1; row <= tileSettings.RowStack; row++)
        {
            for (int column = 1; column <= tileSettings.ColumnStack; column++)
            {
                TileStack tileStack = tileSettings.BoardSetting[stackNum];
                GameObject tile = Instantiate(blankTile, tileStack.pos, tileSettings.BoardSetting[stackNum].rot);
                Transform transform = tile.transform;
                Vector3 localScale = transform.localScale;

                int tileNumber = (row*column*(stackNum+1));
                
                transform.localScale = new Vector3(
                    localScale.x*tileSettings.Scale,
                    localScale.y*tileSettings.Scale,
                    localScale.z*tileSettings.Scale
                );
                transform.position = SetTilePos(tile, column, row, tileStack.axis);
                tile.name = tileNumber.ToString();
                transform.SetParent(GameObject.Find("Tiles").transform, true);
            }
        }
    }

    public void CreateTiles()
    {
        for (int i = 0; i < tileSettings.BoardSetting.Length; i++)
        {
            CreateTileStack(i);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
