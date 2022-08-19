using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class IHasMood : MonoBehaviour
{
    abstract public Character.Mood GetMood();
}
