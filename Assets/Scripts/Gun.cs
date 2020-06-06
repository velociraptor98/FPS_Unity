using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject bullet;
    public bool canAutoFire;
    public float fireRate;
    [HideInInspector]
    public float fireCounter;
    public int currentAmmo,pickupAmount;
    [SerializeField] private float ZoonAmount;
    public Transform FirePoint;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
        }
    }
    public GameObject GetBullet()
    {
        return this.bullet;
    }
    public void GetAmmo()
    {
        currentAmmo = pickupAmount;
        UIController.instance.ammoText.text = "Ammo: " + currentAmmo;
    }
    public float GetZoomAmount()
    {
        return this.ZoonAmount;
    }
}
