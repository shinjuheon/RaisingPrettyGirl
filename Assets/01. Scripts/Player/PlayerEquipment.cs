using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    [Serializable]
    public class PlayerEquipData
    {
        public ItemSaveData EquipWeapon = new ItemSaveData();
        public ItemSaveData EquipNeckLace = new ItemSaveData();
        public List<ItemSaveData> EquipRings = new List<ItemSaveData>();
        public List<ItemSaveData> EquipEarrings = new List<ItemSaveData>();

        public PlayerEquipData()
        {
            
        }

        public PlayerEquipData(WeaponItem weapon, NecklaceItem necklace, List<RingItem> rings,
            List<EarringItem> earrings)
        {
            EquipWeapon = weapon != null ? new ItemSaveData(weapon) : new ItemSaveData();
            EquipNeckLace = necklace != null ? new ItemSaveData(necklace) : new ItemSaveData();
            EquipRings = new List<ItemSaveData>();
            EquipEarrings = new List<ItemSaveData>();

            foreach (var ring in rings)
            {
                if (ring != null && ring.ItemID != 0)
                {
                    EquipRings.Add(new ItemSaveData(ring));
                }
            }

            foreach (var earring in earrings)
            {
                if (earring != null && earring.ItemID != 0)
                {
                    EquipEarrings.Add(new ItemSaveData(earring));
                }
            }
            
        }

        public PlayerEquipData(LitJson.JsonData json)
        {
            EquipWeapon = new ItemSaveData(int.Parse(json["EquipWeapon"]["ItemID"].ToString()), int.Parse(json["EquipWeapon"]["ItemType"].ToString()), int.Parse(json["EquipWeapon"]["Enforce"].ToString()));
            Debug.Log(json["EquipNeckLace"].ToString());
            if (json["EquipNeckLace"].ToString() != "True")
            {
                EquipNeckLace = new ItemSaveData(int.Parse(json["EquipNeckLace"]["ItemID"].ToString()), int.Parse(json["EquipNeckLace"]["ItemType"].ToString()), int.Parse(json["EquipNeckLace"]["Enforce"].ToString()));
            }
            else
            {
                EquipNeckLace = null;
            }
            EquipRings = new List<ItemSaveData>();
            EquipEarrings = new List<ItemSaveData>();
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    EquipRings.Add(new ItemSaveData(int.Parse(json["EquipRings"][i]["ItemID"].ToString()), int.Parse(json["EquipRings"][i]["ItemType"].ToString()), int.Parse(json["EquipRings"][i]["Enforce"].ToString())));
                }
                catch (Exception e)
                {
                    EquipRings.Add(null);
                }

                try
                {
                    EquipEarrings.Add(new ItemSaveData(int.Parse(json["EquipEarrings"][i]["ItemID"].ToString()), int.Parse(json["EquipEarrings"][i]["ItemType"].ToString()), int.Parse(json["EquipEarrings"][i]["Enforce"].ToString())));
                }
                catch (Exception e)
                {
                    EquipEarrings.Add(null);
                }
            }
        }
    }

    public class PlayerEquipment : MonoBehaviour
    {
        /*
         * 장착 가능한 개수
         * 무기 1개
         * 목걸이 1개
         * 반지 2개
         * 귀걸이 2개
         */
        [SerializeField] private WeaponItem _equipWeapon;
        [SerializeField] private NecklaceItem _equipNecklace;
        [SerializeField] private List<RingItem> _equipRingItems;
        [SerializeField] private List<EarringItem> _equipEarringItems;

        public WeaponItem EquipWeapon => _equipWeapon;

        public NecklaceItem EquipNecklace
        {
            get
            {
                return _equipNecklace;
            }
            set
            {
                _equipNecklace = value;
            }
        }
        public List<RingItem> EquipRingItems
        {
            get => _equipRingItems;
            set => _equipRingItems = value;
        }

        public List<EarringItem> EquipEarringItems
        {
            get => _equipEarringItems;
            set => _equipEarringItems = value;
        }

        private const int MaxEquipCount = 2;

        private Inventory _inventory;
        private UIManager _uiManager;
        private Player _player;

        private void Awake()
        {
            _equipNecklace = null;
            _equipRingItems = new List<RingItem>();
            _equipEarringItems = new List<EarringItem>();
            for (int i = 0; i < MaxEquipCount; i++)
            {
                _equipRingItems.Add(null);
                _equipEarringItems.Add(null);
            }
            
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            _player = GetComponent<Player>();
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        }

        private void Start()
        {
            if(SaveManager.Instance.ItemSaveData.PlayerEquip.EquipWeapon != null && SaveManager.Instance.ItemSaveData.PlayerEquip.EquipWeapon.ItemID != 0)
            {
                WeaponItem item = (WeaponItem)ItemDataManager.Instance.GetItem(SaveManager.Instance.ItemSaveData.PlayerEquip.EquipWeapon.ItemID,
                    SaveManager.Instance.ItemSaveData.PlayerEquip.EquipWeapon.ItemType,
                    SaveManager.Instance.ItemSaveData.PlayerEquip.EquipWeapon.Enforce);
                _equipWeapon = (WeaponItem)item;
                _player.ChangeWeaponSprite(_equipWeapon.ToSprite());
                Debug.Log($"{item.Enforce}, {_equipWeapon.Enforce}");
            }
            if (SaveManager.Instance.ItemSaveData.PlayerEquip.EquipNeckLace != null && SaveManager.Instance.ItemSaveData.PlayerEquip.EquipNeckLace.ItemID != 0)
            {
                EquipItem(ItemDataManager.Instance.GetItem(SaveManager.Instance.ItemSaveData.PlayerEquip.EquipNeckLace.ItemID, SaveManager.Instance.ItemSaveData.PlayerEquip.EquipNeckLace.ItemType, SaveManager.Instance.ItemSaveData.PlayerEquip.EquipNeckLace.Enforce));
            }
            for(int i = 0; i < 2; i++)
            {
                if (SaveManager.Instance.ItemSaveData.PlayerEquip.EquipEarrings != null && SaveManager.Instance.ItemSaveData.PlayerEquip.EquipEarrings[i] != null && SaveManager.Instance.ItemSaveData.PlayerEquip.EquipEarrings.Count != 0)
                {
                    EquipItem(ItemDataManager.Instance.GetItem(SaveManager.Instance.ItemSaveData.PlayerEquip.EquipEarrings[i].ItemID, SaveManager.Instance.ItemSaveData.PlayerEquip.EquipEarrings[i].ItemType, SaveManager.Instance.ItemSaveData.PlayerEquip.EquipEarrings[i].Enforce), i);
                }
                            
                if (SaveManager.Instance.ItemSaveData.PlayerEquip.EquipRings != null && SaveManager.Instance.ItemSaveData.PlayerEquip.EquipRings[i] != null &&SaveManager.Instance.ItemSaveData.PlayerEquip.EquipRings.Count != 0)
                {
                    EquipItem(ItemDataManager.Instance.GetItem(SaveManager.Instance.ItemSaveData.PlayerEquip.EquipRings[i].ItemID, 
                        SaveManager.Instance.ItemSaveData.PlayerEquip.EquipRings[i].ItemType, SaveManager.Instance.ItemSaveData.PlayerEquip.EquipRings[i].Enforce), i);
                }
            }
        }

        public int GetDamage()
        {
            int dmg = 0;
            if (_equipWeapon.ItemID != 0)
            {
                dmg += _equipWeapon.GetDamage();
            }

            if (_equipNecklace != null && _equipNecklace.ItemID != 0)
            {
                dmg += _equipNecklace.GetDamage();
            }
            foreach (var ring in _equipRingItems)
            {
                if (ring == null) continue;
                dmg += ring.GetDamage();
            }

            foreach (var earring in _equipEarringItems)
            {
                if (earring == null) continue;
                dmg += earring.GetDamage();
            }
            return dmg;
        }

        public float GetCriV()
        {
            float cri = 0;
            if (_equipWeapon.ItemID != 0)
            {
                cri += _equipWeapon.GetCri();
            }

            if (_equipNecklace != null && _equipNecklace.ItemID != 0)
            {
                cri += _equipNecklace.GetCri();
            }
            foreach (var ring in _equipRingItems)
            {
                if (ring == null) continue;
                cri += ring.GetCri();
            }

            foreach (var earring in _equipEarringItems)
            {
                if (earring == null) continue;
                cri += earring.GetCri();
            }
            return cri;
        }

        /// <summary>
        /// 아이템을 장착하는 메소드.
        /// </summary>
        /// <param name="item">장착할 아이템</param>
        public void EquipItem(Item item)
        {
            Debug.Log(item.ItemName+ " 장착");

            switch ((int)item.ItemType)
            {
                case 0:
                    if (_equipWeapon != null)
                    {
                        _inventory.AddItem((Item)_equipWeapon);
                        _equipWeapon = (WeaponItem)item;
                        _inventory.RemoveItem(item);
                        Debug.Log($"{item.Enforce}, {_equipWeapon.Enforce}");
                    }
                    else
                    {
                        _inventory.RemoveItem(item);
                        _equipWeapon = (WeaponItem)item;
                    }
                    _player.ChangeWeaponSprite(_equipWeapon.ToSprite());
                    break;
                case 1:
                    if (_equipNecklace != null && _equipNecklace.ItemID != 0)
                    {
                        _inventory.AddItem((Item)_equipNecklace);
                        _equipNecklace = (NecklaceItem)item;
                        _inventory.RemoveItem(item);
                    }
                    else
                    {
                        _inventory.RemoveItem(item);
                        _equipNecklace = (NecklaceItem)item;
                    }
                    break;
            }
            
            
            _uiManager.ReLoadEquipment();
        }

        public void EquipItemInIt(Item item)
        {
            Debug.Log(item.ItemName+ " 장착");

            switch ((int)item.ItemType)
            {
                case 0:
                    _equipWeapon = new WeaponItem(item);
                    if (_equipWeapon != null)
                    {
                        Debug.Log($"{item.Enforce}, {_equipWeapon.Enforce}");
                    }
                    _player.ChangeWeaponSprite(_equipWeapon.ToSprite());
                    break;
            }
            
            
            _uiManager.ReLoadEquipment();
        }
        
        public void EquipItem(Item item, int slot)
        {
            Debug.Log($"{slot} 슬롯 참조\n{(int)item.ItemType}타입 아이템");
            switch ((int)item.ItemType)
            {
                case 2:
                    if (_equipRingItems[slot] != null && _equipRingItems[slot].ItemID != 0)
                    {  
                        Debug.Log($"{slot} 반지 변경");
                        _inventory.RemoveItem(item);
                        _inventory.AddItem((Item)_equipRingItems[slot]);
                        _equipRingItems[slot] = (RingItem)item;
                    }
                    else
                    {
                        Debug.Log($"{slot} 반지 장착");
                        _inventory.RemoveItem(item);
                        _equipRingItems[slot] = (RingItem)item;
                    }
                    break;
                case 3:
                    if (_equipEarringItems[slot] != null && _equipEarringItems[slot].ItemID != 0)
                    {
                        Debug.Log($"{slot} 이어링 변경");
                        _inventory.RemoveItem(item);
                        _inventory.AddItem((Item)_equipEarringItems[slot]);
                        _equipEarringItems[slot] = (EarringItem)item;
                    }
                    else
                    {
                        Debug.Log($"{slot} 이어링 장착");
                        _inventory.RemoveItem(item);
                        _equipEarringItems[slot] = (EarringItem)item;
                    }
                    break;
            }
            _uiManager.ReLoadEquipment();
        }

        public void DeEquipItem(Item item)
        {
            switch ((int)item.ItemType)
            {
                case 0:
                    if (_equipWeapon != null)
                    {
                        _inventory.AddItem((Item)_equipWeapon);
                        _equipWeapon = null;
//                        Debug.Log($"{item.Enforce}, {_equipWeapon.Enforce}");
                    }
                    _player.ChangeWeaponSprite(_equipWeapon.ToSprite());
                    break;
                case 1:
                    if (_equipNecklace != null)
                    {
                        _inventory.AddItem((Item)_equipNecklace);
                        _equipNecklace = null;
                    }
                    break;
            }
            _uiManager.ReLoadEquipment();
        }
        
        public void DeEquipItem(Item item, int slot)
        {
            switch ((int)item.ItemType)
            {
                case 2:
                    if (_equipRingItems[slot] != null)
                    {  
                        Debug.Log($"{slot} 반지 변경");
                        _inventory.AddItem((Item)_equipRingItems[slot]);
                        _equipRingItems[slot] = null;
                    }
                    break;
                case 3:
                    if (_equipEarringItems[slot] != null)
                    {
                        Debug.Log($"{slot} 이어링 변경");
                        _inventory.AddItem((Item)_equipEarringItems[slot]);
                        _equipEarringItems[slot] = null;
                    }
                    break;
            }
            _uiManager.ReLoadEquipment();
        }

        public PlayerEquipData ToData()
        {
            int weaponID = _equipWeapon != null ? _equipWeapon.ItemID : 0;
            int necklaceID = _equipNecklace != null ? _equipNecklace.ItemID : 0;
            int ringID = _equipRingItems.Count > 0 && _equipRingItems[0] != null ? _equipRingItems[0].ItemID : 0;
            int earringID = _equipEarringItems.Count > 0 && _equipEarringItems[0] != null ? _equipEarringItems[0].ItemID : 0;

            Debug.Log($"{weaponID}, {necklaceID}, {ringID}, {earringID}");
    
            return new PlayerEquipData(_equipWeapon, _equipNecklace, _equipRingItems, _equipEarringItems);
            return new PlayerEquipData(_equipWeapon, _equipNecklace, _equipRingItems, _equipEarringItems);
        }
    }
}
