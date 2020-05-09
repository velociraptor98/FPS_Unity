using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed = 18.0f;
    [SerializeField] private float lifeTime;
    [SerializeField] private GameObject impact;
    [SerializeField] private bool canDamageEnemy,canDamagePlayer;
    private Rigidbody rg;
    void Start()
    {
        rg = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rg)
        {
            rg.velocity = transform.forward * speed;
        }
        lifeTime -= Time.deltaTime;
        if(lifeTime<=0)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy") && canDamageEnemy)
        {
            other.gameObject.GetComponent<EnemyMovement>().Damage();
        }
        if(other.gameObject.CompareTag("Player") && canDamagePlayer)
        {
            // TODO : ADD PLAYER HEALTH SYSTEM
            print("Player Hit");
        }
        Destroy(this.gameObject);
        Instantiate(impact, transform.position + (transform.forward * -(speed * Time.deltaTime)), transform.rotation);
    }
}
