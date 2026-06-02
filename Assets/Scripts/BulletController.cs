using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float speed = 18.0f;
    [SerializeField] private float lifeTime;
    [SerializeField] private int damageValue;
    [SerializeField] private GameObject impact;
    [SerializeField] private bool canDamageEnemy;
    [SerializeField] private bool canDamagePlayer;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && canDamageEnemy)
        {
            other.GetComponent<EnemyMovement>().Damage(damageValue);
        }
        else if (other.CompareTag("HeadShot") && canDamageEnemy)
        {
            other.transform.parent.GetComponent<EnemyMovement>().Damage(damageValue * 2);
        }
        else if (other.CompareTag("Player") && canDamagePlayer)
        {
            PlayerHealth.instance.Damage(10);
        }

        Instantiate(impact, transform.position + transform.forward * -(speed * Time.deltaTime), transform.rotation);
        Destroy(gameObject);
    }
}
