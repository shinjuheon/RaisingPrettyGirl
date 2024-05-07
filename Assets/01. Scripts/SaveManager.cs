using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using JetBrains.Annotations;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

namespace gunggme
{
    [Serializable]
    public class ItemSaveData
    {
        public int ItemID;
        public int ItemType;
        public int Enforce;

        public ItemSaveData()
        {
            ItemID = 0;
            ItemType = 0;
            Enforce = 0;
        }

        public ItemSaveData(int id, int type, int enforce)
        {
            ItemID = id;
            ItemType = type;
            Enforce = enforce;
        }

        public ItemSaveData(Item item)
        {
            ItemID = item.ItemID;
            ItemType = item.ItemType;
            Enforce = item.Enforce;
        }
    }

    public class ItemSaveClass
    {
        public PlayerEquipData PlayerEquip;
        public int Capacity;
        public List<ItemSaveData> Items;


        public ItemSaveClass()
        {
            Capacity = 100;
            PlayerEquip = new PlayerEquipData();
            PlayerEquip.EquipWeapon = new ItemSaveData(1001, 0, 0);
            Items = new List<ItemSaveData>();
        }

        public ItemSaveClass(LitJson.JsonData json)
        {
            PlayerEquip = new PlayerEquipData(json["Equipment"]);
            Items = new List<ItemSaveData>();
            for (int i = 0; i < json["Items"].Count; i++)
            {
                if (json["Items"].Count == 0)
                {
                    Debug.Log("데이터가 없음");
                    break;
                }

                Items.Add(new ItemSaveData(int.Parse(json["Items"][i]["ItemID"].ToString()),
                    int.Parse(json["Items"][i]["ItemType"].ToString()),
                    int.Parse(json["Items"][i]["Enforce"].ToString())));
            }
        }

        public void SetItem(Inventory inventory)
        {
            Items.Clear();
            Capacity = inventory.InventorySpace;
            foreach (var item in inventory.GetItems())
            {
                Items.Add(new ItemSaveData(item.ItemID, item.ItemType, item.Enforce));
            }
        }

        public Param ToParam(Inventory inventory, PlayerEquipData playerEquipment)
        {
            Param param = new Param();
            if (Capacity != inventory.InventorySpace)
            {
                Capacity = inventory.InventorySpace;
                param.Add("Capacity", Capacity);
            }

            List<ItemSaveData> itemSaveDatas = new List<ItemSaveData>();
            foreach (var item in inventory.GetItems())
            {
                itemSaveDatas.Add(new ItemSaveData(item.ItemID, item.ItemType, item.Enforce));
            }

            if (itemSaveDatas != Items)
            {
                Items = itemSaveDatas;
                param.Add("Items", Items);
            }


            if (playerEquipment != PlayerEquip)
            {
                PlayerEquip = playerEquipment;
                param.Add("Equipment", PlayerEquip);
            }

            return param;
        }

        public void SetEquipData(PlayerEquipment pe)
        {
            PlayerEquip = pe.ToData();
        }

        public Param ToParam()
        {
            Param param = new Param();
            param.Add("Capacity", Capacity);
            param.Add("Items", Items);
            param.Add("Equipment", PlayerEquip);

            return param;
        }
    }


    [Serializable]
    public class SkinPetGoodsData
    {
        public List<string> HaveSkinData;
        public List<string> HavePetData;

        public string CurSkin;
        public string CurPet;

        public int Gold;
        public int Diamond;
        public int Scroll;


        public SkinPetGoodsData()
        {
            HavePetData = new List<string>();
            HaveSkinData = new List<string>();

            HaveSkinData.Add("0000");

            CurSkin = "0000";
            CurPet = "";

            Gold = 0;
            Diamond = 0;
            Scroll = 0;
        }

        public SkinPetGoodsData(List<Skin> skins, List<Pet> pets, SkinData curSkin, PetData curPet, int gold, int dia,
            int scroll)
        {
            HaveSkinData = new List<string>();
            HavePetData = new List<string>();

            for (int i = 0; i < 7; i++)
            {
                foreach (var t in skins[i].SkinDatas)
                {
                    HaveSkinData.Add(t.Name);
                }

                foreach (var t in pets[i].PetDatas)
                {
                    HavePetData.Add(t.Name);
                }
            }

            CurSkin = curSkin != null ? curSkin.Name : "";

            CurPet = curPet != null ? curPet.Name : "";

            Gold = gold;
            Diamond = dia;
            Scroll = scroll;
        }

        public SkinPetGoodsData(LitJson.JsonData json)
        {
            HaveSkinData = new List<string>();
            HavePetData = new List<string>();
            for (int i = 0; i < json["HaveSkinData"].Count; i++)
            {
                if (json["HaveSkinData"].Count == 0) return;
                HaveSkinData.Add(json["HaveSkinData"][i].ToString());
            }

            for (int i = 0; i < json["HavePetData"].Count; i++)
            {
                if (json["HavePetData"].Count == 0) break;
                Debug.Log(json["HavePetData"][i].ToString());
                HavePetData.Add(json["HavePetData"][i].ToString());
            }

            CurSkin = json["CurSkin"].ToString();
            CurPet = json["CurPet"].ToString();

            Gold = int.Parse(json["Gold"].ToString());
            Diamond = int.Parse(json["Diamond"].ToString());
            Scroll = int.Parse(json["Scroll"].ToString());
        }

        public Param ToParam()
        {
            Param param = new Param();

            param.Add("HaveSkinData", HaveSkinData);
            param.Add("HavePetData", HavePetData);
            param.Add("CurSkin", CurSkin);
            param.Add("CurPet", CurPet);
            param.Add("Gold", Gold);
            param.Add("Diamond", Diamond);
            param.Add("Scroll", Scroll);

            return param;
        }

        public Param ToParam(List<Skin> skins, List<Pet> pets, SkinData curSkin, PetData curPet, int gold, int dia,
            int scroll)
        {
            Param param = new Param();
            List<string> haveSkinData = new List<string>();
            List<string> havePetData = new List<string>();

            for (int i = 0; i < 7; i++)
            {
                foreach (var t in skins[i].SkinDatas)
                {
                    haveSkinData.Add(t.Name);
                }

                foreach (var t in pets[i].PetDatas)
                {
                    havePetData.Add(t.ToString());
                }
            }

            if (havePetData != HavePetData)
            {
                HavePetData = havePetData;
                param.Add("HavePetData", HavePetData);
            }

            if (haveSkinData != HavePetData)
            {
                HavePetData = havePetData;
                param.Add("HaveSkinData", HavePetData);
            }

            if (curSkin.Name != CurSkin)
            {
                CurSkin = curSkin.Name;
                param.Add("CurSkin", CurSkin);
            }

            if (curPet.Name != CurPet)
            {
                CurPet = curPet != null ? curPet.Name : "";
                param.Add("CurPet", CurPet);
            }

            if (gold != Gold)
            {
                Gold = gold;

                param.Add("Gold", Gold);
            }

            if (dia != Diamond)
            {
                Diamond = dia;
                param.Add("Diamond", Diamond);

            }

            if (scroll != Scroll)
            {
                Scroll = scroll;
                param.Add("Scroll", Scroll);
            }


            return param;
        }

        public List<Skin> GetHaveSkin()
        {
            List<Skin> skins = new List<Skin>();

            for (int i = 0; i < 7; i++)
            {
                skins.Add(new Skin());
                skins[i].SkinDatas = new List<SkinData>();
            }

            foreach (var skin in HaveSkinData)
            {
                if (Resources.Load<SkinData>("Skin/" + skin) == null) continue;
                SkinData skinData = Resources.Load<SkinData>("Skin/" + skin);
                skins[skinData.Rarity].SkinDatas.Add(skinData);
            }

            return skins;
        }

        public List<Pet> GetHavePet()
        {
            List<Pet> pets = new List<Pet>();

            for (int i = 0; i < 7; i++)
            {
                pets.Add(new Pet());
                pets[i].PetDatas = new List<PetData>();
            }
            
            Debug.Log(HavePetData.Count + "개의 펫 데이터가 있습니다.");

            foreach (var pet in HavePetData)
            {
                PetData petData = Resources.Load<PetData>("Pet/" + pet);
                Debug.Log(petData.Name+" 의 펫 불러오기");
                pets[petData.Rarity].PetDatas.Add(petData);
            }

            return pets;
        }

        public SkinData GetCurrentSkinData()
        {
            return Resources.Load<SkinData>("skin/" + CurSkin);
        }

        public PetData GetCurrentPetData()
        {
            return Resources.Load<PetData>("Pet/" + CurPet);
        }
    }

    public class PlayerLevelStatData
    {
        // level
        public int CurLv;
        public float CurEXP;

        public int SP;

        // stats
        public int Dmg;
        public int Dex;
        public int MDef;
        public int HP;
        public int Combat;

        public PlayerLevelStatData()
        {
            CurLv = 1;
            CurEXP = 0;
            SP = 0;
            Dmg = 0;
            Dex = 0;
            MDef = 0;
            HP = 0;
            Combat = 0;
        }

        public PlayerLevelStatData(int level, float exp, int point, int dmg, int dex, int mdef, int hp, int comb)
        {
            CurLv = level;
            CurEXP = exp;
            SP = point;
            Dmg = dmg;
            Dex = dex;
            MDef = mdef;
            HP = hp;
            Combat = comb;
        }

        public PlayerLevelStatData(LitJson.JsonData json)
        {
            //.Log($"PlayerLevelStatData JsonData : {json}");
            CurLv = int.Parse(json["CLv"].ToString());
            //Debug.Log($"PlayerLevelStatData CurLv : {CurLv}");

            CurEXP = float.Parse(json["CEXP"].ToString());
            //Debug.Log($"PlayerLevelStatData CurEXP : {CurEXP}");

            SP = int.Parse(json["SP"].ToString());
            //Debug.Log($"PlayerLevelStatData SP : {SP}");

            Dmg = int.Parse(json["Dmg"].ToString());
            //Debug.Log($"PlayerLevelStatData Dmg : {Dmg}");

            Dex = int.Parse(json["Dex"].ToString());
            //Debug.Log($"PlayerLevelStatData Dex : {Dex}");

            MDef = int.Parse(json["MDef"].ToString());
            //Debug.Log($"PlayerLevelStatData MDef : {MDef}");

            HP = int.Parse(json["HP"].ToString());
            //Debug.Log($"PlayerLevelStatData HP : {HP}");
            if (json.Keys.Contains("Combat"))
            {
                Combat = int.Parse(json["Combat"].ToString());
            }
            else
            {
                Combat = 0;
            }
        }

        public Param ToParam(int level, float exp, int point, int dmg, int dex, int mdef, int hp, int comp)
        {
            Param param = new Param();
            if (level != CurLv)
            {
                CurLv = level;
                param.Add("CLv", CurLv);
            }

            if (CurEXP != exp)
            {
                CurEXP = exp;
                param.Add("CEXP", CurEXP);
            }

            if (point != SP)
            {
                SP = point;
                param.Add("SP", SP);
            }

            if (Dmg != dmg)
            {
                Dmg = dmg;
                param.Add("Dmg", Dmg);
            }

            if (Dex != dex)
            {
                Dex = dex;
                param.Add("Dex", Dex);
            }

            if (MDef != mdef)
            {
                MDef = mdef;
                param.Add("MDef", MDef);
            }

            if (HP != hp)
            {
                HP = hp;
                param.Add("HP", HP);
            }

            if (Combat != comp)
            {
                Combat = comp;
                param.Add("Combat", Combat);
            }

            return param;
        }

        public Param ToParam()
        {
            Param param = new Param();
            param.Add("CLv", CurLv);
            param.Add("CEXP", CurEXP);
            param.Add("SP", SP);
            param.Add("Dmg", Dmg);
            param.Add("Dex", Dex);
            param.Add("MDef", MDef);
            param.Add("HP", HP);
            param.Add("Combat", Combat);
            return param;
        }
    }

    public class StageCouponData
    {
        public List<string> UseCoupon;
        public int MaxStage;
        public int CurStage;
        public int CurFloor;

        public StageCouponData()
        {
            UseCoupon = new List<string>();
            MaxStage = 1;
            CurStage = 1;
            CurFloor = 1;
        }

        public StageCouponData(List<string> use, int stage, int cstage, int curFloor)
        {
            UseCoupon = new List<string>();
            UseCoupon = use;
            MaxStage = stage;
            CurStage = cstage;
            CurFloor = curFloor;
        }

        public StageCouponData(LitJson.JsonData json)
        {
            UseCoupon = new List<string>();
            for (int i = 0; i < json["UCoupon"].Count; i++)
            {
                if (json["UCoupon"].Count == 0)
                {
                    break;
                }

                UseCoupon.Add(json["UCoupon"][i].ToString());
            }

            MaxStage = int.Parse(json["MStage"].ToString());
            CurStage = int.Parse(json["CStage"].ToString());
            try
            {
                CurFloor = int.Parse(json["CFloor"].ToString());
            }
            catch (Exception e)
            {
                CurFloor = 1;
                throw;
            }
        }

        public Param ToParam()
        {
            Param param = new Param();
            param.Add("UCoupon", UseCoupon);
            param.Add("MStage", MaxStage);
            param.Add("CStage", CurStage);
            param.Add("CFloor", CurFloor);
            return param;
        }

        public Param ToParam(List<string> use, int stage, int cstage)
        {
            Param param = new Param();
            if (use != UseCoupon)
            {
                UseCoupon = use;
                param.Add("UCoupon", UseCoupon);
            }

            if (stage != MaxStage)
            {
                MaxStage = stage;
                param.Add("MStage", MaxStage);
            }

            if (cstage != CurStage)
            {
                CurStage = cstage;
                param.Add("CStage", CurStage);
            }

            return param;
        }
    }

    public class UserInfoData
    {
        public string gamerId;
        [CanBeNull] public string countryCode;
        public string nickname;
        [CanBeNull] public string inDate;
        public string emailForFindPassword;
        [CanBeNull] public string subscriptionType;
        public string federationId;

        public override string ToString()
        {
            return $"gamerId: {gamerId}\n" +
                   $"countryCode: {countryCode}\n" +
                   $"nickname: {nickname}\n" +
                   $"inDate: {inDate}\n" +
                   $"emailForFindPassword: {emailForFindPassword}\n" +
                   $"subscriptionType: {subscriptionType}\n" +
                   $"federationId: {federationId}\n";
        }
    }

    public class SaveManager : Singletone<SaveManager>
    {
        public SkillData SkillData;
        public ItemSaveClass ItemSaveData;
        public SkinPetGoodsData ETCData;
        public PlayerLevelStatData LevelStatData;
        public StageCouponData StageCoupon;
        private UserInfoData _userInfo;
        public UserInfoData UserInfo => _userInfo;

        private Inventory _inventory;
        private PlayerEquipment _playerEquipment;
        private GoodsManager _goodsManager;
        private PlayerSkinSystem _playerSkinSystem;
        private PetSystem _petSystem;
        private PlayerStat _playerStat;
        private StageManager _stageManager;
        private CodeSystem _codeSystem;
        private UIManager _uiManager;
        private Player _player;
        public GameObject userDataLoadfail_Text;
        private bool loadingComplete = false;


        private string combatRowInData;


        // skinData
        public List<Skin> haveSkins; // skins[등급][번호]
        public List<Pet> havePets;

        public SkinData CurSkin;
        public PetData CurPet;

        [SerializeField] private string _invenIndate;
        [SerializeField] private string _skinPetGoodsIndate;
        [SerializeField] private string _playerLevelStatDataIndate;
        [SerializeField] private string _stageData;

        protected override void Awake()
        {
            base.Awake();
            Debug.Log("SaveManager Awake() called");
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        //private void Start()
        //{
        //    if (SceneManager.GetActiveScene().buildIndex == 1)
        //    {
        //        UpdateData();
        //    }
        //}

        void Update()
        {
            if (Backend.IsInitialized)
            {
                Backend.AsyncPoll();
            }
        }
        // 1. InitData가 호출된 후 게임이 시작되어야 하는데 그게 호출안되는 상황이 발생한다.
        // 2. 로그인했을때 예외상황 살펴보기
        // 3. 구글계정 - 메일 - 크래시에 대한 설명 - 크롬 브라우저 -> 계정 여러개 띄울 수 있다. -> 메일 확인
        // 로그인
        // 패키지명
        //
        //public void CombatDataInsert()
        //{
        //    // To Do : APK에서 테스트할떄는 뒤끝서버에 Combat변수가 올라가지만, APK를 지우고 라이브서버에서 플레이할때는
        //    // Combat변수가 올라간 상태로 플레이하기떄문에 LoadData -> GetLevelStatData함수를 통과할 수 없다.
        //    // 그래서 예외처리가 필요하다. -> 1. 어떻게 예외처리할지 알아볼것
        //    // 2. 뒤끝서버가 점검중일경우 BackEnd bor.Success쪽에 예외처리를 띄어줘야한다. (완)
        //    // 3. Loaddata()함수의 Wait For Until()을 2분동안 통과하지 못할경우의 예외처리 (완)
        //    Debug.Log("CombatDataInsert()시작");
        //    Backend.GameData.Get("PlayerLevelStatData", new Where(), 10, bro =>
        //    {
        //        if (!bro.IsSuccess())
        //        {
        //            Debug.LogError($"데이터 조회 실패: {bro.GetErrorCode()}");
        //            return;
        //        }

        //        var rows = bro.GetReturnValuetoJSON()["rows"];
        //        if (rows.Count <= 0)
        //        {
        //            Debug.Log("데이터가 존재하지 않습니다.");
        //            return;
        //        }

        //        var combatValue = bro.FlattenRows()[0].ContainsKey("Combat") ? bro.FlattenRows()[0]["Combat"] : null;
                
        //        //Debug.Log($"CombatDataInsert combatValue : {combatValue}");

        //        if (combatValue == null || combatValue.ToString() == string.Empty)
        //        {
        //            Debug.Log($"combatValue == null || combatValue.ToString() == string.Empty");

        //            Param param = new Param();

        //            param.Add("Combat", LevelStatData.Combat);

        //            Debug.Log("$Combat 데이터 삽입완료");
        //            //UpdateData();
        //            //Debug.Log($"Combatvalue 2 : {combatValue}");

        //            //Backend.GameData.Insert("PlayerLevelStatData", param, callback =>
        //            //{
        //            //    Debug.Log($"PlayerLevelStatData안에 Combat Insert 시작");
        //            //    if (callback.IsSuccess())
        //            //    {
        //            //        Debug.Log("Combat 데이터 삽입 시작");
                            
        //            //        combatRowInData = callback.GetInDate(); // combatRowInData를 InData()로 사용하면 될듯.
        //            //        Debug.Log($"First CombatRowInData : {combatRowInData}");

        //            //        UpdateData();

        //            //        Debug.Log($"Second CombatRowInData : {combatRowInData}");

        //            //        Debug.Log("데이터 삽입 성공");
        //            //    }
        //            //    else
        //            //    {
        //            //        Debug.LogError($"데이터 삽입 실패: {callback.GetErrorCode()}");
        //            //    }
        //            //});
        //        }
        //        else
        //        {
        //            Debug.Log("PlayerLevelStatData테이블에 Combat데이터가 이미 존재합니다.");
        //            return;
        //        }
        //    });
        //}

        public void InitData()
        {
            ItemSaveData = new ItemSaveClass();
            Param invenParam = ItemSaveData.ToParam();
            Backend.PlayerData.InsertData("InventoryData", invenParam);
            ETCData = new SkinPetGoodsData();
            Backend.PlayerData.InsertData("SkinPetGoods", ETCData.ToParam());
            LevelStatData = new PlayerLevelStatData(); // 생성자를 만들고
            Backend.PlayerData.InsertData("PlayerLevelStatData", LevelStatData.ToParam()); // PlayerLevelStatData테이블 내에 Insert ToParam()
            StageCoupon = new StageCouponData();
            Backend.PlayerData.InsertData("StageCoupon", StageCoupon.ToParam());
            SkillData = new SkillData();
            Backend.PlayerData.InsertData("SkillData", SkillData.ToParam());
            Debug.Log("InitDataSucess");
            StartCoroutine(LoadDataInit("NameSetScene"));
        }

        public void InitData_NonMove()
        {
            ItemSaveData = new ItemSaveClass();
            Param invenParam = ItemSaveData.ToParam();
            Backend.PlayerData.InsertData("InventoryData", invenParam);
            ETCData = new SkinPetGoodsData();
            Backend.PlayerData.InsertData("SkinPetGoods", ETCData.ToParam());
            LevelStatData = new PlayerLevelStatData();
            Backend.PlayerData.InsertData("PlayerLevelStatData", LevelStatData.ToParam());
            StageCoupon = new StageCouponData();
            Backend.PlayerData.InsertData("StageCoupon", StageCoupon.ToParam());
            SkillData = new SkillData();
            Backend.PlayerData.InsertData("SkillData", SkillData.ToParam());
            StartCoroutine(LoadDataInit("NameSetScene"));
        }

        public IEnumerator Coroutine_UpdateData()
        {
            while (true)
            {
                yield return new WaitForSeconds(300);
                Backend.AsyncPoll();
                UpdateData();
            }
        }

        public void UpdateData()
        {
            int saveNum = 0;
            FindComponents();
            Debug.Log("저장 시작");
            _playerStat.UpdateCombat();
            ItemSaveData.SetItem(_inventory);
            ItemSaveData.SetEquipData(_playerEquipment);

            ETCData = new SkinPetGoodsData(haveSkins, havePets, CurSkin, CurPet, _goodsManager.GoldCnt,
                _goodsManager.DiaCnt, _goodsManager.ScrollCnt);
            Param skinParam = ETCData.ToParam();
            LevelStatData = new PlayerLevelStatData(_playerStat.Level, _playerStat.CurEXP, _playerStat.StatPoint,
                _playerStat.DmgLevel, _playerStat.Dex, _playerStat.MDefLevel, _playerStat.HPLv, _playerStat.Combat);
            Param levelParam = LevelStatData.ToParam();
            StageCoupon = new StageCouponData(_codeSystem.UsedCode, _stageManager.MaximumStage,
                _stageManager.CurrentStage, _stageManager.CurrentFloor);
            Param stageParam = StageCoupon.ToParam();
            Param skillParam = SkillData.ToParam();

            Backend.PlayerData.UpdateMyLatestData("InventoryData", ItemSaveData.ToParam(), callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError("인벤토리 갱신 실패");
                    return;
                }

                saveNum++;
                _invenIndate = callback.GetInDate();
                Debug.Log("저장 시작");
            });

            Backend.PlayerData.UpdateMyLatestData("SkinPetGoods", skinParam, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError("스킨 데이터 갱신 실패");
                    return;
                }

                saveNum++;
                _skinPetGoodsIndate = callback.GetInDate();
                Debug.Log("저장 완료");
            });

            Backend.PlayerData.UpdateMyLatestData("PlayerLevelStatData", levelParam, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError("레벨 스탯 갱신 실패");
                    return;
                }
                saveNum++;
                _playerLevelStatDataIndate = callback.GetInDate();
                Debug.Log("저장 완료");
            });

            Backend.PlayerData.UpdateMyLatestData("StageCoupon", stageParam, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.Log("스테이지 갱신 실패");
                    return;
                }

                saveNum++;
                _stageData = callback.GetInDate();
                Debug.Log("저장 완료");
            });

            Backend.PlayerData.UpdateMyLatestData("SkillData", skillParam, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.Log("스킬 데이터 갱신 실패");
                    return;
                }

                saveNum++;
                Debug.Log("저장 완료");
            });

            Debug.Log(saveNum + " 저장");
        }
        
        public void UpdateData(Action call = null, Action failCall = null)
        {
            int saveNum = 0;
            FindComponents();
            Debug.Log("저장 시작");
            _playerStat.UpdateCombat();
            ItemSaveData.SetItem(_inventory);
            ItemSaveData.SetEquipData(_playerEquipment);

            ETCData = new SkinPetGoodsData(haveSkins, havePets, CurSkin, CurPet, _goodsManager.GoldCnt,
                _goodsManager.DiaCnt, _goodsManager.ScrollCnt);
            Param skinParam = ETCData.ToParam();
            LevelStatData = new PlayerLevelStatData(_playerStat.Level, _playerStat.CurEXP, _playerStat.StatPoint,
                _playerStat.DmgLevel, _playerStat.Dex, _playerStat.MDefLevel, _playerStat.HPLv, _playerStat.Combat);
            Param levelParam = LevelStatData.ToParam();
            StageCoupon = new StageCouponData(_codeSystem.UsedCode, _stageManager.MaximumStage,
                _stageManager.CurrentStage, _stageManager.CurrentFloor);
            Param stageParam = StageCoupon.ToParam();
            Param skillParam = SkillData.ToParam();

            Backend.PlayerData.UpdateMyLatestData("InventoryData", ItemSaveData.ToParam(), callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError("인벤토리 갱신 실패");
                    failCall?.Invoke();
                    return;
                }

                saveNum++;
                _invenIndate = callback.GetInDate();
                Debug.Log("저장 시작");
            });

            Backend.PlayerData.UpdateMyLatestData("SkinPetGoods", skinParam, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError("스킨 데이터 갱신 실패");
                    failCall?.Invoke();
                    return;
                }

                saveNum++;
                _skinPetGoodsIndate = callback.GetInDate();
                Debug.Log("저장 완료");
            });

            Backend.PlayerData.UpdateMyLatestData("PlayerLevelStatData", levelParam, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError("레벨 스탯 갱신 실패");
                    failCall?.Invoke();
                    return;
                }

                saveNum++;
                _playerLevelStatDataIndate = callback.GetInDate();
                Debug.Log("저장 완료");
            });

            Backend.PlayerData.UpdateMyLatestData("StageCoupon", stageParam, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.Log("스테이지 갱신 실패");
                    failCall?.Invoke();
                    return;
                }

                saveNum++;
                _stageData = callback.GetInDate();
                Debug.Log("저장 완료");
            });

            Backend.PlayerData.UpdateMyLatestData("SkillData", skillParam, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.Log("스킬 데이터 갱신 실패");
                    failCall?.Invoke();
                    return;
                }

                saveNum++;
                Debug.Log("저장 완료");
            });

            if (call != null)
            {
                call();
            }
            Debug.Log(saveNum + " 저장");
        }

        public IEnumerator LoadData(string sceneName)
        {
            bool invenLoad = false;
            bool levelStatLoad = false;
            bool goodsLoads = false;
            bool stageLoads = false;
            bool userInfoLoads = false;
            bool skillDataLoads = false;

            StartCoroutine(TrackLoadingTime());

            //CombatDataInsert();

            try
            {
                GetLevelStatData("", () =>
                {
                    Debug.Log($"GetLevelStatData callback is called");
                    levelStatLoad = true;
                    
                });
                GetInvenData("", () =>
                {
                    Debug.Log($"GetInvenData callback is called");
                    invenLoad = true;

                });

                Debug.Log($"LoadData 1");
                

                GetGoodsSkinPetData("", () =>
                {
                    Debug.Log($"GetGoodsSkinPetData callback is called");
                    goodsLoads = true;
                });

                Debug.Log($"LoadData 2");

                GetStageCoupon("", () =>
                {
                    Debug.Log($"GetStageCoupon callback is called");
                    stageLoads = true;
                });

                Debug.Log($"LoadData 3");

                GetUserInfo(() =>
                {
                    Debug.Log($"GetUserInfo callback is called");
                    userInfoLoads = true;
                    if (string.IsNullOrEmpty(_userInfo.nickname))
                    {
                        sceneName = "NameSetScene";
                    }
                });

                Debug.Log($"LoadData 4");

                GetSkillData("", () =>
                {
                    Debug.Log($"GetSkillData callback is called");
                    skillDataLoads = true;
                });

                Debug.Log($"LoadData 5");

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


            Debug.Log($"before WaitUntil 스텟데이터 :{levelStatLoad}");

            yield return new WaitUntil(() => invenLoad && levelStatLoad && goodsLoads && stageLoads && userInfoLoads && skillDataLoads);

            loadingComplete = true;

            Debug.Log($"after WaitUntil 스텟데이터 :{levelStatLoad}");

            Debug.Log($"모든 데이터 불러오기 성공, {sceneName} 이동");
            SceneManager.LoadScene(sceneName);
        }

        IEnumerator TrackLoadingTime()
        {
            float userDataLoadDelayTime = 0;

            while (userDataLoadDelayTime < 120.0f)
            {
                if (loadingComplete)  // 로딩 완료 플래그 검사
                {
                    yield break;  // 로딩이 완료되면 코루틴 종료
                }
                yield return null;
                userDataLoadDelayTime += Time.deltaTime;
            }

            // 120초가 지나면 로딩 실패 메시지 표시 및 게임 종료
            userDataLoadfail_Text.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            Application.Quit();
        }

        public IEnumerator LoadDataInit(string sceneName)
        {

            yield return new WaitForSeconds(0.1f);
            Debug.Log($"모든 데이터 불러오기 성공, {sceneName} 이동");
            SceneManager.LoadScene(sceneName);
        }

        public void GetInvenData(string firstkey, Action call = null)
        {

            if (string.IsNullOrEmpty(firstkey))
            {
                Backend.PlayerData.GetMyData("InventoryData", 10, callback =>
                {
                    Debug.Log($"GetInvendata GetMydataCallback : {callback}");
                    if (!callback.IsSuccess())
                    {
                        Debug.LogError(callback.GetErrorCode());
                        FindObjectOfType<LoginManager>().LoginFailed();
                        return;
                    }

                    Debug.Log("인벤 로드 시작");
                    if (callback.HasFirstKey())
                    {
                        GetInvenData(callback.FirstKeystring(), call);
                    }
                    else
                    {
                        _invenIndate = callback.GetInDate();

                        Debug.Log("인벤 데이터 불러오기 완료\n" + callback.FlattenRows().ToJson());
                        try
                        {
                            ItemSaveData = new ItemSaveClass(callback.FlattenRows()[0]);
                            call();
                            //FindComponents();
                            // 플레이어 장비
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            throw;
                        }
                    }
                });
                return;
            }

            Backend.PlayerData.GetMyData("InventoryData", 10, firstkey, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError(callback.GetErrorCode());
                    FindObjectOfType<LoginManager>().LoginFailed();
                    return;
                }

                if (callback.HasFirstKey())
                {
                    GetInvenData(callback.FirstKeystring(), call);
                }
                else
                {
                    Debug.Log("인벤 데이터 불러오기 완료\n" + callback.FlattenRows().ToJson());
                    try
                    {
                        ItemSaveData = new ItemSaveClass(callback.FlattenRows()[0]);
                        call();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw;
                    }
                }
            });
        }

        public void GetGoodsSkinPetData(string firstkey, Action call = null)
        {

            if (string.IsNullOrEmpty(firstkey))
            {

                Backend.PlayerData.GetMyData("SkinPetGoods", 10, callback =>
                {
                    if (!callback.IsSuccess())
                    {
                        Debug.LogError("재화, 스킨, 펫 정보 실패 : " + callback.GetErrorCode());
                        FindObjectOfType<LoginManager>().LoginFailed();
                        return;
                    }

                    Debug.Log("재화, 스킨, 펫 정보 불러오기 시작");
                    if (callback.HasFirstKey())
                    {
                        GetInvenData(callback.FirstKeystring(), call);
                    }
                    else
                    {
                        Debug.Log("재화, 스킨, 펫 정보 로드 완료\n" + callback.FlattenRows().ToJson());
                        try
                        {
                            _skinPetGoodsIndate = callback.GetInDate();
                            ETCData = new SkinPetGoodsData(callback.FlattenRows()[0]);
                            if (ETCData.GetCurrentPetData() != null)
                            {
                                CurPet = ETCData.GetCurrentPetData();
                            }

                            havePets = ETCData.GetHavePet();
                            haveSkins = ETCData.GetHaveSkin();
                            CurSkin = ETCData.GetCurrentSkinData();
                            call();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                });
                return;
            }

            Backend.PlayerData.GetMyData("SkinPetGoods", 10, firstkey, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.Log(callback.GetErrorCode());
                    FindObjectOfType<LoginManager>().LoginFailed();
                    return;
                }

                if (callback.HasFirstKey())
                {
                    GetInvenData(callback.FirstKeystring(), call);
                }
                else
                {
                    Debug.Log("재화, 스킨, 펫 정보 로드 완료\n" + callback.FlattenRows().ToJson());
                    try
                    {
                        _skinPetGoodsIndate = callback.GetInDate();
                        ETCData = new SkinPetGoodsData(callback.FlattenRows()[0]);
                        if (ETCData.GetCurrentPetData() != null)
                        {
                            CurPet = ETCData.GetCurrentPetData();
                        }

                        havePets = ETCData.GetHavePet();
                        haveSkins = ETCData.GetHaveSkin();
                        CurSkin = ETCData.GetCurrentSkinData();
                        call();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            });
        }

        public void GetLevelStatData(string firstkey, Action call = null)
        {
            if (string.IsNullOrEmpty(firstkey))
            {
                Backend.PlayerData.GetMyData("PlayerLevelStatData", 10, callback =>
                {
                    if (!callback.IsSuccess())
                    {
                        Debug.LogError("불러오기 실패");
                        FindObjectOfType<LoginManager>().LoginFailed();
                        return;
                    }
                    
                    //Debug.Log($"GetLevelStatData 스텟데이터 불러오기 시작 callback.HasFirstKey() : {callback.HasFirstKey()}");

                    if (callback.HasFirstKey()) // 가져와야할 데이터가 100개가 넘어갈시callback.HasFirstKey() = true
                    {                           // true일시 Backend.PlayerData.GetMyData("PlayerLevelStatData", 10, firstkey, callback =>로 간다.
                                                // 즉 가져와야할 데이터가 100개가 넘어가지 않으면 실행되지 않는다.
                        Debug.Log($"HasFirstKey Action() : {callback.HasFirstKey()}");
                        GetLevelStatData(callback.FirstKeystring(), call);
                    }
                    else
                    {
                        // 이 부분에서 데이터 처리
                        _playerLevelStatDataIndate = callback.GetInDate(); // PlayerLevelStatData테이블의 InData
                        Debug.Log($"_playerLevelStatDataIndate : {_playerLevelStatDataIndate}");
                        try
                        {
                            //var flattenRows = callback.FlattenRows();

                            //JArray jsonArray = JArray.Parse(flattenRows.ToJson());

                            //JObject firstElement = (JObject)jsonArray[0];

                            //if (firstElement["Combat"] != null)
                            //{
                            LevelStatData = new PlayerLevelStatData(callback.FlattenRows()[0]);
                            //}
                            //else
                            //{

                            //}
                            //LevelStatData = new PlayerLevelStatData(callback.FlattenRows()[0]);
                            Param levelParam = LevelStatData.ToParam();

                            // 생성자를 호출하고
                            // Param을 넣어준다.
                            // PlayerLevelStatData(callback.FlattenRows()[0]);은 매개변수가 LitJson이다. LitJson은 백엔드에서 값을 가져올떄 LitJson값을 받는다.
                            // 고로 백엔드에 Combat의 값이 존재해야한다

                            Backend.PlayerData.UpdateMyLatestData("PlayerLevelStatData", levelParam, callback =>
                            {
                                if (!callback.IsSuccess())
                                {
                                    Debug.LogError("레벨 스탯 갱신 실패");
                                    return;
                                }
                                _playerLevelStatDataIndate = callback.GetInDate();
                                Debug.Log("저장 완료");
                            });
                            // Updatedata

                            //if (flattenRows != null && flattenRows.Count > 0 && flattenRows[0] != null)
                            //{
                            //    var firstRow = flattenRows[0]; // 첫 번째 행 가져오기

                            //    // "Combat" 키가 존재하지 않으면 추가
                            //    if (!firstRow.ContainsKey("Combat"))
                            //    {
                            //        Debug.Log($"flattenRows[0]에 Combat 키가 존재하지 않습니다.");
                            //        Param param = new Param();
                            //        param.Add("Combat", LevelStatData.Combat);
                            //        Backend.PlayerData.UpdateMyLatestData("PlayerLevelStatData", param, (callback) =>
                            //        {
                            //            // 이후 처리
                            //        });
                            //    }
                            //    else
                            //    {
                            //        Debug.Log("이미 Combat 데이터가 존재합니다.");
                            //    }

                            //    Debug.Log($"Second callback.FlattenRows()[0] : {flattenRows.ToJson()}");

                            //    if (firstRow != null)
                            //    {
                            //        LevelStatData = new PlayerLevelStatData(firstRow);
                            //    }
                            //    else
                            //    {
                            //        LevelStatData = new PlayerLevelStatData(); // PlayerLevelStatData테이블의 row가 비어있을경우.
                            //    }
                            //}
                            //else
                            //{
                            //    Debug.Log($"!flattenRows[0].ContainsKey(Combat)에 진입하지 못했습니다.");
                            //}

                            //if (flattenRows != null && flattenRows.Count > 0 && flattenRows[0] != null)
                            //{
                            //    if (!flattenRows[0].ContainsKey("Combat"))
                            //    {
                            //        Debug.Log($"flattenRows[0].ContainsKey(Combat)가 존재하지 않습니다.");
                            //        Param param = new Param();

                            //        //Debug.Log($"LevelStatData.Combat : {LevelStatData.Combat}");
                            //        param.Add("Combat", LevelStatData.Combat);
                            //    }
                            //    else
                            //    {
                            //        Debug.Log("이미 Combat데이터가 존재합니다.");
                            //    }

                            //    Debug.Log($"Second callback.FlattenRows()[0] : {flattenRows.ToJson()}");

                            //    if (callback.FlattenRows()[0] != null)
                            //    {
                            //        LevelStatData = new PlayerLevelStatData(callback.FlattenRows()[0]); // 이 부분이구나.
                            //    }
                            //    else
                            //    {
                            //        LevelStatData = new PlayerLevelStatData(); // PlayerLevelStatData테이블의 row가 비어있을경우.
                            //    }
                            //}
                            //else
                            //{
                            //    Debug.Log($"!flattenRows[0].ContainsKey(Combat)에 진입하지 못했습니다.");
                            //}
                            //LevelStatData = new PlayerLevelStatData(callback.FlattenRows()[0]);

                            //Debug.Log(
                            //    $"{SaveManager.Instance.LevelStatData.CurLv} {SaveManager.Instance.LevelStatData.CurEXP}" +
                            //    $"{SaveManager.Instance.LevelStatData.SP} {SaveManager.Instance.LevelStatData.Dmg} {SaveManager.Instance.LevelStatData.Dex}" +
                            //    $"{SaveManager.Instance.LevelStatData.MDef} {SaveManager.Instance.LevelStatData.HP} ");
                            Debug.Log("모든 스텟데이터 불러오기 성공");
                            call();
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                    }
                });
                return;
            }

            Backend.PlayerData.GetMyData("PlayerLevelStatData", 10, firstkey, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError("스텟 데이터 불러오기 실패");
                    FindObjectOfType<LoginManager>().LoginFailed();
                    return;
                }

                if (callback.HasFirstKey())
                {
                    Debug.Log($"HasFirstKey NON Action() : {callback.HasFirstKey()}");
                    GetLevelStatData(callback.FirstKeystring(), call);
                }
                else
                {
                    _playerLevelStatDataIndate = callback.GetInDate();
                    try
                    {
                        LevelStatData = new PlayerLevelStatData(callback.FlattenRows()[0]);
                        
                        Debug.Log(
                            $"{SaveManager.Instance.LevelStatData.CurLv} {SaveManager.Instance.LevelStatData.CurEXP}" +
                            $"{SaveManager.Instance.LevelStatData.SP} {SaveManager.Instance.LevelStatData.Dmg} {SaveManager.Instance.LevelStatData.Dex}" +
                            $"{SaveManager.Instance.LevelStatData.MDef} {SaveManager.Instance.LevelStatData.HP} ");
                        Debug.Log("스텟 데이터 불러오기 성공");
                        call();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            });
        }

        public void GetStageCoupon(string firstkey, Action call = null)
        {
            if (string.IsNullOrEmpty(firstkey))
            {
                Backend.PlayerData.GetMyData("StageCoupon", 10, callback =>
                {
                    if (!callback.IsSuccess())
                    {
                        Debug.Log("스테이지 쿠폰 데이터 불러오기 실패");
                        FindObjectOfType<LoginManager>().LoginFailed();
                        return;
                    }

                    if (callback.HasFirstKey())
                    {
                        GetStageCoupon(callback.FirstKeystring(), call);
                    }
                    else
                    {
                        try
                        {
                            StageCoupon = new StageCouponData(callback.FlattenRows()[0]);
                            _stageData = callback.GetInDate();
                            call();
                            Debug.Log($"GetStageCoupon 1 스테이지 쿠폰 데이터 불러오기 성공");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            throw;
                        }
                    }
                });
                return;
            }

            Backend.PlayerData.GetMyData("StageCoupon", 10, firstkey, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.Log("스테이지 쿠폰 데이터 불러오기 실패");
                    FindObjectOfType<LoginManager>().LoginFailed();
                    return;
                }

                if (callback.HasFirstKey())
                {
                    GetStageCoupon(callback.FirstKeystring(), call);
                }
                else
                {

                    try
                    {
                        Debug.Log("GetStageCoupon 2 스테이지 쿠폰 데이터 불러오기 성공");
                        StageCoupon = new StageCouponData(callback.FlattenRows()[0]);
                        _stageData = callback.GetInDate();
                        call();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw;
                    }
                }
            });
        }

        public void GetSkillData(string firstkey, Action call = null)
        {
            if (string.IsNullOrEmpty(firstkey))
            {
                Backend.PlayerData.GetMyData("SkillData", 10, callback =>
                {
                    if (!callback.IsSuccess())
                    {
                        Debug.Log("스킬 데이터 불러오기 실패");
                        FindObjectOfType<LoginManager>().LoginFailed();
                        return;
                    }

                    if (callback.HasFirstKey())
                    {
                        GetStageCoupon(callback.FirstKeystring(), call);
                    }
                    else
                    {
                        try
                        {
                            SkillData = new SkillData(callback.FlattenRows()[0]);
                            call();
                            Debug.Log("GetSkillData 1 스킬 데이터 불러오기 성공");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            throw;
                        }
                    }
                });
                return;
            }

            Backend.PlayerData.GetMyData("SkillData", 10, firstkey, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.Log("스킬 데이터 불러오기 실패");
                    FindObjectOfType<LoginManager>().LoginFailed();
                    return;
                }

                if (callback.HasFirstKey())
                {
                    GetStageCoupon(callback.FirstKeystring(), call);
                }
                else
                {
                    try
                    {
                        SkillData = new SkillData(callback.FlattenRows()[0]);
                        call();
                        Debug.Log("GetSkillData 2 스킬 데이터 불러오기 성공");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw;
                    }
                }
            });
        }

        public void GetUserInfo(Action call = null)
            {
                Backend.BMember.GetUserInfo(callback =>
                {
                    if (!callback.IsSuccess())
                    {
                        Debug.LogError("Error : " + callback.ToString());
                        FindObjectOfType<LoginManager>().LoginFailed();
                        return;
                    }

                    try
                    {
                        JsonData userInfoJson = callback.GetReturnValuetoJSON()["row"];

                        _userInfo = new UserInfoData();

                        _userInfo.gamerId = userInfoJson["gamerId"]?.ToString();
                        _userInfo.countryCode = userInfoJson["countryCode"]?.ToString();
                        if (string.IsNullOrEmpty(userInfoJson["nickname"].ToString()))
                        {
                            call?.Invoke();
                            return;
                        }
                        _userInfo.nickname = userInfoJson["nickname"].ToString();
                        _userInfo.inDate = userInfoJson["inDate"]?.ToString();
                        _userInfo.emailForFindPassword = userInfoJson["emailForFindPassword"]?.ToString();
                        _userInfo.subscriptionType = userInfoJson["subscriptionType"]?.ToString();
                        _userInfo.federationId = userInfoJson["federationId"]?.ToString();
                        if (call != null) call();
                        Debug.Log(_userInfo.ToString());
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        call?.Invoke();
                    }
                });
         }
        
        public void GetUserInfoTest(Action call = null)
        {
            Backend.BMember.GetUserInfo(callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError("Error : " + callback.ToString());
                    FindObjectOfType<LoginManager>().LoginFailed();
                    return;
                }

                JsonData userInfoJson = callback.GetReturnValuetoJSON()["row"];

                _userInfo = new UserInfoData();

                _userInfo.gamerId = userInfoJson["gamerId"]?.ToString();
                _userInfo.countryCode = userInfoJson["countryCode"]?.ToString();
                if (string.IsNullOrEmpty(userInfoJson["nickname"].ToString()))
                {
                    Debug.Log(userInfoJson["nickname"].Count);
                    call?.Invoke();
                    return;
                }
                _userInfo.nickname = userInfoJson["nickname"].ToString();
                _userInfo.inDate = userInfoJson["inDate"]?.ToString();
                _userInfo.emailForFindPassword = userInfoJson["emailForFindPassword"]?.ToString();
                _userInfo.subscriptionType = userInfoJson["subscriptionType"]?.ToString();
                _userInfo.federationId = userInfoJson["federationId"]?.ToString();
                Debug.Log(_userInfo.ToString());
            });
        }

            //
            private void FindComponents()
            {
                _codeSystem = GameObject.Find("CodeSystem").GetComponent<CodeSystem>();
                _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
                _goodsManager = GameObject.Find("GoodsManager").GetComponent<GoodsManager>();
                _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
                _playerEquipment = GameObject.Find("Player").GetComponent<PlayerEquipment>();
                _playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
                _playerSkinSystem = GameObject.Find("Player").GetComponent<PlayerSkinSystem>();
                _petSystem = GameObject.Find("Player").GetComponent<PetSystem>();
                _stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
                _player = GameObject.Find("Player").GetComponent<Player>();
            }

            Item GetItemData(int id, int type, int enforce)
            {
                return ItemDataManager.Instance.GetItem(id, type, enforce);
            }
        
        
        private void OnApplicationQuit()
        {
            UpdateData();
            Debug.Log("데이터 저장 완료");
        }
    }
 }