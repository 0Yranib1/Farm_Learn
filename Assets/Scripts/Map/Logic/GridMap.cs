using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    private Tilemap currentTilemap;

    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            if (mapData != null)
            {
                mapData.tileProperties.Clear();
            }
        }
    }

    private void OnDisable()
    {
        currentTilemap = GetComponent<Tilemap>();
        if (mapData != null)
        {
            UpdateTileProperties();
#if UNITY_EDITOR
            if (mapData != null)
            {
                EditorUtility.SetDirty(mapData);
            }
            #endif
        }
    }

    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();
        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                //绘制地图左下角坐标
                Vector3Int starPos = currentTilemap.cellBounds.min;
                //绘制地图右上角坐标
                Vector3Int endPos = currentTilemap.cellBounds.max;
                
                for (int x = starPos.x; x <= endPos.x; x++)
                {
                    for (int y = starPos.y; y <= endPos.y; y++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                        {
                            TileProperty newTile = new TileProperty
                            {
                                tileCoordinate = new Vector2Int(x, y),
                                gridType = gridType,
                                boolTypeValue = true
                            };
                            mapData.tileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}
