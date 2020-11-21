using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

[JsonObject(MemberSerialization.OptIn)]
public class Interactable : MonoBehaviour
{
    /// <summary>
    /// True if the object is directly interactable, i.e. a button.
    /// False if the object can only be triggered by other triggers.
    /// </summary>
    [JsonProperty]
    public bool interactable = false;
    [JsonProperty]
    public float interactionCooldown = 0.5f;
    [JsonProperty]
    protected float timeSinceInteraction = float.MaxValue;

    public InteractEvent onInteract;

    public void FixedUpdate()
    {
        timeSinceInteraction += Time.fixedDeltaTime;
    }

    /// <summary>
    /// Player based interaction.
    /// </summary>
    public void Interact(Character whoDidIt)
    {
        if (interactable && timeSinceInteraction > interactionCooldown)
        {
            // Reset timer
            timeSinceInteraction = 0f;

            // Update animator
            GetComponent<Animator>()?.SetTrigger("onInteract");

            // Let Unity's event system handle the rest.
            onInteract?.Invoke(gameObject);

            // See if this interaction leads to points
            // Debug.Log(">>> found a Scoring-component: " + GetComponent<Scoring>());
            GetComponent<Scoring>()?.InteractionBonus(whoDidIt);

            GetComponent<APLSynced>()?.APLSync();
        }
    }

    /// <summary>
    /// System based trigger.
    /// </summary>
    public void Trigger()
    {
        // Update animator
        GetComponent<Animator>()?.SetTrigger("onInteract");

        // Let Unity's event system handle the rest.
        onInteract?.Invoke(gameObject);

        GetComponent<APLSynced>()?.APLSync();
    }
}

[System.Serializable]
public class InteractEvent : UnityEvent<GameObject>
{ }
