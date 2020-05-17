using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    [SerializeField] public Slider slider;
    [SerializeField] public Text playerHealth;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
