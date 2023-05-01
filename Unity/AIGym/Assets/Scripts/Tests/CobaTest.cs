using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class CobaTest
    {
        [SetUp]
        public void AlwaysRunBefore()
        {
            SceneManager.LoadScene("Scenes/Main");
            Debug.Log(">>> Scene Main loaded");
        }
        
        [UnityTest]
        [Timeout(960000)]
        public IEnumerator TestMainScene()
        {
            yield return new WaitForSeconds(900);
        }
    }
}