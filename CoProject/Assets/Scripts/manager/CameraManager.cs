using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed;

    private Camera cam;

    private void Start() 
    {
        cam = GetComponent<Camera>();    
    }

    private void LateUpdate() 
    {
        if(SequenceManager.isPlayingTimeline) return;
        // var height = cam.orthographicSize;
        // var width = height * Screen.width / Screen.height;

        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
