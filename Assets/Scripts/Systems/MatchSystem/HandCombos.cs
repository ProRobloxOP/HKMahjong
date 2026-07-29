using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandCombos", menuName = "Scriptable Objects/HandCombos")]
public class HandCombos : ScriptableObject
{
     public static HandRank EightFlowers(PlayerHand playerHand)
     {
          Dictionary<string, List<Tile>> tiles = playerHand.GetCurrentTiles();
          int total = tiles["Flower"].Count + tiles["Season"].Count;
          return new HandRank
          {
               points = (total == 8)? 8 : 0
          };
     }
     
     public static HandRank AllFlowers(PlayerHand playerHand)
     {
          Dictionary<string, List<Tile>> tiles = playerHand.GetCurrentTiles();
          int flowers = 0, seasons = 0;

          foreach (Tile tile in tiles["Flower"])
          {
               flowers += tile.suit.Equals("Flower")? 1 : 0;
               seasons += tile.suit.Equals("Season")? 1 : 0;

               if (flowers == TileSettings.general["Flower"] || seasons == TileSettings.general["Season"])
               {
                    return new HandRank
                    {
                         points = 2
                    };
               }
          }

          return new HandRank
          {
               points = 0
          };
     }
}
