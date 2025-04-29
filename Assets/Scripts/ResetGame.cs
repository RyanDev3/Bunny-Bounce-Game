using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleReset : MonoBehaviour
{
    [Tooltip("Choose which scene to load when resetting")]
    public string targetScene = "Tutorial"; // Default

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!string.IsNullOrEmpty(targetScene))
                SceneManager.LoadScene(targetScene);
            else
                Debug.LogError("No scene selected for reset!");
        }
    }
}