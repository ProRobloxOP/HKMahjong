using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;

public class MainClient : MonoBehaviour
{

    public static event Func<PlayerHand> GetMainClientHand;
    private int playerIndex = 1;

    private void OnEnable()
    {
        GetMainClientHand += GetClientHand;
        TileCreator.CreatedTilesEvent += SetupClientHand;
        RoundLogic.DrawTile += DrawTile;
    }
    private void OnDisable() 
    { 
        GetMainClientHand -= GetClientHand;
        TileCreator.CreatedTilesEvent -= SetupClientHand; 
        RoundLogic.DrawTile -= DrawTile;
    }


    private PlayerHand clientHand;

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

        clientHand.SetupPlayerHand(Tiles, 1, true);
    }

    private PlayerHand GetClientHand()
    {
        return clientHand;
    }

    private void DropTile()
    {
        List<Tile> tiles = new List<Tile>{};

        foreach (List<Tile> tileList in clientHand.tiles.Values)
        {
            foreach (Tile tile in tileList)
            {
                if (tile.open) { continue; }
                tiles.Add(tile);
            }
        }
        if (tiles.Count == 0) { return; }

        int n = UnityEngine.Random.Range(0, tiles.Count - 1);
        clientHand.DropTile(playerIndex, tiles[n]);
    }

    private void DrawTile(int playerIndex)
    {
        print(playerIndex);
        if (playerIndex != this.playerIndex) { return; }
        clientHand.DrawTilesFromWall(1);
        DropTile();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clientHand = PlayerHand.CreateInstance<PlayerHand>();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
