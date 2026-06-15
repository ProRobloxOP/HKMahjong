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
    private int playerIndex = 1;
    private VisualElement rootElement;
    private VisualElement handContainer;

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


    private PlayerHand clientHand;

    private void OnTileDrop(int playerIndex, Tile tile)
    {
        refreshHandList(); 
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
