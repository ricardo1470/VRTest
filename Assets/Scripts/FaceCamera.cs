using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (_mainCamera != null)
        {
            transform.LookAt(_mainCamera.transform);
            transform.rotation = Quaternion.LookRotation(_mainCamera.transform.forward);
        }
    }
}
