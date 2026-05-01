using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;

    public GameObject punchHitbox;

    // 🔥 Ground check nuevo (REEMPLAZA OnCollision)
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isPunching;
    private bool isHurt;

    private PunchHitbox punchScript;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (punchHitbox != null)
        {
            punchHitbox.transform.SetParent(transform);
            punchHitbox.transform.localPosition = Vector3.zero;
            punchHitbox.SetActive(false);

            punchScript = punchHitbox.GetComponent<PunchHitbox>();
        }
    }

    void Update()
    {
        if (isHurt) return;

        // 🔥 reemplazo de OnCollision
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        HandleJump();

        if (Input.GetKeyDown(KeyCode.J) && !isPunching)
        {
            anim.SetTrigger("Attack");
            StartCoroutine(Punch());
        }
    }

    void FixedUpdate()
    {
        if (isHurt || isPunching)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float move = Input.GetAxisRaw("Horizontal");

        Vector2 velocity = rb.linearVelocity;
        velocity.x = move * speed;
        rb.linearVelocity = velocity;

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isHurt)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    IEnumerator Punch()
    {
        isPunching = true;

        float dir = transform.localScale.x;
        punchHitbox.transform.localPosition = new Vector3(1.2f * dir, 0f, 0f);

        if (punchScript != null)
            punchScript.Show();

        punchHitbox.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        if (punchScript != null)
            punchScript.Hide();

        punchHitbox.SetActive(false);

        isPunching = false;
    }

    public void TakeDamage()
    {
        if (isHurt) return;

        StartCoroutine(HurtRoutine());
    }

    IEnumerator HurtRoutine()
    {
        isHurt = true;

        anim.SetTrigger("Hurt");

        rb.linearVelocity = new Vector2(-transform.localScale.x * 3f, rb.linearVelocity.y);

        yield return new WaitForSeconds(0.3f);

        isHurt = false;
    }
}