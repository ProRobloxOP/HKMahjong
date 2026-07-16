using System;
using System.Collections;
using UnityEngine;

public class RoundLogic : MonoBehaviour
{
    public static event Action<int> DrawTile;
    private WaitForSeconds turnDelay;

    private void OnEnable()
     {
         PlayerHand.PlayerDroppedTile += SwitchPlayerTurn; 
         WelcomeScreen.StartRound += StartRound;
     } 
    private void OnDisable()
     {
          PlayerHand.PlayerDroppedTile -= SwitchPlayerTurn;
          WelcomeScreen.StartRound -= StartRound;
     } 

     private void SwitchPlayerTurn(int lastPlayer, Tile droppedTile)
     {
          StartCoroutine(SwitchPlayerCorountine(lastPlayer, droppedTile));
     }

     private IEnumerator SwitchPlayerCorountine(int lastPlayer, Tile droppedTile)
     {
         int currPlayer = (lastPlayer == 4)? 1 : lastPlayer + 1;
         yield return turnDelay;
         DrawTile?.Invoke(currPlayer);
     }

     private void StartRound()
     {
          SwitchPlayerTurn(0, null);
     }

     // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
     {
          turnDelay = new WaitForSeconds(0.3f);
     }

    // Update is called once per frame
    void Update()
    {

    }
}
