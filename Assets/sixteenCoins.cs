using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using KModkit;

public class sixteenCoins : MonoBehaviour
{
	
	public KMBombInfo Bomb;
	public KMBombModule Module;
	public KMAudio Audio;
	public KMSelectable[] coins;
	public TextMesh[] coinTextMeshes;

	private static int _moduleIdCounter = 1;
	private int _moduleId = 0;
	private bool[] coinWasInitiallyOne = new bool[16];
	private int indexOfDesiredCoin;


	// Run once per module, while loading screen shows
	void Start () 
	{
		_moduleId = _moduleIdCounter++;
		GenerateModule();
	}

	void GenerateModule ()
	{
		for (int i = 0; i < coins.Length; i++)
		{
			RandomizeAndSetCoinAtIndex(i);
			indexOfDesiredCoin = DetermineIndexOfDesiredCoin();
		}
	}


	void RandomizeAndSetCoinAtIndex(int index)
	{
		bool willBeAOne = Random.Range(0, 2) == 0;  // 50%
		coinWasInitiallyOne[index] = willBeAOne;
		if (willBeAOne)
		{
			AssignStringToCoinAtIndex(index, "1");
		}
		else
		{
			AssignStringToCoinAtIndex(index, "0");
		}
	}

	void AssignStringToCoinAtIndex(int index, string str)
	{
		coinTextMeshes[index].text = str;
	}

	int DetermineIndexOfDesiredCoin ()
	{
		return 4;
	}

	void Awake () 
	{
		for (int i = 0; i < coins.Length; i++) 
		{
			int j = i;
			coins[i].OnInteract += delegate () 
			{
				HandleClickCoinAtIndex(j);
				return false;
			};
		}
	}

	void HandleClickCoinAtIndex (int index)
	{
		Audio.PlayGameSoundAtTransform(
			KMSoundOverride.SoundEffect.ButtonPress, coins[index].transform);
		coins[index].AddInteractionPunch();
		FlipCoinAtIndex(index);
		if (indexOfDesiredCoin != index)
		{
			Module.HandleStrike();
		}
		else
		{
			Module.HandlePass();
		}
	}

	void FlipCoinAtIndex (int index)
	{
		coinTextMeshes[index].text = coinTextMeshes[index].text == "0" ? "1" : "0";
	}

}
