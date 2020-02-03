/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;
using SFB;

public class UIFileSelect : MonoBehaviour
{
    private static ExtensionFilter[] extensions = new[] { new ExtensionFilter("Level Files", "csv"), new ExtensionFilter("All Files", "*"), };

    public void Open()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Level", "", extensions, false);
        if (paths.Length < 1)
            return;

        /**
        //Local path from Resources folder
        var fs = paths[0].Split(new string[] { "\\Resources\\" }, System.StringSplitOptions.None);
        if (fs.Length < 2) {
            UserErrorInfo.ErrorWriter.AddMessage("Error loading file, only load from Resources folder");
            return;
        }
            
        //Remove file extension
        string f = fs[1].Split(new char[] { '.' } , 2)[0];
        FindObjectOfType<Lab>().InitiateNewLevel(f);
        **/
        FindObjectOfType<Lab>().InitiateNewLevel(new EnvironmentConfig() { level_path = paths[0] });
    }
}
