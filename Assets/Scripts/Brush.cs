﻿using UnityEngine;
using System.Collections;

public class Brush : MonoBehaviour
{
    public static Brush Instance { get; private set; }

    public int currentBrush;

    public TileThemes[] brushThemes;
    public Decoration[] decorations;
    public GameObject highlightPrefab;
    public enum Brushes
    {
        ground,
        level1,
        level2,
        emptyDecoration,
        healthpack,
        armorVest,
        damageBoost
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetBrush(int type)
    {
        Instance.currentBrush = type;
    }

    // Bucket like paint tool that uses recursive flood fill algorithm.
    public void FillRegion(int posX, int posY, TileThemes fill, TileThemes old)
    {
        
        if ((posX < 0) || (posX >= TileMap.rows)) return;
        if((posY < 0) || (posY >= TileMap.columns)) return;

        Tile tile = TileMap.GetTileWithIndex(posX, posY);
        if(tile.envTileID == old.envTileID)
        {
            tile.SetTile(fill);
            FillRegion(posX + 1, posY, fill, old);
            FillRegion(posX, posY + 1, fill, old);
            FillRegion(posX - 1, posY, fill, old);
            FillRegion(posX , posY - 1, fill, old);
        }


    }

}