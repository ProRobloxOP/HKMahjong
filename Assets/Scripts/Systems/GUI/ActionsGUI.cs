using System.Collections.Generic;
using UnityEngine;

public class ActionsGUI : MonoBehaviour
{
    private Dictionary<string, GameObject> actionGUIs = new Dictionary<string, GameObject>();
    private PlayerHand clientHand;

    void OnEnable()
    {
        MainClient.getClientHand += SetClientHand;
        PlayerHand.PlayerDroppedTile += OnTileDrop;
    }

    void OnDisable()
    {
        MainClient.getClientHand -= SetClientHand;
        PlayerHand.PlayerDroppedTile -= OnTileDrop;
    }

    private bool PlayerIsBefore(int playerIndex)
    {
        return (playerIndex == 4)? clientHand.GetPlayerIndex() == 1 : clientHand.GetPlayerIndex() - 1 == playerIndex;
    } 

    private void OnTileDrop(int playerIndex, Tile droppedTile)
    {
        if (playerIndex == clientHand.GetPlayerIndex())
        {
            foreach (GameObject actionGUI in actionGUIs.Values)
            {
                actionGUI.SetActive(false);
            }
            return;
        }

        Dictionary<string, List<Tile>> currTiles = clientHand.GetCurrentTiles();
        if (HandActions.CanKong(currTiles, droppedTile).Count > 0) { actionGUIs["Gang"].SetActive(true); }
        if (HandActions.CanPong(currTiles, droppedTile).Count > 0) { actionGUIs["Peng"].SetActive(true); }
        if (PlayerIsBefore(playerIndex) && HandActions.CanCheung(currTiles, droppedTile).Count > 0){ actionGUIs["Chi"].SetActive(true); }
    }

    private void OnTileDraw()
    {
        
    }

    private void SetClientHand(PlayerHand clientHand)
    {
        this.clientHand = clientHand;
    }

    private void FindActionGUIs()
    {
        Transform contentTransform = transform.Find("Viewport").Find("Content");
        
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            Transform actionGUI = contentTransform.GetChild(i);
            actionGUIs[actionGUI.name] = actionGUI.gameObject;
        }
    }

    void Start()
    {
        FindActionGUIs();
    }
}
