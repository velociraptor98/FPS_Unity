using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerHealth instance;
    [SerializeField] private int maxHealth = 100,currentHealth;
    private float invincibleTimer = 0.5f;
    private float invincibleCounter;
    void Start()
    {
        currentHealth = maxHealth;
        instance = this;
        UIController.instance.slider.maxValue = maxHealth;
        UIController.instance.slider.value = currentHealth;
        UIController.instance.playerHealth.text = "Health: " + currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(invincibleCounter > 0)
        {
            invincibleCounter -= Time.deltaTime;
        }
    }
    public void Damage(int hitPoint)
    {
        if (invincibleCounter <= 0)
        {
            currentHealth -= hitPoint;
            if (currentHealth <= 0)
            {
                //Destroy(this.gameObject);
                gameObject.SetActive(false);
                currentHealth = 0;
                GameManager.instance.OnPlayerDead();
            }
            UIController.instance.slider.value = currentHealth;
            UIController.instance.playerHealth.text = "Health: " + currentHealth;
            invincibleCounter = invincibleTimer;
        }
    }
}
