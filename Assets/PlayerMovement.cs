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

    public AudioClip jumpSound;
public AudioClip punchSound;
public AudioClip hurtSound;

private AudioSource audioSource;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 120;
    public int currentHealth;
    public Image healthBarFill;

    [Header("Interacción")]
    private bool canEnterCave = false;
    private float stayTimer = 0f;

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
audioSource = GetComponentInChildren<AudioSource>();

        currentHealth = maxHealth;

        Debug.Log($"[Player] Vida inicial: {currentHealth} / {maxHealth}");
    }

    void Start()
    {
        SetupPunch();
        UpdateHealthBar();

        if (PlayerSpawn.spawnPosition != Vector3.zero)
        {
            transform.position = PlayerSpawn.spawnPosition;
        }
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
        // buffer de interacción
        if (stayTimer > 0)
            stayTimer -= Time.deltaTime;
        else
            canEnterCave = false;

        if (isHurt || isDead) return;

        CheckGround();
        HandleJump();
        HandlePunchInput();
        HandleInteraction();
    }

    // =========================
    // 🧭 INTERACCIÓN CUEVA
    // =========================
    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("APRETE E");

            if (canEnterCave || stayTimer > 0)
            {
                Debug.Log("ENTRANDO A LA CUEVA");
                EnterCave();
            }
            else
            {
                Debug.Log("NO ESTOY EN LA CUEVA");
            }
        }
    }

    void EnterCave()
    {
        SceneManager.LoadScene("CuevaScene");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ENTRÉ EN TRIGGER: " + other.name);

        if (other.CompareTag("CaveEntrance"))
        {
            canEnterCave = true;
            stayTimer = 0.2f;

            Debug.Log("CaveEntrance detectado");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CaveEntrance"))
        {
        canEnterCave = false;
        Debug.Log("SALÍ DEL TRIGGER");
    }
    }

    // =========================
    // MOVIMIENTO
    // =========================

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

                 audioSource.PlayOneShot(punchSound);

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

    // =========================
    // VIDA
    // =========================

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

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
        healthBarFill.fillAmount = Mathf.Clamp01(value);
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