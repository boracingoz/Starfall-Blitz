using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    int score = 0;
    public int timer = 10;

    public GameObject obstacle;
    public float maxPos;
    public float spawnRate;
    public Transform spawnPoint;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InvokeRepeating("Timer", 1f, 1f);
        GameStart();  
    }

    public void ScoreUp()
    {
        score++;
        scoreText.text = "Score " + score.ToString();
    }

    public void Timer()
    {
        timer--;
        timerText.text = "Timer " + timer.ToString();

        if (timer == 0)
        {
            //ShowGameMenuPanel
        }
    }

    void SpawnObstacle()
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x = Random.Range(-maxPos, maxPos);

        Instantiate(obstacle, spawnPos, Quaternion.identity);
    }

    public void GameStart()
    {
        InvokeRepeating("SpawnObstacle", 1f, spawnRate);
    }

    public void GameOver()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
