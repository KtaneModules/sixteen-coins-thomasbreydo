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


	private enum CoinType
	{
		Tails = 0,
		Heads = 1
	}
	private static int _moduleIdCounter = 1;
	private int _moduleId = 0;
	private CoinType[] currentCoinTypes = new CoinType[16];
	private int indexOfTargetCoin;

	// 0    1    2    3            0000  0001  0010  0011
	// 4    5    6    7            0100  0101  0110  0111
	// 8    9    10   11           1000  1001  1010  1011
	// 12   13   14   15           1100  1101  1110  1111

	int[] evenColumnIndices = {1, 5, 9, 13, 3, 7, 11, 15};
	int[] evenRowIndices = {4, 5, 6, 7, 12, 13, 14, 15};
	int[] rightHalfIndices = {2, 6, 10, 14, 3, 7, 11, 15};
	int[] bottomHalfIndices = {8, 9, 10, 11, 12, 13, 14, 15};

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
		}
		indexOfTargetCoin = DetermineIndexOfTargetCoin();
		Debug.LogFormat("[16 Coins #{0}]: Target coin is at index {1} (0b{2})",
				_moduleId,
				indexOfTargetCoin,
				Convert.ToString(indexOfTargetCoin, 2));
	}

	void RandomizeAndSetCoinAtIndex(int index)
	{
		CoinType initialCoinType = Random.Range(0, 2) == 0 ?
									   CoinType.Heads : CoinType.Tails;
		currentCoinTypes[index] = initialCoinType;
		SetCoinTextMeshAtIndex(index, initialCoinType);
	}

	int DetermineIndexOfTargetCoin ()
	{
		int indexOfTargetCoin = 0;
		if (IsOdd(NumberOfHeadsInBottomHalf())) indexOfTargetCoin++;
		indexOfTargetCoin <<= 1;
		if (IsOdd(NumberOfHeadsInEvenRows())) indexOfTargetCoin++;
		indexOfTargetCoin <<= 1;
		if (IsOdd(NumberOfHeadsInRightHalf())) indexOfTargetCoin++;
		indexOfTargetCoin <<= 1;
		if (IsOdd(NumberOfHeadsInEvenColumns())) indexOfTargetCoin++;
		return indexOfTargetCoin;
	}

	bool IsOdd (int x)
	{
		return x % 2 == 1;
	}

	int NumberOfHeadsInEvenColumns ()
	{
		return NumberOfHeadsInIndices(ref evenColumnIndices);
	}
	int NumberOfHeadsInRightHalf ()
	{
		return NumberOfHeadsInIndices(ref rightHalfIndices);
	}

	int NumberOfHeadsInEvenRows ()
	{
		return NumberOfHeadsInIndices(ref evenRowIndices);
	}

	int NumberOfHeadsInBottomHalf ()
	{
		return NumberOfHeadsInIndices(ref bottomHalfIndices);
	}


	int NumberOfHeadsInIndices(ref int[] indices)
	{
		int nHeads = 0;
		foreach (int i in indices)
		{
			if (currentCoinTypes[i] == CoinType.Heads) nHeads += 1;
		}
		return nHeads;
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
		if (indexOfTargetCoin != index)
		{
			Module.HandleStrike();
		}
		else
		{
			currentCoinTypes[index] = currentCoinTypes[index]
                             == CoinType.Heads ? CoinType.Tails : CoinType.Heads;
			Module.HandlePass();
		}
	}

	void SetCoinTextMeshAtIndex (int index, CoinType coinType)
	{
		if (coinType == CoinType.Heads)
		{
			coinTextMeshes[index].text = "H";
			coinTextMeshes[index].color = new Color(0.3137254901960784f, 0.0f, 0.0f);
		}
		else
		{
			coinTextMeshes[index].text = "T";
			coinTextMeshes[index].color = new Color(0.03137254901960784f, 0, 0.19215686274509805f);
		}
	}
}
