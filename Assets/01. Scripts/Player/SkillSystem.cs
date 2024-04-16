using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using gunggme;
using UnityEngine;
using UnityEngine.UI;

public class SkillData
{
    public List<string> HaveSkills;

    public SkillData()
    {
        HaveSkills = new List<string>();
    }

    public SkillData(LitJson.JsonData json)
    {
        HaveSkills = new List<string>();

        for (int i = 0; i < json["HaveSkills"].Count; i++)
        {
            HaveSkills.Add(json["HaveSkills"][i].ToString());
        }
    }

    public Param ToParam()
    {
        Param param = new Param();

        param.Add("HaveSkills", HaveSkills);
        
        return param;
    }
}

public class SkillSystem : MonoBehaviour
{
    
    private List<Slider> _sliderList;
    [SerializeField] private List<string> _haveSkillList;
    [SerializeField] private List<float> _skillCoolTimeList;
    // Blizzard
    [SerializeField] private BlizzardSkill _blizzardContainer;
    // Meteor
    [SerializeField] private List<Vector2> _meteorStartList;
    [SerializeField] private Transform _bulletBorderRight;
    [SerializeField] private Transform _bulletBorderBottom;
    
    public bool isAuto = false;


    private Player _player;
    private MonsterManager _monsterManager;
    private void Start()
    {
        _monsterManager = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _sliderList = GameObject.Find("Skills").GetComponentsInChildren<Slider>().ToList();

#if UNITY_EDITOR
        _haveSkillList = new List<string> {"Normal", "Advanced", "Rare", "Hero", "Legend", "Transcendence", "God"};
#else
        _haveSkillList = SaveManager.Instance.SkillData.HaveSkills;
#endif
        isAuto = PlayerPrefs.GetInt("isAuto") == 1 ? true : false;
        for (int i = 0; i < _haveSkillList.Count; i++)
            _skillCoolTimeList.Add(0f);
        foreach (var t in _sliderList)
        {
            t.gameObject.SetActive(_haveSkillList.Find(index => index == t.name) != null);
        }
    }

    private void Update()
    {
        UpdateSkillCoolTime();
        DoAutoSKill();
    }

    private void UpdateSkillCoolTime()
    {
        for (int i = 0; i < _skillCoolTimeList.Count; i++)
        {
            if (_skillCoolTimeList[i] > 0)
            {
                _skillCoolTimeList[i] -= Time.deltaTime;
                _sliderList[i].value = _skillCoolTimeList[i] / 30f;
            }
        }
    }

    private int _nextSkillIdx = 0;
    private float _waitTime = 0f;
    private void DoAutoSKill()
    {
        if (!isAuto)
            return;
        if (_player._nearEnem == null) return;
        if (_haveSkillList.Count == 0)
            return;
        if (_waitTime > 0)
        {
            _waitTime -= Time.deltaTime;
            return;
        }
        _waitTime = 0.5f;

        if (_nextSkillIdx >= _skillCoolTimeList.Count)
            _nextSkillIdx = 0;

        if (_skillCoolTimeList[_nextSkillIdx] > 0)
        {
            _nextSkillIdx++;
            return;
        }

        SkillBtn(_nextSkillIdx++);
    }

    public void SkillBtn(int skillNum)
    {
        if (_skillCoolTimeList[skillNum] > 0)
            return;
        if (!_sliderList[skillNum].gameObject.activeSelf)
            return;
        
        _skillCoolTimeList[skillNum] = 30f;
        GameObject temp;

        switch (skillNum)
        {
            case 0: // Common(일반스킬)
                SoundManager.Instance.PlaySound(SoundType.VFX, 1);
                Transform targetTrans = _player._nearEnem ? _player._nearEnem.transform : _bulletBorderRight;
                temp = PoolManager.Instance.Get(0);
                temp.GetComponent<Bullet>().SetTarget(targetTrans, Mathf.RoundToInt(_player._playerStat.Dmg * 1.05f), Vector2.one * 0.7f);
                temp.GetComponent<Bullet>().SetSprite(2);
                temp.transform.position = _player._firePos.position;
                break;
            
            case 1: // Advanced(파이어볼)
                SoundManager.Instance.PlaySound(SoundType.VFX, 2);
                temp = PoolManager.Instance.Get(6);
                temp.GetComponent<Bullet>().SetTarget(_bulletBorderRight, Mathf.RoundToInt(_player._playerStat.Dmg * 1.1f));
                temp.transform.position = _player._firePos.position;
                break;
            
            case 2: // Rare
                
                _monsterManager.RemainEnemy[0].GetComponent<Enemy>().SetStunTime();
                break;
            
            case 3: // Hero(블리자드)
                // StartCoroutine(PoolManager.Instance.Get(6).GetComponent<BlizzardSkill>().DoSkill(_monsterManager,
                //     Mathf.RoundToInt(_player._playerStat.Dmg * 1.3f)));
                SoundManager.Instance.PlaySound(SoundType.VFX, 3);
                StartCoroutine(_blizzardContainer.DoSkill(_monsterManager,
                    Mathf.RoundToInt(_player._playerStat.Dmg * 1.3f)));
                break;
            
            case 4: // Legend
                for (int i = 0; i < 3; i++)
                {
                    temp = PoolManager.Instance.Get(7);
                    temp.GetComponent<Bullet>().SetTarget(_bulletBorderBottom, Mathf.RoundToInt(_player._playerStat.Dmg * 1.5f), Vector2.one * 0.7f);
                    temp.GetComponent<Bullet>().SetSprite(i);
                    temp.transform.position = _meteorStartList[i];
                }
                break;
            
            case 5: // Transcen
                temp = PoolManager.Instance.Get(8);
                temp.GetComponent<Bullet>().SetTarget(_bulletBorderBottom, Mathf.RoundToInt(_player._playerStat.Dmg * 3.5f));
                temp.transform.position = _meteorStartList[1];
                break;
            
            case 6: // God(무적)
                // 대미지 10배 증가는 PlayerStat.Dmg에 있음
                SoundManager.Instance.PlaySound(SoundType.VFX, 4);
                _player.SetGodModTime();
                break;
        }
    }

    public void GetSkill(string skillName)
    {
        if (_haveSkillList.Find(i => i == skillName) != null)
            return;
        
        _haveSkillList.Add(skillName);
        int max = 0;
        for (int i = 0; i < _sliderList.Count; i++)
        {
            if (_haveSkillList.Find(index => index == _sliderList[i].name) != null)
            {
                _sliderList[i].gameObject.SetActive(true);
                max = i;
            }
        }
        
        for (int i = _skillCoolTimeList.Count; i <= max; i++)
            _skillCoolTimeList.Add(0f);
    }
    
    public void AutoSKillBtn(GameObject childObj)
    {
        childObj.SetActive(isAuto);
        PlayerPrefs.SetInt("isAuto", isAuto ? 1 : 0);
        isAuto = !isAuto;
    }
}
