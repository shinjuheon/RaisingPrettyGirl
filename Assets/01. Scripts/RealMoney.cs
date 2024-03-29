using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using UnityEngine;

namespace gunggme
{
    public class RealMoney : MonoBehaviour
    {
        private ShopItemSlotUI _slotUI;

        private void Awake()
        {
            _slotUI = GetComponent<ShopItemSlotUI>();
        }

        public void OnPurchaseComplate(Product product)
        {
            var dast = _slotUI._shopItem as ShopDiamond;
            if (product.definition.id ==  dast.Diamond_ID)
            { 
                Debug.Log(dast.Diamonds + " 구매 완료");
                _slotUI.Buy();
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.Log("구매 실패");
        }
    }
}
