using UnityEngine;

public class ShipCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // End game when colliding with an island
        if (collision.gameObject.CompareTag("Island"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver("The ship crashed into the rocks!");
        }
    }
}
