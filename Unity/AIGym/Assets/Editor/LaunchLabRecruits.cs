using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.CodeCoverage;

class LaunchLabRecruits {
    
    // "labrecruits/launchUnityWithCoverage.bat" contains the command line execution example
    // Invoke this method when you run Unity from the command line to:
    static void load() {
        // Indicate the scene we want to load (AIGym Main scene) when start the play mode
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Main.unity");

        // Start the play mode in Unity
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = true; 
        #endif

        // Start recording code coverage (plugin dependency added and launchUnityWithCoverage enabling the execution)
        // https://docs.unity3d.com/Packages/com.unity.testtools.codecoverage@0.2
        CodeCoverage.StartRecording();
    }
}
