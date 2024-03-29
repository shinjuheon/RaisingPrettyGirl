using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace gunggme
{
    [Serializable]
    public enum GachaType
    {
        skin,
        pet,
        weapon,
        ring,
        necklace,
        earring,
        skill
    }

    [Serializable]
    public class GachaSkin
    {
        public List<SkinData> SkinDatas;
    }

    [Serializable]
    public class Pet
    {
        public List<PetData> PetDatas;
    }

    [Serializable]
    public class ItemIdList
    {
        public List<int> ItemId;
    }

    [Serializable]
    public class ItemData
    {
        public List<ItemIdList> ItemList;
    }

    [Serializable]
    public class SkillNameList
    {
        public List<string> SkillList;
    }
    public class GachaSystem : MonoBehaviour
    {
        [Header("뽑기에서 나오는 모든것")]
        [SerializeField] private List<Skin> _gachaSkins; // [등급][idx]
        [SerializeField] private List<Pet> _gachaPets;
        [SerializeField] private List<ItemData> _gachaitems;   // [종류][등급][idx]
        [SerializeField] private List<SkillNameList> _skillNames;

        private Animator _gachaAnim;
        private RectTransform _backLightTrans;
        private Image _cardImg;
        private Queue<Sprite> _spriteQueue;

        private PercentageDataManager _percentageDataManager;
        private PetSystem _petSystem;
        private PlayerSkinSystem _playerSkinSystem;
        private Inventory _inventory;
        private SkillSystem _skillSystem;
        private GoodsManager _goodsManager;

        public bool IsFirst = false;
        private void Awake()
        {
            _gachaAnim = transform.GetChild(0).GetComponent<Animator>();
            _backLightTrans = _gachaAnim.GetComponent<RectTransform>();
            _cardImg = _gachaAnim.transform.GetChild(0).GetComponent<Image>();
            _spriteQueue = new Queue<Sprite>();
            
            _percentageDataManager = GameObject.Find("PercentageDataManager").GetComponent<PercentageDataManager>();
            _playerSkinSystem = GameObject.Find("Player").GetComponent<PlayerSkinSystem>();
            _petSystem = _playerSkinSystem.gameObject.GetComponent<PetSystem>();
            _skillSystem = GameObject.Find("SkillSystem").GetComponent<SkillSystem>();
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            _goodsManager = GameObject.Find("GoodsManager").GetComponent<GoodsManager>();
        }

        private void Update()
        {
            if (!gameObject.activeSelf)
                return;
            
            if (Input.GetMouseButtonDown(0) || IsFirst)
            {
                _backLightTrans.localScale = Vector3.zero;
                if (_spriteQueue.Count > 0)
                {
                    _cardImg.sprite = _spriteQueue.Dequeue();
                    _gachaAnim.SetTrigger("Show");
                    IsFirst = false;
                }
                else
                {
                    IsFirst = false;
                    _cardImg.sprite = null;
                    gameObject.SetActive(false);
                }
            }
        }

        public void GachaCall(GachaType type)
        {
            Debug.Log(_spriteQueue.Count + " 현재 남은 뽑기 수");
            int rarity = -1;
            int randomIdx;
            float randomValue = Random.Range(0f, 100f);
            for (int i = 6; i >= 0; i--)
            {
                float value = float.Parse(_percentageDataManager.Data_skin[i]["per"].ToString());
                if (randomValue <= value)
                {
                    rarity = i;
                    break;
                }

                randomValue -= value;
            }

            if (rarity == -1)
            {
                throw new Exception("Rarity 변경 안됨");
            }
            
            switch (type)
            {
                case GachaType.skin:
                    randomIdx = Random.Range(0, _gachaSkins[rarity].SkinDatas.Count);
                  
                    _spriteQueue.Enqueue(Resources.Load<SpriteAtlas>("Skin/" + _gachaSkins[rarity].SkinDatas[randomIdx].Name).GetSprite("Body"));
                    
                    _playerSkinSystem.AddSKin(_gachaSkins[rarity].SkinDatas[randomIdx]);
                    break;
                
                case GachaType.pet:
                    randomIdx = Random.Range(0, _gachaPets[rarity].PetDatas.Count);
                    
                    _spriteQueue.Enqueue(Resources.Load<Sprite>("Pet/" + _gachaPets[rarity].PetDatas[randomIdx].Name));
                    
                    _petSystem.AddPet(_gachaPets[rarity].PetDatas[randomIdx]);
                    break;
                
                case GachaType.weapon:
                    randomIdx = Random.Range(0, _gachaitems[0].ItemList[rarity].ItemId.Count);
                    
                    Debug.Log($"{rarity} {randomIdx}");
                    Item item = ItemDataManager.Instance.GetItem(_gachaitems[0].ItemList[rarity].ItemId[randomIdx], 0);
                    _spriteQueue.Enqueue(Resources.Load<Sprite>("Equipment/0/"+item.ItemID));

                    _inventory.AddItem(item);
                    _goodsManager.GetScroll(5);
                    break;
                
                case GachaType.earring:
                    randomIdx = Random.Range(0, _gachaitems[1].ItemList[rarity].ItemId.Count);
                    
                    Item earringItem = ItemDataManager.Instance.GetItem(_gachaitems[1].ItemList[rarity].ItemId[randomIdx], 3);
                    _spriteQueue.Enqueue(Resources.Load<Sprite>("Equipment/1/"+earringItem.ItemID));

                    _inventory.AddItem(earringItem);
                    _goodsManager.GetScroll(5);
                    break;
                
                case GachaType.necklace:
                    randomIdx = Random.Range(0, _gachaitems[2].ItemList[rarity].ItemId.Count);

                    Item neckItem = ItemDataManager.Instance.GetItem(_gachaitems[2].ItemList[rarity].ItemId[randomIdx], 1);
                    _spriteQueue.Enqueue(Resources.Load<Sprite>("Equipment/2/"+neckItem.ItemID));
                    
                    _inventory.AddItem(neckItem);
                    _goodsManager.GetScroll(5);
                    break;
                
                case GachaType.ring:
                    randomIdx = Random.Range(0, _gachaitems[3].ItemList[rarity].ItemId.Count);
                    
                    Item ringItem = ItemDataManager.Instance.GetItem(_gachaitems[3].ItemList[rarity].ItemId[randomIdx], 2);
                    _spriteQueue.Enqueue(Resources.Load<Sprite>("Equipment/3/"+ringItem.ItemID));

                    _inventory.AddItem(ringItem);
                    _goodsManager.GetScroll(5);
                    break;
                
                case GachaType.skill:
                    randomIdx = Random.Range(0, _skillNames[rarity].SkillList.Count);
                    
                    _spriteQueue.Enqueue(Resources.Load<SpriteAtlas>("Skill/Skills").GetSprite(_skillNames[rarity].SkillList[randomIdx]));
                    
                    _skillSystem.GetSkill(_skillNames[rarity].SkillList[randomIdx]);
                    break;
            }

            if (rarity == -1)
            {
                throw new Exception("rarity 변수 값이 안바뀜");
            }
        }
    }
}
