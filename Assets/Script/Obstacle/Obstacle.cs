using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float initialFallSpeed = 2f;
    public float speedIncreaseAmount = 0.5f;

    private float currentFallSpeed;
    private bool isActive = false;

    private void OnEnable()
    {
        isActive = true;
        ResetObstacle();
    }

    private void OnDisable()
    {
        isActive = false;
    }

    void Update()
    {
        if (!isActive) return;

        transform.Translate(Vector3.down * currentFallSpeed * Time.deltaTime);

        if (transform.position.y <= -6f)
        {
            GameManager.instance.ObstacleFailed();
        }
    }

    private void OnMouseDown()
    {
        if (!isActive) return;

        GameManager.instance.ObstacleDestroyed();

        ReturnToPool();
    }

    public void ResetObstacle()
    {
        currentFallSpeed = initialFallSpeed;
        isActive = true;
    }

    public void IncreaseSpeed()
    {
        if (isActive)
        {
            currentFallSpeed += speedIncreaseAmount;
        }
    }

    void ReturnToPool()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ReturnObstacleToPool(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("ClickArea"))
        {
            GameManager.instance.ObstacleDestroyed();
            ReturnToPool();
        }
    }
}