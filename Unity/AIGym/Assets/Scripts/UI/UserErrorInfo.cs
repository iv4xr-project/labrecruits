/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public enum ErrorType
{
    General,
    WorldError,
    ConnectionError
}

/// <summary>
/// Write messages to a linefeed
/// </summary>
public class UserErrorInfo : MonoBehaviour
{
    public static UserErrorInfo ErrorWriter = null;
    
    public readonly string logPath = "GeneralInfo";
    public readonly float messageDuration = 8;

    List<string> loggedErrors = new List<string>();
    Text[] lines;
    float[] timeLeft;
    private int index, nlines;

    void Awake()
    {
        if (!ErrorWriter) ErrorWriter = this;

        timeLeft = new float [transform.childCount];
        nlines = transform.childCount;
        lines = new Text[nlines];
        for (int i = 0; i < nlines; i++)
        {
            timeLeft[i] = messageDuration;
            lines[i] = (transform.GetChild(i).gameObject.GetComponent<Text>());
        }
    }

    void Update()
    {
        // Update the time left for all messages, remove them if they are expired.
        for (int i = 0; i < nlines; i++)
        {
            if (lines[i].text == "") continue;

            timeLeft[i] -= Time.deltaTime;
            if (timeLeft[i] <= 0)
                SetTopMessage(i, "");
        }
    }

    /// <summary>
    /// Add world debug information to an error message
    /// </summary>
    public void AddWorldMessage(string message, string field, Vector3 position, bool showOnScreen = true)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("[").Append(ErrorType.WorldError.ToString()).Append("] ").Append(message);
        sb.Append(" In: '").Append(field).Append("', ");
        sb.Append(" floor ").Append(position.y).Append(", cell (" + position.x + "," + position.z + ").");

        AddMessage(sb.ToString(), showOnScreen, ErrorType.WorldError);
    }

    /// <summary>
    /// Add a message to store it. By default, the message is shown on the linefeed
    /// </summary>
    public void AddMessage(string message, bool showOnScreen = true, ErrorType type = ErrorType.General)
    {
        loggedErrors.Add(message);

        if (showOnScreen)
        {
            SetTopMessage(index, message);
            ++index;
            if (index >= nlines)
                index = 0;
        }
    }

    /// <summary>
    /// Sets the message of a textfield, and pushes it to the top
    /// </summary>
    /// <param name="message"></param>
    /// <param name="i"></param>
    private void SetTopMessage(int i, string message)
    {
        lines[i].text = message;
        lines[i].transform.SetAsFirstSibling();
        timeLeft[i] = messageDuration;
    }

    private string AddTypeToMessage(string message, ErrorType type)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("<b> [").Append(type.ToString()).Append("] ").Append("</b> ").Append(message);
        return sb.ToString();
    }

    /// <summary>
    /// Save all messages since last log.
    /// </summary>
    public void FlushErrorLog()
    {
        // return; // @Incomplete, messages are now stored in My Documents/AIGym-Logs.

        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "AIGym-Logs");
        Directory.CreateDirectory(path);
        path = Path.Combine(path, System.DateTime.Now.ToString("yyyyMMddTHHmmss"));
        Directory.CreateDirectory(path);
        string filePath = Path.Combine(path, logPath + ".txt"); //Temporary file naming convention

        if (!File.Exists(filePath))
        {
            using (StreamWriter sr = new StreamWriter(filePath))
            {
                for (int i = 0; i < loggedErrors.Count; i++)
                {
                    sr.WriteLine(loggedErrors[i]);
                }
            }
        }

        loggedErrors.Clear();
        // AddMessage("Error file logged");
    }

    public void ClearMessages() 
    {
        for (int i = 0; i < nlines; i++)
            lines[i].text = "";
    }
}
