using System;
using System.Threading.Tasks;
using UnityEngine;

public class RoundLogic : MonoBehaviour
{
    public static event Action<int> DrawTile;
    int currPlayer = 0;

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

    private async void SwitchPlayerTurn(int lastPlayer, Tile droppedTile)
     {
          currPlayer = (currPlayer == 4)? 1 : currPlayer + 1;
          await Task.Delay(300);
          DrawTile?.Invoke(currPlayer);
     }

     private void StartRound()
     {
          SwitchPlayerTurn(0, null);
     }

     // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
     {
          
     }

    // Update is called once per frame
    void Update()
    {

    }
}
