using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUi : MonoBehaviour {

	public Text _coinsText;
	public Text _gemsText;
	bool _updatedOnce = false;

	// Use this for initialization
	void OnEnable()
	{
		CurrencyManager.coinsChangedEvent += UpdateCoinsUi;
		//CurrencyManager.gemsChangedEvent += UpdateGemsUi;
	}

	void Start()
	{
		if (!_updatedOnce)
		{
			UpdateCoinsUi(CurrencyManager.instance._coinsPref);
			//UpdateGemsUi(CurrencyManager.instance._gemsPref);


		}
	}
	// Update is called once per frame
	void OnDisable()
	{
		CurrencyManager.coinsChangedEvent -= UpdateCoinsUi;
		//CurrencyManager.gemsChangedEvent -= UpdateGemsUi;
	}

	void UpdateCoinsUi(string _currencyName)
	{
		if (!_coinsText)
			return;
		int _value = PlayerPrefs.GetInt(_currencyName, 0);
		_coinsText.text = "" + _value;
		_updatedOnce = true;
		Debug.Log(_value);
	}

	//void UpdateGemsUi(string _currencyName)
	//{
	//	if (!_gemsText)
	//		return;

	//	int _value = PlayerPrefs.GetInt(_currencyName, 0);
	//	_gemsText.text = "" + _value;
	//	_updatedOnce = true;
	//	Debug.Log(_value);
	//}
}
