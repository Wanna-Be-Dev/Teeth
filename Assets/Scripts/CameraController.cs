using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraController : MonoBehaviour
{
    public Camera currentCamera;

    private Vector3 cameraPos;
    private Vector3 cameraRot;

    private Vector3 defaultCameraPos;
    private Vector3 defaultCameraRot;

    private Vector3 velocity = Vector3.zero;

    [SerializeField]
    [Header("Camera Settings")]
    [Space]
    public float cameraSpeed;
    [Space]
    public float mouseSpeed;
    //Camera Transform Borders
    #if UNITY_EDITOR
    [MinMax(-0.5f,0.5f), Header("BoundsX", order = 1)]
    #endif
    public Vector2 Xpos = new Vector2(-0.1f,0.19f);
    #if UNITY_EDITOR
    [MinMax(-0.5f, 0.5f), Header("BoundsY", order = 1)]
    #endif
    public Vector2 Ypos = new Vector2(-0.05f, 0.15f);

    //Camera Rotate Borders
    #if UNITY_EDITOR
    [MinMax(-50f, 50f), Header("RotationX", order = 1)]
    #endif
    public Vector2 Xrot = new Vector2(-10f, 20f);
    public Vector2 Yrot=  new Vector2(150, 220);

    [Space]
    public float smoothTime = 0.3F;

    void Start()
    {
        currentCamera = Camera.main;
        defaultCameraPos = cameraPos = currentCamera.transform.position;
        defaultCameraRot = cameraRot = currentCamera.transform.eulerAngles;
    }
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.W))
            {
                cameraPos.y += cameraSpeed / 100;
            }
            if (Input.GetKey(KeyCode.S))
            {
                cameraPos.y -= cameraSpeed / 100;
            }
            if (Input.GetKey(KeyCode.A))
            {
                cameraPos.x += cameraSpeed / 100;
            }
            if (Input.GetKey(KeyCode.D))
            {
                cameraPos.x -= cameraSpeed / 100;
            }

            cameraRot += mouseSpeed * new Vector3(Input.GetAxisRaw("Mouse Y"), -1 * Input.GetAxisRaw("Mouse X"), 0); 
        }

        cameraPos.x = Mathf.Clamp(cameraPos.x, Xpos.x, Xpos.y);
        cameraPos.y = Mathf.Clamp(cameraPos.y, Ypos.x, Ypos.y);

        cameraRot.x = Mathf.Clamp(cameraRot.x, Xrot.x, Xrot.y);
        cameraRot.y = ClampAngle(cameraRot.y, 150, 220);

        if (Input.GetKey(KeyCode.Space))
        {
            cameraPos = defaultCameraPos;
            cameraRot = defaultCameraRot;
        }
          
        currentCamera.transform.position = Vector3.SmoothDamp(currentCamera.transform.position, cameraPos, ref velocity, smoothTime);
        currentCamera.transform.eulerAngles = cameraRot;
    }

    //Camera fix
    public static float ClampAngle(float current, float min, float max)
    {
        float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
        float hdtAngle = dtAngle * 0.5f;
        float midAngle = min + hdtAngle;

        float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
        if (offset > 0)
            current = Mathf.MoveTowardsAngle(current, midAngle, offset);
        return current;
    }

}
