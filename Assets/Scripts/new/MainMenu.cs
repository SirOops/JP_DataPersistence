using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1000)]
public class MainMenu : MonoBehaviour
{
    public InputField playerNameInput;
    public Button startButton;
    public Button highScoreButton;
    public Button exitButton;

    public bool firstSceneChange;

    [SerializeField] private TMPro.TextMeshProUGUI highScoreText;

    private string currentPlayerName;

    private void Awake()
    {
        Debug.Log("I am awake " + this.name);
        firstSceneChange = PlayerDataManager.Instance.firstSceneChange;
    }

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        highScoreButton.onClick.AddListener(ShowHighScores);
        exitButton.onClick.AddListener(Exit);
        if (!firstSceneChange)
        {
            LoadPlayerData();
        }
        highScoreText.text = PlayerDataManager.Instance.LoadTopHighScoreAsString();
    }

    void LoadPlayerData()
    {
        currentPlayerName = PlayerDataManager.Instance.LoadPlayerName();
        Debug.Log("Loaded player name: " + currentPlayerName);
        playerNameInput.text = currentPlayerName;
    }

    void SavePlayerData(string playerName)
    {
        // Save player name
        PlayerDataManager.Instance.SavePlayerName(playerName);
    }

    public void StartGame()
    {
        string playerName = playerNameInput.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name cannot be empty!");
            return;
        }

        SavePlayerData(playerName);
        PlayerDataManager.Instance.firstSceneChange = false;
        Debug.Log("Starting game for player: " + playerName);
        SceneManager.LoadScene("MainGameScene");
    }

    public void ShowHighScores()
    {
        Debug.Log("Showing high scores");
        SceneManager.LoadScene("HighScoreScene");
    }

    public void LoadTopHighScore()
    {
        highScoreText.text = PlayerDataManager.Instance.LoadTopHighScoreAsString();
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}