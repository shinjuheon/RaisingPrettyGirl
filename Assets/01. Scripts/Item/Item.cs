using System;
using LitJson;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace gunggme
{
    [Serializable]
    public abstract class Item
    {
        public string ItemName; // 아이템 이름
        public int ItemID; // 아이템 고유 ID
        public string ItemIconSprite;
        public int ItemType; // 아이템 종류 
        public int Rarity;
        public int Damage; // 아이템 공격력

        public int Enforce;
        public float AttackSpeed; // 무기의 공격 속도
        public float Accuracy; // 무기의 명중률
        public float Critical; // 목걸이의 크리티컬
        public int HashCode { get; private set; }
        public Item()
        {
            ItemName = "";
            ItemID = 0;
            ItemIconSprite = null;
            ItemType = 0;
            Damage = 0;
            AttackSpeed = 0;
            Accuracy = 0;
            Critical = 0;
            Rarity = 0;
            HashCode = 0;
        }

        public Item(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity)
        {
            ItemName = itemName;
            ItemID = itemID;
            ItemIconSprite = icon;
            ItemType = itemType;
            Damage = damage;
            AttackSpeed = attackSpeed;
            Accuracy = accuracy;
            Critical = critical;
            Rarity = rarity;
            HashCode = this.GetHashCode();
        }
        
        public Item(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity, int enforce) :  this(itemName, itemID, icon, itemType, damage, attackSpeed, accuracy, critical, rarity)
        {
            Enforce = enforce;
        }


        public Item(Item item)
        {
            Enforce = item.Enforce;
            ItemName = item.ItemName;
            ItemID = item.ItemID;
            ItemIconSprite = item.ItemIconSprite;
            ItemType = item.ItemType;
            Damage = item.Damage;
            AttackSpeed = item.AttackSpeed;
            Accuracy = item.Accuracy;
            Critical = item.Critical;
            HashCode = item.HashCode;
        }

        public Item(LitJson.JsonData json)
        {
            Enforce = int.Parse(json["Enforce"].ToString());
            ItemName = json["ItemName"].ToString();
            ItemID = int.Parse(json["ItemID"].ToString());
            ItemIconSprite = json["ItemIconSprite"].ToString();
            ItemType = int.Parse(json["ItemType"].ToString());
            Damage = int.Parse(json["Damage"].ToString());
            AttackSpeed = float.Parse(json["AttackSpeed"].ToString());
            Accuracy = float.Parse(json["Accuracy"].ToString());
            Critical = float.Parse(json["Critical"].ToString());
            HashCode = this.GetHashCode();
        }
        
        public bool TryEnforce()
        {
            if (Enforce == 12)
            {
                return true;
            }
            int enforceRan = Random.Range(1, 101);
            switch ((int)ItemType)
            {
                case 0:
                    if (enforceRan <= ItemDataManager.Instance.GetWeaponEnforcePer(Enforce))
                    {
                        Enforce++;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case 1:
                case 2:
                case 3:
                    if (enforceRan <= ItemDataManager.Instance.GetAccEnforcePer(Enforce))
                    {
                        Enforce++;
                    }
                    else
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }
        
        public virtual Sprite ToSprite()
        {
            Sprite sprite = null;
            return sprite;
        }

        public override string ToString()
        {
            string temp = $"마법력 : {GetDamage()}\n";
            if (GetAttSpeed() != 0)
            {
                temp += $"공격 속도 : {GetAttSpeed()} %\n";
            }
            if (GetAccuracy() != 0)
            {
                temp += $"명중률 : {GetAccuracy()}\n";
            }
            if (GetCri() != 0)
            {
                temp += $"크리티컬 : {GetCri()}\n";
            }
            return temp;
        }

        public abstract int GetDamage();
        public abstract float GetAttSpeed();
        public abstract float GetAccuracy();
        public abstract float GetCri();
    }
    
    // 무기 아이템 클래스
    [Serializable]
    public class WeaponItem : Item
    {
        public override string ToString()
        {
            string temp = $"마법력 : {GetDamage()}\n";
            if (GetAttSpeed() != 0)
            {
                temp += $"공격 속도 : {GetAttSpeed()} %\n";
            }
            if (GetAccuracy() != 0)
            {
                temp += $"명중률 : {GetAccuracy()}\n";
            }
            if (GetCri() != 0)
            {
                temp += $"크리티컬 : {GetCri()}\n";
            }
            return temp;
        }

        public override Sprite ToSprite()
        {
            return Resources.Load<SpriteAtlas>("Weapon").GetSprite(ItemIconSprite);
        }

        public WeaponItem()
        {
        }

        public WeaponItem(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity) : base(itemName, itemID, icon, itemType, damage, attackSpeed, accuracy, critical, rarity)
        {
        }

        public WeaponItem(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity, int enforce) : base(itemName, itemID, icon, itemType, damage, attackSpeed, accuracy, critical, rarity, enforce)
        {
        }

        public WeaponItem(JsonData json) : base(json)
        {
        }

        public WeaponItem(Item item) : base(item)
        {
        }

        public override int GetDamage()
        {
            int dmg = Damage;
            for (int i = 0; i < Enforce; i++)
            {
                switch (i)
                {
                    case 1 or 2 or 3 or 4 or 5 or 6:
                        dmg += 1;
                        break;
                    case 7 or 8 or 9:
                        dmg += 2;
                        break;
                    case 10 or 11 or 12:
                        dmg += 3;
                        break;
                }
            }
            return dmg;
        }

        public override float GetAttSpeed()
        {
            return AttackSpeed + 100f;
        }

        public override float GetAccuracy()
        {
            return Accuracy + Enforce;
        }

        public override float GetCri()
        {
            return Critical;
        }
    }

    // 귀걸이 아이템 클래스
    [Serializable]
    public class EarringItem : Item
    {
        public override string ToString()
        {
            string temp = $"마법력 : {GetDamage()}\n";
            if (GetAttSpeed() != 0)
            {
                temp += $"공격 속도 : {GetAttSpeed()} %\n";
            }
            if (GetAccuracy() != 0)
            {
                temp += $"명중률 : {GetAccuracy()}\n";
            }
            if (GetCri() != 0)
            {
                temp += $"크리티컬 : {GetCri()}\n";
            }

            if (Enforce != 0)
            {
                temp += $"방어력 : {Enforce}\n";
            }
            return temp;
        }


        public override Sprite ToSprite()
        {
            return Resources.Load<SpriteAtlas>("AccAtlas").GetSprite(ItemIconSprite);
        }

        public EarringItem()
        {
        }

        public EarringItem(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity) : base(itemName, itemID, icon, itemType, damage, attackSpeed, accuracy, critical, rarity)
        {
        }

        public EarringItem(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity, int enforce) : base(itemName, itemID, icon, itemType, damage, attackSpeed, accuracy, critical, rarity, enforce)
        {
        }

        public EarringItem(JsonData json) : base(json)
        {
        }

        public EarringItem(Item item) : base(item)
        {
        }

        public override int GetDamage()
        {
            return ItemDataManager.Instance.GetDmg(this);
        }

        public override float GetAttSpeed()
        {
            return ItemDataManager.Instance.GetAttackSpeed(this) + 100f;
        }

        public override float GetAccuracy()
        {
            return ItemDataManager.Instance.GetAccuracy(this);
        }

        public override float GetCri()
        {
            return ItemDataManager.Instance.GetCri(this);
        }
    }

    // 목걸이 아이템 클래스
    [Serializable]
    public class NecklaceItem : Item
    {
        public override string ToString()
        {
            string temp = $"마법력 : {GetDamage()}\n";
            if (GetAttSpeed() != 0)
            {
                temp += $"공격 속도 : {GetAttSpeed()} %\n";
            }
            if (GetAccuracy() != 0)
            {
                temp += $"명중률 : {GetAccuracy()}\n";
            }
            if (GetCri() != 0)
            {
                temp += $"크리티컬 : {GetCri()}\n";
            }
            if (Enforce != 0)
            {
                temp += $"방어력 : {Enforce}\n";
            }
            return temp;
        }
        public override Sprite ToSprite()
        {
            return Resources.Load<SpriteAtlas>("AccAtlas").GetSprite(ItemIconSprite);
        }

        public NecklaceItem()
        {
        }

        public NecklaceItem(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity) : base(itemName, itemID, icon, itemType, damage, attackSpeed, accuracy, critical, rarity)
        {
        }

        public NecklaceItem(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity, int enforce) : base(itemName, itemID, icon, itemType, damage, attackSpeed, accuracy, critical, rarity, enforce)
        {
        }

        public NecklaceItem(JsonData json) : base(json)
        {
        }

        public NecklaceItem(Item item) : base(item)
        {
        }

        public override int GetDamage()
        {
            return ItemDataManager.Instance.GetDmg(this);
        }

        public override float GetAttSpeed()
        {
            return ItemDataManager.Instance.GetAttackSpeed(this) + 100f;
        }

        public override float GetAccuracy()
        {
            return ItemDataManager.Instance.GetAccuracy(this);
        }

        public override float GetCri()
        {
            return ItemDataManager.Instance.GetCri(this);
        }
    }

    // 반지 아이템 클래스
    [Serializable]
    public class RingItem : Item
    {
        public override string ToString()
        {
            string temp = $"마법력 : {GetDamage()}\n";
            if (GetAttSpeed() != 0)
            {
                temp += $"공격 속도 : {GetAttSpeed()} %\n";
            }
            if (GetAccuracy() != 0)
            {
                temp += $"명중률 : {GetAccuracy()}\n";
            }
            if (GetCri() != 0)
            {
                temp += $"크리티컬 : {GetCri()}\n";
            }
            if (Enforce != 0)
            {
                temp += $"방어력 : {Enforce}\n";
            }
            return temp;
        }
        
        public override Sprite ToSprite()
        {
            return Resources.Load<SpriteAtlas>("AccAtlas").GetSprite(ItemIconSprite);
        }

        public RingItem()
        {
        }

        public RingItem(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity) : base(itemName, itemID, icon, itemType, damage, attackSpeed, accuracy, critical, rarity)
        {
        }

        public RingItem(string itemName, int itemID, string icon, int itemType, int damage, float attackSpeed, float accuracy, float critical, int rarity, int enforce) : base(itemName, itemID, icon, itemType, damage, attackSpeed, accuracy, critical, rarity, enforce)
        {
        }

        public RingItem(JsonData json) : base(json)
        {
        }

        public RingItem(Item item) : base(item)
        {
        }

        public override int GetDamage()
        {
            return ItemDataManager.Instance.GetDmg(this);
        }

        public override float GetAttSpeed()
        {
            return ItemDataManager.Instance.GetAttackSpeed(this) + 100f;
        }

        public override float GetAccuracy()
        {
            return ItemDataManager.Instance.GetAccuracy(this);
        }

        public override float GetCri()
        {
            return ItemDataManager.Instance.GetCri(this);
        }
    }
}
