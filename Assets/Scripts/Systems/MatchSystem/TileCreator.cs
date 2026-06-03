using System;
using System.Collections.Generic;
using UnityEngine;

public class TileCreator : MonoBehaviour
{

    [SerializeField] private TileSettings tileSettings;
    [SerializeField] private GameObject blankTile;

    public void CreateTiles()
    {
        for (int row = 1; row <= tileSettings.RowStack; row++)
        {
            for (int column = 1; column <= tileSettings.ColumnStack; column++)
            {
                GameObject tile = Instantiate(blankTile, new Vector3(-300, 40, 250), Quaternion.Euler(new Vector3(-90, 90, 0)));
                Transform transform = tile.transform;
                Vector3 renderer = tile.GetComponent<Renderer>().bounds.size;
                transform.position = new Vector3(transform.position.x + (row-1)*renderer.x*1.08f, transform.position.y + (column -1)*renderer.y*1.05f, transform.position.z);
                tile.name = (row*column).ToString();
                transform.SetParent(GameObject.Find("Tiles").transform, true);
            }
        }
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
