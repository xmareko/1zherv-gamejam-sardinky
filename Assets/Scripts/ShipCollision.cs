using UnityEngine;

public class ShipCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit an Island (assuming you tag islands as "Island")
        if (collision.gameObject.CompareTag("Island"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver("The ship crashed into the rocks!");
            }
        }
    }
}