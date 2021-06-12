﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HangarController : MonoBehaviour
{
    public static HangarController instance;
    public HangarUI hangarUi;

    [Header("Game Data")]
    public ResourceValues resources;

    private void Awake()
    {
        #region Singleton
        HangarController[] list = FindObjectsOfType<HangarController>();
        if (list.Length > 1)
        {
            Destroy(this);
            Debug.Log("Multiple instances of the HangarController component detected. Destroying an instance.");
        }
        else
        {
            instance = this;
        }
        #endregion
    }

    private void Start()
    {
        RegisterListeners();
        hangarUi.RegisterListeners();
        hangarUi.UpdateModulePrices();
        ActivateUI();
    }

    public void ActivateUI()
    {
        LevelRecord levelRecord = GameManager.instance.levelRecord;

        if (GameManager.instance.levelRecord != null)
        {

            if (levelRecord.ironCollected != 0 || levelRecord.silverCollected != 0 || levelRecord.goldCollected != 0)
            {
                CalculateResources();
                hangarUi.SetEndLevelText();
                SellResources();
            }
            else
            {
                GameManager.instance.levelRecord = null;
                SetToNavigation();
            }

        }
        else
        {
            SetToNavigation();
        }

        PlayerProfile profile = ProfileManager.instance.currentProfile;

        if (profile.isDead)
        {
            profile.isDead = false;
            profile.balance -= GameManager.instance.CalcDeathCost();
            EventManager.TriggerEvent("ReturnedFromDeath");
            ProfileManager.instance.SaveProfile();
        }
    }

    #region Private Methods
    private void RegisterListeners()
    {
    }

    private void SetToNavigation()
    {
        EventManager.TriggerEvent("UpdateBalance");
        hangarUi.SetScreen("NavigationMenu");
    }

    private void CalculateResources()
    {
        LevelRecord record = GameManager.instance.levelRecord;
        float profitMultiplier = hangarUi.stats.currentProfitBoost;

        record.ironTotalValue = record.ironCollected * resources.ironValue * profitMultiplier;
        record.silverTotalValue = record.silverCollected * resources.silverValue * profitMultiplier;
        record.goldTotalValue = record.goldCollected * resources.goldValue * profitMultiplier;

        record.ironBonusValue = record.ironTotalValue - (record.ironCollected * resources.ironValue);
        record.silverBonusValue = record.silverTotalValue - (record.silverCollected * resources.silverValue);
        record.goldBonusValue = record.goldTotalValue - (record.goldCollected * resources.goldValue);
    }


    private void SellResources()
    {
        Debug.Log("Sell Resources called");
        LevelRecord record = GameManager.instance.levelRecord;

        float total = record.ironTotalValue + record.silverTotalValue + record.goldTotalValue;
        ProfileManager.instance.currentProfile.balance += total;
        ProfileManager.instance.SaveProfile();
        GameManager.instance.levelRecord = null;
        EventManager.TriggerEvent("UpdateBalance");
    }
    #endregion

    private void TestingMode()
    {

    }
}
