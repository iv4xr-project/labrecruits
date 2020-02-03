/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

/*
 * 
 * This test is not used because the topright agent select menu is not used atm
namespace UITests
{
    public class UIAgentViewTest
    {
        private CameraBehaviour _cameraBehaviour;
        private UIViewBehaviour _uiViewBehaviour;
        private UIToggleBehaviour[] _uiToggleList;
        

        [SetUp]
        public void AlwaysRunBefore()
        {
            SceneManager.LoadScene("Scenes/Main");
        }

        [UnityTest]
        public IEnumerator Does_Eye_Icon_Enable()
        {
            yield return null;

            SetupTest();

            foreach (UIToggleBehaviour uiToggle in _uiToggleList)
            {
                var toggle = uiToggle.GetComponent<Toggle>();
                _cameraBehaviour.AttachToCharacter(uiToggle.character);
                yield return null;
                Assert.AreEqual(toggle.isOn, true);
                yield return null;
            }
        }
        
        [UnityTest]
        public IEnumerator Does_Eye_Icon_Disable()
        {
            yield return null;

            SetupTest();

            foreach (UIToggleBehaviour uiToggle in _uiToggleList)
            {
                _cameraBehaviour.AttachToCharacter(uiToggle.character);
                yield return null;

                UIToggleBehaviour[] exclude = {uiToggle};
                foreach (UIToggleBehaviour uiToggleOff in _uiToggleList.Except(exclude))
                {
                    var toggleOff = uiToggleOff.GetComponent<Toggle>();
                    Assert.AreEqual(toggleOff.isOn, false);
                }
                yield return null;
            }
        }
        
        private void SetupTest()
        {
            _cameraBehaviour = GameObject.FindWithTag("Lab").GetComponent<CameraBehaviour>();
            _uiViewBehaviour = GameObject.FindWithTag("UIView").GetComponent<UIViewBehaviour>();
            _uiToggleList = _uiViewBehaviour.gameObject.GetComponentsInChildren<UIToggleBehaviour>();
        }
    }
}
*/