/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
    private RectTransform _rectTransform;
    public GameObject player;
    public Image _icon;
    public float popupTime;
    public CanvasGroup canvasGroup;
    public float camDistance;

    public float minZoom;
    public float maxZoom;
    public float minScale;
    public float maxScale;
    
    public Dictionary<Icon, Sprite> iconsMap = new Dictionary<Icon, Sprite>();
    public IconSprite[] iconSprites;
    
    public enum Icon
    {
        Cross,
        Question,
        Check,
        Gears,
    }
    
    [Serializable]
    public struct IconSprite
    {
        public Icon Icon;
        public Sprite Sprite;
    }
    
    private void Awake()
    {
        foreach (IconSprite iconSprite in iconSprites)
            iconsMap[iconSprite.Icon] = iconSprite.Sprite;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        ShowPopup(Icon.Gears);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Reposition();
    }

    //Makes sure that the UIPopup keeps facing the active camera
    void Reposition()
    {
        Camera cam = GameObject.FindWithTag("TopDownView").GetComponentInChildren<Camera>();
        if (!cam) return;

        canvasGroup.alpha = cam.enabled ? canvasGroup.alpha : 0f;
        canvasGroup.gameObject.transform.LookAt(cam.transform);
    }

    //Starts a new popup with a fade-in and out effect
    public void ShowPopup(Icon icon)
    {
        StartCoroutine(PopUp(icon));
    }
    
    //Coroutine for the Popup process
    IEnumerator PopUp(Icon icon)
    {
        _icon.sprite = iconsMap[icon];
        yield return Fade(true);
        yield return new WaitForSeconds(popupTime);
        yield return Fade(false);
    }

    //Coroutine for the fade effect
    IEnumerator Fade(bool fadeIn)
    {
        for (float ft = 1f; ft >= -0.1; ft -= 0.1f)
        {
            var alpha = fadeIn ? 1f - ft : ft;
            canvasGroup.alpha = alpha;
            yield return new WaitForSeconds(.1f);
        }
    }
    
}
