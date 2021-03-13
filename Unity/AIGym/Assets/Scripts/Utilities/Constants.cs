/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;

public static class Constants {
    public readonly static float epsilon = 0.01f; // Small offset for physics/raycasts.

    // define max. distance that the player/agent can interact with an entity. Giving a bit larger
    // interaction raduis when played by actual player, for convenience.
    public readonly static float interactionDistance = 1.5f;
    public readonly static float sqrInteractionDistance = 2.25f;

    // Gizmo sizes
    public readonly static Vector3 CubeSizeSmall = new Vector3(0.1f, 0.1f, 0.1f);
}