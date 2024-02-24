using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeOutText : MonoBehaviour
{
    [SerializeField] Text fadeText;
    [SerializeField] float fadeTime = 1f;
    IEnumerator fading;

    public void FadeOut(string text, Color color, bool isBold, UnityAction<bool> callback)
    {
        if (fading != null) StopCoroutine(fading);
        fadeText.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
        fading = IEFadeOut(text, color, callback);
        if (gameObject.activeInHierarchy) StartCoroutine(fading);
    }

    private IEnumerator IEFadeOut(string text, Color color, UnityAction<bool> callback)
    {
        float elapsedTime = 0f;
        Color tColor = color;
        fadeText.text = text;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            fadeText.color = new Color(tColor.r, tColor.g, tColor.b, alpha);
            yield return null;
        }

        callback(true);
    }

}
