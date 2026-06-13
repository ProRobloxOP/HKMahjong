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
    private string[] blankTilePath;
    private GameObject blankTile;

    private Vector3 SetTilePosX(Transform tileTransform, Vector3 tileBounds, int column, int row)
    {
        return new Vector3(tileTransform.position.x + (column-1)*tileBounds.x*TileSettings.general["axisSpacing"], tileTransform.position.y + (row -1)*tileBounds.y*TileSettings.general["ySpacing"], tileTransform.position.z);
    }

    private Vector3 SetTilePosZ(Transform tileTransform, Vector3 tileBounds, int column, int row)
    {
        return new Vector3(tileTransform.position.x, tileTransform.position.y + (row -1)*tileBounds.y*TileSettings.general["ySpacing"], tileTransform.position.z + (column-1)*tileBounds.z*TileSettings.general["axisSpacing"]);
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
        Dictionary<String, float> suitSums = new Dictionary<string, float>
        {
            ["Char"] = TileSettings.general["char"] - TileTracker.Normal["Char"].Sum(pair => pair.Value),
            ["Circle"] = TileSettings.general["circle"] - TileTracker.Normal["Circle"].Sum(pair => pair.Value),
            ["Stick"] = TileSettings.general["stick"] - TileTracker.Normal["Stick"].Sum(pair => pair.Value),

            ["Dragon"] = TileSettings.general["dragon"] - TileTracker.Special["Dragon"].Sum(pair => pair.Value),
            ["Wind"] = TileSettings.general["wind"] - TileTracker.Special["Wind"].Sum(pair => pair.Value),
            ["Flower"] = TileSettings.general["flower"] - TileTracker.Special["Flower"].Sum(pair => pair.Value),
            ["Season"] = TileSettings.general["season"] - TileTracker.Special["Season"].Sum(pair => pair.Value)
        };
        int leftover = (int) TileSettings.general["total"] - TileTracker.total;
        float n = UnityEngine.Random.Range(1, leftover);

        foreach (var pair in suitSums)
        {
            n -= pair.Value;
            if (n < 0) { return pair.Key; }
        }

        return null;
    }

    /*private Tile assignNormalTile(String suit)
    {
        Dictionary<int, int> usedTiles = TileTracker.Normal[suit];
        Dictionary<String, String> tilePrefixes = new Dictionary<string, string>
        {
          ["Char"] = "M",
          ["Circle"] = "C",
          ["Stick"] = "S"  
        };
        
        int total = 
    }*/

    private void CreateTileStack(int stackNum)
    {
        for (int row = 1; row <= TileSettings.general["rowStack"]; row++)
        {
            for (int column = 1; column <= TileSettings.general["columnStack"]; column++)
            {
                TileStack tileStack = TileSettings.boardSetting[stackNum];
                GameObject tile = Instantiate(blankTile, tileStack.pos, TileSettings.boardSetting[stackNum].rot);
                Transform transform = tile.transform;
                Vector3 localScale = transform.localScale;

                int tileNumber = (row*column*(stackNum+1));
                
                transform.localScale = new Vector3(
                    localScale.x*TileSettings.general["scale"],
                    localScale.y*TileSettings.general["scale"],
                    localScale.z*TileSettings.general["scale"]
                );
                transform.position = SetTilePos(tile, column, row, tileStack.axis);
                tile.name = tileNumber.ToString();
                transform.SetParent(GameObject.Find("Tiles").transform, true);
            }
        }
    }

    public void CreateTiles()
    {
        blankTile = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tiles/Blank.prefab");

        for (int i = 0; i < TileSettings.boardSetting.Length; i++)
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
