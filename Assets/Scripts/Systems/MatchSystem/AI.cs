using System;
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

        int n = UnityEngine.Random.Range(0, playerHand.tiles["Char"].Count - 1);
        Tile tile = playerHand.tiles["Char"][n];
        playerHand.DropTile(tile);

        print("Cheungs: " + HandActions.ContainsCheung(playerHand.tiles).Count);
        print("Pongs: " + HandActions.ContainsPong(playerHand.tiles).Count);
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
