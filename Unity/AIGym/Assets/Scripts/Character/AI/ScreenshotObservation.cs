/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;

class ScreenshotObservation
{
    byte[] bytes;

    /// <summary>
    /// Uses a character's camera to create a screenshot.
    /// </summary>
    public void TakeScreenshot(Character character)
    {
        Camera cam = character.GetComponentInChildren<Camera>();

        RenderTexture screenShotTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);
        cam.targetTexture = screenShotTexture;
        RenderTexture.active = screenShotTexture;

        //Render what camera sees to screenShotTexture (the active RenderTexture)
        cam.Render();

        Texture2D screenShot = new Texture2D(cam.pixelWidth, cam.pixelHeight, TextureFormat.RGB24, false);
        //Read pixels from the active RenderTexture into a Texture2D
        screenShot.ReadPixels(new Rect(0, 0, cam.pixelWidth, cam.pixelHeight), 0, 0);

        cam.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(screenShotTexture);

        bytes = screenShot.EncodeToJPG();
    }
}

