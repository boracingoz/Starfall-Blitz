using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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

    private int totalScore = 0;
    private float gameTimer;
    private float currentSpawnRate;
    private Queue<GameObject> obstaclePool = new Queue<GameObject>();
    private List<GameObject> activeObstacles = new List<GameObject>();
    private bool gameActive = false;

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
    }

    public void ObstacleFailed()
    {
        GameOver();
    }

    void StartGame()
    {
        gameActive = true;

        for (int i = 0; i < 3; i++)
        {
            SpawnObstacle();
        }

        StartCoroutine(GameTimer());
        StartCoroutine(SpeedIncrease());

        InvokeRepeating("SpawnObstacle", currentSpawnRate, currentSpawnRate);
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
        if (!gameActive) return;

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
        StopAllCoroutines();
        CancelInvoke();
        ClearAllObstacles();

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
        StopAllCoroutines();
        CancelInvoke();
        ClearAllObstacles();

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}