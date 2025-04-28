using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleReset : MonoBehaviour
{
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
}