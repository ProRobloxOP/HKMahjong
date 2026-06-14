using UnityEngine;

[CreateAssetMenu(fileName = "HandCombos", menuName = "Scriptable Objects/HandCombos")]
public class HandCombos : ScriptableObject
{
     public static HandRank EightFlowers(PlayerHand playerHand)
     {
          int total = playerHand.tiles["Flower"].Count + playerHand.tiles["Season"].Count;
          return new HandRank
          {
               points = (total == 8)? 8 : 0
          };
     }
     
     public static HandRank AllFlowers(PlayerHand playerHand)
     {
          int flowers = 0, seasons = 0;

          foreach (Tile tile in playerHand.tiles["Flower"])
          {
               flowers += (tile.suit.Equals("Flower"))? 1 : 0;
               seasons += (tile.suit.Equals("Season"))? 1 : 0;

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
