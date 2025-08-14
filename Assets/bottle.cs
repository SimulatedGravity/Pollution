using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    [SerializeField] Collider2D hitbox;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem particles;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.instance.levelScore++;
            hitbox.enabled = false;
            animator.SetTrigger("grab");
            particles.Play();
        }
    }
}
