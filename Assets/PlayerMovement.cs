using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] private ParticleSystem boostParticles;
    [SerializeField] private BoxCollider2D boxHitbox;
    [SerializeField] private float acceleration;
    [SerializeField] private float brakeForce;
    [SerializeField] private float jumpForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float holdHeight;
    public float grounded;
    private bool doubleJump = true;
    [NonSerialized] public static PlayerMovement instance;
    private float throwForce;
    public float maxThrowForce;
    public float strength;

    private Box heldBox;

    public void BoxClicked(Box source)
    {
        if (heldBox == null)
        {
            throwForce = -1;
            heldBox = source;
            heldBox.rb.simulated = false;
            boxHitbox.enabled = true;
            heldBox.transform.position = transform.position + Vector3.up * holdHeight;
            heldBox.transform.rotation = Quaternion.identity;
            heldBox.transform.parent = transform;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        boxHitbox.enabled = false;
    }

    private void Update()
    {
        grounded -= Time.deltaTime;
        if (Input.GetButtonDown("Jump") && grounded < 0 && doubleJump)
        {
            boostParticles.Stop();
            boostParticles.Play();
            doubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            transform.Translate(Vector2.up * 0.01f);
        }
        if (grounded > 0)
        {
            doubleJump = true;
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                transform.Translate(Vector2.up * 0.01f);
                grounded = 0;
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (throwForce > 0.05)
            {
                heldBox.transform.parent = null;
                boxHitbox.enabled = false;
                heldBox.rb.simulated = true;
                heldBox.rb.velocity = Vector3.Normalize(Camera.main.ScreenToWorldPoint(Input.mousePosition) - heldBox.transform.position)*(throwForce*strength+strength);
                heldBox = null;
            }
        }
        if (Input.GetButton("Fire1"))
        {
            if (throwForce != -1)
            {
                if (heldBox != null)
                {
                    if (throwForce < maxThrowForce)
                    {
                        throwForce += Time.deltaTime;
                    }
                }
                else
                {
                    throwForce = 0;
                }
            }
        }
        else
        {
            throwForce = 0;
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
