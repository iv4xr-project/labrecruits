using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void OnEnable() { Camera.onPreRender += PreRender; }
    void OnDisable() { Camera.onPreRender -= PreRender; }

    void PreRender(Camera cam)
    {
        if ((cam.cullingMask & (1 << gameObject.layer)) != 0)
        {
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }
}
