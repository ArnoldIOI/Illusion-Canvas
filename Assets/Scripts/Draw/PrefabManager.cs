using UnityEngine;


public class PrefabManager : MonoBehaviour
{
    public GameObject[] prefabs;    
    public Vector3[] sizes;       

    private int currentShapeIndex = 0; // 0, 1
    private int currentColorIndex = 0; // 0, 1, 2
    private int currentSizeIndex = 0;
    private int index = 0;

    public static PrefabManager Instance { get; private set; }  
    public GameObject currentInstance;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreateNewInstance();
        PrefabManager.Instance.currentInstance.SetActive(false);
    }

    public void NextShape()
    {
        currentShapeIndex = (currentShapeIndex + 1) % 2;
        UpdatePrefabInstance();
    }

    public void NextColor()
    {
        currentColorIndex = (currentColorIndex + 1) % 3;
        // UpdateColor();
        UpdatePrefabInstance();
    }

    public void NextSize()
    {
        currentSizeIndex = (currentSizeIndex + 1) % sizes.Length;
        UpdateSize();
    }

    private void CreateNewInstance()
    {
        if (currentInstance != null)
            Destroy(currentInstance);
        if (currentShapeIndex == 0)
        {
            index = currentColorIndex;
        }
        else
        {
            index = currentColorIndex + 3;
        }
        currentInstance = Instantiate(prefabs[index], Vector3.zero, Quaternion.identity);
        // UpdateColor();
        UpdateSize();
    }

    private void UpdatePrefabInstance()
    {
        CreateNewInstance(); 
    }

    // private void UpdateColor()
    // {
    //     if (currentInstance != null)
    //     {
    //         Renderer renderer = currentInstance.GetComponent<Renderer>();
    //         if (renderer != null)
    //         {
    //             Material material = new Material(renderer.sharedMaterial); // 创建新的材质副本
    //             material.color = colors[currentColorIndex]; // 设置颜色
    //             renderer.material = material; // 应用新的材质
    //         }
    //     }
    // }


    private void UpdateSize()
    {
        if (currentInstance != null)
            currentInstance.transform.localScale = sizes[currentSizeIndex];
    }
}
