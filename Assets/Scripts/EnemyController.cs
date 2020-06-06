using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float distanceToChase = 10.0f;
    [SerializeField] private bool isChasing = false;
    [SerializeField] private float distanceToLose = 15.0f;
    [SerializeField] private float distanceToStop = 2.0f;
    private Vector3 startPoint;
    private NavMeshAgent agent;
    [SerializeField] private float keepChaseTime,chaseCounter;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate;
    [SerializeField] private Animator anim;
    private float fireCount;
    private Vector3 targetPoint;
    [SerializeField] private float waitBetweenShots=1.0f, shotWaitCounter,timeToShoot = 0.5f,shootTimeCounter; 
    void Start()
    {
        keepChaseTime = 5.0f;
        startPoint = transform.position;
        agent = this.GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        shotWaitCounter = timeToShoot;
        shotWaitCounter = waitBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;
        if (!isChasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) <= distanceToChase)
            {
                isChasing = true;
                shotWaitCounter = timeToShoot;
                shotWaitCounter = waitBetweenShots;
            }
            if (chaseCounter > 0)
            {
                chaseCounter -= Time.deltaTime;
                if (chaseCounter <= 0)
                {
                    agent.destination = startPoint;
                }
            }
            if (agent.remainingDistance <= 0.25f)
            {
                anim.SetBool("isMoving", false);
            }
            else
            {
                anim.SetBool("isMoving", true);
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPoint) > distanceToStop)
            {
                agent.destination = targetPoint;
            }
            else
            {
                agent.destination = transform.position;
            }
            if(Vector3.Distance(transform.position, targetPoint) >=distanceToLose)
            {
                isChasing = false;
                chaseCounter = keepChaseTime; 
            }
            if (shotWaitCounter >= 0)
            {
                shotWaitCounter -= Time.deltaTime;
                if (shotWaitCounter <= 0)
                {
                    shootTimeCounter = timeToShoot;
                }
                anim.SetBool("isMoving", true);
            }
            else
            {
                if (PlayerController.instance.gameObject.activeInHierarchy)
                {
                    shootTimeCounter -= Time.deltaTime;
                    if (shootTimeCounter > 0)
                    {
                        fireCount -= Time.deltaTime;
                        if (fireCount <= 0)
                        {
                            fireCount = fireRate;
                            firePoint.LookAt(PlayerController.instance.transform.position + new Vector3(0.0f, 1.2f, 0.0f));
                            Vector3 targetDirection = PlayerController.instance.transform.position - transform.position;
                            float angle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);
                            if (Mathf.Abs(angle) < 30.0f)
                            {
                                anim.SetTrigger("FireShot");
                                Instantiate(bullet, firePoint.position, firePoint.rotation);
                            }
                            else
                            {
                                shotWaitCounter = waitBetweenShots;
                            }

                        }
                        agent.destination = transform.position;
                    }
                    else
                    {
                        shotWaitCounter = waitBetweenShots;
                    }
                    anim.SetBool("isMoving", false);
                }
            }
        }
    }
}
