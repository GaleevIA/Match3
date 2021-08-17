using UnityEngine;
using UniRx;
using System;

public class GameController : MonoBehaviour 
{
    public static GameController instance;

    [SerializeField] private BoardController board;   
    [SerializeField] private int roundGoalIncrease = 35;
    [SerializeField] private int resetTimer = 120;
    [SerializeField] private int roundOneGoal = 10;

    [HideInInspector] public int score, totalScore, round, scoreRoundGoal;
    [HideInInspector] public float timer;
    [HideInInspector] public bool isSelected;

    public Subject<bool> OnGameOver = new Subject<bool>();

    public GameObject additionUI, shuffleUI, nextRoundUIController;

    private int selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2;
    private bool isSwitching = false;
    private bool gameover = false;

    void Awake() 
    {
        instance = this;
        Setup();
    }

    private void Start()
    {
        Time.timeScale = 1;       
    }

    void Update() 
    {
        UpdateTimer();
    }

    //Настройка первичных параметров
    private void Setup() 
    {
        round = 1;
        timer = resetTimer;
        scoreRoundGoal = roundOneGoal;
        isSelected = false;
    }

    //Обновляем таймер, если таймер опустился до нуля, то заканчиваем игру
    private void UpdateTimer() 
    {
        if (timer <= 0 && !gameover) 
            GameOver();
        else 
            timer -= Time.deltaTime;
    }

    //Проверим можно ли совершить ход
    public void CheckIfPossibleMove() 
    {
        if (PerpendicularMove() && !isSwitching) 
        {
            if (board.CheckIfSwitchIsPossible(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2, true)) 
            {
                AudioController.instance.Play("Swap");

                board.MakeSpriteSwitch(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2);

                isSwitching = true;

                ResetCoordinates();

                isSelected = false;
            }
        }
    }

    //Проверяем можем ли мы поменять выбранные гемы
    bool PerpendicularMove() 
    {
        if ((selectedXCor_1 + 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) ||           
                   (selectedXCor_1 - 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) ||    
                   (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 + 1 == selectedYCor_2) ||    
                   (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 - 1 == selectedYCor_2)) 
        {
            return true;
        } 
        else 
            return false;
    }


    //Добавление очков и обновление таймера, в случае есть текущее количество очков мы набрали
    public void AddScore(int sequenceCount) 
    {
        score += sequenceCount - 1;
        totalScore += sequenceCount - 1;

        UIController.instance.AdditionPopUp(additionUI, 1.0f, sequenceCount - 1);

        if (score >= scoreRoundGoal) 
        {
            StartCoroutine(UIController.instance.PopUpFadeAway(nextRoundUIController, 3.0f));

            scoreRoundGoal += roundGoalIncrease;
            round += 1;
            score = 0;
            timer = resetTimer;
        }
    }

    public void SetIsSelected(bool status) 
    {
        AudioController.instance.Play("Select");

        isSelected = status;
    }

    public void SetIsSwitching(bool status) 
    {
        isSwitching = status;
    }

    public void SetSelectedCoordinates(bool firstSelection, int x, int y) 
    {
        if (firstSelection) 
        {
            selectedXCor_1 = x;
            selectedYCor_1 = y;
        } 
        else 
        {
            selectedXCor_2 = x;
            selectedYCor_2 = y;
        }
    }

    private void ResetCoordinates() 
    {
        selectedXCor_1 = 0;
        selectedYCor_1 = 0;
        selectedXCor_2 = 0;
        selectedYCor_2 = 0;
    }

    //Обработка события окончания игры
    private void GameOver() 
    {
        gameover = true;
      
        board.DeactivateBoard();

        SaveResult();

        OnGameOver.OnNext(true);

        Time.timeScale = 0;
    }

    private void SaveResult()
    {
        string saveData = "";

        if (PlayerPrefs.HasKey("TableOfRecords"))
            saveData = PlayerPrefs.GetString("TableOfRecords");

        saveData += $"{DateTime.Now} - очки: {totalScore} - раунд: {round};";

        PlayerPrefs.SetString("TableOfRecords", saveData);
    }
}