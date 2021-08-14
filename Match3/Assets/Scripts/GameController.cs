using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public enum TileType
{
    Gold,
    Crystal,
    Iron,
    Wood,
    Meat,
    Tool
}

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject gameField;
    [SerializeField] private Sprite[] tileImages;

    private Tuple<GameObject, TileType>[,] inGameTiles;
    private Tuple<GameObject, TileType>[] inReserveTiles;
    private Camera camera;
    private bool swapIsStarted;
    private GameObject currentTile;

    private void Start()
    {
        camera = Camera.main;

        inGameTiles = new Tuple<GameObject, TileType>[10, 10];
        inReserveTiles = new Tuple<GameObject, TileType>[20];

        InitGameField();
    }

    private void InitGameField()
    {
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                var tileType = GetRandomTileType();
                var position = new Vector2(i*50, j*50);

                var currentTile = Instantiate(tilePrefab, gameField.transform);
                currentTile.transform.localPosition = position;
                currentTile.GetComponent<Image>().sprite = GetTileImage(tileType);

                inGameTiles[i, j] = new Tuple<GameObject, TileType>(currentTile, tileType);

                var cButton = currentTile.GetComponent<Button>();
                cButton.OnClickAsObservable().Subscribe(_ => OnTileClick(currentTile));
            }
        }
    }

    private TileType GetRandomTileType()
    {
        var i = UnityEngine.Random.Range(0, 6);

        var type = TileType.Gold;

        switch (i)
        {
            case 1: type = TileType.Crystal; break;
            case 2: type = TileType.Iron; break;
            case 3: type = TileType.Wood; break;
            case 4: type = TileType.Meat; break;
            case 5: type = TileType.Tool; break;
        }

        return type;
    }

    private Sprite GetTileImage(TileType type)
    {
        Sprite sprite = null;

        switch (type)
        {
            case TileType.Gold: sprite = tileImages[0]; break;
            case TileType.Crystal: sprite = tileImages[1]; break;
            case TileType.Iron: sprite = tileImages[2]; break;
            case TileType.Wood: sprite = tileImages[3]; break;
            case TileType.Meat: sprite = tileImages[4]; break;
            case TileType.Tool: sprite = tileImages[5]; break;
        }

        return sprite;
    }

    private void OnTileClick(GameObject tile)
    {
        if (swapIsStarted)
            SwapEnd(tile);
        else
            SwapStart(tile);
    }

    private void SwapStart(GameObject tile)
    {
        swapIsStarted = true;

        currentTile = tile;
    }

    private void SwapEnd(GameObject tile)
    {      
        if (tile != currentTile && CheskDistance(tile))
        {
            var tempPos = currentTile.transform.position;

            currentTile.transform.position = tile.transform.position;

            tile.transform.position = tempPos;
        }
        
        swapIsStarted = false;

        currentTile = null;
    }

    private bool CheskDistance(GameObject tile)
    {
        var distanceCheck = false;

        var curPosition = currentTile.transform.position;
        var swapPosition = tile.transform.position;

        if ((Mathf.Abs(curPosition.x - swapPosition.x) == 50 && curPosition.y == swapPosition.y)
            || (Mathf.Abs(curPosition.y - swapPosition.y) == 50 && curPosition.x == swapPosition.x))
            distanceCheck = true;

        return distanceCheck;
    }
}