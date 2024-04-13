using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainGame : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    public Animator animatorBall;

    public Text ScoreText;
    public Text HighScoreText;

    public GameObject GameOverText;
    public Button restartButton;
    public Button backButton;
    public Button exitButton;

    public Text currentPlayerText;
    private string currentPlayerName;

    private int m_Points;
    private bool IsGameOver;
    private bool IsStarted;
    private bool IsPaused;
    private bool IsNewHighScore = false;

    private void Awake()
    {
        Debug.Log("I am awake " + this.name);
    }


    private void Start()
    {
        InitializeBricks();
        InitializeButtons();
        LoadPlayerData();
        HighScoreText.text = PlayerDataManager.Instance.LoadTopHighScoreAsString();
    }

    private void Update()
    {
        if (!IsStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
            IsStarted = !IsStarted;
        }

        if (!IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                IsPaused = !IsPaused;
            }

            if (IsPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
       
    }

    private void InitializeBricks()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };

        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                float positionX = -1.5f + step * x;
                float positionY = 2.5f + i * 0.3f;
                Vector3 position = new Vector3(positionX, positionY, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void InitializeButtons()
    {
        restartButton.onClick.AddListener(RestartLevel);
        backButton.onClick.AddListener(GoBackToMenu);       

        restartButton.onClick.AddListener(SaveHighScore);
        backButton.onClick.AddListener(SaveHighScore);
        
    }

    private void StartGame()
    {
        if (Ball != null)
        {
            float randomDirection = Random.Range(-1.0f, 1.0f);
            Vector3 forceDir = new Vector3(randomDirection, 1, 0).normalized;
            Ball.transform.SetParent(null);
            animatorBall.enabled = true;
            Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogError("Ball is not assigned.");
        }
    }

    private void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score: {m_Points}";
        TrackHighScore(m_Points);
    }

    public void GameOver()
    {
        TrackHighScore(m_Points);
        IsGameOver = true;
        GameOverText.SetActive(true);
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        GameOverText.SetActive(true);
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        GameOverText.SetActive(false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }  

    public void GoBackToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    void LoadPlayerData()
    {
        currentPlayerName = PlayerDataManager.Instance.LoadPlayerName();
        Debug.Log("Loaded player name: " + currentPlayerName);
        //show        
        currentPlayerText.text = currentPlayerName;
    }
  

    public void TrackHighScore(int newScore)
    {
        int currentHighScore = PlayerDataManager.Instance.GetCurrentHighScore();
        if (newScore > currentHighScore)
        {
            HighScoreText.text = "New High Score: " + currentPlayerName + " With " + newScore + " ponits!";
            HighScoreText.color = Color.yellow;
            StartCoroutine(FlashText(HighScoreText, 0.2f, 3));
            HighScoreText.color = Color.yellow;            
            IsNewHighScore = true;                              
        }
    }

    IEnumerator FlashText(Text text, float duration, int repeat)
    {
        for (int i = 0; i < repeat; i++)
        {
            text.enabled = false;
            yield return new WaitForSeconds(duration/repeat);
            text.enabled = true;
        }
    }

    void SaveHighScore()
    {
        PlayerDataManager.Instance.SaveHighScores(currentPlayerName, m_Points);
    }
}