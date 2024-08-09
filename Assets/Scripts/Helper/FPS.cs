using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public Text fpsText;
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        float msec = deltaTime * 1000.0f;
        fpsText.text = $"{msec:0.0} ms ({fps:0.} fps)";
    }
}
