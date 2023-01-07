using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpotLightLogic : MonoBehaviour
{
    CameraBehaviour _cameraBehaviour ;
    float spotLightDistanceAboveCharacter = 3f ; // seemingly, this is 3 unit above the character's feet
    float characterEyeHeight = 1.7f;
    private void Awake()
    {
        _cameraBehaviour = FindObjectOfType<CameraBehaviour>();

        Lab lab = FindObjectOfType<Lab>();
        Light li = this.GetComponentInParent<Light>();
        li.intensity = 2f;
        // the agent height is 1.5
        float visibilityRange = Mathf.Max(2,lab.config.view_distance) ;
        // well eye-height is set to 1.7. the character itself is 1.5, and the camera is
        // 2-unit above the character.
        // So, so the sight radius will be measure from eye-height, which is (2 + 1.5) - 1.7 = 2 - 0.2
        // distance from the spot light:
        float spotAngle = 2f* Mathf.Rad2Deg * Mathf.Atan2(visibilityRange, spotLightDistanceAboveCharacter - characterEyeHeight) ;
        li.range = Mathf.Max(
            spotLightDistanceAboveCharacter + 0.5f,
            2f*visibilityRange) ;
        li.spotAngle = spotAngle;
        //Debug.Log(">>> visibility range = " + visibilityRange);
        //Debug.Log(">>> spot-angle = " + li.spotAngle);
    }

    // Update is called once per frame
    void Update()
    {
        if (_cameraBehaviour.attachedCharacter == null) return;
        Transform agentToFollow = _cameraBehaviour.attachedCharacter.gameObject.transform;
        Vector3 newPosition = new Vector3(
            agentToFollow.position.x,
            agentToFollow.position.y + spotLightDistanceAboveCharacter,
            agentToFollow.position.z);
        this.transform.SetPositionAndRotation(newPosition, new Quaternion(0, 0, 0, 0));
        this.transform.LookAt(_cameraBehaviour.attachedCharacter.gameObject.transform);
        //Debug.Log(">> pos spotlight = " + this.transform.position);
        
    }
}
