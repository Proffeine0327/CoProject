using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Rect cameraClamp;
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed;

    private Camera cam;

    private void Start() 
    {
        cam = GetComponent<Camera>();    
    }

    private void Update() 
    {
        var height = cam.orthographicSize;
        var width = height * Screen.width / Screen.height;

        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);

        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, cameraClamp.x + width, cameraClamp.x + cameraClamp.width - width),
            Mathf.Clamp(transform.position.y, cameraClamp.y + height, cameraClamp.y + cameraClamp.height - height),
            -10
        );
    }
}
