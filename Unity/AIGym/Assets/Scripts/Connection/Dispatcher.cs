/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Solution found on: https://stackoverflow.com/questions/41330771/use-unity-api-from-another-thread-or-call-a-function-in-the-main-thread
/// Then adjusted to our needs
/// </summary>

public class Dispatcher : MonoBehaviour
{
    /// <summary>
    /// our (singleton) instance
    /// </summary>
    private static Dispatcher instance = null;

    /// <summary>
    /// Holds actions received from another Thread. Will be coped to actionQueueCopy then executed from there
    /// </summary>
    private static List<Action> actionQueue = new List<Action>();

    /// <summary>
    /// holds Actions copied from actionQueuesUpdateFunc to be executed
    /// note that actionQueueCopy is not static
    /// the reason we use a second queue is so we can execute all Actions, while the static queue can be filled up again by other threads
    /// </summary>
    private List<Action> actionQueueCopy = new List<Action>();

    /// <summary>
    /// Used to know if whe have new Action function to execute. This prevents the use of the lock keyword every frame, and will speed up the process!
    /// </summary>
    private volatile static bool actionQueueIsEmpty = true;

    /// <summary>
    /// Used to initialize the Dispatcher. Call this on the main thread!
    /// </summary>
    /// <param name="visible"></param>
    public static void Init(bool visible = false)
    {
        if (instance != null)
            return;

        if (Application.isPlaying)
        {
            // add an invisible game object to the scene
            GameObject obj = new GameObject("MainThreadExecuter");

            if (!visible)
                obj.hideFlags = HideFlags.HideAndDontSave;

            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<Dispatcher>();
        }
    }

    /// <summary>
    /// Do not destroy when loading a new scene
    /// </summary>
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Add an action to the queue to be executed on the main thread
    /// </summary>
    /// <param name="action">Action to execute on the main thread</param>
    public static void ExecuteInUpdate(Action action)
    {
        if (action == null)
            throw new ArgumentNullException("The action is null");

        lock (actionQueue)
        {
            actionQueue.Add(action);
            actionQueueIsEmpty = false;
        }
    }

    /// <summary>
    /// In the mainthread, copy the static queue to the local queue.
    /// During execution of the actions from the local queue, the static queue is already free and can be used by other threads again!
    /// </summary>
    public void Update()
    {
        if (actionQueueIsEmpty)
            return;

        //Clear the old actions
        actionQueueCopy.Clear();
        lock (actionQueue)
        {
            //Copy actions to main thread queue
            actionQueueCopy.AddRange(actionQueue);
            //Now clear the queue since we've done copying it
            actionQueue.Clear();
            actionQueueIsEmpty = true;
        }

        // execute the functions
        foreach(Action action in actionQueueCopy)
            action.Invoke();
    }

    /// <summary>
    /// Disable the <see cref="Dispatcher"/>
    /// </summary>
    public void OnDisable()
    {
        if (instance == this)
            instance = null;
    }

    /// <summary>
    /// Destroys the current <see cref="Dispatcher"/> instance.
    /// </summary>
    void OnDestroy()
    {
        instance = null;
    }
}
