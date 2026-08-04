using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainClient : MonoBehaviour
{
    public static event Action<PlayerHand> getClientHand;
    private int playerIndex = 1;

    private void OnEnable()
    {
        TileCreator.CreatedTilesEvent += SetupClientHand;
        RoundLogic.DrawTile += DrawTile;
    }
    private void OnDisable() 
    { 
        TileCreator.CreatedTilesEvent -= SetupClientHand; 
        RoundLogic.DrawTile -= DrawTile;
    }

    private static PlayerHand clientHand;

    private void SetupClientHand()
    {
        GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
        GameObject Tiles = null;

        foreach (GameObject gameObject in rootObjs)
        {
            if (gameObject.name.Equals("Tiles"))
            {
                Tiles = gameObject;
                break;
            }
        }

        clientHand.SetupPlayerHand(Tiles, playerIndex, RoundLogic.GetDealerIndex() == playerIndex);
        getClientHand?.Invoke(clientHand);
    }

    private IEnumerator DrawTileCoroutine(int playerIndex)
    {
        if (playerIndex != this.playerIndex || clientHand.IsActionPending()) { yield break; }
        clientHand.DrawTilesFromWall(1);
    }

    private void DrawTile(int playerIndex)
    {
        StartCoroutine(DrawTileCoroutine(playerIndex));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clientHand = ScriptableObject.CreateInstance<PlayerHand>();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
