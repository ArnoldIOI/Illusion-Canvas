using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mediapipe.Unity.Sample.HandTracking
{
    public class GestureConsumer : MonoBehaviour
    {   
        public string exportDir;
        public GameObject objectToSpawn;
        public float spawnCooldown = 0f; // seconds
        public float rotationSpeed = 20f; // Speed of rotation interpolation
        private Quaternion targetRotation;
        private float lastSpawnTime = 0f;
        private float sceneLoadCooldown = 1.0f;
        private float lastLoadTime = 0f;

        private float lastExportTime = 0f;
        private float exportCooldown = 5.0f;

        // private int exportCount = 0;
        private bool isRotationActiveY = false;
        private bool isRotationActiveX = false;

        void Start()
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
            lastLoadTime = Time.time;
            lastExportTime = Time.time;
            HandGesture.OnGestureDetected += HandleGestureDetected;
            targetRotation = transform.rotation;
        }

        void OnEnable()
        {
            HandGesture.OnGestureDetected += HandleGestureDetected;
        }

        void OnDisable()
        {
            HandGesture.OnGestureDetected -= HandleGestureDetected;
        }

        void Update()
        {
            if (isRotationActiveY)
            {
                float rotationThisFrame = rotationSpeed * Time.deltaTime;
                transform.Rotate(0, rotationThisFrame, 0);
            }
            if (isRotationActiveX)
            {
                float rotationThisFrame = rotationSpeed * Time.deltaTime;
                transform.Rotate(rotationThisFrame, 0, 0);
            }
        }

        private void HandleGestureDetected(HandGestureData data)
        {
            isRotationActiveY = false;
            isRotationActiveX = false;
            if (SceneManager.sceneCount > 1)
            {
                lastLoadTime = Time.time + 3.0f;
                gameObject.SetActive(false);
                // Debug.Log("It's in Menu Now!");
                return;
            } 
            else
            {
                gameObject.SetActive(true);
            }
            if (data.IsGestureDetected && Time.time >= lastSpawnTime + spawnCooldown)
            {
                Debug.Log("Hand gesture detected in consumer script.");
                if (data.GestureType == 1) // 0-None; 1-index; 2-middle; 3-ring; 4-pinky; 5-convergence
                {
                    Debug.Log("Index finger Pinched with Thumb");
                    SpawnNewObj(data.GesturePosition);
                    lastSpawnTime = Time.time;
                }
                else if(data.GestureType == 2) // Middle finger
                {
                    Debug.Log("Middle finger Pinched with Thumb - Try to Export");
                    if (Time.time > lastExportTime + exportCooldown)
                    {
                        exportOBJ();
                        Debug.Log("Exporting...");
                        lastExportTime = Time.time;
                    }

                }
                else if(data.GestureType == 3) // Ring finger
                {
                    Debug.Log("Ring finger Pinched with Thumb - Rotating Object y");
                    isRotationActiveY = true;
                }
                else if(data.GestureType == 4) // Pinky finger
                {
                    Debug.Log("Pinky finger Pinched with Thumb - Rotating Object x");
                    isRotationActiveX = true;
                }
                else if(data.GestureType == 5)
                {
                    Debug.Log("All fingers converged - Go to Menu");
                    if (Time.time >= lastLoadTime + sceneLoadCooldown) {
                        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
                        lastLoadTime = Time.time;
                    }
                }
            }
        }

        private void SpawnNewObj(Vector3 position)
        {
            Camera mainCamera = Camera.main;
            float cameraDistance = 10f;

            // flip y due to different coordination system of Unity and MediaPipe
            Vector3 screenPosition = new Vector3(position.x * UnityEngine.Screen.width, (1 - position.y) * UnityEngine.Screen.height, position.z + cameraDistance);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

            GameObject newObj = Instantiate(PrefabManager.Instance.currentInstance, worldPosition, Quaternion.identity);
            newObj.SetActive(true);
            newObj.transform.SetParent(this.transform);

            int targetLayer = LayerMask.NameToLayer("Draw");
            newObj.layer = targetLayer;

            if (newObj.TryGetComponent<Renderer>(out Renderer renderer))
            {
                Material prefabMaterial = PrefabManager.Instance.currentInstance.GetComponent<Renderer>().sharedMaterial;
                Material newMaterial = new Material(prefabMaterial);
                renderer.material = newMaterial;
            }
        }

        private void RotateObject(float degrees)
        {
            // Update the target rotation
            targetRotation *= Quaternion.Euler(0, degrees, 0);
        }

        private void exportOBJ()
        {
            string exportPath = exportDir + Time.time.ToString() + ".obj";
            GameObjectExporter.ExportOBJ(exportPath, "Canvas");
        }

        void OnDestroy()
        {
            HandGesture.OnGestureDetected -= HandleGestureDetected;
        }
    }
}
