using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float acceleration;
    [SerializeField] private float brakeForce;
    [SerializeField] private float jumpForce;
    [SerializeField] private float maxSpeed;
    public float grounded;
    public static PlayerMovement instance;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        grounded -= Time.deltaTime;
        if (Input.GetButtonDown("Jump") && grounded > 0)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            transform.Translate(Vector2.up*0.01f);
            grounded = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float speedModifier = 1;
        float horizontal = Input.GetAxis("Horizontal");
        bool brake = Input.GetButton("Brake");
        if (!brake)
        {
            if ((horizontal > 0 && rb.velocity.x < 0) || (horizontal < 0 && rb.velocity.x > 0))
            {
                speedModifier = 3;
            }
            else if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            {
                speedModifier = 0;
            }
            if (!(grounded > 0))
            {
                speedModifier /= 2;
            }
            rb.AddForce(Vector2.right * horizontal * acceleration * speedModifier);
        }
        else
        {
            if (Mathf.Abs(rb.velocity.x) < 0.1)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                if (rb.velocity.x > 0)
                {
                    rb.AddForce(Vector2.left * brakeForce);
                }
                else if (rb.velocity.x < 0)
                {
                    rb.AddForce(Vector2.right * brakeForce);
                }
            }
        }
    }
}
