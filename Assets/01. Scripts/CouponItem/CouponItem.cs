using System.Collections;
using System.Collections.Generic;
using gunggme;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemObject", menuName = "CouponItem/NewItem")]
public class CouponItem : ScriptableObject
{
    [SerializeField] private int _getDiamondCnt;
    [SerializeField] private int _getGoldCnt;
    [SerializeField] private int _getScrollCnt;
    [SerializeField] private ItemSaveData[] items;
    [SerializeField] private PetData[] _pets;
    [SerializeField] private SkinData[] _skinDatas;
    public List<string> skills;
    
    public virtual void UseCoupon()
    {
        Inventory inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        GoodsManager goodsManager = GameObject.Find("GoodsManager").GetComponent<GoodsManager>();
        PetSystem petSystem = GameObject.Find("Player").GetComponent<PetSystem>();
        PlayerSkinSystem playerSkinSystem = petSystem.GetComponent<PlayerSkinSystem>();

        foreach (var item in items)
        {
            inventory.AddItem(ItemDataManager.Instance.GetItem(item.ItemID, item.ItemType, item.Enforce));
        }

        foreach (var pet in _pets)
        {
            petSystem.AddPet(pet);
        }

        foreach (var skin in _skinDatas)
        {
            playerSkinSystem.AddSKin(skin);
        }
        
        SkillSystem skillSystem = FindObjectOfType<SkillSystem>();
        foreach (var skill in skills)
        {
            skillSystem.GetSkill(skill);
        }
        
        goodsManager.GetGold(_getGoldCnt);
        goodsManager.GetDiamond(_getDiamondCnt);
        goodsManager.GetScroll(_getScrollCnt);
    }
}
