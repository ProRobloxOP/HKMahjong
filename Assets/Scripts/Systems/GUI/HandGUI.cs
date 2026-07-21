using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGUI : MonoBehaviour
{
    private Dictionary<string, List<Tile>> tiles;
    private List<Tile> handList;
    private PlayerHand clientHand;

    void OnEnable()
    {
        MainClient.getClientHand += GetClientHand;
        WelcomeScreen.StartRound += SetupHandUI;
    }

    void OnDisable()
    {
        MainClient.getClientHand -= GetClientHand;
        WelcomeScreen.StartRound -= SetupHandUI;
    }

    private void FillHandList()
    {
        handList = new List<Tile>();

        foreach (List<Tile> tileList in tiles.Values)
        {
            foreach (Tile tile in tileList)
            {
                handList.Add(tile);
            }
        }
    }

    private UnityEngine.UI.Button CreateTileUI(Tile tile)
    {
        if (tile.suit == "Flower" || tile.suit == "Season") { return null; }
        Transform contentTransform = transform.Find("Viewport").Find("Content");
        GameObject tileUITemplate = contentTransform.Find("Template").gameObject;
        GameObject tileUI = Instantiate(tileUITemplate, contentTransform);

        RectTransform tileRect = tileUI.GetComponent<RectTransform>();
        UnityEngine.UI.Button tileButton = tileUI.GetComponent<UnityEngine.UI.Button>();
        UnityEngine.UI.Image tileImage = tileUI.GetComponent<UnityEngine.UI.Image>();
        Texture2D tileTexture = (Texture2D) Resources.Load("Images/HKTiles/" + tile.ToString()); 

        tileUI.name = tile.id.ToString();
        tileRect.localPosition = Vector3.zero;
        tileRect.localRotation = Quaternion.identity;
        tileRect.localScale = Vector3.one;

        tileImage.sprite = Sprite.Create(
            tileTexture,
            new Rect(0, 0, tileTexture.width, tileTexture.height),
            new Vector2(0.5f, 0.5f)
        );
        tileUI.SetActive(true);

        return tileButton;
    }

    private IEnumerator Setup()
    {
        yield return new WaitUntil(() => clientHand != null);
        tiles = clientHand.tiles;
        FillHandList();

        foreach (Tile tile in handList)
        {
            CreateTileUI(tile);
        }
    }

    private void GetClientHand(PlayerHand clientHand)
    {
        this.clientHand = clientHand;
    }

    private void SetupHandUI()
    {
        StartCoroutine(Setup());
    }
}
