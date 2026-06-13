using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public struct Tile
{
    public int? number;
    public string suit;
    public string name;
}

public class TileCreator : MonoBehaviour
{

    [SerializeField] private TileSettings tileSettings;
    [SerializeField] private GameObject blankTile;

    private Vector3 SetTilePosX(Transform tileTransform, Vector3 tileBounds, int column, int row)
    {
        return new Vector3(tileTransform.position.x + (column-1)*tileBounds.x*tileSettings.AxisSpacing, tileTransform.position.y + (row -1)*tileBounds.y*tileSettings.YSpacing, tileTransform.position.z);
    }

    private Vector3 SetTilePosZ(Transform tileTransform, Vector3 tileBounds, int column, int row)
    {
        return new Vector3(tileTransform.position.x, tileTransform.position.y + (row -1)*tileBounds.y*tileSettings.YSpacing, tileTransform.position.z + (column-1)*tileBounds.z*tileSettings.AxisSpacing);
    }

    private Vector3 SetTilePos(GameObject tile, int column, int row, String axis)
    {
        if (axis == "x")
        {
            return SetTilePosX(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row);
        }
        return SetTilePosZ(tile.transform, tile.GetComponent<Renderer>().bounds.size, column, row);
    }

    private void CreateTileStack(int stackNum)
    {
        for (int row = 1; row <= tileSettings.RowStack; row++)
        {
            for (int column = 1; column <= tileSettings.ColumnStack; column++)
            {
                TileStack tileStack = tileSettings.BoardSetting[stackNum];
                GameObject tile = Instantiate(blankTile, tileStack.pos, tileSettings.BoardSetting[stackNum].rot);
                Transform transform = tile.transform;
                Vector3 localScale = transform.localScale;

                int tileNumber = (row*column*(stackNum+1));
                
                transform.localScale = new Vector3(
                    localScale.x*tileSettings.Scale,
                    localScale.y*tileSettings.Scale,
                    localScale.z*tileSettings.Scale
                );
                transform.position = SetTilePos(tile, column, row, tileStack.axis);
                tile.name = tileNumber.ToString();
                transform.SetParent(GameObject.Find("Tiles").transform, true);
            }
        }
    }

    public void CreateTiles()
    {
        for (int i = 0; i < tileSettings.BoardSetting.Length; i++)
        {
            CreateTileStack(i);
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
