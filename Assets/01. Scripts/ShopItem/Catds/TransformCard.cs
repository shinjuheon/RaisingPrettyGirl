using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    [CreateAssetMenu(fileName = "TransformCard", menuName = "Shop/TransformCard")]
    public class TransformCard : ShopItem
    {
        public int Count;
        public GachaType Gachatype;
        public int MinLevel;
        public int MaxLevel;
        public override void ApplyReward()
        {
            // 뽑기 시작
            GachaSystem gachaSystem = GameObject.Find("Canvas").transform.Find("GachaBG").GetComponent<GachaSystem>();
            gachaSystem.gameObject.SetActive(true);
            gachaSystem.IsFirst = true;
            for (int i = 0; i < Count; i++)
            {
                gachaSystem.GachaCall(Gachatype);
            }
            // gachaSystem.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Show");
            base.ApplyReward();
        }
    }
}
