using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using JetBrains.Annotations;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public PlayerLevelStatData()
        {
            CurLv = 1;
            CurEXP = 0;
            SP = 0;
            Dmg = 0;
            Dex = 0;
            MDef = 0;
            HP = 0;
        }

        public PlayerLevelStatData(int level, float exp, int point, int dmg, int dex, int mdef, int hp)
        {
            CurLv = level;
            CurEXP = exp;
            SP = point;
            Dmg = dmg;
            Dex = dex;
            MDef = mdef;
            HP = hp;
        }

        public PlayerLevelStatData(LitJson.JsonData json)
        {
            CurLv = int.Parse(json["CLv"].ToString());
            CurEXP = float.Parse(json["CEXP"].ToString());
            SP = int.Parse(json["SP"].ToString());
            Dmg = int.Parse(json["Dmg"].ToString());
            Dex = int.Parse(json["Dex"].ToString());
            MDef = int.Parse(json["MDef"].ToString());
            HP = int.Parse(json["HP"].ToString());
        }

        public Param ToParam(int level, float exp, int point, int dmg, int dex, int mdef, int hp)
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
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void Update()
        {
            if (Backend.IsInitialized)
            {
                Backend.AsyncPoll();
            }
        }

        public void InitData()
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
            ItemSaveData.SetItem(_inventory);
            ItemSaveData.SetEquipData(_playerEquipment);

            ETCData = new SkinPetGoodsData(haveSkins, havePets, CurSkin, CurPet, _goodsManager.GoldCnt,
                _goodsManager.DiaCnt, _goodsManager.ScrollCnt);
            Param skinParam = ETCData.ToParam();
            LevelStatData = new PlayerLevelStatData(_playerStat.Level, _playerStat.CurEXP, _playerStat.StatPoint,
                _playerStat.DmgLevel, _playerStat.Dex, _playerStat.MDefLevel, _playerStat.HPLv);
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
            ItemSaveData.SetItem(_inventory);
            ItemSaveData.SetEquipData(_playerEquipment);

            ETCData = new SkinPetGoodsData(haveSkins, havePets, CurSkin, CurPet, _goodsManager.GoldCnt,
                _goodsManager.DiaCnt, _goodsManager.ScrollCnt);
            Param skinParam = ETCData.ToParam();
            LevelStatData = new PlayerLevelStatData(_playerStat.Level, _playerStat.CurEXP, _playerStat.StatPoint,
                _playerStat.DmgLevel, _playerStat.Dex, _playerStat.MDefLevel, _playerStat.HPLv);
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

            try
            {
                GetLevelStatData("", () =>
                {
                    levelStatLoad = true;
                });
                GetInvenData("", () =>
                {
                    invenLoad = true;
                });
                GetGoodsSkinPetData("", () =>
                {
                    goodsLoads = true;
                });
                GetStageCoupon("", () =>
                {
                    stageLoads = true;
                });
                GetUserInfo(() =>
                {
                    userInfoLoads = true;
                    if (string.IsNullOrEmpty(_userInfo.nickname))
                    {
                        sceneName = "NameSetScene";
                    }
                });
                
                GetSkillData("", () =>
                {
                    skillDataLoads = true;
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            yield return new WaitUntil(() => invenLoad && levelStatLoad && goodsLoads && stageLoads && userInfoLoads && skillDataLoads);

            Debug.Log($"모든 데이터 불러오기 성공, {sceneName} 이동");
            SceneManager.LoadScene(sceneName);
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

                    Debug.Log("불러오기 시작");

                    if (callback.HasFirstKey())
                    {
                        GetLevelStatData(callback.FirstKeystring(), call);
                    }
                    else
                    {
                        _playerLevelStatDataIndate = callback.GetInDate();
                        Debug.Log(callback.FlattenRows()[0]);
                        try
                        {
                            LevelStatData = new PlayerLevelStatData(callback.FlattenRows()[0]);
                            Debug.Log(
                                $"{SaveManager.Instance.LevelStatData.CurLv} {SaveManager.Instance.LevelStatData.CurEXP}" +
                                $"{SaveManager.Instance.LevelStatData.SP} {SaveManager.Instance.LevelStatData.Dmg} {SaveManager.Instance.LevelStatData.Dex}" +
                                $"{SaveManager.Instance.LevelStatData.MDef} {SaveManager.Instance.LevelStatData.HP} ");
                            Debug.Log("불러오기 성공");
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
                    Debug.LogError("불러오기 실패");
                    FindObjectOfType<LoginManager>().LoginFailed();
                    return;
                }

                if (callback.HasFirstKey())
                {
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
                        Debug.Log("불러오기 성공");
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
                            Debug.Log("스테이지 쿠폰 데이터 불러오기 성공");
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
                        Debug.Log("스테이지 쿠폰 데이터 불러오기 성공");
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
                            Debug.Log("스킬 데이터 불러오기 성공");
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
                        Debug.Log("스킬 데이터 불러오기 성공");
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
        }
    }