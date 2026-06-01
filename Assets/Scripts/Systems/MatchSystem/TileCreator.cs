using System;
using System.Collections.Generic;
using UnityEngine;

public class TileCreator : MonoBehaviour
{

    [SerializeField] private TileSettings tileSettings;
    [SerializeField] private GameObject blankTile;

    public void CreateTiles()
    {
        blankTile = Instantiate(blankTile, new Vector3(60, 0, 0), Quaternion.identity);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
