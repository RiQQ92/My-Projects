using UnityEngine;
using System.Collections;

public class worldLevelInfo : MonoBehaviour
{
	public bool isComplete = false;
	public bool isSecretComplete = false;

	public int levelIndex = 1;

	void Start()
	{
		for(int i = 0; i < publicStorage.levelsFinished.Count; i++)
		{
			if((int)publicStorage.levelsFinished[i] == levelIndex)
			{
				isComplete = true;
				break;
			}
		}
	}
}
