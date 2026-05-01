using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 7f;

    [Header("Ataque")]
    public GameObject punchHitbox;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 120;
    public int currentHealth;
    public Image healthBarFill;

    private Rigidbody2D rb;
    private Animator anim;
    private PunchHitbox punchScript;

    private bool isGrounded;
    private bool isPunching;
    private bool isHurt;
    private bool isDead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 🔥 asegura consistencia total (evita valores viejos del Inspector)
        currentHealth = maxHealth;

        Debug.Log($"[Player] Vida inicial: {currentHealth} / {maxHealth}");
    }

    void Start()
    {
        SetupPunch();
        UpdateHealthBar();
    }

    void SetupPunch()
    {
        if (punchHitbox == null) return;

        punchHitbox.transform.SetParent(transform);
        punchHitbox.transform.localPosition = Vector3.zero;
        punchHitbox.SetActive(false);

        punchScript = punchHitbox.GetComponent<PunchHitbox>();
    }

    void Update()
    {
        if (isHurt || isDead) return;

        CheckGround();
        HandleJump();
        HandlePunchInput();
    }

    void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }

    void HandlePunchInput()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isPunching)
        {
            if (anim != null)
                anim.SetTrigger("Attack");

            StartCoroutine(Punch());
        }
    }

    void FixedUpdate()
    {
        if (isHurt || isPunching || isDead)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float move = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDead)
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

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"💔 Vida actual: {currentHealth} / {maxHealth}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        if (!isHurt)
            StartCoroutine(HurtRoutine());
    }

    IEnumerator HurtRoutine()
    {
        isHurt = true;

        if (anim != null)
            anim.SetTrigger("Hurt");

        rb.linearVelocity = new Vector2(-transform.localScale.x * 3f, rb.linearVelocity.y);

        yield return new WaitForSeconds(0.3f);

        isHurt = false;
    }

    void UpdateHealthBar()
    {
        if (healthBarFill == null) return;

        float value = (float)currentHealth / maxHealth;
        value = Mathf.Clamp01(value);

        healthBarFill.fillAmount = value;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        rb.linearVelocity = Vector2.zero;

        if (anim != null)
            anim.SetTrigger("Death");

        Invoke(nameof(RestartLevel), 2f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}