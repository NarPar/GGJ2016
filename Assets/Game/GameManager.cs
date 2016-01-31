﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static float POPULARITY_INTERVAL = 0.2f;

    public InputField wizardNameField;

    public string wizardName;
    public List<Spell> allSpells = new List<Spell>();

    public MenuManager menuManager;
    //public WizardDetailsUI wizardDetailsUI;
    public SpellHistoryMenu spellHistoryMenu;
    public RegionDetailsMenu regionDetailsMenu;
    public NotificationManager notificationManager;

    public float agingMultiplier = 1f;

    Wizard playerWizard;
    public Wizard PlayerWizard
    {
        get { return playerWizard; }
    }

    SpellManager spellManager;
    WizardManager wizardManager;

    public void Awake()
    {
        if (!menuManager)
            menuManager = GameObject.FindObjectOfType<MenuManager>();

        if (!notificationManager)
            notificationManager = GameObject.FindObjectOfType<NotificationManager>();

        if (!spellHistoryMenu)
            spellHistoryMenu = GameObject.FindObjectOfType<SpellHistoryMenu>();

        if (!regionDetailsMenu)
            regionDetailsMenu = GameObject.FindObjectOfType<RegionDetailsMenu>();

       // if (!wizardDetailsUI)
       //     wizardDetailsUI = GameObject.FindObjectOfType<WizardDetailsUI>();

        spellManager = GetComponent<SpellManager>();
        wizardManager = GetComponent<WizardManager>();
    }

    public void MapNodeClicked(string region)
    {
        regionDetailsMenu.Init(region);
    }

    public void ConfirmWizardNameClicked()
    {
        if (wizardNameField && wizardNameField.text != "")
        {
            wizardName = wizardNameField.text;
            
            menuManager.ShowNextScreen();
        }
    }

    public void ResearchNewSpellClicked(bool fromFirstScreen=false)
    {
        if (wizardNameField && wizardNameField.text != "")
        {
            playerWizard = wizardManager.GenerateWizard(wizardName);

            playerWizard.CurrentSpell = spellManager.GenerateRandomSpell(playerWizard);
            
            allSpells.Add(playerWizard.CurrentSpell);

            notificationManager.QueueNotification("You've researched the " + playerWizard.CurrentSpell.Name + " spell!");
            notificationManager.QueueNotification("Promote your spell! You can change which region you're promoting in by clicking the region icon.", 8f);

            spellHistoryMenu.UpdateSpellList();

            if (fromFirstScreen) menuManager.HideScreen();
        }
    }

    public void TravelToRegionClicked()
    {
        string region = regionDetailsMenu.Region;
        Debug.Log("Clicked to travel to the " + region + " region!");
    }

    float popularityIntervalTimer = 0f;
    void Update()
    {
        if (gameOver) return;

        popularityIntervalTimer -= Time.deltaTime;
        if (popularityIntervalTimer <= 0f)
        {
            if (playerWizard != null) playerWizard.AgeWizard(Time.deltaTime*10f*agingMultiplier); // every second is a 5th of a wizard year

            BroadcastMessage("PopularityTick"); // tell listening classes to update properties relative to your spells popularity

            gameObject.SendMessage("UpdateUI");
            popularityIntervalTimer = POPULARITY_INTERVAL;

            CheckGameEnd();
        }
    }

    void PopularityTick()
    {
        //Debug.Log("Popularity Ticked!");
    }

    bool gameOver = false;
    void CheckGameEnd()
    {
        if (playerWizard != null && playerWizard.Age >= 200)
        {
            gameOver = true;
            gameObject.SendMessage("OnGameOver");
        }
    }
}
