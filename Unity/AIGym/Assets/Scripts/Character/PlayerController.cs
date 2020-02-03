/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;

/// <summary>
/// Applies human input to a character
/// </summary>
public class PlayerController : MonoBehaviour
{
    private CameraBehaviour _cameraBehaviour;
    private Character _character;

    private void Awake()
    {
        _cameraBehaviour = GetComponent<CameraBehaviour>();
    }

    /// <summary>
    /// Read input from the player and apply it to the character
    /// </summary>
    void Update()
    {
        //@Todo: refactor this into a one time change, outside of Update.
        _character = _cameraBehaviour.attachedCharacter;

        if (_character == null)
            return;

        Vector3 right = Camera.main.transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);

        Vector3 direction = 
            (Input.GetKey(KeyCode.W) ? forward : Vector3.zero) +
            (Input.GetKey(KeyCode.A) ? -right : Vector3.zero) + 
            (Input.GetKey(KeyCode.S) ? -forward : Vector3.zero) +
            (Input.GetKey(KeyCode.D) ? right : Vector3.zero);
        
        if (direction != Vector3.zero) _character.Move(direction);

        if (Input.GetKeyDown(KeyCode.E)) _character.Interact();
    }
}