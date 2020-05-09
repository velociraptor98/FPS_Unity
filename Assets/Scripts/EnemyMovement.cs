using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private int health = 5;
    // Update is called once per frame
    public void Damage(int hitPoint = 1)
    {
        this.health -= hitPoint;
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    void Update()
    {
       
    }
}
