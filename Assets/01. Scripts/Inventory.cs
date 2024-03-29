using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.Serialization;

namespace gunggme
{

    public class Inventory : MonoBehaviour
    {
        public delegate void OnItemChanged();
        public OnItemChanged OnItemChangedCallback; // 아이템이 변경될 때 호출할 콜백

        public int InventorySpace = 100; // 인벤토리 용량 제한
        [SerializeField] private List<Item> _items = new List<Item>(); // 인벤토리 아이템 리스트

        private void Awake()
        {
            OnItemChangedCallback -= RefreshInventory;
            OnItemChangedCallback += RefreshInventory;
        }

        private void Start()
        {
            foreach (var item in SaveManager.Instance.ItemSaveData.Items)
            {
                AddItem(ItemDataManager.Instance.GetItem(item.ItemID, item.ItemType, item.Enforce));
            }
            
            StartCoroutine(SaveManager.Instance.Coroutine_UpdateData());
        }

        // 인벤토리에 아이템을 추가합니다.
        public bool AddItem(Item item)
        {
            if (_items.Count >= InventorySpace) // 인벤토리 용량을 초과하는지 확인
            {
                Debug.Log("인벤토리가 가득 찼습니다.");
                return false;
            }

            Item temp = item.ItemType switch
            {
                0 => new WeaponItem(item),
                1 => new NecklaceItem(item),
                2 => new RingItem(item),
                3 => new EarringItem(item),
                _ => null
            };

            //Debug.Log(temp.GetInstanceID());
            _items.Add(temp);
            OnItemChangedCallback?.Invoke(); // 아이템 변경 콜백 호출
            return true;
        }

        // 인벤토리에서 아이템을 제거합니다.
        public void RemoveItem(Item item)
        {
            _items.Remove(_items.Find(i => (i.HashCode == item.HashCode)));
            OnItemChangedCallback?.Invoke(); // 아이템 변경 콜백 호출
        }

        // 인벤토리에 있는 모든 아이템을 반환합니다.
        public List<Item> GetItems()
        {
            return _items;
        }

        public void SetItems(Item item)
        {
            AddItem(item);
            for (int i = 0; i < _items.Count; i++)
            {
                switch (_items[i].ItemType)
                {
                    case 0:
                        _items[i] = new WeaponItem(_items[i]);
                        break;
                    case 1:
                        _items[i] = new NecklaceItem(_items[i]);
                        break;
                    case 2:
                        _items[i] = new RingItem(_items[i]);
                        break;
                    case 3:
                        _items[i] = new EarringItem(_items[i]);
                        break;
                }
            }
            OnItemChangedCallback?.Invoke();
        }

        void RefreshInventory()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                switch (_items[i].ItemType)
                {
                    case 0:
                        _items[i] = new WeaponItem(_items[i]);
                        break;
                    case 1:
                        _items[i] = new NecklaceItem(_items[i]);
                        break;
                    case 2:
                        _items[i] = new RingItem(_items[i]);
                        break;
                    case 3:
                        _items[i] = new EarringItem(_items[i]);
                        break;
                }
            }
        }

        // 인벤토리 용량을 추가합니다.
        public void IncreaseInventorySpace(int amount)
        {
            InventorySpace += amount;
            Debug.Log("인벤토리 용량이 증가했습니다. 현재 용량: " + InventorySpace);
            OnItemChangedCallback?.Invoke(); // .업데이트 콜백 호출
        }
    }
}
