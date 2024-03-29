using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    [CreateAssetMenu(fileName = "ShopPackage", menuName = "Shop/Package")]
    public class OpenPackage : ShopItem
    {
        public ItemPackage[] PackageItems;
        public SkinData skinData;
        public override void ApplyReward()
        {
            PlayerSkinSystem pss = GameObject.Find("Player").GetComponent<PlayerSkinSystem>();
            pss.AddSKin(skinData);
            Inventory inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            foreach (var Item in PackageItems)
            {
                inventory.AddItem(ItemDataManager.Instance.GetItem(Item.ItemId, (int)Item.ItemType, Item.Enforce));
            }

            IsBuy = true;
            base.ApplyReward();
        }
    }
}
