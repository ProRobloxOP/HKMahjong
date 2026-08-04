using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundLogic : MonoBehaviour
{
    public static event Action<int> DrawTile;
    private WaitForSeconds turnDelay;
    private static int dealerIndex;
    private List<Tile> wall;

    private void OnEnable()
     {
         PlayerHand.TileDropped += SwitchPlayerTurn; 
         WelcomeScreen.StartRound += StartRound;
     } 
    private void OnDisable()
     {
          PlayerHand.TileDropped -= SwitchPlayerTurn;
          WelcomeScreen.StartRound -= StartRound;
     } 

     public static int GetDealerIndex()
     {
          return dealerIndex;
     }

     private IEnumerator SwitchPlayerCorountine(int lastPlayer)
     {
         int currPlayer = (lastPlayer == 4)? 1 : lastPlayer + 1;
         yield return turnDelay;
         DrawTile?.Invoke(currPlayer);
     }

     private void SwitchPlayerTurn(int lastPlayer, Tile droppedTile)
     {
          StartCoroutine(SwitchPlayerCorountine(lastPlayer));
     }

     private void RandomizeStartDraws(int startingTileId)
     {
          List<Tile> newEnd = wall.GetRange(startingTileId, wall.Count - startingTileId);
          wall.RemoveRange(startingTileId, newEnd.Count);
          wall.AddRange(newEnd);
     }

     private void StartRound()
     {
          RandomizeStartDraws(UnityEngine.Random.Range(0, wall.Count));
          SwitchPlayerTurn(dealerIndex - 1, null);
     }

     // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
     {
          dealerIndex = UnityEngine.Random.Range(1, 5);
          turnDelay = new WaitForSeconds(0.3f);
          wall = TileCreator.wall;
     }

    // Update is called once per frame
    void Update()
    {

    }
}
