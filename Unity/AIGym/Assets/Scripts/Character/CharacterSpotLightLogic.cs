using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpotLightLogic : MonoBehaviour
{
    CameraBehaviour _cameraBehaviour ;
    float spotLightDistanceAboveCharacterHead = 2 ;

    private void Awake()
    {
        _cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        Light li = this.GetComponentInParent<Light>();
        li.intensity = 3;
        // the agent height is 1.5
        float visibilityRange = (4f + 1) ;
        float spotAngle = 2f* Mathf.Rad2Deg * Mathf.Atan2(visibilityRange, spotLightDistanceAboveCharacterHead - 0.2f) ;
        li.range = (2* visibilityRange) + 1 ;
        li.spotAngle = spotAngle;
        Debug.Log(">>> spot-angle = " + li.spotAngle);
    }

    // Update is called once per frame
    void Update()
    {
        if (_cameraBehaviour.attachedCharacter == null) return;
        Transform agentToFollow = _cameraBehaviour.attachedCharacter.gameObject.transform;
        Vector3 newPosition = new Vector3(
            agentToFollow.position.x,
            agentToFollow.position.y + spotLightDistanceAboveCharacterHead ,
            agentToFollow.position.z);
        this.transform.SetPositionAndRotation(newPosition, new Quaternion(0, 0, 0, 0));
        this.transform.LookAt(_cameraBehaviour.attachedCharacter.gameObject.transform);
        //Debug.Log(">> pos spotlight = " + this.transform.position);
        
    }
}
