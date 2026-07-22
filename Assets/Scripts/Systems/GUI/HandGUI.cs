using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGUI : MonoBehaviour
{
    private bool canTileDrop = false;
    private PlayerHand clientHand;

    void OnEnable()
    {
        MainClient.getClientHand += SetClientHand;
        WelcomeScreen.StartRound += UpdateHandUI;
        RoundLogic.DrawTile += EnableTileDrop;
    }

    void OnDisable()
    {
        MainClient.getClientHand -= SetClientHand;
        WelcomeScreen.StartRound -= UpdateHandUI;
        RoundLogic.DrawTile -= EnableTileDrop;
    }

    private void EnableTileDrop(int playerIndex)
    {
        if (playerIndex != clientHand.GetPlayerIndex()) { return; }
        canTileDrop = true;
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

    private void OnTileClick(Tile tile)
    {
        if (!canTileDrop) { return; }
        clientHand.DropTile(clientHand.GetPlayerIndex(), tile);
        canTileDrop = false;
    }

    private UnityEngine.UI.Button CreateTileUI(Tile tile)
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

        return tileButton;
    }

    private IEnumerator UpdateHand()
    {
        yield return new WaitUntil(() => clientHand != null);

        Dictionary<string, List<Tile>> tiles = clientHand.GetCurrentTiles();
        ClearContentUI();

        foreach (Tile tile in clientHand.GetHandList(true))
        {
            CreateTileUI(tile);
        }
    }

    private void SetClientHand(PlayerHand clientHand)
    {
        this.clientHand = clientHand;
        clientHand.OnUpdate(() => StartCoroutine(UpdateHand()));
    }

    public void UpdateHandUI()
    {
        StartCoroutine(UpdateHand());
    }
}
