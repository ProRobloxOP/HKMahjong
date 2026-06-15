using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mono.Cecil;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

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
    public Dictionary<string, List<Tile>> tiles = new Dictionary<string, List<Tile>>
    {
        ["Char"] = new List<Tile>{},
        ["Circle"] = new List<Tile>{},
        ["Stick"] = new List<Tile>{},

        ["Dragon"] = new List<Tile>{},
        ["Wind"] = new List<Tile>{},
        ["Flower"] = new List<Tile>{}
    }; // suit -> Tile
    public List<Tile>[] droppedTiles = new List<Tile>[]
    {
        new List<Tile>{},
        new List<Tile>{},
        new List<Tile>{},
        new List<Tile>{}
    };
    public static event Action<int, Tile> PlayerDroppedTile;
    private GameObject Tiles;
    //private bool allConcealed;
    public int playerIndex;

    private int CompareOrder(String[] order, Tile tile1, Tile tile2)
    {
        String name1 = tile1.name;
        String name2 = tile2.name;
        if (name1.Equals(name2)) { return 0; }

        foreach (String rank in order)
        {
            if (name1.Equals(rank)){ return 1; }
            if (name2.Equals(rank)) { return -1; }
        }

        return -1;
    }
    
    private void DrawFlower(Tile tile)
    {
        List<Tile> flowerTiles = tiles["Flower"];
        flowerTiles.Add(tile);
        DrawTilesFromWall(1);
        tile.open = true;
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
        String[] order = {"White", "Green", "Red"};
        dragonTiles.Add(tile);

        dragonTiles.Sort((tile1, tile2) => CompareOrder(order, tile1, tile2));
    }

    private void DrawWindTile(Tile tile)
    {
        List<Tile> windTiles = tiles["Wind"];
        String[] order = {"East", "South", "West", "North"};
        windTiles.Add(tile);

        windTiles.Sort((tile1, tile2) => CompareOrder(order, tile1, tile2));
    }

    public void DrawTilesFromWall(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            Dictionary<String, Action<Tile>> addMethods = new Dictionary<String, Action<Tile>>
            {
                ["Dragon"] = DrawDragonTile,
                ["Wind"] = DrawWindTile,
                ["Flower"] = DrawFlower,
                ["Season"] = DrawFlower
            };
            List<Tile> wall = TileCreator.wall;
            Tile tile;

            if (wall.Count() == 0) { return; }
            tile = wall[0];
            TileCreator.RemoveTile(Tiles, tile.id);
            wall.RemoveAt(0);

            if (addMethods.ContainsKey(tile.suit)) { addMethods[tile.suit](tile); continue; }
            DrawNormalTile(tile);
        }
    }

    private void VisualizeDrop(int playerIndex, Tile tile)
    {
        DropRow dropRow = TileSettings.dropSetting[playerIndex - 1];
        List<Tile> playerDropped = droppedTiles[playerIndex - 1];
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tiles/" + tile.ToString() + ".prefab");

        GameObject tileObj = TileCreator.CreateTile(prefab, dropRow.pos, dropRow.rot, tile.id);
        tileObj.transform.position = TileCreator.SetTilePos(tileObj, playerDropped.Count + 1, 1, dropRow.axis, dropRow.direction);
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

    public Dictionary<String, List<Tile>> SetupPlayerHand(GameObject Tiles, int playerIndex, bool dealer)
    {
        this.playerIndex = playerIndex;
        this.Tiles = Tiles;
        //allConcealed = true;
        DrawTilesFromWall((dealer)? 14 : 13);

        return tiles;
    }
}
