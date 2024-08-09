using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.Unity.Sample.HandTracking
{
    public class HandGesture : MonoBehaviour
    {
        public static event Action<HandGestureData> OnGestureDetected;

        private static HandGesture _instance;

        private float pinchThreshold = 0.09f;
        public float convergenceThreshold = 0.11f;

        public static HandGesture Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("HandGestureSingleton");
                    _instance = singletonObject.AddComponent<HandGesture>();
                    DontDestroyOnLoad(singletonObject);
                }
                return _instance;
            }
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void DetectGestures(NormalizedLandmarkList landmarksList)
        {
            var fingerTips = new List<Vector3>
            {
                new Vector3(landmarksList.Landmark[4].X, landmarksList.Landmark[4].Y, landmarksList.Landmark[4].Z),  // Thumb
                new Vector3(landmarksList.Landmark[8].X, landmarksList.Landmark[8].Y, landmarksList.Landmark[8].Z),  // Index
                new Vector3(landmarksList.Landmark[12].X, landmarksList.Landmark[12].Y, landmarksList.Landmark[12].Z), // Middle
                new Vector3(landmarksList.Landmark[16].X, landmarksList.Landmark[16].Y, landmarksList.Landmark[16].Z), // Ring
                new Vector3(landmarksList.Landmark[20].X, landmarksList.Landmark[20].Y, landmarksList.Landmark[20].Z)  // Pinky
            };

            if (AreFingersConverging(fingerTips))
            {
                // Debug.Log("All finger tips are converging.");
                HandGestureData gestureData = new HandGestureData(true, 5, fingerTips[0]);
                OnGestureDetected?.Invoke(gestureData);
            }
            else
            {
                DetectPinchBetweenAnyFingersAndThumb(fingerTips);
            }
        }

        private bool AreFingersConverging(List<Vector3> fingerTips)
        {
            // the first one is Thumb
            var thumbTip = fingerTips[0];
            float averageDistance = 0;
            int count = 0;

            for (int j = 1; j < fingerTips.Count; j++)
            {
                averageDistance += Vector3.Distance(thumbTip, fingerTips[j]);
                count++;
            }
            averageDistance /= count;

            return averageDistance < convergenceThreshold;
        }

        private void DetectPinchBetweenAnyFingersAndThumb(List<Vector3> fingerTips)
        {
            string[] fingerNames = { "Thumb", "Index", "Middle", "Ring", "Pinky" };
            // the first one is Thumb
            var thumbTip = fingerTips[0];
            for (int j = 1; j < fingerTips.Count; j++)
            {
                if (Vector3.Distance(thumbTip, fingerTips[j]) < pinchThreshold)
                {
                    // Debug.Log($"Pinch detected between Thumb and {fingerNames[j]}.");
                    Vector3 pinchPosition = (thumbTip + fingerTips[j]) / 2;
                    HandGestureData gestureData = new HandGestureData(true, j, pinchPosition);
                    OnGestureDetected?.Invoke(gestureData);
                    return; // only detect the first pair and ignore the rest
                }
            }
        }
    }

    public class HandGestureData
    {
        public bool IsGestureDetected;
        // public GestureType Type;
        public int GestureType;
        public Vector3 GesturePosition;

        public HandGestureData(bool isGestureDetected, int type, Vector3 gesturePosition)
        {
            IsGestureDetected = isGestureDetected;
            GestureType = type;
            GesturePosition = gesturePosition;
        }
    }

    // public enum GestureType
    // {
    //     None = 0,
    //     ThumbIndexPinch = 1,
    //     ThumbMiddlePinch = 2,
    //     ThumbRingPinch = 3,
    //     ThumbPinkyPinch = 4,
    //     Convergence = 5
    // }
}