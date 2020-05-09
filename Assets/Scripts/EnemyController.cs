using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine.AI;
using UnityEngine;

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
    private float fireCount;
    private Vector3 targetPoint;
    void Start()
    {
        keepChaseTime = 5.0f;
        startPoint = transform.position;
        agent = this.GetComponent<NavMeshAgent>();
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
                fireCount = 1f;
            }
            if (chaseCounter > 0)
            {
                chaseCounter -= Time.deltaTime;
                if (chaseCounter <= 0)
                {
                    agent.destination = startPoint;
                }
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
            fireCount -= Time.deltaTime;
            if(fireCount<=0)
            {
                fireCount = fireRate;
                Instantiate(bullet, firePoint.position, firePoint.rotation);
            }
        }
    }
}
