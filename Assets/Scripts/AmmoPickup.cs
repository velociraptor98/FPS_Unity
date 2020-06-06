using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isCollected = false;
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player") && isCollected == false)
        {
            PlayerController.instance.GetActiveGun().GetAmmo();
            Destroy(this.gameObject);
            isCollected = true;
        }
    }
}
