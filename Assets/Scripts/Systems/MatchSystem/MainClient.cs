using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor;

public class MainClient : MonoBehaviour
{

    public UIDocument tileUI;
    public VisualTreeAsset tileHand;
    public VisualTreeAsset tileAsset;
    private List<string> handList;

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
        refreshHandList();
    }

    //private void refreshHandList()
    //{
    //    var tiles = clientHand.tiles;
    //    handList = new List<string>();
    //    foreach (List<Tile> tileList in tiles.Values)
    //    {
    //        foreach (Tile tile in tileList)
    //        {
    //            handList.Add(tile.ToString());
    //        }
    //    }
    //    VisualElement root = tileUI.rootVisualElement;
    //    VisualElement handContainer = root.Q<VisualElement>("Hand");
    //    foreach (string tile in handList)
    //    {
    //        VisualElement singleTileWrapper = new VisualElement();
    //        tileAsset.CloneTree(singleTileWrapper);
    //        string backgroundPath = "Assets/Images/Tiles/" + tile + ".png";
    //        Sprite background = AssetDatabase.LoadAssetAtPath<Sprite>(backgroundPath);
    //        singleTileWrapper.style.backgroundImage = new StyleBackground(background);
    //        handContainer.Add(singleTileWrapper);
    //    }
    //}
    private void refreshHandList()
    {
        var tiles = clientHand.tiles;
        handList = new List<string>();

        foreach (List<Tile> tileList in tiles.Values)
            foreach (Tile tile in tileList)
                handList.Add(tile.ToString());

        VisualElement root = tileUI.rootVisualElement;
        VisualElement handContainer = root.Q<VisualElement>("Hand");

        handContainer.Clear();

        foreach (string tileName in handList)
        {
            VisualElement singleTileWrapper = new VisualElement();
            tileAsset.CloneTree(singleTileWrapper);

            VisualElement tileElement = singleTileWrapper.Q<VisualElement>("Tile"); 
            Sprite background = Resources.Load<Sprite>("Assets/Images/Tiles/" + tileName);

            if (background != null)
                tileElement.style.backgroundImage = new StyleBackground(background);
            else
                Debug.LogWarning($"Could not find sprite for tile: {tileName}");

            handContainer.Add(singleTileWrapper);
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
}
