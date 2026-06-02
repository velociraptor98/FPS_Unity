using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private float gameRestartTimer = 3.0f;
    [SerializeField] private InputActionAsset inputActions;

    private InputAction _pauseAction;

    private void Awake()
    {
        instance     = this;
        _pauseAction = inputActions.FindActionMap("UI", throwIfNotFound: true)
                                   .FindAction("Pause", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        if (_pauseAction == null) return;
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        if (_pauseAction == null) return;
        _pauseAction.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (_pauseAction.WasPressedThisFrame())
            PauseUnpause();
    }

    public void OnPlayerDead()
    {
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(gameRestartTimer);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseUnpause()
    {
        if (UIController.instance.PauseScreen.activeInHierarchy)
        {
            UIController.instance.PauseScreen.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale   = 1f;
        }
        else
        {
            UIController.instance.PauseScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale   = 0f;
        }
    }
}
