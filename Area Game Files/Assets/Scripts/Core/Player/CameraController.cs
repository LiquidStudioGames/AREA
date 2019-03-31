using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool invert = false;
    public float mouseSensitivity = 8f;
    public GameObject cam;

    private Transform root;
    private Transform head;
    private NetworkTag networkTag;

    public float jaw;
    public float pitch;

    void Start()
    {
        root = transform.parent;
        networkTag = GetComponent<NetworkTag>(); 

        if (!networkTag.IsMine)
        {
            Vector3 position = cam.transform.position;
            Destroy(cam);
            cam = new GameObject("Head");
            cam.transform.position = position;
            cam.transform.SetParent(root, true);
        }

        head = cam.transform;
    }

    void LateUpdate()
    {
        if (networkTag.IsMine)
        {
            if (!invert)
            {
                jaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            }

            else
            {
                jaw -= Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch += Input.GetAxis("Mouse Y") * mouseSensitivity;
            }

            jaw = jaw > 180f ? jaw - 360f : jaw < -180f ? jaw + 360f : jaw;
            pitch = Mathf.Clamp(pitch, -85f, 85f);
        }

        root.rotation = Quaternion.Euler(new Vector3(0f, jaw, 0f));
        head.rotation = Quaternion.Euler(new Vector3(pitch, jaw, 0f));
    }
}