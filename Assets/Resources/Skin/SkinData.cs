using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace gunggme
{
    [CreateAssetMenu(fileName = "new skin data", menuName = "Skin/data")]
    public class SkinData : ScriptableObject
    {
        public string Name;
        public float AttackSpeed;
        public int Rarity;

        public SpriteAtlas ToAtlas()
        {
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>("Skin/"+Name);
            return atlas;
        }
    }
}
