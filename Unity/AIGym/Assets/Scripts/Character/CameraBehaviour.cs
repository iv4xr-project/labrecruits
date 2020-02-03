/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public int renderFloorHeight = 2;
    public int renderFloorSkip = 4;
    public Character attachedCharacter;
    private Camera _topDownCamera;
    private ThirdPersonCameraBehaviour _thirdPersonCameraBehaviour;
    public UIFeedback uiFeedback;
    private World _world;
    public int playerFloor = 0;
    public int cameraFloor = 3;

    void Start()
    {
        _topDownCamera = GameObject.FindWithTag("TopDownView").GetComponentInChildren<Camera>();
        _thirdPersonCameraBehaviour = GameObject.FindWithTag("TopDownView").GetComponent<ThirdPersonCameraBehaviour>();
        _world = GameObject.FindWithTag("World").GetComponent<World>();
    }

    /// <summary>
    /// Attached to given agent's character object
    /// </summary>
    public void AttachToCharacter(Character character)
    {
        attachedCharacter = character;
        
        foreach (Camera c in Camera.allCameras)
            c.enabled = false;
        _topDownCamera.enabled = true;

        _thirdPersonCameraBehaviour.FollowAgent(attachedCharacter.gameObject);

        // Set the attached agent as the active agent to show in the UI
        uiFeedback.ChangeAgent(attachedCharacter);

        playerFloor = attachedCharacter.GetFloor();
        cameraFloor = playerFloor + renderFloorHeight;
        
        DisableFloors();
    }

    /// <summary>
    /// Detaches from the agent
    /// </summary>
    public void Detach()
    {
        attachedCharacter = null;
        for (int i = 0; i < Camera.allCameras.Length; i++)
            Camera.allCameras[i].enabled = false;

        _topDownCamera.enabled = true;
        _thirdPersonCameraBehaviour.Unfollow();
        
    }

    /// <summary>
    /// Switches between First person and Third person Camera
    /// </summary>
    public void SwitchPov()
    {
        if (attachedCharacter == null)
            return;

        Camera firstPersonCamera = attachedCharacter.gameObject.GetComponentInChildren<Camera>();
        firstPersonCamera.enabled = !firstPersonCamera.enabled;
        _topDownCamera.enabled = !_topDownCamera.enabled;


        //turn off ceiling in third person, enable in first person
        if (firstPersonCamera.enabled)
            EnableAllFloors();
        else
            DisableFloors();
    }

    // Update is called once per frame
    void Update()
    {
        //enable a object with a camera in the tagsList to be clicked and switch the camera to this camera
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = FindClickedObject(Input.mousePosition);
            if (obj != null && obj.TryGetComponent<Character>(out Character c))
                AttachToCharacter(c);
        }

        if (Input.GetKeyDown(KeyCode.R))
            SwitchPov();

        if (attachedCharacter != null)
        {
            int currentFloor = attachedCharacter.GetFloor();
            if (currentFloor != playerFloor)
            {
                playerFloor = currentFloor;
                cameraFloor = playerFloor + renderFloorHeight;
                DisableFloors();
            }
        }
        else
        {
            int inputDirection = (Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKeyDown(KeyCode.DownArrow) ? 1 : 0);
            if (inputDirection != 0 && !_thirdPersonCameraBehaviour.CR_RUNNING && cameraFloor + renderFloorSkip * inputDirection > 0 )
            {
                cameraFloor += renderFloorSkip * inputDirection;
                _thirdPersonCameraBehaviour.TranslateAnimation(new Vector3(0, renderFloorSkip * inputDirection, 0), 0.3f);
                DisableFloors();
            }
        }
    }

    /// <summary>
    /// Render objects from all floors
    /// </summary>
    private void EnableAllFloors()
    {
        foreach (Transform item in _world.transform)
        {
            if (!item.name.Contains("Floor")) continue;

            int floorNumber = int.Parse(item.name.Split(' ')[1]);
            item.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Deactivate all floors above a certain height
    /// </summary>
    private void DisableFloors()
    {
        foreach (Transform item in _world.transform)
        {
            if (!item.name.Contains("Floor")) continue;

            int floorNumber = int.Parse(item.name.Split(' ')[1]);
            item.gameObject.SetActive(floorNumber <= cameraFloor);
        }
    }

    /// <summary>
    /// Finds the object on which is clicked in the screen
    /// </summary>
    /// <param name="mousePosition">the screen position of the mouse</param>
    /// <returns>The found object or null</returns>
    public GameObject FindClickedObject(Vector3 mousePosition)
    {
        // Make a raycast to find which object is clicked
        Camera cam = Camera.allCameras[0];
        Ray ray = cam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
            return null;
        
        return hit.collider.gameObject;
    }
}
