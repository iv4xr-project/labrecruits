using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Toggleable : MonoBehaviour
{
    [JsonProperty]
    public bool isActive = false;
    
    public ToggleEvent onToggle;

    public void SetState(bool state)
    {
        // Do nothing if nothing changes
        if (isActive == state) return;

        // Change state
        isActive = state;

        // Update animator
        GetComponent<Animator>()?.SetBool("toggleState", isActive);

        // Let Unity's event system handle the rest.
        onToggle.Invoke(isActive);

        GetComponent<APLSynced>()?.APLSync();
    }

    /// <summary>
    /// Toggling the state of this Toggable (true to false and the other way around).
    /// </summary>
    public void ToggleState()
        => SetState(!isActive);

    /// <summary>
    /// Set the state of this toggable to true/active.
    /// </summary>
    public void Activate()
    {
        isActive = true;
    }
}

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }
