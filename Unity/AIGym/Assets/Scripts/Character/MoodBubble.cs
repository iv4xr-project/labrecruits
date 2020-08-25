using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class MoodBubble : MonoBehaviour
{
    public Character character;
    public Text textField;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        var m = character.GetMood();
        GetComponent<Canvas>().enabled = DateTime.Now - m.lastSet < TimeSpan.FromSeconds(5) && m.value != "";
        textField.text = m.value;
    }
}
