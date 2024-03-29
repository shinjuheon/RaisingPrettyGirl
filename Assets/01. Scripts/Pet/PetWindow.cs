using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class PetWindow : MonoBehaviour
    {
        private Image _currentPetImg;
        private Button _selectBtn;
        private Button _deEquipBtn;
        public List<Pet> _petDatas;

        private PetSystem _playerPetSystem;
        private PetPreview _petPreview;

        private void Awake()
        {
            _petPreview = this.GetComponent<PetPreview>();
            
            _currentPetImg = transform.Find("CurrentPet").GetComponent<Image>();
            _selectBtn = transform.Find("SelectBtn").GetComponent<Button>();
            _deEquipBtn = transform.Find("DeEquipBtn").GetComponent<Button>();
            _playerPetSystem = FindObjectOfType<PetSystem>();
            
            MakePetData();
        }

        private void OnEnable()
        {
            UpdateSelectBtn();
        }

        private void MakePetData()
        {
            _petDatas = new List<Pet>();
            for (int i = 0; i < 7; i++)
            {
                _petDatas.Add(new Pet());
                _petDatas[i].PetDatas = new List<PetData>();
            }
            
            var petData = Resources.LoadAll<PetData>("Pet/").ToList();
            foreach (var pet in petData)
            {
                _petDatas[pet.Name[0] - '0'].PetDatas.Add(pet);
            }
        }
        private bool CheckHasPet(PetData petData)
        {
            return SaveManager.Instance.havePets[petData.Rarity].PetDatas
                .Find(i => i.Name == petData.Name) != null;
        }

        private void UpdateSelectBtn()
        {
            bool isSame = _playerPetSystem.CurrentPet != null &&
                          _playerPetSystem.CurrentPet.Name ==
                          _petDatas[_petPreview.curRarity].PetDatas[_petPreview.curPetIdx].Name;
            _selectBtn.gameObject.SetActive(!isSame);
            _deEquipBtn.gameObject.SetActive(isSame);
        }

         public void SelectBtn()
        {
            SelectPet(_petDatas[_petPreview.curRarity].PetDatas[_petPreview.curPetIdx]);
        }
        private void SelectPet(PetData selectedPet)
        {
            if (!CheckHasPet(selectedPet))
            {
                throw new Exception("펫이 없습니다");
                return;
            }
            _playerPetSystem.DeEquipPet();
            _playerPetSystem.ChangePet(selectedPet);
            UpdateSelectBtn();
        }

        public void DeEquipPetBtn()
        {
            DeEquipPet();
        }
        private void DeEquipPet()
        {
            _playerPetSystem.DeEquipPet();
            UpdateSelectBtn();
        }
    }
}
