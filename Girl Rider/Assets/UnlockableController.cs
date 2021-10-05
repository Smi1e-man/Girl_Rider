using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableController : MonoBehaviour
{
    [System.Serializable]
    public class UnlockableItem
    {
        public Sprite sprite;
        public float deltaProgress;
    }

    public List<UnlockableItem> unlockableItems;


    public UnlockableItem currentUnlockableItem;

    public int currentUnlockableIndex;
    public float currentUnlockableProgress;

    public bool isAnythingToUnlock;


   
    public void Start()
    {
        LoadData();
    }

    public void LoadData()
    {
        currentUnlockableIndex = PlayerPrefs.GetInt("currentUnlockableIndex", 0);
        currentUnlockableProgress = PlayerPrefs.GetFloat("currentUnlockableProgress", 0);

        isAnythingToUnlock = currentUnlockableIndex < unlockableItems.Count;

        if (isAnythingToUnlock)
            currentUnlockableItem = unlockableItems[currentUnlockableIndex];
    }

    public bool AddProgressAndSave()
    {
        currentUnlockableProgress = Mathf.Clamp01(currentUnlockableProgress + currentUnlockableItem.deltaProgress);
        bool isUnlocked = false;

        //Debug.Log("************ PROGRESS ADDED");
        if (Mathf.RoundToInt(currentUnlockableProgress * 100) >= 100)
        {
            isUnlocked = true;
            currentUnlockableIndex++;
            currentUnlockableProgress = 0;

            //Debug.Log("************ PROGRESS UNLOCKED");

        }

        Save();

        return isUnlocked;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("currentUnlockableIndex", currentUnlockableIndex);
        PlayerPrefs.SetFloat("currentUnlockableProgress", currentUnlockableProgress);
    }

}
