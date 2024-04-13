using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class HighScoreMenu : MonoBehaviour
{
    public Text highScoreBoardText;
    public Button resetButton;
    public Button backButton;


    void Start()
    {
        LoadHighScores();        
        resetButton.onClick.AddListener(ResetHighScores);
        backButton.onClick.AddListener(GoBackToMenu);
    }

    void LoadHighScores()
    {
        highScoreBoardText.text = PlayerDataManager.Instance.LoadHighScoresAsString();
    }   
  

    public void ResetHighScores()
    {
        PlayerDataManager.Instance.ResetHighScores();
        LoadHighScores();
    }

    void GoBackToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}