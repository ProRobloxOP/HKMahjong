using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mono.Cecil;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;

public class PlayerHand : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    static public Dictionary<string, List<Tile>> hand = new Dictionary<string, List<Tile>>
    {
        ["Char"] = new List<Tile>{},
        ["Circle"] = new List<Tile>{},
        ["Stick"] = new List<Tile>{},

        ["Dragon"] = new List<Tile>{},
        ["Wind"] = new List<Tile>{},
        ["Flower"] = new List<Tile>{}
    }; // suit -> Tile
    private void OnEnable() => TileCreator.CreatedTilesEvent += SetupPlayerHand;
    private void OnDisable() => TileCreator.CreatedTilesEvent -= SetupPlayerHand;

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
        List<Tile> flowerTiles = hand["Flower"];
        flowerTiles.Append(tile);
        DrawTilesFromWall(1);
    }

    private void DrawNormalTile(Tile tile)
    {
        List<Tile> suitTiles = hand[tile.suit];
        suitTiles.Append(tile);

        suitTiles.Sort((tile1, tile2) => ((int) tile1.number).CompareTo(tile2.number));
    }

    private void DrawDragonTile(Tile tile)
    {
        List<Tile> dragonTiles = hand["Dragon"];
        String[] order = {"White", "Green", "Red"};
        dragonTiles.Append(tile);

        dragonTiles.Sort((tile1, tile2) => CompareOrder(order, tile1, tile2));
    }

    private void DrawWindTile(Tile tile)
    {
        List<Tile> windTiles = hand["Wind"];
        String[] order = {"East", "South", "West", "North"};
        windTiles.Append(tile);

        windTiles.Sort((tile1, tile2) => CompareOrder(order, tile1, tile2));
    }

    private void DrawTilesFromWall(int iterations)
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
            Tile tile = wall[0];
            TileCreator.DropTile(gameObject, tile.id);
            wall.RemoveAt(0);

            if (addMethods.ContainsKey(tile.suit)) { addMethods[tile.suit](tile); continue; }
            DrawNormalTile(tile);
        }
    }

    private void SetupPlayerHand()
    {
        DrawTilesFromWall(14);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
