using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager instance;
    [SerializeField]private float GameRestartTimer = 3.0f;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void OnPlayerDead()
    {
        StartCoroutine(Restart());
    }
    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(GameRestartTimer);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
