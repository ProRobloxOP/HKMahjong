using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "HandActions", menuName = "Scriptable Objects/HandActions")]
public class HandActions : ScriptableObject
{
     public static List<List<Tile>> ContainsPong(Dictionary<string, List<Tile>> hand)
     {
          List<List<Tile>> pongs = new List<List<Tile>>{};
          string[] noCheck = new string[] {"Flower"};

          foreach (string suit in hand.Keys)
          {
               List<Tile> suitTiles = hand[suit];
               List<Tile> currentPong = new List<Tile>{};
               if (noCheck.Contains(suit)) { continue; }

               foreach (Tile tile in suitTiles)
               {
                    tile.inPong = false;
               }

               for (int i = 0; i < suitTiles.Count; i++)
               {
                    Tile target = suitTiles[i];
                    if (target.inPong || target.inCheung) { continue; }

                    currentPong.Add(target);
                    for (int j = i + 1; j < suitTiles.Count; j++)
                    {
                         Tile other = suitTiles[j];
                         if (other.number.IsUnityNull() && !target.number.IsUnityNull()) { break; }
                         if (!other.number.IsUnityNull() && target.number.IsUnityNull()) { break; }
                         if (other.number.IsUnityNull() && target.number.IsUnityNull() && !other.name.Equals(target.name)){ break; }
                         if (target.number != other.number) { break; }

                         currentPong.Add(other);
                    }

                    if (currentPong.Count < 3) { currentPong.Clear(); continue; }
                    foreach (Tile tile in currentPong)
                    {
                         tile.inPong = true;
                    }

                    pongs.Add(currentPong);
                    currentPong = new List<Tile>{};
               }
          }

          return pongs;
     }

     public static List<List<Tile>> ContainsCheung(Dictionary<string, List<Tile>> hand)
     {
          List<List<Tile>> cheungs = new List<List<Tile>>{};
          string[] check = new string[] {"Char", "Stick", "Circle"};
          
          foreach (String suit in check)
          {
               List<Tile> suitTiles = hand[suit];
               List<Tile> currentCheung = new List<Tile>{};

               foreach (Tile tile in suitTiles)
               {
                    tile.inCheung = false;
               }

               for (int i = 0; i < suitTiles.Count; i++)
               {
                    Tile target = suitTiles[i];
                    if (target.inCheung) { continue; }

                    currentCheung.Add(target);
                    for (int j = i + 1; j < suitTiles.Count; j++)
                    {
                         Tile other = suitTiles[j];
                         if (other.number - target.number > 1) { break; }
                         if (other.number == target.number) { continue; }

                         currentCheung.Add(other);
                         if (currentCheung.Count == 3) { break; }
                    }

                    if (currentCheung.Count < 3) { currentCheung.Clear(); continue; }
                    foreach (Tile tile in currentCheung)
                    {
                         tile.inCheung = true;
                    }

                    cheungs.Add(currentCheung);
                    currentCheung = new List<Tile>{};
               }
          }

          return cheungs;
     }
     public static List<Tile> CanKong(Dictionary<string, List<Tile>> hand, Tile tile)
    {
        List<Tile> pongList = CanPong(hand, tile);
        if (pongList.Count < 3) { pongList.Clear(); }
        return pongList;
    }

    public static List<Tile> CanCheung(Dictionary<string, List<Tile>> hand, Tile tile)
    {
        List<Tile> cheungTiles = new List<Tile>();
        List<Tile> backCheung = new List<Tile>();
        List<Tile> fowardCheung = new List<Tile>();
        if (tile.number.IsUnityNull()) { return cheungTiles; }

        foreach (Tile ownedTile in hand[tile.suit])
        {
            int numDiff = (int)(tile.number - ownedTile.number);
            if (numDiff > 0 && numDiff <= 2)
            {
                backCheung.Add(ownedTile);
            }
            if (numDiff < 0 && numDiff >= -2)
            {
                fowardCheung.Add(ownedTile);
            }
        }

        if (backCheung.Count() > 1) { cheungTiles.AddRange(backCheung.GetRange(0, backCheung.Count())); }
        if (fowardCheung.Count() > 1) { cheungTiles.AddRange(fowardCheung.GetRange(0, fowardCheung.Count())); }
        
        return cheungTiles;
    }

    public static List<Tile> CanPong(Dictionary<string, List<Tile>> hand, Tile tile)
    {
        List<Tile> pongTiles = new List<Tile>();

        foreach (Tile ownedTile in hand[tile.suit])
        {
            if (!ownedTile.suit.Equals(tile.suit)) { continue; }
            if (!tile.number.IsUnityNull() && ownedTile.number == tile.number)
            {
                pongTiles.Add(ownedTile);
                continue;
            }
            if (!ownedTile.name.IsUnityNull() && ownedTile.name.Equals(tile.name))
            {
                pongTiles.Add(ownedTile);
            }
        }
        if (pongTiles.Count() < 1) { pongTiles.Clear(); }

        return pongTiles;
    }
}
