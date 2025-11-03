using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    public PanelActivator panelActivator;
    public CoinCollectEffect coinCollectEffect;
    public CoinCollectEffect gemsCollectEffect;
    public void addCoins(int coins)
    {
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.AddCoins(coins);
        }
    }
    public void addGems(int Gems)
    {
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.AddGems(Gems);
        }
    }
    public void playCoinCollectionEffect()
    {
        coinCollectEffect.PlayEffect();
    }
    public void playGemsCollectionEffect() 
    {

        gemsCollectEffect.PlayEffect();


    }
}
