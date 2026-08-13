using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private static List<int> playersActionPending = new List<int>();
    private static int playerCanWin = 0;
    private Tile lastDroppedTile;

    void OnEnable()
    {
        ActionsGUI.PlayerAcceptedAction += PlayerAcceptedAction;
        PlayerHand.TileDropped += OnTileDrop;
    }

    void OnDisable()
    {
        ActionsGUI.PlayerAcceptedAction -= PlayerAcceptedAction;
        PlayerHand.TileDropped -= OnTileDrop;
    }

    public static List<int> GetActionPendingPlayers()
    {
        return playersActionPending;
    }

    private void OnTileDrop(int playerIndex, Tile droppedTile)
    {
        this.lastDroppedTile = droppedTile;
    }

    private IEnumerator WaitToDraw(int playerIndex)
    {
        yield return WaitToDraw(playerIndex, false);
    }

    private IEnumerator WaitToDraw(int playerIndex, bool noDraw)
    {
        int drawingPlayerIndex = RoundLogic.GetDrawingPlayerIndex();
        if (drawingPlayerIndex != playerIndex)
        {
            if (!playersActionPending.Contains(drawingPlayerIndex))
            {
                RoundLogic.PlayerDrawTile(drawingPlayerIndex);
            }
            yield break;
        }
        if (noDraw) { RoundLogic.SwitchPlayerCorountine(); } else { RoundLogic.PlayerDrawTile(drawingPlayerIndex); }
    }

    private void RemoveDroppedTile(Tile tile)
    {
        GameObject droppedTiles = GameObject.Find("DroppedTiles");
        Transform tileTransform = droppedTiles.transform.Find(tile.id.ToString());

        foreach (PlayerHand playerHand in RoundLogic.GetPlayerHands())
        {
            playerHand.GetDroppedTiles()[tile.ownerIndex - 1].Remove(tile);
        }
        
        Destroy(tileTransform.gameObject);
    }

    private void ShowMeldTiles(PlayerHand playerHand, List<Tile> meldTiles)
    {
        List<List<Tile>> openMelds = playerHand.GetOpenMelds();

        for (int i = 0; i < meldTiles.Count; i++)
        {
            Tile meldTile = meldTiles[i];
            GameObject tilePrefab = Resources.Load<GameObject>("Prefabs/Tiles/" + meldTile.ToString());
            GameObject tileObject = TileCreator.CreateTile(tilePrefab, TileSettings.OutsideHandSettings[0].pos, TileSettings.OutsideHandSettings[0].rot, meldTile.id);
            tileObject.transform.position = TileSettings.OutsideHandSettings[0].pos;
            tileObject.transform.rotation = TileSettings.OutsideHandSettings[0].rot;
            tileObject.transform.SetParent(GameObject.Find("TestTiles").transform);

            tileObject.transform.position = TileCreator.SetTilePos(tileObject, openMelds.Count * 3 + i+1, 1, TileSettings.OutsideHandSettings[0].axis, TileSettings.OutsideHandSettings[0].direction);
            meldTile.inCheung = true;
            meldTile.open = true; 
        }
    }

    private IEnumerator PengAcceptedTile(int playerIndex, Tile acceptedTile)
    {
        yield return PengAcceptedTile(playerIndex, acceptedTile, false);
    }

    private IEnumerator PengAcceptedTile(int playerIndex, Tile acceptedTile, bool gangCheck)
    {
        PlayerHand playerHand = RoundLogic.GetPlayerHand(playerIndex);
        List<Tile> meldTiles = gangCheck? HandActions.CanKong(playerHand.GetCurrentTiles(), acceptedTile) : 
            HandActions.CanPong(playerHand.GetCurrentTiles(), acceptedTile);
        
        if (meldTiles.Count == 0) { yield break; }
        yield return new WaitUntil(() => playerCanWin == 0);
        if (meldTiles.Count > 2) { meldTiles.RemoveRange(2, meldTiles.Count - 2); }

        playersActionPending.Remove(playerIndex);
        meldTiles.Add(acceptedTile);
        playerHand.AddOpenMeld(meldTiles);

        ShowMeldTiles(playerHand, meldTiles);
        RemoveDroppedTile(acceptedTile);
        
        if (gangCheck){ playerHand.DrawTilesFromWall(1, true); }
    }

    private IEnumerator ChiAcceptedTile(int playerIndex, Tile acceptedTile)
    {
        PlayerHand playerHand = RoundLogic.GetPlayerHand(playerIndex);
        List<Tile> meldTiles = HandActions.CanCheung(playerHand.GetCurrentTiles(), acceptedTile);
        Dictionary<string, List<Tile>> sortedMeld = new Dictionary<string, List<Tile>>{
            [acceptedTile.suit] = meldTiles,
        };
        if (meldTiles.Count == 0) { yield break; }

        playersActionPending.Remove(playerIndex);
        yield return new WaitUntil(() => playersActionPending.Count == 0);
        PlayerHand.SortGeneralTile(sortedMeld, acceptedTile);
        playerHand.AddOpenMeld(meldTiles);

        ShowMeldTiles(playerHand, meldTiles);
        RemoveDroppedTile(acceptedTile);
        StartCoroutine(WaitToDraw(playerIndex, true));
    }

    private void PlayerAcceptedAction(int playerIndex, string actionName)
    {
        PlayerHand playerHand = RoundLogic.GetPlayerHand(playerIndex);
        playerHand.SetStatus("ActionPending", false);
        // if (actionName == "Hu") {}
        if (actionName == "Gang" || actionName == "Peng") { StartCoroutine(PengAcceptedTile(playerIndex, lastDroppedTile, actionName == "Geng")); }
        if (actionName == "Chi") { StartCoroutine(ChiAcceptedTile(playerIndex, lastDroppedTile)); }
        if (actionName == "Cancel") { StartCoroutine(WaitToDraw(playerIndex)); }
    }

    public static void SetPendingActions(int lastPlayer, Tile droppedTile)
    {
        foreach (PlayerHand playerHand in RoundLogic.GetPlayerHands())
        {
            Dictionary<string, List<Tile>> currTiles = playerHand.GetCurrentTiles();
            int playerIndex = playerHand.GetPlayerIndex();

            if (lastPlayer == playerIndex) { continue; }
            if (playerIndex != 1) { continue; } // TESTING
            if (HandActions.CanWin(currTiles))
            {
                playerHand.SetStatus("ActionPending", true);
                playerCanWin++;
            }
            if (HandActions.CanKong(currTiles, droppedTile).Count > 0 || HandActions.CanPong(currTiles, droppedTile).Count > 0) { playerHand.SetStatus("ActionPending", true); }
            if (RoundLogic.PlayerIsBefore(lastPlayer, playerIndex) && HandActions.CanCheung(currTiles, droppedTile).Count > 0) { playerHand.SetStatus("ActionPending", true); }
        }
    }

    public static void CheckHandForActions(PlayerHand playerHand, Tile tile)
    {
        Dictionary<string, List<Tile>> currTiles = playerHand.GetCurrentTiles();
        if (HandActions.ContainsKong(currTiles, true).Count > 0 || !tile.IsUnityNull() && HandActions.CanKong(currTiles, tile).Count > 0 || HandActions.CanWin(currTiles))
        {
            playerHand.SetStatus("ActionPending", true);
        }
    }

    public static void CheckAnyPlayerActionPending()
    {
        playersActionPending.Clear();
        playerCanWin = 0;

        foreach (PlayerHand playerHand in RoundLogic.GetPlayerHands())
        {
            if (!(bool)playerHand.GetStatus("ActionPending")) { continue; }
            playersActionPending.Add(playerHand.GetPlayerIndex());
        }
    }
}
