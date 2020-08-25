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

    public void ToggleState()
        => SetState(!isActive);
}

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }
