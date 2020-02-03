/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine.Events;

/// <summary>
/// ThreadSafeUnityEvent are just UnityEvents, but invoke the methods on the main thread via the Dispatcher.
/// </summary>

public class ThreadSafeUnityEvent : UnityEvent
{
    public new void Invoke() => Dispatcher.ExecuteInUpdate(() => base.Invoke());
}

public class ThreadSafeUnityEvent<T0> : UnityEvent<T0>
{
    public new void Invoke(T0 arg0) => Dispatcher.ExecuteInUpdate(() => base.Invoke(arg0));
}

public class ThreadSafeUnityEvent<T0, T1> : UnityEvent<T0, T1>
{
    public new void Invoke(T0 arg0, T1 arg1) => Dispatcher.ExecuteInUpdate(() => base.Invoke(arg0, arg1));
}

public class ThreadSafeUnityEvent<T0, T1, T2> : UnityEvent<T0, T1, T2>
{
    public new void Invoke(T0 arg0, T1 arg1, T2 arg2) => Dispatcher.ExecuteInUpdate(() => base.Invoke(arg0, arg1, arg2));
}
