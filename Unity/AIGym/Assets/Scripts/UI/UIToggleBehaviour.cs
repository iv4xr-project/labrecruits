/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIToggleBehaviour : MonoBehaviour, IPointerClickHandler
{
    public Character character;
    public CameraBehaviour cameraBehaviour;
    private Toggle _toggle;
    
    // Start is called before the first frame update
    void Start()
    {
        _toggle = GetComponent<Toggle>();
    }
    
    void Update()
    {
        _toggle.isOn = cameraBehaviour.attachedCharacter == character;
    }
    
    /// <summary>
    /// Attaches the camera system to the agent
    /// </summary>
    void AttachAgent()
    {
        if (_toggle.isOn)
            cameraBehaviour.AttachToCharacter(character);
        else
            cameraBehaviour.Detach();
    }

    /// <summary>
    /// When the user clicks on the UI toggle
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        AttachAgent();
    }
}
