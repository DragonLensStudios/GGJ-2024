using System;
using System.Collections;
using System.Collections.Generic;
using DLS.Managers;
using TMPro;
using UnityEngine;

public class CountDownTimerUI : MonoBehaviour
{
    [field: SerializeField] TMP_Text CountDownText { get; set; }

    private void Update()
    {
        CountDownText.text = $"{TimeManager.Instance.CountDownTimer.Minute}:{TimeManager.Instance.CountDownTimer.Second:00}";
    }
}
