/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.IO;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WorldTests
{
    public class ColorScreenTest
    {
        private string level_path = "Levels/Small/ColorScreen";
        private World _world;
        
        [SetUp]
        public void AlwaysRunBefore()
        {
            SceneManager.LoadScene("Scenes/Main");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            GameObject.FindWithTag("Lab").GetComponent<Lab>().config.level_path = level_path;
            _world = GameObject.FindWithTag("World").GetComponent<World>();
        }

        /// <summary>
        /// Test if the colorscreen correctly changes for every interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TestColorButtons()
        {
            yield return null;

            string pageOne = _world.SplitSheets(Utils.LoadText(level_path))[0];
            string[] rows = pageOne.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            ColorScreen cs = GameObject.Find(rows[0].Split(',')[1]).GetComponent<ColorScreen>();
            Color predictedColor = new Color();

            for (int i = 0; i < rows.Length; i++)
            {
                string[] cells = rows[i].Split(',');
                ColorButton cb = GameObject.Find(cells[0]).GetComponent<ColorButton>();
                predictedColor += cb.GetColor();

                cb.Trigger();

                var testColor = new Color(Mathf.Min(predictedColor.r, 1), Mathf.Min(predictedColor.g, 1), Mathf.Min(predictedColor.b, 1));

                Assert.IsTrue(cs.GetColor() == testColor);
            }
        }

        /// <summary>
        /// Test if the colorscreen correctly removes colors
        /// </summary>
        [UnityTest]
        public IEnumerator TestTurningColorButtonsOff()
        {
            yield return null;
            
            string pageOne = _world.SplitSheets(Utils.LoadText(level_path))[0];
            string[] rows = pageOne.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            string[] cells = rows[0].Split(',');
            ColorButton cb = GameObject.Find(cells[0]).GetComponent<ColorButton>();
            ColorScreen cs = GameObject.Find(cells[1]).GetComponent<ColorScreen>();

            cb.Trigger();
            cb.Trigger();

            Assert.IsTrue(cs.GetColor() == Color.black);
        }

        //[TearDown]
        public void AlwaysRunAfter()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}
