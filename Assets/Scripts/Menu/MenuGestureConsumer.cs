using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mediapipe.Unity.Sample.HandTracking;

public class MenuGestureConsumer : MonoBehaviour
{   
    // public PrefabManager prefabManager;
    public string mainSceneName = "Draw"; 


    private float coolDown = 1.0f;
    private float lastTime = 0f;

    void Start()
    {
        PrefabManager.Instance.currentInstance.SetActive(true);
        lastTime = Time.time;
        HandGesture.OnGestureDetected += HandleGestureDetected;
    }

    void OnEnable()
    {
        HandGesture.OnGestureDetected += HandleGestureDetected;
    }

    void OnDisable()
    {
        HandGesture.OnGestureDetected -= HandleGestureDetected;
    }

    private void HandleGestureDetected(HandGestureData data)
    {
        if (data.IsGestureDetected && Time.time > lastTime + coolDown)
        {
            lastTime = Time.time;
            switch (data.GestureType)
            {
                case 1:  // Index finger Pinched with Thumb
                    // Debug.Log("Returning to Main Scene");
                    // ReturnToMainScene();
                    break;
                case 2:  // Index finger Pinched with Thumb
                    Debug.Log("Switching to next Shape.");
                    PrefabManager.Instance.NextShape();
                    break;
                case 3:  // All fingers converged
                    Debug.Log("Switching to next Color.");
                    PrefabManager.Instance.NextColor();
                    break;
                case 4:  // All fingers converged
                    Debug.Log("Switching to next Size.");
                    PrefabManager.Instance.NextSize();
                    break;
                case 5:  // All fingers converged
                    Debug.Log("Returning to Main Scene");
                    ReturnToMainScene();
                    break;
                default:
                    Debug.Log("Gesture not assigned to any action.");
                    break;
            }
        }
    }

    void ReturnToMainScene()
    {
        SceneManager.UnloadSceneAsync("Menu");
    }

    void OnDestroy()
    {
        PrefabManager.Instance.currentInstance.SetActive(false);
        HandGesture.OnGestureDetected -= HandleGestureDetected;
    }
}
