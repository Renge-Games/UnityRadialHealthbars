using System;
using System.Collections;
using System.Collections.Generic;
using RengeGames.HealthBars;
using UnityEngine;

public class Example : MonoBehaviour
{
    private UltimateCircularHealthBar hb;
    [Range(0, 1)] public float healthPercent = 1;

    private void Start()
    {
        hb = GetComponent<UltimateCircularHealthBar>();
    }

    private void Update()
    {
        hb.SetPercent(healthPercent);
    }
}
