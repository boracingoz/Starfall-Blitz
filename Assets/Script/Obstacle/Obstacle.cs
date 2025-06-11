using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -6f)
        {
            GameManager.instance.GameOver();
        }
    }

    private void OnMouseDown()
    {
        GameManager.instance.ScoreUp();

        Destroy(gameObject);
    }
}
