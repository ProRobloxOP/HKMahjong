using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "HandActions", menuName = "Scriptable Objects/HandActions")]
public class HandActions : ScriptableObject
{
     public static bool CanWin(Dictionary<string, List<Tile>> hand)
     {
          return CanWin(hand, null);
     }

     public static bool CanWin(Dictionary<string, List<Tile>> hand, Tile droppedTile)
     {
          if (!droppedTile.IsUnityNull()) { hand[droppedTile.suit].Add(droppedTile); }
          Debug.Log(ContainsCheung(hand).Count);
          Debug.Log(ContainsPong(hand).Count);
          int totalMelds = ContainsPong(hand).Count + ContainsCheung(hand).Count;
          string[] noCheck = new string[] { "Flower", "Season" };
          List<Tile> pair = new List<Tile> { };
          if (totalMelds < 4) { return false; }

          foreach (string suit in hand.Keys)
          {
               List<Tile> suitTiles = hand[suit];
               if (noCheck.Contains(suit)) { continue; }

               foreach (Tile tile in suitTiles)
               {
                    if (tile.inCheung || tile.inPong) { continue; }
                    pair.Add(tile);

                    if (pair.Count > 1) { break; }
               }
               if (pair.Count > 1) { break; }
          }
          if (pair.Count != 2) { return false; }

          Tile lone1 = pair[0];
          Tile lone2 = pair[1];

          if (lone1.number.IsUnityNull() && !lone2.number.IsUnityNull()) { return false; }
          if (!lone1.number.IsUnityNull() && lone2.number.IsUnityNull()) { return false; }
          if (lone1.number.IsUnityNull() && lone2.number.IsUnityNull() && !lone1.name.Equals(lone2.name)) { return false; }
          if (lone1.number != lone2.number) { return false; }

          return true;
     }

     public static List<List<Tile>> ContainsKong(Dictionary<string, List<Tile>> hand)
     {
          return ContainsKong(hand, false);
     }

     public static List<List<Tile>> ContainsKong(Dictionary<string, List<Tile>> hand, bool noFullOutsideMelds)
     {
          List<List<Tile>> pongs = ContainsPong(hand);
          for (int i = 0; i < pongs.Count; i++)
          {
               List<Tile> tiles = pongs[i];
               bool foundInside = false;

               if (tiles.Count != 4)
               {
                    pongs.Remove(tiles);
                    i--;
                    continue;
               }
               if (!noFullOutsideMelds){ continue; }

               foreach (Tile tile in tiles)
               {
                    if (tile.open) { continue; }
                    foundInside = true;
                    break;
               }
               
               if (!foundInside) { continue; }
               pongs.Remove(tiles);
               i--; 
          }
          
          return pongs;
     }

     public static List<List<Tile>> ContainsPong(Dictionary<string, List<Tile>> hand)
     {
          List<List<Tile>> pongs = new List<List<Tile>> { };
          string[] noCheck = new string[] { "Flower", "Season" };

          foreach (string suit in hand.Keys)
          {
               List<Tile> suitTiles = hand[suit];
               List<Tile> currentPong = new List<Tile> { };
               if (noCheck.Contains(suit)) { continue; }

               foreach (Tile tile in suitTiles)
               {
                    if (tile.open) { continue; }
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
                         if (other.number.IsUnityNull() && target.number.IsUnityNull() && !other.name.Equals(target.name)) { break; }
                         if (target.number != other.number) { break; }

                         currentPong.Add(other);
                    }

                    if (currentPong.Count < 3) { currentPong.Clear(); continue; }
                    foreach (Tile tile in currentPong)
                    {
                         tile.inPong = true;
                    }

                    pongs.Add(currentPong);
                    currentPong = new List<Tile> { };
               }
          }

          return pongs;
     }

     public static List<List<Tile>> ContainsCheung(Dictionary<string, List<Tile>> hand)
     {
          List<List<Tile>> cheungs = new List<List<Tile>> { };
          string[] check = new string[] { "Char", "Stick", "Circle" };

          foreach (string suit in check)
          {
               List<Tile> suitTiles = hand[suit];
               List<Tile> currentCheung = new List<Tile> { };

               foreach (Tile tile in suitTiles)
               {
                    if (tile.open) { continue; }
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
                    currentCheung = new List<Tile> { };
               }
          }

          return cheungs;
     }
     public static List<Tile> CanKong(Dictionary<string, List<Tile>> closedHand, Tile tile)
     {
          List<Tile> pongList = CanPong(closedHand, tile);
          if (pongList.Contains(tile)) { pongList.Remove(tile); }
          if (pongList.Count < 3) { pongList.Clear(); }
          return pongList;
     }

     public static List<Tile> CanPong(Dictionary<string, List<Tile>> closedHand, Tile tile)
     {
          List<Tile> pongTiles = new List<Tile>();
          string[] noCheck = new string[] { "Flower", "Season" };

          if (noCheck.Contains(tile.suit)) { return pongTiles; }
          foreach (Tile ownedTile in closedHand[tile.suit])
          {
               if (!ownedTile.suit.Equals(tile.suit) || ownedTile.open) { continue; }
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
          if (pongTiles.Count() < 2) { pongTiles.Clear(); }

          return pongTiles;
     }

     public static List<Tile> CanCheung(Dictionary<string, List<Tile>> closedHand, Tile tile)
     {
          List<Tile> cheungTiles = new List<Tile>();
          List<Tile> backCheung = new List<Tile>();
          List<Tile> fowardCheung = new List<Tile>();
          List<int> checkedInts = new List<int>();
          bool foundAdjacentTile = false;

          if (tile.number.IsUnityNull()) { return cheungTiles; }

          foreach (Tile ownedTile in closedHand[tile.suit])
          {
               int tileNumber = (int)ownedTile.number;
               int numDiff = (int)(tile.number - tileNumber);
               if (ownedTile.open || checkedInts.Contains(tileNumber)) { continue; }
               if (numDiff > 0 && numDiff <= 2)
               {
                    checkedInts.Add(tileNumber);
                    backCheung.Add(ownedTile);
               }
               if (numDiff < 0 && numDiff >= -2)
               {
                    checkedInts.Add(tileNumber);
                    fowardCheung.Add(ownedTile);
               }
          }

          foreach (Tile ownedTile in fowardCheung)
          {
               if ((int) (ownedTile.number - tile.number) != 1) { continue; }
               foundAdjacentTile = true;
               break; 
          }
          if (!foundAdjacentTile) { fowardCheung.Clear(); }
          foundAdjacentTile = false;

          foreach (Tile ownedTile in backCheung)
          {
               if ((int) (ownedTile.number - tile.number) != -1) { continue; }
               foundAdjacentTile = true;
               break; 
          }
          if (!foundAdjacentTile) { backCheung.Clear(); }

          cheungTiles.AddRange(backCheung);
          cheungTiles.AddRange(fowardCheung);
          if (cheungTiles.Count < 2) { cheungTiles.Clear(); }

          return cheungTiles;
     }
}
