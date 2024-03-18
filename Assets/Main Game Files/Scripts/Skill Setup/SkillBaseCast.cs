using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillBaseCast : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject generalSettings;
    [SerializeField] private GameObject skillSettings;
    [SerializeField] private GameObject swordsmanSkills;
    [SerializeField] private GameObject objectPoolManagerGO;
    [SerializeField] private GameObject damageUIManager;
    [SerializeField] private GameObject skillDurationWindow;

    [Header("UI Damage")]
    [SerializeField] private GameObject damageTextPrefab;


    [Header("UI Cooldown Window Item")]
    [SerializeField] private GameObject skillCooldownItemPrefab;

    [Space(2)]

    [Header("Variable Declarations and other assignment")]
    [SerializeField] private bool enableQuickSkillCancelDelay;

    private int skillID;
    private bool enableCancellingSkill;
    private bool didCastSkill;
    private bool isCastingSkill;
    private AnimationPlayer animationPlayer;
    private TargetManager targetManager;
    private WeaponWeilding weaponWeilding;
    private BasicAnimation basicAnimation;
    private ObjectPoolManager objectPoolManager;
    private ObjectPoolManager damageTextObjectPoolManager;
    private PlayerStatsController playerStatsController;
    private PlayerStatsManager playerStatsManager;
    private TagLookAtCamera tagLookAtCamera;
    private SkillReference skillReference;
    private ObjectPoolManager buffEffectPool;

    private Coroutine coroutineCancelDelay;

    /* Swordsman Skills */
    private Ice_Crack ice_Crack;
    private LightningStrike lightningStrike;
    private GroundImpact groundImpact;
    private Whirlwind whirlwind;
    private LightningOrb lightningOrb;
    private IceDownWave iceDownWave;
    private SpeedUp speedUp;
    private HealthUp healthUp;
    private BerserkAura berserkAura;
    private SwordsBlessing swordsBlessing;

    #region GetSet Properties
    public int GetSetSkillID {
        get { return skillID; }
        set { skillID = value; }
    }

    public AnimationPlayer GetSetAnimationPlayer {
        get { return animationPlayer; }
        set { animationPlayer = value; }
    }

    public TargetManager GetSetTargetManager {
        get { return targetManager; }
        set { targetManager = value; }
    }

    public WeaponWeilding GetSetWeaponWeilding {
        get { return weaponWeilding; }
        set { weaponWeilding = value; }
    }

    public BasicAnimation GetSetBasicAnimation {
        get { return basicAnimation; }
        set { basicAnimation = value; }
    }

    public SkillReference GetSetSkillReference {
        get { return skillReference; }
        set { skillReference = value; }
    }

    public ObjectPoolManager GetSetObjectPoolManager {
        get { return objectPoolManager; }
        set { objectPoolManager = value; }
    }

    public PlayerStatsController GetSetPlayerStatsController {
        get { return playerStatsController; }
        set { playerStatsController = value; }
    }

    public PlayerStatsManager GetSetPlayerStatsManager {
        get { return playerStatsManager; }
        set { playerStatsManager = value; }
    }

    public bool GetSetDidCastSkill {
        get { return didCastSkill; }
        set { didCastSkill = value; }
    }

    public bool GetSetEnableCancelingSkill {
        get { return enableCancellingSkill; }
        set { enableCancellingSkill = value; }
    }
    
    public bool GetSetIsCastingSkill {
        get { return isCastingSkill; }
        set { isCastingSkill = value; }
    }

    public GameObject GetSetSkillWindowCooldownGO {
        get { return skillCooldownItemPrefab; }
        set { skillCooldownItemPrefab = value; }
    }

    public GameObject GetSetSkillDurationWindow {
        get { return skillDurationWindow; }
        set { skillDurationWindow = value; }
    }
    #endregion

    private void Awake() {
        /* Base Components */
        playerStatsManager = generalSettings.GetComponent<PlayerStatsManager>();
        animationPlayer = generalSettings.GetComponent<AnimationPlayer>();
        weaponWeilding = generalSettings.GetComponent<WeaponWeilding>();
        basicAnimation = generalSettings.GetComponent<BasicAnimation>();
        playerStatsController = generalSettings.GetComponent<PlayerStatsController>();
        targetManager = skillSettings.GetComponent<TargetManager>();
        skillReference = skillSettings.GetComponent<SkillReference>();
        objectPoolManager = objectPoolManagerGO.GetComponent<ObjectPoolManager>();
        damageTextObjectPoolManager = damageUIManager.GetComponent<ObjectPoolManager>();
        tagLookAtCamera = damageUIManager.GetComponent<TagLookAtCamera>();

        /* Swordsman Skills */
        ice_Crack = swordsmanSkills.GetComponent<Ice_Crack>();
        lightningStrike = swordsmanSkills.GetComponent<LightningStrike>();
        groundImpact = swordsmanSkills.GetComponent<GroundImpact>();
        whirlwind = swordsmanSkills.GetComponent<Whirlwind>();
        lightningOrb = swordsmanSkills.GetComponent<LightningOrb>();
        iceDownWave = swordsmanSkills.GetComponent<IceDownWave>();
        speedUp = swordsmanSkills.GetComponent<SpeedUp>();
        healthUp = swordsmanSkills.GetComponent<HealthUp>();
        berserkAura = swordsmanSkills.GetComponent<BerserkAura>();
        swordsBlessing = swordsmanSkills.GetComponent<SwordsBlessing>();
    }

    public void CastSelectedSkill(int _skillID) {
        skillID = _skillID;

        switch (skillID) {
            case 1:
                ice_Crack.ActivateSkill();
            break;
            case 2:
                groundImpact.ActivateSkill();
            break;
            case 3:
                lightningOrb.ActivateSkill();
            break;
            case 4:
                lightningStrike.ActivateSkill();
            break;
            case 5:
                healthUp.ActivateSkill();
            break;
            case 6:
                speedUp.ActivateSkill();
            break;
            case 7:
                iceDownWave.ActivateSkill();
            break;
            case 8:
                whirlwind.ActivateSkill();
            break;
            case 9:
                berserkAura.ActivateSkill();
            break;
            case 10:
                swordsBlessing.ActivateSkill();
            break;
        }
    }

    public void InitializeCancelDelay() {
        if (!enableQuickSkillCancelDelay) {
            enableCancellingSkill = false;

            if (coroutineCancelDelay != null) {
                StopCoroutine(coroutineCancelDelay);
            }

            StartCoroutine(nameof(StartInitializeCancelDelay));
        }
    }

    private IEnumerator StartInitializeCancelDelay() {
        yield return new WaitForSeconds(1.5F);
        enableCancellingSkill = true;
    }

    public void DisplayDamage(Vector3 damageTextPosition,float damage) {
        int iDamage = (int)Math.Round(damage);

        GameObject damageTextHolder = damageTextObjectPoolManager.SpawnFromPool(damageTextPrefab.name.ToString());
        TextMeshProUGUI damageText = damageTextHolder.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        damageText.SetText(iDamage.ToString());
        damageTextHolder.transform.position = damageTextPosition;
        damageTextHolder.SetActive(true);
        damageTextHolder.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: damageTextHolder);
    }
}
