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
        StartCoroutine(RefreshNextFrame());

        for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
        {
            int n = UnityEngine.Random.Range(1, 4);
            String suit = (n == 1) ? "Circle" : (n == 2) ? "Stick" : "Char";
            List<Tile> tileList = clientHand.tiles[suit];

            n = UnityEngine.Random.Range(0, tileList.Count - 1);
            clientHand.DropTile(1, tileList[n]);
        }
    }

    private IEnumerator RefreshNextFrame()
    {
        yield return null; // wait one frame for UIDocument to finish initializing
        refreshHandList();
    }

    private void refreshHandList()
    {
        rootElement = tileUI.rootVisualElement;
        PrintHierarchy(rootElement);

        ScrollView handScroll = rootElement.Q<ScrollView>("Hand");
        handScroll.contentContainer.style.flexDirection = FlexDirection.Row;
        handScroll.contentContainer.style.flexWrap = Wrap.NoWrap;
        handScroll.mode = ScrollViewMode.Horizontal;
        handScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

        handScroll.Clear();

        List<Tile> allTiles = clientHand.tiles.Values.SelectMany(t => t).ToList();

        foreach (Tile tile in allTiles)
        {
            VisualElement wrapper = new VisualElement();
            tileAsset.CloneTree(wrapper);
            VisualElement tileEl = wrapper.Q<VisualElement>("Tile");
            if (tileEl == null) continue;

            Sprite background = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/HKTiles/" + tile.ToString() + ".png");
            if (background != null)
                tileEl.style.backgroundImage = new StyleBackground(background);

            // capture tile in local variable for the lambda
            Tile capturedTile = tile;
            tileEl.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    clientHand.DropTile(clientHand.playerIndex, capturedTile);
                    clientHand.DrawTilesFromWall(1); // draw next tile
                    refreshHandList();
                }
            });
            handScroll.Add(wrapper);
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
