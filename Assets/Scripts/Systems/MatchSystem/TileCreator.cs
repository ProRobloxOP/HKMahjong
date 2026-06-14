using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
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
        ["Char"] = new Dictionary<int, int>{},
        ["Circle"] = new Dictionary<int, int>{},
        ["Stick"] = new Dictionary<int, int>{}
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
    public static Dictionary<int, Tile> tiles = new Dictionary<int, Tile>{};
    private GameObject blankTile;

    private Vector3 SetTilePosX(Transform tileTransform, Vector3 tileBounds, int column, int row)
    {
        return new Vector3(tileTransform.position.x + (column-1)*tileBounds.x*TileSettings.general["AxisSpacing"], tileTransform.position.y + (row -1)*tileBounds.y*TileSettings.general["YSpacing"], tileTransform.position.z);
    }

    private Vector3 SetTilePosZ(Transform tileTransform, Vector3 tileBounds, int column, int row)
    {
        return new Vector3(tileTransform.position.x, tileTransform.position.y + (row -1)*tileBounds.y*TileSettings.general["YSpacing"], tileTransform.position.z + (column-1)*tileBounds.z*TileSettings.general["AxisSpacing"]);
    }

    private Vector3 SetTilePos(GameObject tile, int column, int row, String axis)
    {
        if (axis == "x")
        {
            return SetTilePosX(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row);
        }
        return SetTilePosZ(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row);
    }

    private String AssignRandomSuit()
    {
        Dictionary<String, float> suitSums = new Dictionary<string, float>
        {
            ["Char"] = TileSettings.general["Char"] - TileTracker.Normal["Char"].Sum(pair => pair.Value),
            ["Circle"] = TileSettings.general["Circle"] - TileTracker.Normal["Circle"].Sum(pair => pair.Value),
            ["Stick"] = TileSettings.general["Stick"] - TileTracker.Normal["Stick"].Sum(pair => pair.Value),

            ["Dragon"] = TileSettings.general["Dragon"] - TileTracker.Special["Dragon"].Sum(pair => pair.Value),
            ["Wind"] = TileSettings.general["Wind"] - TileTracker.Special["Wind"].Sum(pair => pair.Value),
            ["Flower"] = TileSettings.general["Flower"] - TileTracker.Special["Flower"].Sum(pair => pair.Value),
            ["Season"] = TileSettings.general["Season"] - TileTracker.Special["Season"].Sum(pair => pair.Value)
        };
        int leftover = (int) TileSettings.general["Total"] - TileTracker.total;
        float n = UnityEngine.Random.Range(1, leftover);

        foreach (var pair in suitSums)
        {
            n -= pair.Value;
            if (n <= 0) { return pair.Key; }
        }

        return null;
    }

    private Tile AssignNormalTile(String suit)
    {
        Dictionary<int, int> usedTiles = TileTracker.Normal[suit];
        Dictionary<String, String> tilePrefixes = new Dictionary<string, string>
        {
          ["Char"] = "M",
          ["Circle"] = "C",
          ["Stick"] = "S"  
        };
        
        int total = (int) TileSettings.general[suit] - usedTiles.Sum(pair => pair.Value);
        int n = UnityEngine.Random.Range(1, total);
        int num = 1;

        for (int i = 1; i <= 9; i++)
        {
            int leftover = 4 - ((usedTiles.ContainsKey(i))? usedTiles[i] : 0);
            n -= leftover;
            num = i;

            if (n <= 0) { break; }
        }

        usedTiles[num] = (usedTiles.ContainsKey(num))? usedTiles[num] + 1 : 1;
        TileTracker.total++;

        return new Tile
        {
          number = num,
          suit = suit  
        };
    }

    private Tile AssignSpecialTile(String suit)
    {
        Dictionary<String, int> usedTiles = TileTracker.Special[suit];
        String[] tileTypes = (suit.Equals("Dragon"))? new string[] {"White", "Red", "Green"} : new string[] {"1", "2", "3", "4"};
        int total = (int) TileSettings.general[suit] - usedTiles.Sum(pair => pair.Value);
        int n = UnityEngine.Random.Range(1, total);
        String name = "";

        foreach (string tileName in tileTypes)
        {
            int leftover = 4 - ((usedTiles.ContainsKey(tileName))? usedTiles[tileName] : 0);
            n -= leftover;
            name = tileName;

            if (n <= 0) { break; }
        }

        usedTiles[name] = (usedTiles.ContainsKey(name))? usedTiles[name] + 1 : 1;
        TileTracker.total++;

        return new Tile
        {
            name = name,
            suit = suit
        };
    }

    private Tile AssignNewTile(int id)
    {
        String suit = AssignRandomSuit();
        Tile tile = (!TileTracker.Normal.ContainsKey(suit))? AssignSpecialTile(suit) : AssignNormalTile(suit);
        tile.id = id;

        return tile;
    }

    private void CreateTileStack(int stackNum)
    {
        for (int row = 1; row <= TileSettings.general["RowStack"]; row++)
        {
            for (int column = 1; column <= TileSettings.general["ColumnStack"]; column++)
            {
                TileStack tileStack = TileSettings.boardSetting[stackNum];
                GameObject tile = Instantiate(blankTile, tileStack.pos, TileSettings.boardSetting[stackNum].rot);
                Transform transform = tile.transform;
                Vector3 localScale = transform.localScale;

                int tileNumber = (row*column*(stackNum+1));
                tiles[tileNumber] = AssignNewTile(tileNumber);
                
                transform.localScale = new Vector3(
                    localScale.x*TileSettings.general["Scale"],
                    localScale.y*TileSettings.general["Scale"],
                    localScale.z*TileSettings.general["Scale"]
                );
                transform.position = SetTilePos(tile, column, row, tileStack.axis);
                tile.name = tileNumber.ToString();
                transform.SetParent(GameObject.Find("Tiles").transform, true);
            }
        }
    }

    public void CreateTiles()
    {
        for (int i = 0; i < TileSettings.boardSetting.Length; i++)
        {
            CreateTileStack(i);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blankTile = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tiles/Blank.prefab");
        CreateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
