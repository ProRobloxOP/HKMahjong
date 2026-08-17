using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundLogic : MonoBehaviour
{
     private static List<PlayerHand> playerHands = new List<PlayerHand> ();
     private static int dealerIndex, drawingPlayerIndex;
     private static int playersLoaded = 0;

     public static event Action<int, Tile> PlayerActionPendingUI;
     public static event Action<int> DrawTile;
     public static event Action BeginGame;
     private static WaitForSeconds turnDelay;
     private static List<Tile> wall;
     
     private void OnEnable()
     {
          WelcomeScreen.StartRound += OnPlayerLoaded;
          PlayerHand.TileDropped += OnTileDrop; 
          
     } 
    private void OnDisable()
     {
          WelcomeScreen.StartRound -= OnPlayerLoaded;
          PlayerHand.TileDropped -= OnTileDrop;
     } 

     public static int GetDealerIndex()
     {
          return dealerIndex;
     }

     public static int GetDrawingPlayerIndex()
     {
          return drawingPlayerIndex;
     }

     public static bool PlayerIsBefore(int checkPlayer, int targetPlayer)
     {
          return (checkPlayer == 4)? targetPlayer == 1 : checkPlayer + 1 == targetPlayer;
     }

     public static List<PlayerHand> GetPlayerHands()
     {
          return playerHands;
     }

     public static PlayerHand GetPlayerHand(int playerIndex)
     {
          return playerHands[playerIndex - 1];
     }

     public static void PlayerDrawTile(int playerIndex)
     {
          DrawTile?.Invoke(playerIndex);
     }

     public static IEnumerator SwitchPlayerCorountine()
     {
         PlayerActions.CheckAnyPlayerActionPending();

         if (PlayerActions.GetActionPendingPlayers().Count > 0) { yield break; }
         yield return turnDelay;
         DrawTile?.Invoke(drawingPlayerIndex);
     }
     

     public static void SwitchPlayerTurn(MonoBehaviour coroutineRunner, int lastPlayer, Tile droppedTile)
     {
          drawingPlayerIndex = (lastPlayer == 4)? 1 : lastPlayer + 1;
          PlayerActions.SetPendingActions(lastPlayer, droppedTile);
          PlayerActionPendingUI?.Invoke(lastPlayer, droppedTile);
          coroutineRunner.StartCoroutine(SwitchPlayerCorountine());
     }

     private void OnTileDrop(int lastPlayer, Tile droppedTile)
     {
          SwitchPlayerTurn(this, lastPlayer, droppedTile);
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

     private static void SpawnOutsideHand()
     {
         for (int i = 0; i < 3; i++)
          {
                Tile tile = wall[i];
               GameObject tilePrefab = Resources.Load<GameObject>("Prefabs/Tiles/" + tile.ToString());
               GameObject tileObject = TileCreator.CreateTile(tilePrefab, TileSettings.OutsideHandSettings[0].pos, TileSettings.OutsideHandSettings[0].rot, tile.id);
               tileObject.transform.position = TileSettings.OutsideHandSettings[0].pos;
               tileObject.transform.rotation = TileSettings.OutsideHandSettings[0].rot;
               tileObject.transform.SetParent(GameObject.Find("TestTiles").transform);

               tileObject.transform.position = TileCreator.SetTilePos(tileObject, i+1, 1, TileSettings.OutsideHandSettings[0].axis, TileSettings.OutsideHandSettings[0].direction);
          }
     }

     public static void Init()
     {
          dealerIndex = UnityEngine.Random.Range(1, 5);
          turnDelay = new WaitForSeconds(0.3f);
          wall = TileCreator.GetWallTiles();

          RandomizeStartDraws();
          //SpawnOutsideHand();

          for (int i = 0; i < 4; i++)
          {
               PlayerHand playerHand = ScriptableObject.CreateInstance<PlayerHand>();
               int playerIndex = i+1;
               playerHand.Setup(TileCreator.GetTileObjects(), playerIndex, playerIndex == dealerIndex);
               playerHand.OnDraw(PlayerActions.CheckHandForActions);
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
