using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    public float gameDuration = 10f;

    [Header("Spawn Settings")]
    public GameObject obstaclePrefab;
    public Transform spawnPoint;
    public float maxPos;
    public float initialSpawnRate = 2f;
    public float speedIncreaseInterval = 5f;
    public float speedMultiplier = 1.2f;

    [Header("Object Pool")]
    public int poolSize = 3;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject pausePanel;
    public Button pauseButton; 

    private int totalScore = 0;
    private float gameTimer;
    private float currentSpawnRate;
    private Queue<GameObject> obstaclePool = new Queue<GameObject>();
    private List<GameObject> activeObstacles = new List<GameObject>();
    private bool gameActive = false;
    private bool gamePaused = false;

    private void Awake()
    {
        instance = this;
        InitializeObjectPool();
        currentSpawnRate = initialSpawnRate;
        gameTimer = gameDuration;
    }

    private void Start()
    {
        UpdateUI();
        StartGame();
        AudioManager.Instance.PlayRandomGameplayMusic();
    }

    void InitializeObjectPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab);
            obj.SetActive(false);
            obstaclePool.Enqueue(obj);
        }
    }

    public void ObstacleDestroyed()
    {
        totalScore += 5;
        UpdateUI();
        AudioManager.Instance.PlayMeteorDestroySFX();
    }

    public void ObstacleFailed()
    {
        GameOver();
    }

    void StartGame()
    {
        gameActive = true;
        gamePaused = false;

        for (int i = 0; i < 3; i++)
        {
            SpawnObstacle();
        }

        StartCoroutine(GameTimer());
        StartCoroutine(SpeedIncrease());

        InvokeRepeating("SpawnObstacle", currentSpawnRate, currentSpawnRate);
    }

    public void PauseGame()
    {
        if (!gameActive || gamePaused) return;

        gamePaused = true;
        Time.timeScale = 0f;
        AudioManager.Instance.PauseMusic();

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!gameActive || !gamePaused) return;

        AudioManager.Instance.PlayButtonClickSFX();
        gamePaused = false;
        Time.timeScale = 1f; 
        AudioManager.Instance.ResumeMusic(); 

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    IEnumerator GameTimer()
    {
        while (gameTimer > 0 && gameActive)
        {
            yield return new WaitForSeconds(1f);
            gameTimer--;
            UpdateUI();
        }

        if (gameActive)
        {
            GameWin();
        }
    }

    IEnumerator SpeedIncrease()
    {
        while (gameActive)
        {
            yield return new WaitForSeconds(speedIncreaseInterval);
            currentSpawnRate /= speedMultiplier;
            currentSpawnRate = Mathf.Max(currentSpawnRate, 0.5f);

            CancelInvoke("SpawnObstacle");
            InvokeRepeating("SpawnObstacle", 0f, currentSpawnRate);

            foreach (GameObject obs in activeObstacles)
            {
                if (obs != null && obs.activeInHierarchy)
                {
                    Obstacle obstacleScript = obs.GetComponent<Obstacle>();
                    if (obstacleScript != null)
                    {
                        obstacleScript.IncreaseSpeed();
                    }
                }
            }
        }
    }

    void SpawnObstacle()
    {
        if (!gameActive || gamePaused) return;

        if (activeObstacles.Count >= 3) return;

        if (obstaclePool.Count > 0)
        {
            GameObject obstacle = obstaclePool.Dequeue();

            Vector3 spawnPos = spawnPoint.position;
            spawnPos.x = Random.Range(-maxPos, maxPos);

            obstacle.transform.position = spawnPos;
            obstacle.SetActive(true);

            Obstacle obstacleScript = obstacle.GetComponent<Obstacle>();
            if (obstacleScript != null)
            {
                obstacleScript.ResetObstacle();
            }

            activeObstacles.Add(obstacle);
        }
    }

    public void ReturnObstacleToPool(GameObject obstacle)
    {
        obstacle.SetActive(false);
        activeObstacles.Remove(obstacle);
        obstaclePool.Enqueue(obstacle);
    }

    void GameWin()
    {
        gameActive = false;
        gamePaused = false;
        Time.timeScale = 1f;
        StopAllCoroutines();
        CancelInvoke();
        ClearAllObstacles();

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayWinSFX();

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        else
        {
            Invoke("MainMenu", 2f);
        }
    }

    public void GameOver()
    {
        gameActive = false;
        gamePaused = false;
        Time.timeScale = 1f;
        StopAllCoroutines();
        CancelInvoke();
        ClearAllObstacles();

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayGameOverSFX();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    void ClearAllObstacles()
    {
        foreach (GameObject obs in activeObstacles.ToArray())
        {
            if (obs != null)
            {
                ReturnObstacleToPool(obs);
            }
        }
        activeObstacles.Clear();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + totalScore.ToString();

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(gameTimer).ToString();
    }

    public void RestartGame()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        // ESC tu?u ile pause
        if (Input.GetKeyDown(KeyCode.Escape) && gameActive)
        {
            if (gamePaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void OnPauseButtonPressed()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        PauseGame();
    }

    public void OnNextLevelPressed()
    {
        AudioManager.Instance.PlayButtonClickSFX();

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Level4")
        {
            SceneManager.LoadScene("End");
        }
        else
        {
            
            if (currentScene.StartsWith("Level"))
            {
                string levelNumberStr = currentScene.Substring(5); 
                if (int.TryParse(levelNumberStr, out int currentLevelNum))
                {
                    int nextLevelNum = currentLevelNum + 1;
                    string nextSceneName = "Level" + nextLevelNum;
                    SceneManager.LoadScene(nextSceneName);
                }
            }
        }
    }

    public void OnWinMainMenuPressed()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // GAME OVER PANEL BUTTONS
    public void OnTryAgainPressed()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnGameOverMainMenuPressed()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // PAUSE PANEL BUTTONS
    public void OnResumePressed()
    {
        ResumeGame();
    }

    public void OnPauseRestartPressed()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPauseMainMenuPressed()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // HELPER METHODS
    private string GetNextLevelName(string currentLevel)
    {
        // Level isimlerini burada tan?mlay?n
        switch (currentLevel)
        {
            case "Level1":
                return "Level2";
            case "Level2":
                return "Level3";
            case "Level3":
                return "Level4";
            case "Level4":
                return "Level5";
            // Daha fazla level ekleyebilirsiniz
            default:
                return null; // Son level veya bilinmeyen level
        }
    }

    public void MainMenu()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        Time.timeScale = 1f; // Time scale'i s?f?rla
        SceneManager.LoadScene("MainMenu");
    }
}