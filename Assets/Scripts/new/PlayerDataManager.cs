using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    private const string PlayerDataFilePath = "playerData.json";
    private const int maxHighScores = 7;

    private string playerName;
    private List<HighScoreEntry> highScoreList = new List<HighScoreEntry>();

    public bool firstSceneChange = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("I am awake " + this.name);
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPlayerName();
        LoadHighScores();        
    }

    public void SavePlayerName(string playerName)
    {
        this.playerName = playerName;

        PlayerData data = new PlayerData();
        data.playerName = playerName;

        string json = JsonUtility.ToJson(data);
        string filePath = Path.Combine(Application.persistentDataPath, PlayerDataFilePath);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Saved current player's name to: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving player name: " + e.Message);
        }
    }

    public string LoadPlayerName()
    {
        string filePath = Path.Combine(Application.persistentDataPath, PlayerDataFilePath);

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                playerName = data.playerName;
                Debug.Log("Loaded current player's name from: " + filePath);
                return playerName;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading player name: " + e.Message);
            }
        }

        return null;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SaveHighScores(string playerNameHigh, int score)
    {  

        // Add the new high score entry
        highScoreList.Add(new HighScoreEntry(playerNameHigh, score));

        // Sort the high score list in descending order
        highScoreList.Sort((a, b) => b.score.CompareTo(a.score));

        // Ensure only the top 7 entries are kept
        if (highScoreList.Count > maxHighScores)
            highScoreList = highScoreList.GetRange(0, maxHighScores);

        // Save the high scores to file
        SaveHighScoresToFile();
    }  

    private void SaveHighScoresToFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, PlayerDataFilePath);

        PlayerData data = new PlayerData();
        data.playerName = playerName;
        data.highScores = highScoreList; // Assign the high scores list

        string json = JsonUtility.ToJson(data);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Saved high scores to: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving high scores: " + e.Message);
        }
    }

    private void LoadHighScores()
    {
        string filePath = Path.Combine(Application.persistentDataPath, PlayerDataFilePath);

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                playerName = data.playerName;
                highScoreList = data.highScores;
                Debug.Log("Loaded high scores from: " + filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading high scores: " + e.Message);
            }
        }
    }

    public string LoadTopHighScoreAsString()
    {
        // Ensure the high scores list is sorted before fetching the top score
        highScoreList.Sort((a, b) => b.score.CompareTo(a.score)); // Sort descending

        if (highScoreList.Count > 0)
        {
            HighScoreEntry topScore = highScoreList[0];
            return $"High Score: {topScore.playerNameHigh} with {topScore.score} points";
        }
        else
        {
            return "No high scores available";
        }
    }

    public string LoadHighScoresAsString()
    {
        // Ensure the high scores list is sorted before fetching the top scores
        highScoreList.Sort((a, b) => b.score.CompareTo(a.score)); // Sort descending

        if (highScoreList.Count > 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.AppendLine("High Scores:");

            foreach (HighScoreEntry entry in highScoreList)
            {
                stringBuilder.AppendLine($"{entry.playerNameHigh} with {entry.score} points");
            }

            return stringBuilder.ToString();
        }
        else
        {
            return "No high scores available";
        }
    }

    public int GetCurrentHighScore()
    {
        int currentHighScore = 0;

        foreach (HighScoreEntry entry in highScoreList)
        {
            if (entry.score > currentHighScore)
            {
                currentHighScore = entry.score;
            }
        }

        return currentHighScore;
    }

    public void ResetHighScores()
    {
        // Clear the high score list
        highScoreList.Clear();

        // Save the empty high scores list to file
        SaveHighScoresToFile();

        Debug.Log("High scores reset.");
    }

    [System.Serializable]
    private class PlayerData
    {
        public string playerName;
        public List<HighScoreEntry> highScores = new List<HighScoreEntry>();
    }

    [System.Serializable]
    private class HighScoreEntry
    {
        public string playerNameHigh;
        public int score;

        public HighScoreEntry(string playerNameHigh, int score)
        {
            this.playerNameHigh = playerNameHigh;
            this.score = score;
        }
    }
}