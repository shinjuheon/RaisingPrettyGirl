using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace gunggme
{
    [Serializable]
    public enum Rarity
    {
        normal = 0,
        advanced = 1,
        rare = 2,
        hero = 3,
        legend = 4,
        transce = 5,
        god = 6,
    }
    [Serializable]
    public class Skin
    {
        public List<SkinData> SkinDatas;
    }

    public class PlayerSkinSystem : MonoBehaviour
    {
        public SpriteRenderer[] parts;

        private void Start()
        {
            Debug.Log(SaveManager.Instance.CurSkin.Name);
            ChangeSkin(SaveManager.Instance.CurSkin);
        }

        public void AddSKin(SkinData skinData)
        {
            if(SaveManager.Instance.haveSkins[skinData.Rarity].SkinDatas.Find(i => i.Name == skinData.Name) == null)
            {
                Debug.Log($"{skinData.Rarity}, {skinData.Name}");
                SaveManager.Instance.haveSkins[skinData.Rarity].SkinDatas.Add(skinData);
            }
        }
        public void ChangeSkin(Rarity rarity, int skinIdx)
        {
            foreach(SpriteRenderer part in parts)
            { 
                if (SaveManager.Instance.haveSkins[(int)rarity].SkinDatas[skinIdx].ToAtlas().GetSprite(part.name) == null) part.sprite = null;
               part.sprite = SaveManager.Instance.haveSkins[(int)rarity].SkinDatas[skinIdx].ToAtlas().GetSprite(part.name);
            }
            SaveManager.Instance.CurSkin = SaveManager.Instance.haveSkins[(int)rarity].SkinDatas[skinIdx];
        }

        public void ChangeSkin(SkinData skinData)
        {
            foreach(SpriteRenderer part in parts)
            {
                if (skinData.ToAtlas().GetSprite(part.name) == null) part.sprite = null;
                part.sprite = skinData.ToAtlas().GetSprite(part.name);
            }
            SaveManager.Instance.CurSkin = skinData;
        }
    }
}
 