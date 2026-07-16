using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class HandGUI : MonoBehaviour
{
    private PlayerHand clientHand;
    public Dictionary<string, List<Tile>> tiles;
    private List<string> handList;

    private void fillHandList()
    {
        handList = new List<string>();

        foreach (List<Tile> tileList in tiles.Values)
        {
            foreach (Tile tile in tileList)
            {
                handList.Add(tile.ToString());
            }
        }
    }

    private UnityEngine.UI.Button CreateTileUI(Tile tile)
    {
        Transform contentTransform = transform.Find("Viewport").Find("Content");
        GameObject tileUITemplate = contentTransform.Find("Template").gameObject;
        GameObject tileUI = Instantiate(tileUITemplate);
        UnityEngine.UI.Button tileButton = tileUI.GetComponent<UnityEngine.UI.Button>();
        UnityEngine.UI.Image tileImage = tileButton.image;
        Texture2D tileTexture = (Texture2D) Resources.Load("Images/HKTiles/" + tile.ToString()); 

        tileUI.name = tile.id.ToString();
        tileImage.sprite = Sprite.Create(
            tileTexture,
            new Rect(0, 0, tileTexture.width, tileTexture.height),
            new Vector2(0.5f, 0.5f)
        );
        tileUI.transform.SetParent(contentTransform);
        tileUI.SetActive(true);

        return tileButton;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clientHand = MainClient.GetClientHand();
        tiles = clientHand.tiles;
        fillHandList();

        foreach (Tile tile in tiles["Char"])
        {
            CreateTileUI(tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
