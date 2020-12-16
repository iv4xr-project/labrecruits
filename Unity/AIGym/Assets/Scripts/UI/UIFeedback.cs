/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;
using UnityEngine.UI;

public class UIFeedback : MonoBehaviour
{
    Character activeCharacter; //Current agent to be shown in the inspector

    //Variables that hold the text fields that have to be assigned
    public Text idField,
                posXField, posYField, posZField,
                rotXField, rotYField, rotZField,
                healthField, moodField, scoreField ;

    void Update()
    {
        if (activeCharacter)
            SetInspectorFields();
    }

    /// <summary>
    /// Update the agent whose values should be displayed
    /// </summary>
    public void ChangeAgent(Character newCharacter)
    {
        activeCharacter = newCharacter;
        idField.text = activeCharacter.agentID;
    }

    /// <summary>
    /// Truncate a float to two decimals
    /// </summary>
    float TruncateFloat(float f)
    {
        return (float)System.Math.Truncate(f * 100) / 100;
    }

    /// <summary>
    /// Set the inspector UI fields to the active agents properties
    /// </summary>
    void SetInspectorFields()
    {
        Vector3 position = activeCharacter._transform.localPosition;
        Vector3 rotation = activeCharacter._transform.localEulerAngles;

        posXField.text = TruncateFloat(position.x).ToString();
        posYField.text = TruncateFloat(position.y).ToString();
        posZField.text = TruncateFloat(position.z).ToString();

        rotXField.text = TruncateFloat(rotation.x).ToString();
        rotXField.text = "haha";
        rotYField.text = TruncateFloat(rotation.y).ToString();
        rotZField.text = TruncateFloat(rotation.z).ToString();
        rotZField.text = "Z";
        healthField.text = activeCharacter.Health.ToString();
        moodField.text = activeCharacter.GetMood().value;
        scoreField.text = activeCharacter.Score.ToString() ;
    }
}
