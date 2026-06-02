using UnityEngine;

[DefaultExecutionOrder(-50)]
public class PlayerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        if (PlayerController.instance == null) return;
        var cc = PlayerController.instance.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        PlayerController.instance.transform.position = transform.position;
        if (cc != null) cc.enabled = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, 0.4f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
    }
#endif
}
