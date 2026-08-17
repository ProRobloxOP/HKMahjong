using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionsGUI : MonoBehaviour
{
    private Dictionary<string, GameObject> actionGUIs = new Dictionary<string, GameObject>();
    public static event Action<int, string> PlayerAcceptedAction;
    private PlayerHand clientHand;

    void OnEnable()
    {
        MainClient.SetClientHand += SetClientHand;
        RoundLogic.PlayerActionPendingUI += OnTileDrop;
    }

    void OnDisable()
    {
        MainClient.SetClientHand -= SetClientHand;
        RoundLogic.PlayerActionPendingUI -= OnTileDrop;
    }

    private void HideActionGUIs()
    {
       foreach (GameObject actionGUI in actionGUIs.Values)
        {
            actionGUI.SetActive(false);
        } 
    }

    private void CancelActionListener()
    {
        PlayerAcceptedAction?.Invoke(clientHand.GetPlayerIndex(), "Cancel");
        HideActionGUIs();
    }

    private void GangActionListener()
    {
        PlayerAcceptedAction?.Invoke(clientHand.GetPlayerIndex(), "Gang");
        HideActionGUIs();
    }

    private void PengActionListener()
    {
        PlayerAcceptedAction?.Invoke(clientHand.GetPlayerIndex(), "Peng");
        HideActionGUIs();
    }

    private void ChiActionListener()
    {
        PlayerAcceptedAction?.Invoke(clientHand.GetPlayerIndex(), "Chi");
        HideActionGUIs();
    }

    private void OnTileDrop(int playerIndex, Tile droppedTile)
    {
        Dictionary<string, List<Tile>> currTiles = clientHand.GetCurrentTiles();
        if (droppedTile.suit == "Flower" || droppedTile.suit == "Season") { return; }
        if (playerIndex == clientHand.GetPlayerIndex())
        {
            foreach (GameObject actionGUI in actionGUIs.Values)
            {
                actionGUI.SetActive(false);
            }
            return;
        }

        if (!(bool) clientHand.GetStatus("ActionPending")) { return; }
        if (HandActions.CanKong(currTiles, droppedTile).Count > 0) { actionGUIs["Gang"].SetActive(true); }
        if (HandActions.CanPong(currTiles, droppedTile).Count > 0) { actionGUIs["Peng"].SetActive(true); }
        if (RoundLogic.PlayerIsBefore(playerIndex, clientHand.GetPlayerIndex()) && HandActions.CanCheung(currTiles, droppedTile).Count > 0){ actionGUIs["Chi"].SetActive(true); }
        actionGUIs["Cancel"].SetActive(true);
    }

    private void OnTileDraw(PlayerHand playerHand, Tile drawedTile)
    {
        Dictionary<string, List<Tile>> currTiles = clientHand.GetCurrentTiles();
        if (!(bool) clientHand.GetStatus("ActionPending")) { return; }
        if (HandActions.CanKong(currTiles, drawedTile).Count > 0 || HandActions.ContainsKong(currTiles).Count > 0)
        {
            actionGUIs["Cancel"].SetActive(true);
            actionGUIs["Gang"].SetActive(true);
        }
    }

    private void SetClientHand(PlayerHand clientHand)
    {
        this.clientHand = clientHand;
        clientHand.OnDraw(OnTileDraw);
    }

    private void FindActionGUIs()
    {
        Transform contentTransform = transform.Find("Viewport").Find("Content");
        
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            Transform actionGUI = contentTransform.GetChild(i);
            actionGUIs[actionGUI.name] = actionGUI.gameObject;
        }

        actionGUIs["Cancel"].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CancelActionListener);
        actionGUIs["Chi"].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ChiActionListener);
        actionGUIs["Peng"].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(PengActionListener);
        actionGUIs["Gang"].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(GangActionListener);
    }

    void Start()
    {
        FindActionGUIs();
    }
}
