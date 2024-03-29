using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    [CreateAssetMenu(fileName = "DiamondShop", menuName = "Shop/Diamond")]
    public class ShopDiamond : ShopItem
    {
        public string Diamond_ID;
        public int Diamonds;
        public override void ApplyReward()
        {
            GoodsManager goodsManager = GameObject.Find("GoodsManager").GetComponent<GoodsManager>();
            goodsManager.GetDiamond(Diamonds);
            base.ApplyReward();
        }
    }
}
