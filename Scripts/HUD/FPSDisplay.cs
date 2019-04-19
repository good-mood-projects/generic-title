using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
 * Simple Object used to show CPU Time and FPS count on the top left corner of the HUD.
 */
public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    public Text textField;

    void Update()
    {
        if (Time.timeScale == 1.0f)
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("CPU Thread {0:0.0} ms. FPS: ({1:0.} fps)", msec, fps);
        textField.text = text;
    }
}