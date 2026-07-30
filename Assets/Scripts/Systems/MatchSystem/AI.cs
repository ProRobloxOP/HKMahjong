using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AI : MonoBehaviour
{
    [SerializeField] private int playerIndex;

    private PlayerHand playerHand;

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

        playerHand.SetupPlayerHand(Tiles, playerIndex, false);
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

        int n = UnityEngine.Random.Range(0, toDrop.Count - 1);
        playerHand.DropTile(playerIndex, toDrop[n]);
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
        playerHand = ScriptableObject.CreateInstance<PlayerHand>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
