using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct HandRank
{
    public int points;
    public float chance;
}

[CreateAssetMenu(fileName = "PlayerHand", menuName = "Scriptable Objects/PlayerHand")]
public class PlayerHand : ScriptableObject
{
    /*private Dictionary<string, Func<PlayerHand, HandRank>> handPoints = new Dictionary<string, Func<PlayerHand, HandRank>>
    {
        ["All Flowers / All Seasons"] = HandCombos.AllFlowers,
        ["Eight Flowers"] = HandCombos.EightFlowers,
        ["Seven Flowers"] = () => {},
        ["No Flower"] = () => {},
        ["Main Flower"] = () => {},

        ["Zi Mo"] = () => {},
        ["Concealed Hand"] = () => {},

        ["Robbing The Kong"] = () => {},
        ["Win By Kong Replacement"] = () => {},
        ["Double Kong Replacement"] = () => {},

        ["Moon Under The Sea"] = () => {},

        ["All Concealed Triplets"] = () => {},
        ["All Quadruplets"] = () => {},
        ["All Triplets"] = () => {},
        ["All Sequences"] = () => {},

        ["Big Three Dragons"] = () => {},
        ["Small Three Dragons"] = () => {},

        ["Red Dragon"] = () => {},
        ["White Dragon"] = () => {},
        ["Green Dragon"] = () => {},

        ["Small Four Winds"] = () => {},
        ["Big Four Winds"] = () => {},
        ["Round Wind"] = () => {},
        ["Seat Wind"] = () => {},

        ["Mixed Flush"] = () => {},
        ["Full Flush"] = () => {},

        ["All Honors"] = () => {},
        ["All Terminals"] = () => {},
        ["Mixed Terminals"] = () => {},

        ["Blessing of Heaven"] = () => {},
        ["Blessing of Earth"] = () => {},
        ["Blessing of Man"] = () => {},

        ["Nine Gates"] = () => {},
        ["Thirteen Orphans"] = () => {},
        ["Seven Pairs"] = () => {}
    };*/
    private Dictionary<string, List<Tile>> tiles = new Dictionary<string, List<Tile>>
    {
        ["Char"] = new List<Tile>{},
        ["Circle"] = new List<Tile>{},
        ["Stick"] = new List<Tile>{},

        ["Dragon"] = new List<Tile>{},
        ["Wind"] = new List<Tile>{},
        ["Flower"] = new List<Tile>{}
    }; // suit -> Tile
    private List<List<Tile>> openMelds = new List<List<Tile>>{};
    private List<Tile>[] droppedTiles = new List<Tile>[]
    {
        new List<Tile>{},
        new List<Tile>{},
        new List<Tile>{},
        new List<Tile>{}
    };

    private Dictionary<string, List<UnityAction<object>>> hookListeners = new Dictionary<string, List<UnityAction<object>>>
    {
        ["OnDraw"] = new List<UnityAction<object>>()
    };
    public static event Action<int, Tile> PlayerDroppedTile;
    private GameObject Tiles;
    //private bool allConcealed;
    private int playerIndex;

    void OnEnable()
    {
        PlayerDroppedTile += OnTileDrop;
    }

    void OnDisable()
    {
        PlayerDroppedTile -= OnTileDrop;
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    public Dictionary<string, List<Tile>> GetCurrentTiles()
    {
        return tiles;
    }

    public List<List<Tile>> GetOpenMelds()
    {
        return openMelds;
    }

    public List<Tile>[] GetDroppedTiles()
    {
        return droppedTiles;
    }

    public List<Tile> GetHandList()
    {
        List<Tile> handList = new List<Tile>();

        foreach (List<Tile> tileList in tiles.Values)
        {
            foreach (Tile tile in tileList)
            {
                handList.Add(tile);
            }
        }

        return handList;
    }

    public List<Tile> GetHandList(bool excludeOpen)
    {
        List<Tile> handList = new List<Tile>();

        foreach (List<Tile> tileList in tiles.Values)
        {
            foreach (Tile tile in tileList)
            {
                if (excludeOpen && tile.open) { continue; }
                handList.Add(tile);
            }
        }

        return handList;
    }

    private int CompareOrder(string[] order, Tile tile1, Tile tile2)
    {
        string name1 = tile1.name;
        string name2 = tile2.name;
        if (name1.Equals(name2)) { return 0; }

        foreach (string rank in order)
        {
            if (name1.Equals(rank)){ return 1; }
            if (name2.Equals(rank)) { return -1; }
        }

        return -1;
    }
    
    private void DrawFlower(Tile tile)
    {
        List<Tile> flowerTiles = tiles["Flower"];
        tile.open = true;
        flowerTiles.Add(tile);
        DrawTilesFromWall(1);
    }

    private void DrawNormalTile(Tile tile)
    {
        List<Tile> suitTiles = tiles[tile.suit];
        suitTiles.Add(tile);

        suitTiles.Sort((tile1, tile2) => ((int) tile1.number).CompareTo(tile2.number));
    }

    private void DrawDragonTile(Tile tile)
    {
        List<Tile> dragonTiles = tiles["Dragon"];
        string[] order = {"White", "Green", "Red"};
        dragonTiles.Add(tile);

        dragonTiles.Sort((tile1, tile2) => CompareOrder(order, tile1, tile2));
    }

    private void DrawWindTile(Tile tile)
    {
        List<Tile> windTiles = tiles["Wind"];
        string[] order = {"East", "South", "West", "North"};
        windTiles.Add(tile);

        windTiles.Sort((tile1, tile2) => CompareOrder(order, tile1, tile2));
    }

    public void DrawTilesFromWall(int iterations)
    {
        List<Tile> wall = TileCreator.wall;

        for (int i = 0; i < iterations; i++)
        {
            if (GetHandList(true).Count == 14) { break; }
            if (wall.Count == 0) { break; }
            Dictionary<string, UnityAction<Tile>> drawMethods = new Dictionary<string, UnityAction<Tile>>
            {
                ["Dragon"] = DrawDragonTile,
                ["Wind"] = DrawWindTile,
                ["Flower"] = DrawFlower,
                ["Season"] = DrawFlower
            };

            Tile tile = wall[0];
            TileCreator.RemoveTile(Tiles, tile.id);
            CallOnDrawListeners(tile);
            wall.RemoveAt(0);

            if (drawMethods.ContainsKey(tile.suit)) { 
                drawMethods[tile.suit](tile);
                continue;
            }
            DrawNormalTile(tile);
        }
    }

    private void OnTileDrop(int playerIndex, Tile droppedTile)
    {
        if (playerIndex == this.playerIndex) { return; }
        droppedTiles[playerIndex - 1].Add(droppedTile);
    }

    private void VisualizeDrop(int playerIndex, Tile tile)
    {
        DropRow dropRow = TileSettings.dropSetting[playerIndex - 1]; // eg. Player 1 -> Index 0 (First Index)
        List<Tile> playerDropped = droppedTiles[playerIndex - 1];
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Tiles/" + tile.ToString());

        GameObject tileObj = TileCreator.CreateTile(prefab, dropRow.pos, dropRow.rot, tile.id);
        tileObj.transform.position = TileCreator.SetTilePos(tileObj, playerDropped.Count % 6, playerDropped.Count / 6, dropRow.axis, dropRow.direction, true);
        tileObj.transform.SetParent(GameObject.Find("DroppedTiles").transform);
        playerDropped.Add(tile);
    }

    public void DropTile(int playerIndex, Tile tile)
    {
        List<Tile> suitList = tiles[tile.suit];
        suitList.Remove(tile);
        tile.open = true;

        PlayerDroppedTile?.Invoke(playerIndex, tile);
        VisualizeDrop(playerIndex, tile);
    }

    public Dictionary<string, List<Tile>> SetupPlayerHand(GameObject Tiles, int playerIndex, bool dealer)
    {
        this.playerIndex = playerIndex;
        this.Tiles = Tiles;
        //allConcealed = true;
        DrawTilesFromWall((dealer)? 14 : 13);

        return tiles;
    }

    private void CallOnDrawListeners(Tile tile)
    {
        List<UnityAction<object>> listeners = hookListeners["OnDraw"];
        if (tile.open) { return; }
        
        foreach (UnityAction<object> listener in listeners)
        {
            listener(tile);
        }
    }

    public void OnDraw(UnityAction<object> call)
    {
        hookListeners["OnDraw"].Add(call);
    }
}
