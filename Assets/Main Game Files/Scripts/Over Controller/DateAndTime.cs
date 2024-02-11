using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    NOTE :
    Use localDateTime if you want to use this 
    on a seconds based task.
 
    Skills : I want to use server time but i think it will be heavy on 
    performance so used localDateTime but it has some limitations like 
    higher level skills that will have higher cooldown

    TODO:
    1. Sync localDateTime with server on instance
 
 */

public class DateAndTime : MonoBehaviour {
    private DateTime localDateTime;
    private float updateInterval = 1f;

    private void Awake() {
        localDateTime = DateTime.Now;
    }

    private void Start() {
        StartCoroutine(nameof(UpdateLocalDateTime));
    }

    public String GetSkillReferenceDateTime() {
        return localDateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private IEnumerator UpdateLocalDateTime() {
        while (true) {
            yield return new WaitForSeconds(updateInterval);
            localDateTime = DateTime.Now;
        }
    }
}
