using UnityEngine.SceneManagement;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Text tableOfRecords;

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject tableOfRecordsPanel;
    [SerializeField] private GameObject aboutProgramPanel;
    [SerializeField] private GameObject exitPanel;

    [Header("Buttons")]
    [Space, SerializeField] private Button buttonStartGame;
    [SerializeField] private Button buttonOpenTableOfRecordsPanel;
    [SerializeField] private Button buttonOpenAboutProgramPanel;
    [SerializeField] private Button buttonOpenExitGamePanel;
    [SerializeField] private Button buttonReturnMenu;
    [SerializeField] private Button buttonExit;
    [SerializeField] private Button buttonCancelExit;

    private MenuPanel menuPanel;

    private void Start()
    {
        Time.timeScale = 1;
        
        UpdateView();

        this.ObserveEveryValueChanged(_ => menuPanel).Subscribe(_ => UpdateView()).AddTo(this);

        buttonStartGame.OnClickAsObservable()
            .Subscribe(_ => SceneManager.LoadScene(1))
            .AddTo(this);

        buttonOpenTableOfRecordsPanel.OnClickAsObservable()
            .Subscribe(_ => 
            { 
                menuPanel = MenuPanel.TableOnRecords; 
                LoadTableOfRecords(); 
            })
            .AddTo(this);

        buttonOpenAboutProgramPanel.OnClickAsObservable()
            .Subscribe(_ => menuPanel = MenuPanel.AboutProgram)
            .AddTo(this);

        buttonOpenExitGamePanel.OnClickAsObservable()
            .Subscribe(_ => menuPanel = MenuPanel.Exit)
            .AddTo(this);

        buttonReturnMenu.OnClickAsObservable()
            .Subscribe(_ => menuPanel = MenuPanel.Main)
            .AddTo(this);

        buttonExit.OnClickAsObservable()
            .Subscribe(_ => Application.Quit())
            .AddTo(this);

        buttonCancelExit.OnClickAsObservable()
            .Subscribe(_ => menuPanel = MenuPanel.Main)
            .AddTo(this);
    }

    private void UpdateView()
    {
        mainPanel.SetActive(menuPanel == MenuPanel.Main);
        tableOfRecordsPanel.SetActive(menuPanel == MenuPanel.TableOnRecords);
        aboutProgramPanel.SetActive(menuPanel == MenuPanel.AboutProgram);
        exitPanel.SetActive(menuPanel == MenuPanel.Exit);
        buttonReturnMenu.gameObject.SetActive(menuPanel == MenuPanel.TableOnRecords || menuPanel == MenuPanel.AboutProgram);
    }

    private void LoadTableOfRecords()
    {
        if (PlayerPrefs.HasKey("TableOfRecords"))
        {
            var loadData = PlayerPrefs.GetString("TableOfRecords");

            tableOfRecords.text = loadData.Replace(';', '\n');
        }       
    }
}

public enum MenuPanel
{
    Main,
    TableOnRecords,
    AboutProgram,
    Exit
}