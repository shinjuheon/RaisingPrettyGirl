using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace gunggme
{
    
    public class PetSystem : MonoBehaviour
    {
        private PlayerStat _playerStat;
        
        [SerializeField] private PetData _currentPet;
        public PetData CurrentPet => _currentPet;
        private SpriteRenderer _petSprite;
        private void Start()
        {
            _playerStat = this.GetComponent<PlayerStat>();
            _petSprite = transform.Find("Pet").GetChild(0).GetComponent<SpriteRenderer>();
            
            if (SaveManager.Instance.ETCData.GetCurrentPetData() != null)
            {
                ChangePet(SaveManager.Instance.ETCData.GetCurrentPetData());
            }
        }

        public void AddPet(PetData petData)
        {
            if (SaveManager.Instance.havePets[petData.Rarity].PetDatas.Find(i => i.Name == petData.Name) == null)
            {
                SaveManager.Instance.havePets[petData.Rarity].PetDatas.Add(petData);
            }
        }

        public void ChangePet(PetData selectedPet)
        {
            _currentPet = selectedPet;

            _petSprite.sprite =
                Resources.Load<Sprite>($"Pet/{selectedPet.Name}");
            SaveManager.Instance.CurPet = selectedPet;
        }

        public void DeEquipPet()
        {
            if (_currentPet == null)
                return;
            
            _petSprite.sprite = null;
            SaveManager.Instance.CurPet = null;
            _currentPet = null;
        }
    }
}
