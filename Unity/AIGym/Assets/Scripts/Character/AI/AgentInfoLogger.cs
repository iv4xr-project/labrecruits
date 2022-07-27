/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Text;
using System.IO;

/// <summary>
/// Write messages to a linefeed
/// </summary>
public class AgentInfoLogger
{
    private StringBuilder log = new StringBuilder();
    public readonly string logPath = "AgentInfo";

    /// <summary>
    /// Logs a message (on a new line)
    /// </summary>
    public void Log(string message) => log.AppendLine(message);

    /// <summary>
    /// Saves all logged messages to file, should be done once per run (at the end)
    /// </summary>
    public void Flush()
    {
        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "AIGym-Logs");
        Directory.CreateDirectory(path);
        path = Path.Combine(path, System.DateTime.Now.ToString("yyyyMMddTHHmmss"));
        Directory.CreateDirectory(path);
        string filePath = Path.Combine(path, logPath + ".txt"); //Temporary file naming convention

        if (!File.Exists(filePath))
        {
            using (StreamWriter sr = new StreamWriter(filePath))
                sr.Write(log.ToString());
        }

        log = new StringBuilder();
    }
}
