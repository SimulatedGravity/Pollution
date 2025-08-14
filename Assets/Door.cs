using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public bool startOpen = false;

    private void Start()
    {
        if (startOpen)
        {
            animator.SetBool("Open", true);
        }

    }

    public void Open()
    {
        animator.SetBool("Open", true);
    }
    public void Close()
    {
        animator.SetBool("Open", false);
    }
}
