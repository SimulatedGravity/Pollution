using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWheel : MonoBehaviour
{
    private PlayerMovement player;
    public float cyoteTime;
    private void Start()
    {
        player = PlayerMovement.instance;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        player.grounded = cyoteTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Respawn"))
        {
            player.Die();
        }
    }
}
