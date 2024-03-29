using System;
using System.Collections;
using System.Collections.Generic;
using gunggme;
using UnityEngine;
using Random = UnityEngine.Random;

public class RewardSystem : MonoBehaviour
{
    private Inventory _inventory;

    private List<int>[] weaponList = new List<int>[3] {new List<int>(), new List<int>(), new List<int>()};

    private StageManager _stageManager;
    
    private void Awake()
    {
        _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        _stageManager = FindObjectOfType<StageManager>();
        
        var weapons = Resources.LoadAll<Sprite>("Equipment/0");

        foreach (var weapon in weapons)
        {
            switch (weapon.name[0])
            {
                case '1':
                    weaponList[0].Add(int.Parse(weapon.name));
                    break;
                case '2':
                    weaponList[1].Add(int.Parse(weapon.name));
                    break;
                case '3':
                    weaponList[2].Add(int.Parse(weapon.name));
                    break;
                
                default:
                    return;
            }
        }
    }

    public void GetReward()
    {
        float randomPercent = Random.Range(0f, 100f);

        if (/*randomPercent <= 70f &&*/ _stageManager.CurrentStage <= 10)
        {
            int randomIdx = Random.Range(0, weaponList[0].Count);
            _inventory.AddItem(ItemDataManager.Instance.GetItem(weaponList[0][randomIdx], 0));
        }
        else if (/*randomPercent <= 90f &&*/ _stageManager.CurrentStage <= 20)
        {
            int randomIdx = Random.Range(0, weaponList[1].Count);
            _inventory.AddItem(ItemDataManager.Instance.GetItem(weaponList[1][randomIdx], 0));
        }
        else if (/*randomPercent <= 100f &&*/ _stageManager.CurrentStage >= 30)
        {
            int randomIdx = Random.Range(0, weaponList[2].Count);
            _inventory.AddItem(ItemDataManager.Instance.GetItem(weaponList[2][randomIdx], 0));
        }
    }
}
