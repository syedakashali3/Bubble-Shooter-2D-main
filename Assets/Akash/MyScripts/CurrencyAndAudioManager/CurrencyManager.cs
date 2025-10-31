using System.Collections;
using UnityEngine;

public class CurrencyManager : MonoBehaviour {

	public static CurrencyManager instance = null;

	public delegate void OnCoinsChangeEvent(string _currencyName);
	public static OnCoinsChangeEvent coinsChangedEvent;

	public delegate void OnGemsChangeEvent(string _currencyName);
	public static OnGemsChangeEvent gemsChangedEvent;

	public string _coinsPref = "Money";
	public string _gemsPref = "Crystals";
	[SerializeField]
	public int _CoinDefaultValue = 0;
	[SerializeField]
	public int _GemsDefaultValue = 0;


	// Use this for initialization

	void Awake()
	{
		if (!instance)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
		if (!PlayerPrefs.HasKey("firstTimeUpdate"))
		{
			PlayerPrefs.SetInt(_coinsPref,PlayerPrefs.GetInt(_coinsPref)+_CoinDefaultValue);
			PlayerPrefs.SetInt(_gemsPref, _GemsDefaultValue);
			PlayerPrefs.SetInt("firstTimeUpdate", 1);

		}

	}

	public void AddCoins(int _coins)
	{
		AddCurrency(_coinsPref, _coins);
	}


    /*	public void SubtractCoins(int _coins)
        {

            SubtractCurrency(_coinsPref, _coins);

        }*/
    public int SubtractCoins(int _coins)
    {
        return SubtractCurrency(_coinsPref, _coins);
    }


    public void AddGems(int _gems)
	{
		AddCurrency(_gemsPref, _gems);
	}


	public void SubtractGems(int _gems)
	{

		SubtractCurrency(_gemsPref, _gems);

	}

	public int GetCoins()
	{
		return PlayerPrefs.GetInt(_coinsPref, _CoinDefaultValue);
	}
	public int GetGems()
	{
		return PlayerPrefs.GetInt(_gemsPref, _GemsDefaultValue);
	}

	void AddCurrency(string _pref, int _value)
	{
		int _defaultvalue = _pref == _coinsPref ? _CoinDefaultValue : _GemsDefaultValue;
		int _oldvalue = PlayerPrefs.GetInt(_pref, _defaultvalue);
		PlayerPrefs.SetInt(_pref, _oldvalue + _value);

		if (_pref == _coinsPref)
		{
			if (coinsChangedEvent != null)
				coinsChangedEvent.Invoke(_pref);
		}
		else
		{
			if (gemsChangedEvent != null)
				gemsChangedEvent.Invoke(_pref);
		}
	}

    /*void SubtractCurrency(string _pref, int _value)
	{
		int _defaultvalue = _pref == _coinsPref ? _CoinDefaultValue : _GemsDefaultValue;

		int _oldvalue = PlayerPrefs.GetInt(_pref, _defaultvalue);
		if (_oldvalue - _value < 0)
		{
			Debug.Log("'dont have enough coins");
			return;
		}
		PlayerPrefs.SetInt(_pref, _oldvalue - _value);

		if (_pref == _coinsPref)
		{
			if (coinsChangedEvent != null)
				coinsChangedEvent.Invoke(_pref);
		}
		else
		{
			if (gemsChangedEvent != null)
				gemsChangedEvent.Invoke(_pref);
		}
	}*/
    int SubtractCurrency(string _pref, int _value)
    {
        int _defaultvalue = _pref == _coinsPref ? _CoinDefaultValue : _GemsDefaultValue;
        int _oldvalue = PlayerPrefs.GetInt(_pref, _defaultvalue);

        if (_oldvalue < _value)
        {
            // not enough currency
            int remainingNeeded = _value - _oldvalue;

            // set coins to 0
            PlayerPrefs.SetInt(_pref, 0);

            Debug.Log($"Not enough {_pref}. Needed {remainingNeeded} more.");

            if (_pref == _coinsPref && coinsChangedEvent != null)
                coinsChangedEvent.Invoke(_pref);
            else if (_pref == _gemsPref && gemsChangedEvent != null)
                gemsChangedEvent.Invoke(_pref);

            return remainingNeeded; // return how many more are needed
        }

        // enough coins, subtract normally
        PlayerPrefs.SetInt(_pref, _oldvalue - _value);

        if (_pref == _coinsPref && coinsChangedEvent != null)
            coinsChangedEvent.Invoke(_pref);
        else if (_pref == _gemsPref && gemsChangedEvent != null)
            gemsChangedEvent.Invoke(_pref);

        return 0; // 0 means deduction was successful
    }

    public int BuyCoins()
    {
        return PlayerPrefs.GetInt(_coinsPref, _CoinDefaultValue);
    }

}
