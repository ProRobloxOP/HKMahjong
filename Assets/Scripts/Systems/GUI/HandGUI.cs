using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandGUI : MonoBehaviour
{

    private Tile blankTile = new Tile { id = -1, name = "Back", suit = "Wind" };
    private PlayerHand clientHand = null;
    private bool canTileDrop = false;

    void OnEnable()
    {
        PlayerHand.TileDropped += CheckHandActions;
        MainClient.SetClientHand += SetClientHand;
        RoundLogic.BeginGame += OnGameBegin;
    }

    void OnDisable()
    {
        PlayerHand.TileDropped -= CheckHandActions;
        MainClient.SetClientHand -= SetClientHand;
        RoundLogic.BeginGame -= OnGameBegin;
    }

    private void ClearContentUI()
    {
        Transform contentTransform = transform.Find("Viewport").Find("Content");

        for (int i = 0; i < contentTransform.childCount; i++)
        {
            Transform tileTransform = contentTransform.GetChild(i);
            if (tileTransform.name == "Template") { continue; }
            Destroy(tileTransform.gameObject);
        }
    }

    private void CheckHandActions(int playerIndex, Tile droppedTile)
    {
        if (playerIndex != clientHand.GetPlayerIndex()) { return; }
        canTileDrop = false;
        UpdateHandUI();
    }

    private void OnTileClick(Tile tile)
    {
        if (!canTileDrop || (bool) clientHand.GetStatus("ActionPending")) { return; }
        clientHand.DropTile(clientHand.GetPlayerIndex(), tile);
        canTileDrop = false;
    }

    private GameObject CreateSpacerTile()
    {
        GameObject spacer = CreateTileUI(blankTile); 
        UnityEngine.UI.Image tileImage = spacer.GetComponent<UnityEngine.UI.Image>();
        RectTransform tileRect = spacer.GetComponent<RectTransform>();
        tileRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tileRect.rect.width*1/2);
        tileImage.enabled = false;

        return spacer;
    }

    private GameObject CreateTileUI(Tile tile)
    {
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
        tileButton.onClick.AddListener(() => OnTileClick(tile));
        tileUI.SetActive(true);

        return tileUI;
    }

    private IEnumerator UpdateHand(Tile drawnTile)
    {
        yield return new WaitUntil(() => clientHand != null);

        Dictionary<string, List<Tile>> tiles = clientHand.GetCurrentTiles();
        ClearContentUI();

        foreach (Tile tile in clientHand.GetHandList(true))
        {
            if (!drawnTile.IsUnityNull() && drawnTile.id == tile.id) { continue; }
            CreateTileUI(tile);
        }

        if (drawnTile.IsUnityNull() || drawnTile.open){ yield break; }
        CreateSpacerTile();
        CreateTileUI(drawnTile);
    }

    private void UpdateHandUI()
    {
        StartCoroutine(UpdateHand(null));
    }

    private void OnTileDraw(Tile tile)
    {
        StartCoroutine(UpdateHand(tile));
        canTileDrop = true;
    }

    private void SetClientHand(PlayerHand clientHand)
    {
        this.clientHand = clientHand;
        clientHand.OnDraw((PlayerHand playerHand, Tile tile) => OnTileDraw(tile));
        UpdateHandUI();
    }

    private void OnGameBegin()
    {
        if (clientHand.GetPlayerIndex() == RoundLogic.GetDealerIndex()) { canTileDrop = true; }
        transform.Find("Viewport").gameObject.SetActive(true);
    }
}
