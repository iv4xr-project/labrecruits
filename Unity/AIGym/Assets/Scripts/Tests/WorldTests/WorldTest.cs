/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WorldTests

{
    public class WorldTest

    {
        private World _world;
        private readonly string level_path = "Levels/Small/Basic";

        [SetUp]
        public void AlwaysRunBefore()

        {
            SceneManager.LoadScene("Scenes/Main");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)

        {
            _world = GameObject.FindWithTag("World").GetComponent<World>();
            GameObject.FindWithTag("Lab").GetComponent<Lab>().config.level_path = level_path;
        }

        [UnityTest]
        //Tests buttons, when activated, open/close doors as specified by the level file
        public IEnumerator Does_WireState_Match_Links()
        {
            yield return null;
            
            string pageOne = _world.SplitSheets(Utils.LoadText(level_path))[0];
            string[] rows = pageOne.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            string[] cells = rows[0].Split(',');

            Interactable sensor = GameObject.Find(cells[0]).GetComponent<Interactable>();
            Toggleable door = GameObject.Find(cells[1]).GetComponent<Toggleable>();

            Debug.Log(sensor + " " + door);

            Assert.IsFalse(door.isActive);

            sensor.GetComponent<Interactable>().Trigger();

            yield return null;

            //Checking if the wirePart is gray (means the wire is disabled)
            GameObject wire = sensor.transform.Find("Wire").gameObject;
            bool wireBroken = wire.GetComponent<Wire>().broken;
            Assert.AreEqual(door.isActive, !wireBroken);
        }
        
        
        [UnityTest]
        public IEnumerator Does_The_CSV_File_Load_Correctly()

        {
            yield return null;
            
            var actualBeforeParse = new List<(string, Vector3, World.EntityGroup)>();
            var expected = new List<(string, Vector3)>();
            var actualAfterParse = new List<(string, Vector3)>();

            //Recreate the list manually for testing
            for (int i = 0; i < 6; i++)
                expected.Add(("w", new Vector3(i, 0, 0)));


            for (int i = 1; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (j == 0 || j == 5)
                        expected.Add(("w", new Vector3(j, 0, i)));
                    else
                    {
                        expected.Add(("f", new Vector3(j, 0, i)));
                        if (j == 1 && i == 1)
                            expected.Add(("d>n^Door 1", new Vector3(j, 0, i)));

                        if (j == 1 && i == 2)
                            expected.Add(("b^Button 1", new Vector3(j, 0, i)));

                        if (j == 1 && i == 3)
                            expected.Add(("a^0", new Vector3(j, 0, i)));
                    }
                }
            }


            for (int i = 0; i < 6; i++)
                expected.Add(("w", new Vector3(i, 0, 6)));

            string[] sheets = _world.SplitSheets(Utils.LoadText(level_path));
            
            actualBeforeParse = _world.ExtractSpawnableObjects(sheets.Skip(1).ToArray())[0];
            
            foreach (var tuple in actualBeforeParse)
                actualAfterParse.Add((tuple.Item1, tuple.Item2));
            
            Assert.AreEqual(expected, actualAfterParse);
        }


        // Tests buttons, when activated, open/close doors as specified by the level file
        [UnityTest]
        public IEnumerator Buttons_Linked_To_Doors_Successfully()
        {
            yield return null;
            
            var pageOne = _world.SplitSheets(Utils.LoadText(level_path))[0];
            string[] rows = pageOne.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
            string[] cells = rows[0].Split(',');

            Interactable sensor = GameObject.Find(cells[0]).GetComponent<Interactable>();
            Toggleable door = GameObject.Find(cells[1]).GetComponent<Toggleable>();
            Debug.Log(sensor + " " + door);

            Assert.IsFalse(door.isActive);

            sensor.GetComponent<Toggleable>().ToggleState();

            yield return null;
            
            Assert.IsTrue(door.isActive);
        }


        //[TearDown]

        public void AlwaysRunAfter()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}