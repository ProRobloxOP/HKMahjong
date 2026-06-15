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

    public UIDocument tileUI;
    public VisualTreeAsset tileAsset;
    private List<string> handList;
    private VisualElement rootElement;
    private VisualElement handContainer;

    private void OnEnable()
    {
        TileCreator.CreatedTilesEvent += SetupClientHand;
        PlayerHand.PlayerDroppedTile += OnTileDrop;
    }
    private void OnDisable()
    {
        TileCreator.CreatedTilesEvent -= SetupClientHand;
        PlayerHand.PlayerDroppedTile -= OnTileDrop;
    }


    private PlayerHand clientHand;

    private void OnTileDrop(int playerIndex, Tile tile)
    {
        clientHand.OnTileDrop(playerIndex, tile);
    }

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

        clientHand.SetupPlayerHand(Tiles, 1);

        for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
        {
            int n = UnityEngine.Random.Range(1, 4);
            String suit = (n == 1)? "Circle" : (n==2)? "Stick" : "Char";
            List<Tile> tileList = clientHand.tiles[suit];

            n = UnityEngine.Random.Range(0, tileList.Count - 1);
            clientHand.DropTile(1, tileList[n]);
        }
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

    private void PrintHierarchy(VisualElement element, int depth = 0)
    {
        Debug.Log($"{new string('-', depth)}{element.GetType().Name} name='{element.name}' class='{string.Join(",", element.GetClasses())}'");
        foreach (var child in element.Children())
        {
            PrintHierarchy(child, depth + 1);
        }
    }
}
