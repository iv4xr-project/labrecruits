/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

//#define USE_CAMERA_TESTS

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

using NotImplementedException = System.NotImplementedException;

namespace CharacterTests
{
    public class MovementTests
    {
        const float MARGIN_OF_ERROR = 0.0005f;

        GameObject character;
        GameObject camera;
        Vector3 initCharacter, initCharacterAngle;
        Vector3 finalCharacter, finalCharacterAngle;

#if USE_CAMERA_TESTS
        GameObject camera;
        Vector3 initCamera, finalCamera;
#endif

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
        public IEnumerator Does_Character_Move_Up_When_Jumping()
        {
            yield return null;

            SetupTest();

            character.GetComponent<Character>().MoveJump();

            yield return new WaitForSeconds(0.4f);

            FinalValues();

            Assert.Less(initCharacter.y, finalCharacter.y);
        }

        [UnityTest]
        public IEnumerator Does_Character_Land_After_Jumping()
        {
            yield return null;

            SetupTest();

            character.GetComponent<Character>().MoveJump();

            yield return new WaitForSeconds(1f);

            FinalValues();

            Assert.AreEqual(initCharacter.y, finalCharacter.y, MARGIN_OF_ERROR);
        }

        [UnityTest]
        public IEnumerator Does_Character_Move_Forwards()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 20; i++)
            {
                character.GetComponent<Character>().Move(Vector3.forward);

                yield return null;
            }

            FinalValues();

            Assert.Less(initCharacter.z, finalCharacter.z);
        }

        [UnityTest]
        public IEnumerator Does_Character_Move_Backwards()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 20; i++)
            {
                character.GetComponent<Character>().Move(Vector3.back);

                yield return null;
            }

            FinalValues();

            Assert.Greater(initCharacter.z, finalCharacter.z);
        }

        [UnityTest]
        public IEnumerator Does_Character_Move_To_The_Left()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 20; i++)
            {
                character.GetComponent<Character>().Move(Vector3.left);

                yield return null;
            }

            FinalValues();

            Assert.Greater(initCharacter.x, finalCharacter.x);
        }

        [UnityTest]
        public IEnumerator Does_Character_Move_To_The_Right()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 20; i++)
            {
                character.GetComponent<Character>().Move(Vector3.right);

                yield return null;
            }

            FinalValues();

            Assert.Less(initCharacter.x, finalCharacter.x);
        }

        [UnityTest]
        public IEnumerator Does_Character_Move_And_Jump()
        {
            yield return null;

            SetupTest();

            character.GetComponent<Character>().MoveJump();

            for (int i = 0; i < 20; i++)
            {
                character.GetComponent<Character>().Move(Vector3.forward);

                yield return null;
            }

            FinalValues();

            Assert.Less(initCharacter.z, finalCharacter.z);
            Assert.Less(initCharacter.y, finalCharacter.y);

        }

        [UnityTest]
        public IEnumerator Does_Character_Turn_Left_Quick()
        {

            yield return null;

            SetupTest();

            character.GetComponent<Character>().Turn(-90);

            yield return null;

            FinalValues();

            Assert.AreEqual(initCharacterAngle.y + 270, finalCharacterAngle.y);
        }

        [UnityTest]
        public IEnumerator Does_Character_Turn_Left_Slow()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 90; i++)
            {
                character.GetComponent<Character>().Turn(-1);

                yield return null;
            }

            FinalValues();

            Assert.AreEqual(initCharacterAngle.y + 270, finalCharacterAngle.y, MARGIN_OF_ERROR);
        }

        [UnityTest]
        public IEnumerator Does_Character_Turn_Right_Quick()
        {

            yield return null;

            SetupTest();

            character.GetComponent<Character>().Turn(90);

            yield return null;

            FinalValues();

            Assert.AreEqual(initCharacterAngle.y + 90, finalCharacterAngle.y);
        }

        [UnityTest]
        public IEnumerator Does_Character_Turn_Right_Slow()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 90; i++)
            {
                character.GetComponent<Character>().Turn(1);

                yield return null;
            }

            FinalValues();

            Assert.AreEqual(initCharacterAngle.y + 90, finalCharacterAngle.y, MARGIN_OF_ERROR);
        }
        /*
        [UnityTest]
        public IEnumerator Does_Character_Look_Up_Quick()
        {
            yield return null;

            SetupTest();

            //Uncomment line and fill in right function at the end once implemented.
            //character.GetComponent<Character>().Look(90);

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 90, finalCamera.x);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Up_Slow()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 90; i++)
            {
                //Uncomment lines and fill in right function at the end once implemented.
                //character.GetComponent<Character>().Look(1);

                //yield return null;
            }

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 90, finalCamera.x, MARGIN_OF_ERROR);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Down_Quick()
        {
            yield return null;

            SetupTest();

            //Uncomment line and fill in right function at the end once implemented.
            //character.GetComponent<Character>().Look(-90);

            yield return null;

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 270, finalCamera.x);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Down_Slow()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 90; i++)
            {
                //Uncomment lines and fill in right function at the end once implemented.
                //character.GetComponent<Character>().Look(-1);

                //yield return null;
            }

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 270, finalCamera.x, MARGIN_OF_ERROR);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Up_Right_Quick()
        {
            yield return null;

            SetupTest();

            //Uncomment line and fill in right function at the end once implemented.
            //character.GetComponent<Character>().Look(90,90);

            yield return null;

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 90, finalCamera.x);
            Assert.AreEqual(initCamera.y + 90, finalCamera.y);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Up_Right_Slow()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 90; i++)
            {
                //Uncomment lines and fill in right function at the end once implemented.
                //character.GetComponent<Character>().Look(1,1);

                //yield return null;
            }

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 90, finalCamera.x, MARGIN_OF_ERROR);
            Assert.AreEqual(initCamera.y + 90, finalCamera.y, MARGIN_OF_ERROR);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Up_Left_Quick()
        {
            yield return null;

            SetupTest();

            //Uncomment line and fill in right function at the end once implemented.
            //character.GetComponent<Character>().Look(90, -90);

            yield return null;

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 90, finalCamera.x);
            Assert.AreEqual(initCamera.y + 270, finalCamera.y);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Up_Left_Slow()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 90; i++)
            {
                //Uncomment lines and fill in right function at the end once implemented.
                //character.GetComponent<Character>().Look(1, -1);

                //yield return null;
            }

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 90, finalCamera.x, MARGIN_OF_ERROR);
            Assert.AreEqual(initCamera.y + 270, finalCamera.y, MARGIN_OF_ERROR);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Down_Right_Quick()
        {
            yield return null;

            SetupTest();

            //Uncomment line and fill in right function at the end once implemented.
            //character.GetComponent<Character>().Look(-90, 90);

            yield return null;

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 270, finalCamera.x);
            Assert.AreEqual(initCamera.y + 90, finalCamera.y);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Down_Right_Slow()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 90; i++)
            {
                //Uncomment lines and fill in right function at the end once implemented.
                //character.GetComponent<Character>().Look(-1, 1);

                //yield return null;
            }

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 270, finalCamera.x, MARGIN_OF_ERROR);
            Assert.AreEqual(initCamera.y + 90, finalCamera.y, MARGIN_OF_ERROR);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Down_Left_Quick()
        {
            yield return null;

            SetupTest();

            //Uncomment line and fill in right function at the end once implemented.
            //character.GetComponent<Character>().Look(-90, -90);

            yield return null;

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 270, finalCamera.x);
            Assert.AreEqual(initCamera.y + 270, finalCamera.y);
        }

        [UnityTest]
        public IEnumerator Does_Character_Look_Down_Left_Slow()
        {
            yield return null;

            SetupTest();

            for (int i = 0; i < 90; i++)
            {
                //Uncomment lines and fill in right function at the end once implemented.
                //character.GetComponent<Character>().Look(-1, -1);

                //yield return null;
            }

            FinalValues();

            throw new NotImplementedException("Code not written, remove this error once test is needed");

            Assert.AreEqual(initCamera.x + 270, finalCamera.x, MARGIN_OF_ERROR);
            Assert.AreEqual(initCamera.y + 270, finalCamera.y, MARGIN_OF_ERROR);
        }
        */

        private void SetupTest()
        {
            character = GameObject.FindWithTag("Player");

            initCharacter = character.transform.position;
#if USE_CAMERA_TESTS
            camera = GameObject.FindWithTag("Agent 1");
            initCamera = camera.transform.eulerAngles;
#endif
            initCharacterAngle = character.transform.eulerAngles;
        }

        private void FinalValues()
        {
            finalCharacter = character.transform.position;

#if USE_CAMERA_TESTS
            finalCamera = camera.transform.eulerAngles;
#endif

            finalCharacterAngle = character.transform.eulerAngles;
        }

        [TearDown]
        public void AlwaysRunAfter()
        {

        }
    }
}
