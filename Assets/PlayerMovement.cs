using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] private ParticleSystem boostParticles;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private BoxCollider2D boxHitbox;
    [SerializeField] private Transform arrow;
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI scoreText;
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
    public bool dead = false;
    public int levelScore = 0;
    static int totalScore = 0;
    public int levelMaxScore = 0;
    static int totalMaxScore = 0;
    private Box heldBox;
    private bool obstructed;

    private void Start()
    {
        Bottle[] bottles = FindObjectsOfType<Bottle>();
        levelMaxScore = bottles.Length;
        totalMaxScore += levelMaxScore;
    }
    public void BoxClicked(Box source)
    {
        if (heldBox == null)
        {
            if (obstructed == false) {
            throwForce = -1;
            heldBox = source;
            heldBox.rb.simulated = false;
            boxHitbox.isTrigger = false;
            heldBox.transform.position = transform.position + Vector3.up * holdHeight;
            heldBox.transform.rotation = Quaternion.identity;
            heldBox.transform.parent = transform;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            totalScore += levelScore;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        obstructed = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        obstructed = false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("tutorial")) 
        {
            totalScore = 0;
            totalMaxScore = 0;
        }
        instance = this;
        boxHitbox.isTrigger = true;
    }

    private void Update()
    {
        if (!dead)
        {
            scoreText.text = levelScore.ToString() + "/" + levelMaxScore.ToString();
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

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);

            if (Input.GetButtonUp("Fire1"))
            {
                arrow.gameObject.SetActive(false);
                if (throwForce > 0.05)
                {
                    heldBox.transform.parent = null;
                    boxHitbox.isTrigger = true;
                    heldBox.rb.simulated = true;
                    heldBox.rb.velocity = Vector3.Normalize(mousePos - heldBox.transform.position) * (throwForce * strength + 2);
                    heldBox = null;
                }
            }
            if (Input.GetButton("Fire1"))
            {
                if (throwForce != -1)
                {
                    if (heldBox != null)
                    {
                        arrow.gameObject.SetActive(true);
                        bar.fillAmount = throwForce / maxThrowForce;

                        Vector3 diff = mousePos - heldBox.transform.position;
                        diff.Normalize();
                        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                        arrow.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
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
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Respawn") && collision.otherCollider != boxHitbox)
        {
            Die();
        }
    }

    public void Die(float time = 2.5f)
    {
        deathParticles.Play();
        rb.constraints = RigidbodyConstraints2D.None;
        dead = true;
        Invoke(nameof(Respawn),time);

        if (heldBox != null)
        {
            arrow.gameObject.SetActive(false);
            heldBox.transform.parent = null;
            boxHitbox.isTrigger = true;
            heldBox.rb.simulated = true;
            heldBox = null;
        }
    }

    void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead)
        {
            if (transform.position.y < -6) Die(1);

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
}
