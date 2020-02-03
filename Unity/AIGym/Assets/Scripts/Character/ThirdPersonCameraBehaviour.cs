/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections;
using UnityEngine;

/// <summary>
/// Handles camera movement from a third person perspective, with and without an attached agent.
/// </summary>
public class ThirdPersonCameraBehaviour : MonoBehaviour
{
    public bool detached = true;

    public float movementSpeed = 0.5f;

    public float minRotationY = 0f;
    public float maxRotationY = 90f;
    public float rotateSpeed = 8.0f;

    public float minZoom = 0f;
    public float maxZoom = 10f;
    public float zoomSpeed = 1.0f;

    public GameObject gameController;
    
    public GameObject attachedCharacter;
    private CameraBehaviour _cameraBehaviour;
    private Transform xAxis;
    private Transform offset;
    private Transform _transform; //Cache Unity transform to prevent unnecessary calls
    public bool CR_RUNNING; //Is the animation Coroutine running?

    void Start()
    {
        _transform = transform;
        _cameraBehaviour = gameController.GetComponent<CameraBehaviour>();
        xAxis = _transform.GetChild(0);
        offset = xAxis.GetChild(0);
    }

    void Update()
    {
        if (!detached)
        {
            if (attachedCharacter == null)
                detached = true;
            else
                transform.position = attachedCharacter.transform.position;
        }

        // Move the camera with the middle mouse button
        if (Input.GetKey(KeyCode.Mouse2))
        {
            if (!detached)
            {
                _cameraBehaviour.Detach();
                detached = true;
            }

            Vector3 cameraAngle = _transform.eulerAngles;
            cameraAngle.x = 0; //Makes sure we dont zoom in
            var direction = Quaternion.Euler(cameraAngle) * Vector3.right * -Input.GetAxis("Mouse X");
            direction += Quaternion.Euler(cameraAngle) * Vector3.forward * -Input.GetAxis("Mouse Y");
            _transform.position += direction * movementSpeed;
        }

        // Scroll zoom
        float zoom = Mathf.Clamp(offset.localPosition.z + Input.mouseScrollDelta.y, -maxZoom, -minZoom);
        offset.localPosition = Vector3.forward * zoom;
            
        // Right click to rotate
        if (Input.GetMouseButton(1))
        {
            // Rotate around the y-axis.
            _transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotateSpeed);

            // Rotate around the local x-axis, clamp this rotation in between specified angles.
            float angle = Mathf.Clamp(xAxis.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * rotateSpeed, minRotationY, maxRotationY);
            xAxis.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
        }
    }
    
    /// <summary>
    /// Attaches the camera to the given agent <see cref="GameObject"/>.
    /// </summary>
    /// <param name="agent">Agent <see cref="GameObject"/> that will be attached</param>
    public void FollowAgent(GameObject agent)
    {
        attachedCharacter = agent;
        detached = false;
    }
    public void Unfollow()
    {
        detached = true;
    }

    /// <summary>
    /// Coroutine for translating the position.
    /// </summary>
    private IEnumerator TranslateCoroutine(Vector3 goalPosition, float time)
    {
        CR_RUNNING = true;
        Vector3 beginPosition = transform.position;
        float totalTime = time;
        while (totalTime > 0)
        {
            transform.position = Vector3.Lerp(goalPosition, beginPosition, totalTime / time);
            totalTime -= Time.deltaTime;
            if (totalTime <= 0)
            {
                transform.position = goalPosition;
                break;
            }
            yield return null;
        }
        CR_RUNNING = false;
    }

    /// <summary>
    /// Translates the position using Lerp to another position in given seconds <see cref="Float"/> .
    /// </summary>
    public void TranslateAnimation(Vector3 movement, float time)
    {
        StartCoroutine(TranslateCoroutine(transform.position+ movement, time));
    }
}
