using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AI : MonoBehaviour
{
    [SerializeField] private int playerIndex;

    private PlayerHand playerHand;

    private void OnEnable()
    {
        RoundLogic.BeginGame += Setup;
        RoundLogic.DrawTile += DrawTile;
    } 
    private void OnDisable()
    {
        RoundLogic.BeginGame -= Setup;
        RoundLogic.DrawTile -= DrawTile;
    } 

    private void DropTile()
    {
        Dictionary<string, List<Tile>> tiles = playerHand.GetCurrentTiles();
        List<Tile> toDrop = new List<Tile>{};

        foreach (List<Tile> tileList in tiles.Values)
        {
            foreach (Tile tile in tileList)
            {
                if (tile.open) { continue; }
                toDrop.Add(tile);
            }
        }
        if (toDrop.Count == 0) { return; }

        int n = Random.Range(0, toDrop.Count - 1);
        playerHand.DropTile(playerIndex, toDrop[n]);
    }

    private void DrawTile(int playerIndex)
    {
        if (playerIndex != this.playerIndex) { return; }
        playerHand.DrawTilesFromWall(1);
        DropTile();
    }

    private void Setup()
    {
        playerHand = RoundLogic.GetPlayerHand(playerIndex);
        if ( (bool) playerHand.GetStatus("IsDealer")) { DropTile(); }
    } 
}
