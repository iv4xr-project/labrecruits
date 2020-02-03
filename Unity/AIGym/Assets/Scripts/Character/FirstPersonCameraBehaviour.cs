/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FirstPersonCameraBehaviour : MonoBehaviour
{
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    
    public float minimumY = -60F;
    public float maximumY = 60F;

    private SkinnedMeshRenderer _renderer;
    private Camera _camera;
    private CameraBehaviour _cameraBehaviour;
    private float rotationY = 0F;
    
    // Start is called before the first frame update
    void Start()
    {
        _renderer = transform.parent.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_camera.enabled)
        {
            // While walking around in First Person we only want to
            // render the shadows of the character to avoid clipping
            _renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            var parent = transform.parent;
            float rotationX = parent.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
            parent.localEulerAngles = new Vector3(0, rotationX, 0);
            transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        }
        else
        {
            _renderer.shadowCastingMode = ShadowCastingMode.On;
        }
    }
}
