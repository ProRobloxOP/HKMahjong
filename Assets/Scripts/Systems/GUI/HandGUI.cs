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
using UnityEngine.SceneManagement;

public class HandGUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerHand playerHand;
    public Dictionary<string, List<Tile>> tiles;
    private List<string> handList;
    void Start()
    {
        tiles = playerHand.tiles;
        handList = new List<string>();
        foreach (List<Tile> tileList in tiles.Values)
        {
            foreach (Tile tile in tileList)
            {
                handList.Add(tile.ToString());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
