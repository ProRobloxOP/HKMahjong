using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Tile
{
    public int id;
    public int ownerIndex;
    public int? number;
    public string suit;
    public string name;

    public bool open;

    public bool fromWall;
    public bool lastTile;
    public bool robbed;

    public bool inCheung;
    public bool inPong;

    private string WindTostring()
    {
        return name;
    }

    private string NormalTostring()
    {
        return number.ToString() + suit[0];
    }

    private string CharTostring()
    {
        return number + "M";
    }

    private string FlowerTostring()
    {
        return name + "F";
    }
    
    private string SeasonTostring()
    {
        return name + "T";
    }

    private string DragonTostring()
    {
        return name[0] + "D";
    }

    override public string ToString()
    {
        Dictionary<string, Func<string>> tostringTypes = new Dictionary<string, Func<string>>
        {
            ["Char"] = CharTostring,
            ["Dragon"] = DragonTostring,
            ["Wind"] = WindTostring,
            ["Season"] = SeasonTostring,
            ["Flower"] = FlowerTostring
        };

        if (tostringTypes.ContainsKey(suit)) { return tostringTypes[suit](); }
        return NormalTostring();
    }
}

class TileTracker
{
    public static Dictionary<string, Dictionary<int, int>> Normal = new Dictionary<string, Dictionary<int, int>>
    {
        ["Char"] = new Dictionary<int, int>{},
        ["Circle"] = new Dictionary<int, int>{},
        ["Stick"] = new Dictionary<int, int>{}
    };
    public static Dictionary<string, Dictionary<string, int>> Special = new Dictionary<string, Dictionary<string, int>>
    {
        ["Dragon"] = new Dictionary<string, int>{},
        ["Wind"] = new Dictionary<string, int>{},
        ["Flower"] = new Dictionary<string, int>{},
        ["Season"] = new Dictionary<string, int>{}
    };
    public static int total = 0;
}

[CreateAssetMenu(fileName = "TileCreator", menuName = "Scriptable Objects/TileCreator")]
public class TileCreator : ScriptableObject
{
    public static event Action CreatedTilesEvent;
    private static List<Tile> DroppedTiles = new List<Tile>{};
    private static List<Tile> WallTiles = new List<Tile>{};
    private static GameObject tileObjects, blankTile;

    public static List<Tile> GetDroppedTiles()
    {
        return DroppedTiles;
    }

    public static List<Tile> GetWallTiles()
    {
        return WallTiles;
    }

    public static GameObject GetTileObjects()
    {
        return tileObjects;
    }

    private static Vector3 SetTilePosX(Transform tileTransform, Vector3 tileBounds, int column, int row, int direction, bool switchRowProp)
    {
        return (switchRowProp != true)?
         new Vector3(tileTransform.position.x + (column-1)*direction*tileBounds.x*TileSettings.General["AxisSpacing"], tileTransform.position.y + (row-1)*tileBounds.y*TileSettings.General["YSpacing"], tileTransform.position.z) : 
         new Vector3(tileTransform.position.x + (column-1)*direction*tileBounds.x*TileSettings.General["AxisSpacing"], tileTransform.position.y, tileTransform.position.z - (row-1)*tileBounds.z*TileSettings.General["AxisSpacing"]*direction);
    }

    private static Vector3 SetTilePosZ(Transform tileTransform, Vector3 tileBounds, int column, int row, int direction, bool switchRowProp)
    {
        return (switchRowProp != true)?
         new Vector3(tileTransform.position.x, tileTransform.position.y + (row-1)*tileBounds.y*TileSettings.General["YSpacing"], tileTransform.position.z + (column-1)*direction*tileBounds.z*TileSettings.General["AxisSpacing"]) : 
         new Vector3(tileTransform.position.x + (row-1)*tileBounds.x*TileSettings.General["AxisSpacing"]*direction, tileTransform.position.y, tileTransform.position.z + (column-1)*direction*tileBounds.z*TileSettings.General["AxisSpacing"]);
    }

    public static Vector3 SetTilePos(GameObject tile, int column, int row, string axis, int direction)
    {
        if (axis == "x")
        {
            return SetTilePosX(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row, direction, false);
        }
        return SetTilePosZ(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row, direction, false);
    }

    public static Vector3 SetTilePos(GameObject tile, int column, int row, string axis, int direction, bool switchRowProp)
    {
        if (axis == "x")
        {
            return SetTilePosX(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row, direction, switchRowProp);
        }
        return SetTilePosZ(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row, direction, switchRowProp);
    }

    private static string AssignRandomSuit()
    {
        Dictionary<string, float> suitSums = new Dictionary<string, float>
        {
            ["Char"] = TileSettings.General["Char"] - TileTracker.Normal["Char"].Sum(pair => pair.Value),
            ["Circle"] = TileSettings.General["Circle"] - TileTracker.Normal["Circle"].Sum(pair => pair.Value),
            ["Stick"] = TileSettings.General["Stick"] - TileTracker.Normal["Stick"].Sum(pair => pair.Value),

            ["Dragon"] = TileSettings.General["Dragon"] - TileTracker.Special["Dragon"].Sum(pair => pair.Value),
            ["Wind"] = TileSettings.General["Wind"] - TileTracker.Special["Wind"].Sum(pair => pair.Value),
            ["Flower"] = TileSettings.General["Flower"] - TileTracker.Special["Flower"].Sum(pair => pair.Value),
            ["Season"] = TileSettings.General["Season"] - TileTracker.Special["Season"].Sum(pair => pair.Value)
        };
        int leftover = (int) TileSettings.General["Total"] - TileTracker.total;
        float n = UnityEngine.Random.Range(1, leftover);

        foreach (var pair in suitSums)
        {
            n -= pair.Value;
            if (n <= 0) { return pair.Key; }
        }

        return null;
    }

    private static Tile AssignNormalTile(string suit)
    {
        Dictionary<int, int> usedTiles = TileTracker.Normal[suit];
        Dictionary<string, string> tilePrefixes = new Dictionary<string, string>
        {
          ["Char"] = "M",
          ["Circle"] = "C",
          ["Stick"] = "S"  
        };
        
        int total = (int) TileSettings.General[suit] - usedTiles.Sum(pair => pair.Value);
        int n = UnityEngine.Random.Range(1, total);
        int num = 1;

        for (int i = 1; i <= 9; i++)
        {
            int leftover = 4 - (usedTiles.ContainsKey(i)? usedTiles[i] : 0);
            n -= leftover;
            num = i;

            if (n <= 0) { break; }
        }

        usedTiles[num] = usedTiles.ContainsKey(num)? usedTiles[num] + 1 : 1;
        TileTracker.total++;

        return new Tile
        {
          number = num,
          suit = suit  
        };
    }

    private static Tile AssignSpecialTile(string suit)
    {
        Dictionary<string, int> usedTiles = TileTracker.Special[suit];
        string[] tileTypes = suit.Equals("Dragon")? new string[] {"White", "Red", "Green"}: 
            suit.Equals("Wind")? new string[] {"East", "South", "North", "West"} : new string[] {"1", "2", "3", "4"};
        int total = (int) TileSettings.General[suit] - usedTiles.Sum(pair => pair.Value);
        int n = UnityEngine.Random.Range(1, total);
        string name = "";

        foreach (string tileName in tileTypes)
        {
            int leftover = 4 - (usedTiles.ContainsKey(tileName)? usedTiles[tileName] : 0);
            n -= leftover;
            name = tileName;

            if (n <= 0) { break; }
        }

        usedTiles[name] = usedTiles.ContainsKey(name)? usedTiles[name] + 1 : 1;
        TileTracker.total++;

        return new Tile
        {
            name = name,
            suit = suit
        };
    }

    private static Tile AssignNewTile(int id)
    {
        string suit = AssignRandomSuit();
        Tile tile = (!TileTracker.Normal.ContainsKey(suit))? AssignSpecialTile(suit) : AssignNormalTile(suit);
        tile.lastTile = (id == TileSettings.General["Total"])? true : false;
        tile.fromWall = true;
        tile.inCheung = false;
        tile.inPong = false;
        tile.robbed = false;
        tile.open = false;
        tile.id = id;

        return tile;
    }

    public static GameObject CreateTile(GameObject prefab, Vector3 pos, Quaternion rot, int tileId)
    {
        GameObject tile = Instantiate(prefab, pos, rot);
        Transform transform = tile.transform;
        Vector3 localScale = transform.localScale;
        
        transform.localScale = new Vector3(
            localScale.x*TileSettings.General["Scale"],
            localScale.y*TileSettings.General["Scale"],
            localScale.z*TileSettings.General["Scale"]
        );

        tile.name = tileId.ToString();
        transform.SetParent(GameObject.Find("Tiles").transform, true);
        return tile;
    }

    private static void CreateTileStack(int stackNum)
    {
        for (int column = 1; column <= TileSettings.General["ColumnStack"]; column++)
        {
            for (int row = 1; row <= TileSettings.General["RowStack"]; row++)
            {
                TileStack tileStack = TileSettings.BoardSetting[stackNum];
                int tileNumber = WallTiles.Count() + 1;
                WallTiles.Add(AssignNewTile(tileNumber));

                GameObject tile = CreateTile(blankTile, tileStack.pos, TileSettings.BoardSetting[stackNum].rot, tileNumber);
                tile.transform.position = SetTilePos(tile, column, row, tileStack.axis, tileStack.direction);
            }
        }
    }

    public static void CreateTiles()
    {
        for (int i = 0; i < TileSettings.BoardSetting.Length; i++)
        {
            CreateTileStack(i);
        }
    }

    public static void RemoveTile(GameObject tiles, int id)
    {
        Destroy(tiles.transform.Find(id.ToString()).gameObject);
        if (tiles.transform.Find((id + 1).ToString()).IsUnityNull() || id % 2 == 0){ return; }

        GameObject nextTile = tiles.transform.Find((id + 1).ToString()).gameObject;
        Vector3 tileBounds = nextTile.GetComponent<Renderer>().bounds.size;
        Transform transform = nextTile.transform;
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y - tileBounds.y*TileSettings.General["YSpacing"], pos.z);
    }

    public static void Init()
    {
        GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
        blankTile = Resources.Load<GameObject>("Prefabs/Tiles/Blank");

        foreach (GameObject gameObject in rootObjs)
        {
            if (gameObject.name.Equals("Tiles"))
            {
                tileObjects = gameObject;
                break;
            }
        }

        CreateTiles();
        RoundLogic.Init();
        CreatedTilesEvent?.Invoke();
    }
}
