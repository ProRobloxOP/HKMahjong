using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundLogic : MonoBehaviour
{
     private static List<PlayerHand> playerHands = new List<PlayerHand> ();
     private static List<int> playersActionPending = new List<int>();
     private static int playersLoaded = 0, playerCanWin = 0;
     private static int dealerIndex, drawingPlayerIndex;

     public static event Action<int, Tile> PlayerActionPendingUI;
     public static event Action<int> DrawTile;
     public static event Action BeginGame;
     private static WaitForSeconds turnDelay;
     private static List<Tile> wall;
     
     private void OnEnable()
     {
          ActionsGUI.PlayerAcceptedAction += PlayerAcceptedAction;
          WelcomeScreen.StartRound += OnPlayerLoaded;
          PlayerHand.TileDropped += SwitchPlayerTurn; 
          
     } 
    private void OnDisable()
     {
          ActionsGUI.PlayerAcceptedAction -= PlayerAcceptedAction;
          WelcomeScreen.StartRound -= OnPlayerLoaded;
          PlayerHand.TileDropped -= SwitchPlayerTurn;
     } 

     public static int GetDealerIndex()
     {
          return dealerIndex;
     }

     public static bool PlayerIsBefore(int checkPlayer, int targetPlayer)
     {
          return (checkPlayer == 4)? targetPlayer == 1 : checkPlayer + 1 == targetPlayer;
     }

     public static PlayerHand GetPlayerHand(int playerIndex)
     {
          return playerHands[playerIndex - 1];
     }

     private void CheckAnyPlayerActionPending()
     {
          playersActionPending.Clear();
          playerCanWin = 0;

          foreach (PlayerHand playerHand in playerHands)
          {
               if (!(bool) playerHand.GetStatus("ActionPending")) { continue; }
               playersActionPending.Add(playerHand.GetPlayerIndex());
          }
     }

     private IEnumerator SwitchPlayerCorountine()
     {
         CheckAnyPlayerActionPending();

         if (playersActionPending.Count > 0) { yield break; }
         yield return turnDelay;
         DrawTile?.Invoke(drawingPlayerIndex);
     }
     private IEnumerator WaitToDraw(int playerIndex)
     {
          yield return WaitToDraw(playerIndex, false);
     }

     private IEnumerator WaitToDraw(int playerIndex, bool noDraw)
     {
          playersActionPending.Remove(playerIndex);
          yield return new WaitUntil(() => playersActionPending.Count == 0);

          if (drawingPlayerIndex != playerIndex) { 
               if (!playersActionPending.Contains(drawingPlayerIndex)){ 
                    DrawTile?.Invoke(drawingPlayerIndex); 
               }
               yield break; 
          }
          if (noDraw) { SwitchPlayerCorountine(); } else { DrawTile?.Invoke(playerIndex); }
     }

     private IEnumerator WaitToPeng(int playerIndex)
     {
          yield return new WaitUntil(() => playerCanWin == 0);
          SwitchPlayerTurn(playerIndex, null); 
          playersActionPending.Remove(playerIndex);
     }

     private void SetPendingActions(int lastPlayer, Tile droppedTile)
     {
          foreach (PlayerHand playerHand in playerHands)
          {
               Dictionary<string, List<Tile>> currTiles = playerHand.GetCurrentTiles();
               int playerIndex = playerHand.GetPlayerIndex();

               if (lastPlayer == playerIndex) { continue; }
               if (playerIndex != 1) { continue; } // TESTING
               if (HandActions.CanWin(currTiles)) {
                    playerHand.SetStatus("ActionPending", true); 
                    playerCanWin++;
               }
               if (HandActions.CanKong(currTiles, droppedTile).Count > 0) { playerHand.SetStatus("ActionPending", true); continue; }
               if (HandActions.CanPong(currTiles, droppedTile).Count > 0) { playerHand.SetStatus("ActionPending", true); continue; }
               if (PlayerIsBefore(lastPlayer, playerIndex) && HandActions.CanCheung(currTiles, droppedTile).Count > 0){ playerHand.SetStatus("ActionPending", true); }
          }
     }

     private void SwitchPlayerTurn(int lastPlayer, Tile droppedTile)
     {
          drawingPlayerIndex = (lastPlayer == 4)? 1 : lastPlayer + 1;
          SetPendingActions(lastPlayer, droppedTile);
          PlayerActionPendingUI?.Invoke(lastPlayer, droppedTile);
          StartCoroutine(SwitchPlayerCorountine());
     }

     private static void CheckHandForActions(PlayerHand playerHand, Tile tile)
     {
          Dictionary<string, List<Tile>> currTiles = playerHand.GetCurrentTiles();
          if (tile.open) { return; }
          if (HandActions.CanKong(currTiles, tile).Count > 0 || HandActions.CanWin(currTiles))
          {
               playerHand.SetStatus("ActionPending", true);
          }
     }

     private void PlayerAcceptedAction(int playerIndex, string actionName)
     {
          PlayerHand playerHand = playerHands[playerIndex - 1];
          playerHand.SetStatus("ActionPending", false);
          // if (actionName == "Hu") {}
          if (actionName == "Peng" || actionName == "Gang") { StartCoroutine(WaitToPeng(playerIndex)); }
          if (actionName == "Chi") { StartCoroutine(WaitToDraw(playerIndex, true)); }
          if (actionName == "Cancel") { StartCoroutine(WaitToDraw(playerIndex)); }
     }

     private static void RandomizeStartDraws()
     {
          int diceSum = 0;
          int startingTileId;

          for (int i = 0; i < 3; i++)
          {
               diceSum += UnityEngine.Random.Range(1, 7);
          }
          startingTileId = (int) TileSettings.BoardSetting[((diceSum+1)%4 + 2) % 4].startNum - 1 + diceSum*2;

          List<Tile> newEnd = wall.GetRange(startingTileId, wall.Count - startingTileId);
          wall.RemoveRange(startingTileId, newEnd.Count);
          wall.InsertRange(0, newEnd);
     }

     public static void Init()
     {
          dealerIndex = UnityEngine.Random.Range(1, 5);
          turnDelay = new WaitForSeconds(0.3f);
          wall = TileCreator.GetWallTiles();

          RandomizeStartDraws();

          for (int i = 0; i < 4; i++)
          {
               PlayerHand playerHand = ScriptableObject.CreateInstance<PlayerHand>();
               int playerIndex = i+1;
               playerHand.Setup(TileCreator.GetTileObjects(), playerIndex, playerIndex == dealerIndex);
               playerHand.OnDraw(CheckHandForActions);
               playerHands.Add(playerHand);
          }
     }

     private void OnPlayerLoaded()
     {
          playersLoaded++;
          if (playersLoaded < 1) { return; }
          BeginGame?.Invoke();
     }

     void Start()
     {
          TileCreator.Init();
     }
}
