using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public enum GoodsType
    {
        Money,
        Diamond,
        Coin,
    }

    [Serializable]
    public class ItemPackage
    {
        public int ItemId;
        public int ItemType;
        public int Enforce;
    }
    
    public class ShopItem : ScriptableObject
    {
        public string ItemName;
        public string[] ItemInformations;
        public Sprite itemSprite;
        public int Price;
        public bool IsBuy;
        public GoodsType Type;
        
        public string GetInformation()
        {
            string temp = ""; 
             foreach (var infor in ItemInformations)
            {
                temp += infor + "\n";
            }

            return temp;
        }
        
        public virtual void ApplyReward()
        {
            Debug.Log("저장");
#if !UNITY_EDITOR
            SaveManager.Instance.UpdateData();
#endif
        }
    }
}
