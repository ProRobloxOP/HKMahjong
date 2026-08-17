using System;
using System.Collections;
using UnityEngine;

public class MainClient : MonoBehaviour
{
    public static event Action<PlayerHand> SetClientHand;
    private static PlayerHand clientHand;
    private int playerIndex = 1;

    private void OnEnable()
    {
        TileCreator.CreatedTilesEvent += Setup;
        RoundLogic.DrawTile += DrawTile;
    }
    private void OnDisable() 
    { 
        TileCreator.CreatedTilesEvent -= Setup;
        RoundLogic.DrawTile -= DrawTile;
    }

    private IEnumerator DrawTileCoroutine(int playerIndex)
    {
        if (playerIndex != this.playerIndex || (bool) clientHand.GetStatus("ActionPending")) { yield break; }
        clientHand.DrawTilesFromWall(1);
    }

    private void DrawTile(int playerIndex)
    {
        StartCoroutine(DrawTileCoroutine(playerIndex));
    }

    private void Setup()
    {
        clientHand = RoundLogic.GetPlayerHand(playerIndex);
        SetClientHand?.Invoke(clientHand);
    }
}
