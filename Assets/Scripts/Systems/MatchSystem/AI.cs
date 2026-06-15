using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AI : MonoBehaviour
{

    private void OnEnable() => TileCreator.CreatedTilesEvent += SetupHand;
    private void OnDisable() => TileCreator.CreatedTilesEvent -= SetupHand;
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

        for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
        {
            int n = UnityEngine.Random.Range(1, 3);
            String suit = (n == 1)? "Circle" : (n==2)? "Stick" : "Char";
            List<Tile> tileList = playerHand.tiles[suit];

            n = UnityEngine.Random.Range(0, tileList.Count - 1);
            playerHand.DropTile(playerIndex, tileList[n]);
        }
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
