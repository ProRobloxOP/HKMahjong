using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerHand : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    static public Dictionary<string, Tile[]> hand = new Dictionary<string, Tile[]>(); // suit -> Tile

    private int compareOrder(String[] order, Tile tile1, Tile tile2)
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
    
    private void addFlower(Tile tile)
    {
        Tile[] flowerTiles = hand["Flowers"];
        flowerTiles.Append(tile);

        //Add another tile
    }

    private void addNormalTile(Tile tile, String suit)
    {
        Tile[] suitTiles = hand[suit];
        suitTiles.Append(tile);

        Array.Sort(suitTiles, (tile1, tile2) => ((int) tile1.number).CompareTo(tile2.number));
    }

    private void addDragonTile(Tile tile)
    {
        Tile[] dragonTiles = hand["Dragons"];
        String[] order = {"White", "Green", "Red"};
        dragonTiles.Append(tile);

        Array.Sort(dragonTiles, (tile1, tile2) => compareOrder(order, tile1, tile2));
    }

    private void addWindTile(Tile tile)
    {
        Tile[] windTiles = hand["Winds"];
        String[] order = {"East", "South", "West", "North"};
        windTiles.Append(tile);

        Array.Sort(windTiles, (tile1, tile2) => compareOrder(order, tile1, tile2));
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
