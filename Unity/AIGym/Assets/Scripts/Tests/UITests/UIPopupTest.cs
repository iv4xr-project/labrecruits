/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UITests
{
    public class UIPopupTest
    {
        private UIPopup _uiPopup;
        private CameraBehaviour _cameraBehaviour;
        private World _world;
        private Character _character;
        private string level_path = "Levels/Small/Basic";

        [SetUp]
        public void AlwaysRunBefore()
        {
            SceneManager.LoadScene("Scenes/Main");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            GameObject.FindWithTag("Lab").GetComponent<Lab>().config.level_path = level_path;
        }


        [UnityTest]
        public IEnumerator Does_Popup_Show()
        {
            yield return null;
            SetupTest();
            
            _uiPopup.ShowPopup(UIPopup.Icon.Check);
            yield return new WaitForSeconds(1.5f);
            Assert.AreEqual( 1f, _uiPopup.canvasGroup.alpha);
        }
        
        [UnityTest]
        public IEnumerator Does_Popup_Leave()
        {
            yield return null;
            SetupTest();
            
            _uiPopup.ShowPopup(UIPopup.Icon.Check);
            yield return new WaitForSeconds(4.5f);
            Assert.AreEqual( 0f, _uiPopup.canvasGroup.alpha);
        }
        
        [UnityTest]
        public IEnumerator Is_Popup_Gone_In_First_Person()
        {
            yield return null;
            SetupTest();
            
            _uiPopup.ShowPopup(UIPopup.Icon.Check);
            yield return new WaitForSeconds(1f);
            
            _cameraBehaviour.SwitchPov();
            yield return new WaitForSeconds(0.2f);
            
            Assert.AreEqual( 0f, _uiPopup.canvasGroup.alpha);
        }
        
        private void SetupTest()
        {
            _cameraBehaviour = GameObject.FindWithTag("Lab").GetComponent<CameraBehaviour>();
            _world = GameObject.FindWithTag("World").GetComponent<World>();
            _character = GameObject.FindObjectOfType<Character>();
            _cameraBehaviour.AttachToCharacter(_character);
            _uiPopup = _character.GetComponentInChildren<UIPopup>();
        }
    }
}