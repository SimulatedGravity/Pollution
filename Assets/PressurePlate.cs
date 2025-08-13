using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Door door;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Button")) {
            if (collision.collider.gameObject.layer != 3)
            {
                Vector2 oldV = collision.collider.attachedRigidbody.velocity;
                float newX = oldV.x;
                if (Mathf.Abs(newX) > 2)
                {
                    newX /= 2;
                }
                collision.collider.attachedRigidbody.velocity = new Vector2(newX, oldV.y);
            }
            door.Open();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Button")) door.Close();
    }
}
