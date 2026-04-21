using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;

    public GameObject punchHitbox;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isPunching;

    private PunchHitbox punchScript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
        HandleJump();
        HandlePunch();
    }

    void FixedUpdate()
    {
        float move = Input.GetAxisRaw("Horizontal");

        Vector2 velocity = rb.linearVelocity;
        velocity.x = move * speed;
        rb.linearVelocity = velocity;

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void HandlePunch()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isPunching)
        {
            StartCoroutine(Punch());
        }
    }

    IEnumerator Punch()
    {
        isPunching = true;

        float dir = transform.localScale.x;

        punchHitbox.transform.localPosition = new Vector3(1.2f * dir, 0f, 0f);

        // 👇 mostrar visual
        if (punchScript != null)
            punchScript.Show();

        punchHitbox.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        // 👇 ocultar visual
        if (punchScript != null)
            punchScript.Hide();

        punchHitbox.SetActive(false);

        isPunching = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}