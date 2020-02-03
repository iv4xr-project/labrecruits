/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


/// <summary>
/// Exposes pointer events to the event system.
/// </summary>
public class UIHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable]
    public class PointerEvent : UnityEvent<PointerEventData> { }
    public PointerEvent onPointerEnter;
    public PointerEvent onPointerExit;

    /// <summary>
    /// Invoke onPointerEnter <see cref="PointerEvent"/>, this is called by the <see cref="GraphicRaycaster"/>.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke(eventData);
    }

    /// <summary>
    /// Invoke onPointerExit <see cref="PointerEvent"/>, this is called by the <see cref="GraphicRaycaster"/>.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke(eventData);
    }
}
