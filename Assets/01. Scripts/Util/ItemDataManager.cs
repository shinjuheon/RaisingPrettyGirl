using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;

namespace gunggme
{
    public class ItemStatData
    {
        public string Item_Name;
        public List<Dictionary<string, object>> Item_Data;

        public ItemStatData(string name, List<Dictionary<string, object>> data)
        {
            Item_Name = name;
            Item_Data = data;
        }
    }
    
    public class ItemDataManager : Singletone<ItemDataManager>
    {
        private List<Dictionary<string, object>> data_Item;
        private List<Dictionary<string, object>> enforce_Per;

        private List<ItemStatData> _necklace_Data;
        private List<ItemStatData> _ring_Data;
        private List<ItemStatData> _earring_Data;

        [FormerlySerializedAs("WeaponSprite")] [Header("Atlas")]
        public SpriteAtlas WeaponSpriteAtlas;

        [FormerlySerializedAs("Accessori")] public SpriteAtlas AccessoriAtlas;

        protected override void Awake()
        {
            base.Awake();
            data_Item = CSVReader.Read("ItemData");
            enforce_Per = CSVReader.Read("Enforce_Percentage");

            _necklace_Data = new List<ItemStatData>();
            _ring_Data = new List<ItemStatData>();
            _earring_Data = new List<ItemStatData>();

            foreach (Dictionary<string, object> t in data_Item)
            {
                switch (int.Parse(t["item_type"].ToString()))
                {
                    case 1:
                        _necklace_Data.Add(new ItemStatData(t["sprite_name"].ToString(),CSVReader.Read("EquipmentItemData/Acc/" + t["sprite_name"].ToString())));
                        break;
                    case 2:
                        _ring_Data.Add(new ItemStatData(t["sprite_name"].ToString(), CSVReader.Read("EquipmentItemData/Acc/" + t["sprite_name"].ToString())));
                        break;
                    case 3:
                        _earring_Data.Add(new ItemStatData(t["sprite_name"].ToString(), CSVReader.Read("EquipmentItemData/Acc/" + t["sprite_name"].ToString())));
                        break;
                }
            }
        }

        public Item GetItem(int itemId, int itemType)
        {
            Item returnValue = null;

            foreach (Dictionary<string, object> t in data_Item)
            {
                if (int.Parse(t["item_id"].ToString()) == itemId)
                {
                    if (int.Parse(t["item_type"].ToString()) != itemType) continue;

                    switch (itemType)
                    {
                        case 0:
                            returnValue = new WeaponItem(t["item_name"].ToString(), int.Parse(t["item_id"].ToString()),
                                t["sprite_name"].ToString(), int.Parse(t["item_type"].ToString()),
                                int.Parse(t["dmg"].ToString()),
                                float.Parse(t["attack_speed"].ToString()), float.Parse(t["accuracy"].ToString()),
                                float.Parse(t["critical"].ToString()),
                                int.Parse(t["rarity"].ToString()));
                            break;
                        case 1:
                            returnValue = new EarringItem(t["item_name"].ToString(), int.Parse(t["item_id"].ToString()),
                                t["sprite_name"].ToString(), int.Parse(t["item_type"].ToString()),
                                int.Parse(t["dmg"].ToString()),
                                float.Parse(t["attack_speed"].ToString()), float.Parse(t["accuracy"].ToString()),
                                float.Parse(t["critical"].ToString()),
                                int.Parse(t["rarity"].ToString()));
                            break;
                        case 2:
                            returnValue = new NecklaceItem(t["item_name"].ToString(),
                                int.Parse(t["item_id"].ToString()),
                                t["sprite_name"].ToString(), int.Parse(t["item_type"].ToString()),
                                int.Parse(t["dmg"].ToString()),
                                float.Parse(t["attack_speed"].ToString()), float.Parse(t["accuracy"].ToString()),
                                float.Parse(t["critical"].ToString()),
                                int.Parse(t["rarity"].ToString()));
                            break;
                        case 3:
                            returnValue = new RingItem(t["item_name"].ToString(), int.Parse(t["item_id"].ToString()),
                                t["sprite_name"].ToString(), int.Parse(t["item_type"].ToString()),
                                int.Parse(t["dmg"].ToString()),
                                float.Parse(t["attack_speed"].ToString()), float.Parse(t["accuracy"].ToString()),
                                float.Parse(t["critical"].ToString()),
                                int.Parse(t["rarity"].ToString()));
                            break;
                    }

                    if (returnValue != null)
                        break;
                }
            }

            return returnValue;
        }

        public Item GetItem(int itemId, int itemType, int enforce)
        {
            Item returnValue = null;

            foreach (Dictionary<string, object> t in data_Item)
            {
                if (int.Parse(t["item_id"].ToString()) == itemId)
                {
                    if (int.Parse(t["item_type"].ToString()) != itemType) continue;

                    switch (itemType)
                    {
                        case 0:
                            returnValue = new WeaponItem(t["item_name"].ToString(), int.Parse(t["item_id"].ToString()),
                                t["sprite_name"].ToString(), int.Parse(t["item_type"].ToString()),
                                int.Parse(t["dmg"].ToString()),
                                float.Parse(t["attack_speed"].ToString()), float.Parse(t["accuracy"].ToString()),
                                float.Parse(t["critical"].ToString()),
                                int.Parse(t["rarity"].ToString()), enforce);
                            break;
                        case 1:
                            returnValue = new NecklaceItem(t["item_name"].ToString(),
                                int.Parse(t["item_id"].ToString()),
                                t["sprite_name"].ToString(), int.Parse(t["item_type"].ToString()),
                                int.Parse(t["dmg"].ToString()),
                                float.Parse(t["attack_speed"].ToString()), float.Parse(t["accuracy"].ToString()),
                                float.Parse(t["critical"].ToString()),
                                int.Parse(t["rarity"].ToString()), enforce);
                            break;
                        case 2:
                            returnValue = new RingItem(t["item_name"].ToString(), int.Parse(t["item_id"].ToString()),
                                t["sprite_name"].ToString(), int.Parse(t["item_type"].ToString()),
                                int.Parse(t["dmg"].ToString()),
                                float.Parse(t["attack_speed"].ToString()), float.Parse(t["accuracy"].ToString()),
                                float.Parse(t["critical"].ToString()),
                                int.Parse(t["rarity"].ToString()), enforce);
                            break;
                        case 3:
                            returnValue = new EarringItem(t["item_name"].ToString(), int.Parse(t["item_id"].ToString()),
                                t["sprite_name"].ToString(), int.Parse(t["item_type"].ToString()),
                                int.Parse(t["dmg"].ToString()),
                                float.Parse(t["attack_speed"].ToString()), float.Parse(t["accuracy"].ToString()),
                                float.Parse(t["critical"].ToString()),
                                int.Parse(t["rarity"].ToString()), enforce);
                            break;
                    }

                    break;
                }
            }

            return returnValue;
        }


        public int GetWeaponEnforcePer(int enforceCount)
        {
            return int.Parse(enforce_Per[enforceCount]["Weapon_Per"].ToString());
        }

        public int GetAccEnforcePer(int enforceCount)
        {
            return int.Parse(enforce_Per[enforceCount]["Accessory_Per"].ToString());
        }

        public int GetDmg(Item item)
        {
            int tempDmg = 0;
            switch (item.ItemType)
            {
                case 1:
                    foreach (var t in _necklace_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempDmg = int.Parse(t.Item_Data[item.Enforce]["dmg"].ToString());
                    }
                    
                    break;
                case 2:
                    foreach (var t in _ring_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempDmg = int.Parse(t.Item_Data[item.Enforce]["dmg"].ToString());
                    }
                    break;
                case 3:
                    foreach (var t in _earring_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempDmg = int.Parse(t.Item_Data[item.Enforce]["dmg"].ToString());
                    }
                    break;
            }
            return tempDmg;
        }

        public float GetAttackSpeed(Item item)
        {
            float tempAttSpeed = 0;
            switch (item.ItemType)
            {
                case 1:
                    foreach (var t in _necklace_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempAttSpeed = float.Parse(t.Item_Data[item.Enforce]["att_speed"].ToString());
                    }
                    
                    break;
                case 2:
                    foreach (var t in _ring_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempAttSpeed = float.Parse(t.Item_Data[item.Enforce]["att_speed"].ToString());
                    }
                    break;
                case 3:
                    foreach (var t in _earring_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempAttSpeed = float.Parse(t.Item_Data[item.Enforce]["att_speed"].ToString());
                    }
                    break;
            }
            return tempAttSpeed;
        }

        public float GetCri(Item item)
        {
            float tempCri = 0;
            switch (item.ItemType)
            {
                case 1:
                    foreach (var t in _necklace_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempCri = int.Parse(t.Item_Data[item.Enforce]["cri"].ToString());
                    }
                    
                    break;
                case 2:
                    foreach (var t in _ring_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempCri = int.Parse(t.Item_Data[item.Enforce]["cri"].ToString());
                    }
                    break;
                case 3:
                    foreach (var t in _earring_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempCri = int.Parse(t.Item_Data[item.Enforce]["cri"].ToString());
                    }
                    break;
            }
            return tempCri;
        }

        public float GetAccuracy(Item item)
        {
            float tempAccuracy = 0;
            switch (item.ItemType)
            {
                case 1:
                    foreach (var t in _necklace_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempAccuracy = int.Parse(t.Item_Data[item.Enforce]["accuracy"].ToString());
                    }
                    
                    break;
                case 2:
                    foreach (var t in _ring_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempAccuracy = int.Parse(t.Item_Data[item.Enforce]["accuracy"].ToString());
                    }
                    break;
                case 3:
                    foreach (var t in _earring_Data.Where(t => t.Item_Name == item.ItemIconSprite))
                    {
                        tempAccuracy = int.Parse(t.Item_Data[item.Enforce]["accuracy"].ToString());
                    }
                    break;
            }
            return tempAccuracy;
        }
    }
}
