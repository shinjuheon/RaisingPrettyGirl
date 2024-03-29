using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField] private Inventory _inventory;
        [SerializeField] private SaveManager _saveManager;
        [SerializeField] private PlayerStat _playerStat;
        [SerializeField] private GoodsManager _goodsManager;
        [SerializeField] private PlayerSkinSystem _playerSkin;

        [SerializeField] private CodeSystem _codeSystem;
        private void Start()
        {
            //SoundManager.Instance.PlaySound(SoundType.BackgroundMusic, 0);

            // for (int i = 0; i < 100; i++)
            // {
            //     _inventory.AddItem(ItemDataManager.Instance.GetItem(1001, 0));
            // } 
            
            // _inventory.AddItem(ItemDataManager.Instance.GetItem(1001, 0));
            // _inventory.AddItem(ItemDataManager.Instance.GetItem(1001, 1));
            // _inventory.AddItem(ItemDataManager.Instance.GetItem(1001, 2));
            // _inventory.AddItem(ItemDataManager.Instance.GetItem(1001, 2));
            // _inventory.AddItem(ItemDataManager.Instance.GetItem(1001, 3));
            // _inventory.AddItem(ItemDataManager.Instance.GetItem(1001, 3));
#if !UNITY_EDITOR
            //SaveManager.Instance.InitData();
            //SaveManager.Instance.LoadData();
#endif
            // _goodsManager.GetGold(2147483647);
            // _goodsManager.GetDiamond(2147483647);
            // _goodsManager.GetScroll(2147483640);

        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _playerStat.UpLevel(1000);
            }
#endif
        }
    }
}
