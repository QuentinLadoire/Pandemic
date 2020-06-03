using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform m_characterCardTarget = null;
    public Transform characterCardTarget
    { 
        get => m_characterCardTarget; 
    }

    Camera m_camera = null;

    public Ray MousePointToRay()
    {
        return m_camera.ScreenPointToRay(Input.mousePosition);
    }

    private void Awake()
    {
        GameStaticRef.cameraController = this;

        m_camera = GetComponent<Camera>();
    }
}
