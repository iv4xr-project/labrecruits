/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class delays execution of code by a specified amount of frames.
/// </summary>
public class Delayer : MonoBehaviour
{
    // keep a list with actions that are not executed
    private List<(int, Action)> list = new List<(int, Action)>();

    // add an action to the delayer
    public void Add(int frames, Action action) => list.Add((frames, action));

    // automatically update the counters for all actions
    public void Update()
    {
        // return if 0 for speed performance
        if (list.Count == 0)
            return;

        // create a new list (a.Item1--; does not work in a loop so I will have to create a new one)
        List<(int, Action)> rest = new List<(int, Action)>();

        foreach (var a in list)
        {
            //invoke if ready
            if (a.Item1 <= 0)
                a.Item2.Invoke();
            // decrease counter if not ready
            else
                rest.Add((a.Item1 - 1, a.Item2));
        }

        // replace the list
        this.list = rest;
    }

}
