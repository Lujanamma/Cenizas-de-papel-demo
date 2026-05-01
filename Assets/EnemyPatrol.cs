using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;

    public Transform leftLimit;
    public Transform rightLimit;

    public Transform player;
    public float attackDistance = 2f;

    public GameObject enemyPunchHitbox;

    private bool movingRight = true;
    private bool isAttacking;

    // 👇 NUEVO
    private bool canAttack;

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // apagar hitbox al iniciar
        if (enemyPunchHitbox != null)
            enemyPunchHitbox.SetActive(false);

        // esperar antes de permitir ataques
        StartCoroutine(EnableAttack());
    }

    IEnumerator EnableAttack()
    {
        yield return new WaitForSeconds(1f);

        canAttack = true;
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            // atacar si el jugador está cerca
            if (distance <= attackDistance && !isAttacking && canAttack)
            {
                StartCoroutine(AttackRoutine());
                return;
            }
        }

        // no caminar mientras ataca
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        Patrol();
    }

    void Patrol()
    {
        if (movingRight)
        {
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);

            // mirar derecha
            transform.localScale = new Vector3(1, 1, 1);

            if (transform.position.x >= rightLimit.position.x)
                movingRight = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);

            // mirar izquierda
            transform.localScale = new Vector3(-1, 1, 1);

            if (transform.position.x <= leftLimit.position.x)
                movingRight = true;
        }
    }

IEnumerator AttackRoutine()
{
    isAttacking = true;

    // detener movimiento
    rb.linearVelocity = Vector2.zero;

    // mirar al jugador
    if (player.position.x > transform.position.x)
        transform.localScale = new Vector3(1, 1, 1);
    else
        transform.localScale = new Vector3(-1, 1, 1);

    // animación
    anim.ResetTrigger("Attack");
    anim.SetTrigger("Attack");
    Debug.Log("ENEMY ATTACK");

    // esperar un poquito antes del golpe
    yield return new WaitForSeconds(0.3f);

    // activar hitbox
    if (enemyPunchHitbox != null)
        enemyPunchHitbox.SetActive(true);

    // duración del golpe
    yield return new WaitForSeconds(0.2f);

    // apagar hitbox
    if (enemyPunchHitbox != null)
        enemyPunchHitbox.SetActive(false);

    // cooldown TOTAL del ataque
    yield return new WaitForSeconds(1.2f);

    isAttacking = false;
}}