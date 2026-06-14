using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mono.Cecil;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class HandGUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Dictionary<string, List<Tile>> hand = PlayerHand.hand;
    static public List<string> tileList = new List<string>();
    private VisualElement handContainer;
    private VisualTreeAsset tileAsset;
    void Start()
    {
        handContainer = GetComponent<UIDocument>().rootVisualElement.Q("TileHand");
        foreach (Tile tile in hand["Char"])
        {
            tileList.Append(tile.name);
        }
        foreach (Tile tile in hand["Circle"])
        {
            tileList.Append(tile.name);
        }
        foreach (Tile tile in hand["Stick"])
        {
            tileList.Append(tile.name);
        }
        foreach (Tile tile in hand["Dragon"])
        {
            tileList.Append(tile.name);
        }
        foreach (Tile tile in hand["Wind"])
        {
            tileList.Append(tile.name);
        }
    
        for (int i = 0; i < tileList.Count; i++)
        {
            var tile = tileAsset.Instantiate();
            handContainer.Add(tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
