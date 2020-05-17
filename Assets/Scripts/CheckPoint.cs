using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private string cpname;
    
    void Start()
    {
        if (PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "_cp"))
        {
            if(PlayerPrefs.GetString(SceneManager.GetActiveScene().name + "_cp") == cpname)
            {
                print("Correct0");
                PlayerController.instance.transform.position = this.transform.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            
            PlayerPrefs.SetString(SceneManager.GetActiveScene().name + "_cp",cpname);
            print(PlayerPrefs.GetString(SceneManager.GetActiveScene().name + "_cp"));
        }
    }
}
