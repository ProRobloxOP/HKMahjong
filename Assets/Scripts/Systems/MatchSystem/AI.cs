using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AI : MonoBehaviour
{

    private void OnEnable()
    {
        TileCreator.CreatedTilesEvent += SetupHand;
        RoundLogic.DrawTile += DrawTile;
    } 
    private void OnDisable()
    {
        TileCreator.CreatedTilesEvent -= SetupHand;
        RoundLogic.DrawTile -= DrawTile;
    } 
    private PlayerHand playerHand;

    [SerializeField] public int playerIndex;

    private void SetupHand()
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

        playerHand.SetupPlayerHand(Tiles, playerIndex);
    } 

    private void DropTile()
    {
        List<Tile> tiles = new List<Tile>{};

        foreach (List<Tile> tileList in playerHand.tiles.Values)
        {
            foreach (Tile tile in tileList)
            {
                if (tile.open) { continue; }
                tiles.Add(tile);
            }
        }
        if (tiles.Count == 0) { return; }

        int n = UnityEngine.Random.Range(0, tiles.Count - 1);
        playerHand.DropTile(playerIndex, tiles[n]);
    }

    private void DrawTile(int playerIndex)
    {
        if (playerIndex != this.playerIndex) { return; }
        playerHand.DrawTilesFromWall(1);
        DropTile();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHand = PlayerHand.CreateInstance<PlayerHand>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
