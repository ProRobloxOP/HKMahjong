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
    }

    private IEnumerator RefreshNextFrame()
    {
        yield return null; // wait one frame for UIDocument to finish initializing
        refreshHandList();
    }

    //private void refreshHandList()
    //{
    //    var tiles = clientHand.tiles;
    //    handList = new List<string>();
    //    foreach (KeyValuePair<string, List<Tile>> kvp in tiles)
    //    {
    //        List<Tile> tileList = kvp.Value;
    //        foreach (Tile tile in tileList)
    //        {
    //            handList.Add(tile.ToString());
    //        }
    //    }
    //    rootElement = tileUI.rootVisualElement;
    //    handContainer = rootElement.Q<VisualElement>("Hand");
    //    foreach (string tile in handList)
    //    {
    //        VisualElement singleTileWrapper = new VisualElement();
    //        tileAsset.CloneTree(singleTileWrapper);
    //        string backgroundPath = "Images/Tiles/" + tile + ".png";
    //        Sprite background = AssetDatabase.LoadAssetAtPath<Sprite>(backgroundPath);
    //        singleTileWrapper.style.backgroundImage = new StyleBackground(background);
    //        handContainer.Add(singleTileWrapper);
    //    }
    //}

    //private void refreshHandList()
    //{
    //    if (clientHand == null)
    //    {
    //        Debug.LogError("refreshHandList: clientHand is null.");
    //        return;
    //    }

    //    var tiles = clientHand.tiles;
    //    if (tiles == null)
    //    {
    //        Debug.LogWarning("refreshHandList: clientHand.tiles is null.");
    //        return;
    //    }

    //    handList = new List<string>();
    //    foreach (KeyValuePair<string, List<Tile>> kvp in tiles)
    //    {
    //        List<Tile> tileList = kvp.Value;
    //        if (tileList == null) continue;
    //        foreach (Tile tile in tileList)
    //        {
    //            if (tile == null) continue;
    //            handList.Add(tile.ToString());
    //        }
    //    }

    //    if (tileUI == null) { Debug.LogError("tileUI is null."); return; }
    //    rootElement = tileUI.rootVisualElement;
    //    if (rootElement == null) { Debug.LogError("rootVisualElement is null."); return; }
    //    handContainer = rootElement.Q<VisualElement>("Hand");
    //    if (handContainer == null) { Debug.LogError("Hand container not found."); return; }
    //    if (tileAsset == null) { Debug.LogError("tileAsset is null."); return; }

    //    handContainer.Clear();
    //    Debug.Log($"handList count: {handList.Count}");
    //    Debug.Log($"handContainer child count before: {handContainer.childCount}");

    //    foreach (string tile in handList)
    //    {
    //        Debug.Log($"Processing tile: {tile}");

    //        VisualElement singleTileWrapper = new VisualElement();
    //        tileAsset.CloneTree(singleTileWrapper);
    //        Debug.Log($"Wrapper child count after CloneTree: {singleTileWrapper.childCount}");

    //        VisualElement tileRoot = singleTileWrapper[0];
    //        Debug.Log($"tileRoot name: {tileRoot.name}, type: {tileRoot.GetType()}");

    //        Sprite background = Resources.Load<Sprite>("Images/Tiles/" + tile);
    //        Debug.Log($"Sprite loaded: {background != null} for path Images/Tiles/{tile}");

    //        tileRoot.style.backgroundImage = new StyleBackground(background);
    //        handContainer.Add(singleTileWrapper);
    //    }

    //    Debug.Log($"handContainer child count after: {handContainer.childCount}");
    //}

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
