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

public class Buttons : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private Dictionary<string, List<Tile>> hand = PlayerHand.hand;
    private List<string> tileList = HandGUI.tileList;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
