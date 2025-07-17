using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Door door;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Button")) door.Open();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Button")) door.Close();
    }
}
