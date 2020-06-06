using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public static CameraController instance;
    [SerializeField] private Transform attachPoint;
    private float startFOV;
    private float targetFOV;
    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private Camera camera;
    void Start()
    {
        instance = this;
        startFOV = camera.fieldOfView;
        targetFOV = camera.fieldOfView;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = attachPoint.transform.position;
        transform.rotation = attachPoint.rotation;
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }
    public void ZoomIn(float zoomAmount)
    {
        targetFOV = zoomAmount;
    }
    public void ZoomOut()
    {
        targetFOV = startFOV;
    }
}
