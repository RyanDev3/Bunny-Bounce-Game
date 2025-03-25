using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public float currentTime;
    public bool countDown;



    void Start()
    {
        
    }
    void Update()
    {
        currentTime = countDown ? currentTime -= Time.deltaTime : currentTime += Time.deltaTime;

        if (countDown && currentTime < 0)
            currentTime = 0;

        timerText.text = currentTime.ToString("0.00");

    }
}


