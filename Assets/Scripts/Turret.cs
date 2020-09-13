using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bullet;
    public float rangeToPlayer, timeBetweenShots = 0.5f;
    private float shotCounter;
    public Transform gun, firePoint;
    private float rotateSpeed = 2f;
    void Start()
    {
        shotCounter = timeBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position,PlayerController.instance.transform.position)< rangeToPlayer)
        {
            gun.LookAt(PlayerController.instance.transform.position + new Vector3(0f, 1.2f, 0f));
            shotCounter -= Time.deltaTime;
            if(shotCounter<=0)
            {
                Instantiate(bullet, firePoint.position, firePoint.rotation);
                shotCounter = timeBetweenShots;
            }
            else
            {
                shotCounter -= Time.deltaTime;
                gun.rotation = Quaternion.Lerp(gun.rotation, Quaternion.Euler(0f, gun.rotation.eulerAngles.y + 10f, 0f),rotateSpeed*Time.deltaTime);
            }
        }
    }
}
