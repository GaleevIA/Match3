using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour 
{
    public static UIController instance;

    [Header("Elements")]
    [SerializeField] private Slider slider;
    [SerializeField] private Text[] roundText, scoreText, timerText, totalScoreText, finalRoundText;
    
    [Header("Panels")]
    [Space, SerializeField] private GameObject mainPanel, gameOverPanel, pausePanel;

    [Header("Buttons")]
    [Space, SerializeField] private Button restartButton, stayButton, pauseButton;
    [SerializeField] private Button[] backToMenuButtons;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameController.instance.ObserveEveryValueChanged(_ => GameController.instance.score)
            .Subscribe(_ => UpdateScoreText())
            .AddTo(this);

        GameController.instance.ObserveEveryValueChanged(_ => GameController.instance.scoreRoundGoal)
            .Subscribe(_ => UpdateScoreText())
            .AddTo(this);

        GameController.instance.ObserveEveryValueChanged(_ => GameController.instance.round)
            .Subscribe(_ => UpdateRoundText())
            .AddTo(this);

        GameController.instance.ObserveEveryValueChanged(_ => GameController.instance.timer)
            .Subscribe(_ => UpdateTimerText())
            .AddTo(this);

        GameController.instance.OnGameOver.Subscribe(_ => DisplayGameOverText()).AddTo(this);

        foreach (var button in backToMenuButtons)
            button.OnClickAsObservable().Subscribe(_ => BackToMenuButtonEvent()).AddTo(this);

        restartButton.OnClickAsObservable().Subscribe(_ => RestartGameButtonEvent()).AddTo(this);

        pauseButton.OnClickAsObservable().Subscribe(_ => PauseButtonEvent()).AddTo(this);

        stayButton.OnClickAsObservable().Subscribe(_ => StayButtonEvent()).AddTo(this);

        mainPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    //Эффект исчезновения надписи с полученными очками
    public IEnumerator PopUpFadeAway(GameObject obj, float time) 
    {
        obj.SetActive(true);

        Color originalcolor = obj.GetComponent<Text>().color;

        for (float t = 1.0f; t >= 0.0f; t -= Time.deltaTime / time) 
        {
            Color newColor = new Color(originalcolor.r, originalcolor.g, originalcolor.b, t);

            obj.GetComponent<Text>().color = newColor;

            yield return null;
        }

        obj.SetActive(false);
    }

    //Вызываем надпись с полученными очками
    public void AdditionPopUp(GameObject obj, float time, int scoreAdd) 
    {
        if (scoreAdd > 0) 
            obj.GetComponent<Text>().text = "+" + scoreAdd + "pts.";

        StartCoroutine(PopUpFadeAway(obj, time));
    }

    //Обновление элемента очки
    private void UpdateScoreText()
    {
        scoreText[0].text = GameController.instance.score + " /" + GameController.instance.scoreRoundGoal;
        scoreText[1].text = GameController.instance.score + " /" + GameController.instance.scoreRoundGoal;
    }

    //Обновление элемента раунд
    private void UpdateRoundText()
    {
        roundText[0].text = "Round #" + GameController.instance.round;
        roundText[1].text = "Round #" + GameController.instance.round;
    }

    //Обновление элементов, связанных с таймером
    private void UpdateTimerText()
    {
        timerText[0].text = Mathf.Round(GameController.instance.timer) + "";
        slider.value = GameController.instance.timer;
    }

    //Обновление интерфейса при окончании игры
    private void DisplayGameOverText() 
    {
        mainPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        pausePanel.SetActive(false);

        totalScoreText[0].text = GameController.instance.totalScore + "";
        totalScoreText[1].text = GameController.instance.totalScore + "";

        finalRoundText[0].text = GameController.instance.round + "";
        finalRoundText[1].text = GameController.instance.round + "";
    }

    private void RestartGameButtonEvent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void BackToMenuButtonEvent()
    {
        SceneManager.LoadScene(0);
    }

    private void StayButtonEvent()
    {
        pausePanel.SetActive(false);

        Time.timeScale = 1;
    }

    private void PauseButtonEvent()
    {
        Time.timeScale = 0;

        pausePanel.SetActive(true);
    }
}