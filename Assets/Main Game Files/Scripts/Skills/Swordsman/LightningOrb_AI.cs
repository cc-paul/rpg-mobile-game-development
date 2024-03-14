using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrb_AI : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject energyBallPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float speed;

    private LightningOrb lightningOrb;
    private Coroutine moveTowardsTargetCoroutine;
    private GameObject targetEnemy;

    #region LightningOrb_AI GetSet Properties 
    public GameObject GetSetTargetEnemy {
        get { return targetEnemy; }
        set { targetEnemy = value; }
    }

    public LightningOrb GetSetLightningOrb {
        get { return lightningOrb; }
        set { lightningOrb = value; }
    }
    #endregion

    private WaitForSeconds skillNullDuration = new WaitForSeconds(0f);
    private WaitForSeconds skillDestoyAfterHit = new WaitForSeconds(2f);

    public void IntializeLightningOrb() {
        if (moveTowardsTargetCoroutine != null) {
            StopCoroutine(moveTowardsTargetCoroutine);
        }

        energyBallPrefab.SetActive(true);
        explosionPrefab.SetActive(false);
        moveTowardsTargetCoroutine = StartCoroutine(nameof(MoveTowardsTarget));
    }

    private IEnumerator MoveTowardsTarget() {
        while (Vector3.Distance(transform.position, targetEnemy.transform.position) > 0.1f) {
            transform.position = Vector3.MoveTowards(transform.position, targetEnemy.transform.position, speed * Time.deltaTime);

            if (!targetEnemy.gameObject.activeSelf) {
                DestroyBall();
                break;
            }

            yield return skillNullDuration;
        }

        energyBallPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        lightningOrb.ApplyDamage(currentTarget: targetEnemy);
        yield return skillDestoyAfterHit;
        DestroyBall();
    }

    private void DestroyBall() {
        gameObject.SetActive(false);
    }
}




/*public class LightningBallTransporter : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject electroHitPrefab;

    [Space(10)]

    [Header("Variable Declarations and Other Assignments")]
    [SerializeField] private float speed = 0.5f;

    private GameObject target;
    private LightningBall lightningBall = null;
    private bool isThereATarget = false;
    private float timeToDestroySkillEffect = 0.5f;

    public bool GetSetIsThereATarget {
        get { return isThereATarget; }
        set { isThereATarget = value; }
    }

    public GameObject GetSetTarget {
        get { return target; }
        set { target = value; }
    }

    public LightningBall GetSetLightningBall {
        get { return lightningBall; }
        set { lightningBall = value; }
    }

    private void Update() {
        if (isThereATarget && target != null) {
            MoveTowardsTarget();
        } else {
            DestroyLightningBall(true, timeToDestroySkillEffect);
        }
    }

    private void MoveTowardsTarget() {
        try {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        } catch (System.Exception) {
            DestroyLightningBall(true, 0f);
        }
    }

    private void OnTriggerEnter(Collider detectedTarget) {
        if (detectedTarget.gameObject.CompareTag(Tags.MOBS)) {
            if (target != null) {
                if (detectedTarget.gameObject == target.transform.parent.gameObject) {
                    lightningBall.ActivateDamage(detectedTarget.gameObject);
                    ShowHitEffect(detectedTarget.gameObject);
                    DestroyLightningBall(false, timeToDestroySkillEffect);
                }
            } else {
                DestroyLightningBall(false, 0f);
            }
        }
    }

    private void ShowHitEffect(GameObject target) {
        GameObject hitEffect = Instantiate(electroHitPrefab, target.transform.position, Quaternion.identity, lightningBall.GetSkillWasteLandParent().transform);

        Destroy(hitEffect, 0.5f);
    }

    private void DestroyLightningBall(bool resetTarget, float timeToDestroySkillEffect) {
        if (resetTarget) {
            isThereATarget = false;
        }
        Destroy(gameObject, timeToDestroySkillEffect);
    }
}
*/