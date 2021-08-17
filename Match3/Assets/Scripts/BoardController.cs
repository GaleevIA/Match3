using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour 
{
    [SerializeField] private GameObject gemPrefab;
    [SerializeField] private Sprite[] gemsSprites;
    [SerializeField] private int gridWidth = 6;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private float tileSize = 10;
    [SerializeField] private float waitTimeMove = 0.5f;

    private int[,] indexGrid;
    private bool reset = true;
    private bool combo = false;
    private Vector3 initialPos;
    private GameObject[,] gems;
    private List<Dictionary<string, int>> matchCoordinates;

    void Awake()
    {
        Setup();
    }

    void Start()
    {
        SetupBoard(false);
    }

    //Установка первичных настроек
    void Setup() 
    {
        initialPos = gameObject.transform.position;
        indexGrid = new int[gridWidth, gridHeight];
        gems = new GameObject[gridWidth, gridHeight];
        matchCoordinates = new List<Dictionary<string, int>>();
    }

    //Первичное создание игрового поля и в случае если нет возможных ходов его перемешивание
    void SetupBoard(bool shuffle) 
    {
        while (reset) 
        {
            for (int x = 0; x < indexGrid.GetLength(0); x++) 
            {
                for (int y = 0; y < indexGrid.GetLength(1); y++) 
                {
                    indexGrid[x, y] = Random.Range(0, gemsSprites.GetLength(0));
                }
            }

            if (!CheckMatches(false) && CheckIfThereArePossibleMoves()) 
                reset = false;
        }

        for (int x = 0; x < indexGrid.GetLength(0); x++) 
        {
            for (int y = 0; y < indexGrid.GetLength(1); y++) 
            {
                if (!shuffle) 
                {
                    GameObject tempGemObj = Gem.Start(gemPrefab, initialPos, x, y, tileSize, gameObject.transform);
                    gems[x, y] = tempGemObj;
                }

                gems[x, y].GetComponent<SpriteRenderer>().sprite = gemsSprites[indexGrid[x, y]];

                Gem tempGem = gems[x, y].GetComponent<Gem>();

                tempGem.SetCoordinates(new Dictionary<string, int>() { { "x", x }, { "y", y } });
            }
        }
    }

    //Проверка на совпадение типов гемов в строках и столбцах
    bool CheckMatches(bool isPlayerMove) 
    {
        bool ifMatch = false;
        
        for (int x = 0; x < indexGrid.GetLength(0) - 2; x++) 
        {
            for (int y = 0; y < indexGrid.GetLength(1); y++) 
            {
                int current = indexGrid[x, y];

                if (current == indexGrid[x + 1, y] && current == indexGrid[x + 2, y]) 
                {
                    if (isPlayerMove) 
                        SaveMatchCoordinates(x, y, true);

                    ifMatch = true;
                }
            }
        }
        
        for (int x = 0; x < indexGrid.GetLength(0); x++) 
        {
            for (int y = 0; y < indexGrid.GetLength(1) - 2; y++) 
            {
                int current = indexGrid[x, y];

                if (current == indexGrid[x, y + 1] && current == indexGrid[x, y + 2]) 
                {
                    if (isPlayerMove) 
                        SaveMatchCoordinates(x, y, false);

                    ifMatch = true;
                }
            }
        }

        return ifMatch;
    }

    //После успешной проверки, меняем гемы местами
    public void MakeSpriteSwitch(int x_1, int y_1, int x_2, int y_2) 
    {
        Sprite spriteTemp_1 = gems[x_1, y_1].GetComponent<SpriteRenderer>().sprite;
        Sprite spriteTemp_2 = gems[x_2, y_2].GetComponent<SpriteRenderer>().sprite;

        gems[x_1, y_1].GetComponent<SpriteRenderer>().sprite = spriteTemp_2;
        gems[x_2, y_2].GetComponent<SpriteRenderer>().sprite = spriteTemp_1;
        gems[x_1, y_1].GetComponent<Gem>().ResetSelection();

        StartCoroutine(TriggerMatch());
    }

    //Проверка возможности поменять местами гемы
    public bool CheckIfSwitchIsPossible(int x_1, int y_1, int x_2, int y_2, bool isPlayerMove) 
    {
        int temp_1 = indexGrid[x_1, y_1];
        int temp_2 = indexGrid[x_2, y_2];

        indexGrid[x_1, y_1] = temp_2;
        indexGrid[x_2, y_2] = temp_1;

        if (CheckMatches(isPlayerMove)) 
        {
            if (isPlayerMove) 
                return true; 
            else 
            {
                indexGrid[x_1, y_1] = temp_1;
                indexGrid[x_2, y_2] = temp_2;

                return true;
            }
        } 
        else 
        {
            indexGrid[x_1, y_1] = temp_1;
            indexGrid[x_2, y_2] = temp_2;
        }

        return false;
    }

    //Сохраняем координаты совпадающих гемов
    void SaveMatchCoordinates(int x, int y, bool isHorizontal) 
    {
        matchCoordinates.Add(new Dictionary<string, int>() { { "x", x }, { "y", y } });

        if (isHorizontal) 
        {
            for (int i = 1; i < (gridWidth - x); i++) 
            {
                if (indexGrid[x, y] == indexGrid[x + i, y]) 
                    matchCoordinates.Add(new Dictionary<string, int>() { { "x", x + i }, { "y", y } });
                else 
                    break;
            }
        } 
        else 
        {
            for (int i = 1; i < (gridHeight - y); i++) 
            {
                if (indexGrid[x, y] == indexGrid[x, y + i]) 
                    matchCoordinates.Add(new Dictionary<string, int>() { { "x", x }, { "y", y + i } });
                else 
                    break;
            }
        }
    }

    //Проверка совпадения гемов
    IEnumerator TriggerMatch() 
    {
        foreach (Dictionary<string, int> unit in matchCoordinates) 
        {
            gems[unit["x"], unit["y"]].GetComponent<SpriteRenderer>().sprite = null;
        }

        GameController.instance.AddScore(matchCoordinates.Count);

        matchCoordinates.Clear();

        yield return new WaitForSeconds(waitTimeMove);

        AudioController.instance.Play("Clear");

        MoveGemsDownwards();

        yield return new WaitForSeconds(waitTimeMove);

        combo = CheckMatches(true);

        if (combo) 
        {
            StartCoroutine(TriggerMatch());
        } 
        else 
        {
            CheckIfThereArePossibleMoves();

            GameController.instance.SetIsSwitching(false);

            combo = false;
        }
    }

    //Добавляем новые гемы после уничтожения
    void MoveGemsDownwards() 
    {
        bool hasNull = true;

        while (hasNull) 
        {
            hasNull = false;

            for (int y = 0; y < gridHeight; y++) 
            {
                for (int x = 0; x < gridWidth; x++) 
                {
                    SpriteRenderer currentGemSprite = gems[x, y].GetComponent<SpriteRenderer>();

                    if (currentGemSprite.sprite == null) 
                    {
                        hasNull = true;

                        if (y != gridHeight - 1) 
                        {
                            SpriteRenderer aboveGemSprite = gems[x, y + 1].GetComponent<SpriteRenderer>();

                            indexGrid[x, y] = indexGrid[x, y + 1];
                            indexGrid[x, y + 1] = 7;

                            currentGemSprite.sprite = aboveGemSprite.sprite;
                            aboveGemSprite.sprite = null;
                        } 
                        else 
                        {
                            indexGrid[x, y] = Random.Range(0, gemsSprites.GetLength(0));
                            currentGemSprite.sprite = gemsSprites[indexGrid[x, y]];
                        }
                    }
                }
            }
        }
    }

    //Проверяем возможность хода
    bool CheckIfThereArePossibleMoves() 
    {
        bool ifPossibleMove = false;

        for (int x = 0; x < gridWidth; x++) 
        {
            for (int y = 0; y < gridHeight; y++) 
            {
                try 
                {
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x, y + 1, false);

                    if (ifPossibleMove) 
                        break;
                } 
                catch (System.IndexOutOfRangeException) { }

                try {
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x, y - 1, false);

                    if (ifPossibleMove) 
                        break;
                } 
                catch (System.IndexOutOfRangeException) { }

                try 
                {
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x + 1, y, false);

                    if (ifPossibleMove) 
                        break;
                } 
                catch (System.IndexOutOfRangeException) { }

                try 
                {
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x - 1, y, false);

                    if (ifPossibleMove) 
                        break;
                } 
                catch (System.IndexOutOfRangeException) { }
            }
            if (ifPossibleMove) {
                break;
            }
        }
        if (!ifPossibleMove) 
        {
            reset = true;

            SetupBoard(true);

            StartCoroutine(UIController.instance.PopUpFadeAway(GameController.instance.shuffleUI, 3.0f));
        }

        return ifPossibleMove;
    }

    public void DeactivateBoard() 
    {
        gameObject.SetActive(false);
    }
}