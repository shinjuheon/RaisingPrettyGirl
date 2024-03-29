using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class SkinPreview : MonoBehaviour
    {
        public bool putBtn;

        public SpriteRenderer[] parts;
        //public List<Skin> haveskins; // skins[등급][번호]
        public PlayerSkinSystem playerSkinSystem;


        public Rarity bestRarity;
        public Rarity curRarity;
        public int curSkinIdx;
        
        [SerializeField] private GameObject _lastpageMessage;
        [SerializeField] private GameObject _didntChange;

        public void OnEnable()
        {
            putBtn = false;
            FindBestRarity();
        }
        
        //현재 있는 스킨중 가장 높은 등급 탐색
        public void FindBestRarity()
        {
            for(int i = 6; i >= 0; i--)
            {
                if (SaveManager.Instance.haveSkins[i].SkinDatas.Count != 0)
                {
                    bestRarity = (Rarity)i;
                    break;
                }
            }
        }
        //스킨 변경 UI 미리보기 변경시 사용(죄송합니다...)
        public void ChangeSkin_UI(Rarity rarity, int skinIdx)
        {
            foreach (SpriteRenderer part in parts)
            {
                if(SaveManager.Instance.haveSkins[(int)rarity].SkinDatas[skinIdx].ToAtlas().GetSprite(part.name) != null)
                {
                    part.sprite = SaveManager.Instance.haveSkins[(int)rarity].SkinDatas[skinIdx].ToAtlas()
                        .GetSprite(part.name);
                }
                else
                {
                    part.sprite = null;
                }
            }
        }
        public void NextPreview(int index)
        {
            //현재 가지고 있는 스킨이 하나 이하면 return
            int skinCount = SaveManager.Instance.haveSkins.Sum(t => t.SkinDatas.Count);

            if (skinCount <= 1)
            {
                Debug.Log(skinCount <= 1);
                if (!_lastpageMessage.activeSelf)
                {
                    _lastpageMessage.SetActive(true);
                }
                return;
            }


            switch (index)
            {
                //범위를 벗어났을 경우
                case 1 when curSkinIdx >= SaveManager.Instance.haveSkins[(int)curRarity].SkinDatas.Count - 1:
                {
                    //다음 레어도 탐색
                    int rarity = (int)curRarity;
                    int count = 0;
                    do
                    {
                        rarity++;
                        count++;
                        if (rarity == 7)
                        {
                            rarity = 0;
                        }
                    } while (SaveManager.Instance.haveSkins[rarity].SkinDatas.Count == 0 && count <= 8);
                    curRarity = (Rarity)rarity;
                    curSkinIdx = 0;
                    break;
                }
                case -1 when curSkinIdx <= 0:
                {
                    int rarity = (int)curRarity;
                    int count = 0;
                    do
                    {
                        count++;
                        rarity--;
                        if (rarity == -1)
                        {
                            rarity = (int)bestRarity;
                        }
                        Debug.Log(rarity + "," + SaveManager.Instance.haveSkins[rarity].SkinDatas.Count);
                    } while (SaveManager.Instance.haveSkins[rarity].SkinDatas.Count == 0 && count <= 8);
                    curRarity = (Rarity)rarity;
                    curSkinIdx = SaveManager.Instance.haveSkins[rarity].SkinDatas.Count - 1;
                    break;
                }
                default:
                    curSkinIdx += index;
                    break;
            }
            putBtn = true;
            ChangeSkin_UI(curRarity, curSkinIdx);
        }
    }
}
