using UnityEngine;
using TMPro;

public class HiddenCollecatbleManager : MonoBehaviour
{
    public static HiddenCollecatbleManager Instance;
    public TMP_Text coinText;
    public int CurrentCoins = 0;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        coinText.text = "Collectables Found: " +CurrentCoins.ToString();
    }

    public void IncreaseCollectables(int v)
    {
        CurrentCoins += v;
        coinText.text = "Collectables Found: " + CurrentCoins.ToString();
    }
}
