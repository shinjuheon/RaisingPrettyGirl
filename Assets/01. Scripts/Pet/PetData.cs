 using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    [CreateAssetMenu(fileName = "new pet", menuName = "Pet/Pet"), Serializable]
    public class PetData : ScriptableObject
    {
        public float ExpBonusPer;
        public int HpBonus;
        public int HealBonus;
        public float HealBonusPer;
        public int AtkBonus;

        public int Rarity;
        public string Name;

        public Sprite ToSprite()
        {
            return Resources.Load<Sprite>("Pet/" + Name);
        }
    }
}
