using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.CodeCoverage;

class LaunchLabRecruits {
    
	// Invoke this method when you run Unity from the command line to:
	// "labrecruits/launchUnityWithCoverage.bat" contains the command line execution example
    static void load() {
        // Start the play mode in Unity
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = true; 
        #endif
        
		// Start the code coverage Unity feature
		// You need to have the code coverage plugin installed in Unity (via the Package Manager for Unity 2019.3 and 2019.4)
		// https://docs.unity3d.com/Packages/com.unity.testtools.codecoverage@0.2
		// https://youtu.be/y75yxUkLB50?t=1451 :)
        CodeCoverage.StartRecording();
        
		// Load the Main scene of AIGym game
        EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/Main.unity", new LoadSceneParameters(LoadSceneMode.Single));
    }
}
