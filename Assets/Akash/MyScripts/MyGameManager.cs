using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGameManager : MonoBehaviour
{
    public static MyGameManager instance;
    public PanelActivator panelActivator;
    public CoinCollectEffect coinCollectEffect;
    public CoinCollectEffect gemsCollectEffect;
    public LevelManager levelManagerRef;
    public Image progressFillBar;
    public List<GameObject> starsFillParentGo;

    private const string StarsKeyPrefix = "StarsParent_";
    private const string ProgressKey = "ProgressFill";
    private void Awake()
    {
        
        instance = this;
    }
    private void Start()
    {
        loadEnableStarsFillParent();
        loadProgressFillBar();
    }

    public void addCoins(int coins)
    {
        if (CurrencyManager.instance != null)
            CurrencyManager.instance.AddCoins(coins);
    }

    public void addGems(int gems)
    {
        if (CurrencyManager.instance != null)
            CurrencyManager.instance.AddGems(gems);
    }

    public void playCoinCollectionEffect()
    {
        coinCollectEffect.PlayEffect();
    }

    public void playGemsCollectionEffect()
    {
        gemsCollectEffect.PlayEffect();
    }

    // ✅ Add new value to existing progress, clamp to 1f, then save
    public void doProgressFillBar(float val)
    {
        float previousVal = PlayerPrefs.GetFloat(ProgressKey, 0f);
        float newVal = Mathf.Clamp01(previousVal + val);

        progressFillBar.fillAmount = newVal;
        saveProgressFillBar(newVal);
    }

    // ✅ Enable stars for current level and save
    public void EnablestarsFillParent()
    {
        int index = levelManagerRef.currentLevel;

        if (index >= 0 && index < starsFillParentGo.Count)
        {
            starsFillParentGo[index].SetActive(true);
            saveEnableStarsFillParent(index);
        }
    }

    // ✅ Save which stars are enabled
    public void saveEnableStarsFillParent(int index)
    {
        PlayerPrefs.SetInt(StarsKeyPrefix + index, 1);
        PlayerPrefs.Save();
    }

    // ✅ Load previously enabled stars
    public void loadEnableStarsFillParent()
    {
        for (int i = 0; i < starsFillParentGo.Count; i++)
        {
            bool wasEnabled = PlayerPrefs.GetInt(StarsKeyPrefix + i, 0) == 1;
            starsFillParentGo[i].SetActive(wasEnabled);
        }
    }

    // ✅ Save total progress
    private void saveProgressFillBar(float val)
    {
        PlayerPrefs.SetFloat(ProgressKey, val);
        PlayerPrefs.Save();
    }

    // ✅ Load total progress
    private void loadProgressFillBar()
    {
        float savedVal = PlayerPrefs.GetFloat(ProgressKey, 0f);
        progressFillBar.fillAmount = savedVal;
    }
    public void restartGameScene()
    {
        if(MySceneLoader.instance != null)
        {
            MySceneLoader.instance.LoadScene("LevelsMode");

        }
    }
}
