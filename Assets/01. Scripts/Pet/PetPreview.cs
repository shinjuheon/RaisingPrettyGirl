using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class PetPreview : MonoBehaviour
    {
        public Image petSprite;
        //public List<Skin> haveskins; // skins[등급][번호]

        private int bestRarity;

        public int _curRarity = 0;
        public int curRarity
        {
            get => _curRarity;
            set
            {
                _curRarity = value;
                if (_curRarity > bestRarity)
                {
                    _curRarity = 0;
                }
                else if (_curRarity < 0)
                {
                    _curRarity = bestRarity;
                }
            }
        }

        public int _curPetIdx;

        public int curPetIdx
        {
            get => _curPetIdx;
            set
            {
                _curPetIdx = value;
                if (_curPetIdx >= SaveManager.Instance.havePets[curRarity].PetDatas.Count)
                {
                    _curPetIdx = 0;
                    curRarity++;
                }
                else if (_curPetIdx < 0)
                {
                    _curPetIdx = SaveManager.Instance.havePets[curRarity].PetDatas.Count - 1;
                    curRarity--;
                }
            }
        }
        
        [SerializeField] private GameObject _lastpageMessage;

        public void OnEnable()
        {
            FindBestRarity();
            ChangeFirstPet();
        }
        
        //현재 있는 스킨중 가장 높은 등급 탐색
        private void FindBestRarity()
        {
            for(int i = 6; i >= 0; i--)
            {
                if (SaveManager.Instance.havePets[i].PetDatas.Count != 0)
                {
                    bestRarity = i;
                    return;
                }
            }
        }

        private void ChangeFirstPet()
        {
            for (int i = 0; i < SaveManager.Instance.havePets.Count; i++)
            {
                if (SaveManager.Instance.havePets[i].PetDatas.Count > 0)
                {
                    petSprite.sprite = SaveManager.Instance.havePets[i].PetDatas[0].ToSprite();
                    curRarity = i;
                    curPetIdx = 0;
                    return;
                }
            }
        }
        
        //스킨 변경 UI 미리보기 변경시 사용(죄송합니다...)
        public void ChangePet_UI(int rarity, int petIdx)
        {
            try
            {
                if (SaveManager.Instance.havePets[rarity].PetDatas[petIdx].ToSprite() != null)
                    petSprite.sprite = SaveManager.Instance.havePets[rarity].PetDatas[petIdx].ToSprite();
                else
                    petSprite.sprite = null;
            }
            catch (ArgumentOutOfRangeException e)
            {
                NextPreview(1);
            }
        }
        
        public void NextPreview(int val)
        {
            if (val != -1 && val != 1)
            {
                Debug.LogError($"val: {val} is not 1 or -1");
                return;
            }
            
            //현재 가지고 있는 스킨이 하나 이하면 return
            int petCount = SaveManager.Instance.havePets.Sum(t => t.PetDatas.Count);
            if (petCount <= 1)
            {
                if (!_lastpageMessage.activeSelf)
                {
                    _lastpageMessage.SetActive(true);
                }
                return;
            }
            curPetIdx += val;

            while (true)
            {
                if (SaveManager.Instance.havePets[curRarity].PetDatas.Count > 0)
                {
                    ChangePet_UI(curRarity, curPetIdx);
                    return;
                }
             
                curPetIdx+=val;
            }
        }
    }
}
