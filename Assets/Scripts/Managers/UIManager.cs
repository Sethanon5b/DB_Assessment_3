﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject loadScreen;
    public Color itemSelectionColor;
    public Color itemBackgroundColor;

    [Header("Error Modal")]
    public GameObject errorModal;
    public Button errorModalCloseButton;
    public Button errorModalOkButton;
    public TextMeshProUGUI errorModalTitle;
    public TextMeshProUGUI errorModalDescription;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public Button returnToHangarButton;
    public Button quitGameButton;
    public Button resumeGameButton;

    [Header("Events")]
    private UnityAction errorModalCantAffordDelegate;
    private UnityAction errorModalPlayerDiedDelegate;
    private UnityAction errorModalReturnedFromDeathDelegate;
    private UnityAction errorModalWrongInputDelegate;
    private UnityAction errorModalNoResultsDelegate;
    private UnityAction pauseMenuDelegate;
    private UnityAction errorModalInvalidProfileNameDelegate;
    private UnityAction errorModalInactiveOnThisPlatformDelegate;

    #region Properties
    public float LoadScreenAlpha => LoadScreenAlphaValue();
    #endregion

    private void Awake()
    {
        #region Singleton
        UIManager[] list = FindObjectsOfType<UIManager>();
        if (list.Length > 1)
        {
            Destroy(this);
            Debug.Log("Multiple instances of the UIManager component detected. Destroying an instance.");
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
    }

    #region Public Methods
    public void LoadScreen(bool active)
    {
        Image image = loadScreen.GetComponent<Image>();
        Color loadColour = image.color;

        if (active)
        {
            loadScreen.SetActive(true);
            loadColour.a = 0f;
            image.color = loadColour;
            image.CrossFadeAlpha(1, 2f, false);
        }
        else
        {
            loadColour.a = 1f;
            image.color = loadColour;
            image.CrossFadeAlpha(0, 2f, false);
            StartCoroutine("DeactivateLoadScreen");
        }
    }

    #endregion

    #region Private Methods
    private void RegisterListeners()
    {
        // Buttons
        returnToHangarButton.onClick.AddListener(delegate { GameManager.instance.LoadLevel(GameStates.Hangar); ShowPauseMenu(); });
        quitGameButton.onClick.AddListener(delegate { Utility.QuitGame(); });
        resumeGameButton.onClick.AddListener(delegate { ShowPauseMenu(); });

        // Custom events
        errorModalCantAffordDelegate = delegate { ErrorModal(ErrorType.CantAfford); };
        EventManager.StartListening("CantAffordItem", errorModalCantAffordDelegate);

        errorModalPlayerDiedDelegate = delegate { ErrorModal(ErrorType.PlayerDeath); };
        EventManager.StartListening("PlayerDeath", errorModalPlayerDiedDelegate);

        errorModalReturnedFromDeathDelegate = delegate { ErrorModal(ErrorType.ReturnFromDeath); };
        EventManager.StartListening("ReturnedFromDeath", errorModalReturnedFromDeathDelegate);

        errorModalWrongInputDelegate = delegate { ErrorModal(ErrorType.WrongInput); };
        EventManager.StartListening("IncorrectInput", errorModalWrongInputDelegate);

        errorModalNoResultsDelegate = delegate { ErrorModal(ErrorType.NoResults); };
        EventManager.StartListening("NoResults", errorModalNoResultsDelegate);
		
        pauseMenuDelegate = delegate { ShowPauseMenu(); };
        EventManager.StartListening("PauseMenu", pauseMenuDelegate);

        errorModalInvalidProfileNameDelegate = delegate { ErrorModal(ErrorType.InvalidProfileName); };
        EventManager.StartListening("InvalidProfileName", errorModalInvalidProfileNameDelegate);

        errorModalInactiveOnThisPlatformDelegate = delegate { ErrorModal(ErrorType.InactiveOnThisPlatform); };
        EventManager.StartListening("InactiveOnThisPlatform", errorModalInactiveOnThisPlatformDelegate);
    }

    private float LoadScreenAlphaValue()
    {
        Image image = loadScreen.GetComponent<Image>();
        return image.color.a;
    }

    private IEnumerator DeactivateLoadScreen()
    {
        while(LoadScreenAlpha > 0f)
        {
            yield return new WaitForEndOfFrame();
        }

        loadScreen.SetActive(false);
    }

    private void ErrorModal(ErrorType type)
    {
        switch (type)
        {
            case ErrorType.Unspecified:
                EventManager.TriggerEvent("UIError");
                errorModalDescription.text = "An unknown error occurred.";
                RemoveModalListeners();
                ApplyStandardModalListeners();
                break;
            case ErrorType.CantAfford:
                EventManager.TriggerEvent("UIError");
                errorModalTitle.text = "Oops.";
                errorModalDescription.text = "You can't afford this item! Try again after collecting some resources.";
                RemoveModalListeners();
                ApplyStandardModalListeners(); 
                break;
            case ErrorType.PlayerDeath:
                EventManager.TriggerEvent("UINotification");
                errorModalTitle.text = "Oops.";
                errorModalDescription.text = "Oh no! Your ship exploded. Don't worry. The Company will give you a new one on your return to base... for a small fee.";
                // Button listeners are handled by the DeathState state 
                break;
            case ErrorType.ReturnFromDeath:
                EventManager.TriggerEvent("UINotification");
                errorModalTitle.text = "New Ship";
                errorModalDescription.text = $"We've issued you a new ship and deducted ${GameManager.instance.CalcDeathCost().ToString("#,#")} from your balance to cover the insurance costs. Try not to let it happen again, otherwise the costs keep going up!";
                ApplyStandardModalListeners();
                break;
            case ErrorType.WrongInput:
                EventManager.TriggerEvent("UIError");
                errorModalTitle.text = "Oops.";
                errorModalDescription.text = "Something went wrong with the submitted input. Try again.";
                ApplyStandardModalListeners();
                break;
            case ErrorType.NoResults:
                EventManager.TriggerEvent("UINotification");
                errorModalTitle.text = "No results.";
                errorModalDescription.text = "Try a different search query and ensure you're using correct casing.";
                ApplyStandardModalListeners();
                break;
            case ErrorType.InvalidProfileName:
                EventManager.TriggerEvent("UIError");
                errorModalTitle.text = "Oops.";
                errorModalDescription.text = "Either this name was invalid or a profile already exists. Cancel or try a different name.";
                ApplyStandardModalListeners();
                break;
            case ErrorType.InactiveOnThisPlatform:
                EventManager.TriggerEvent("UINotification");
                errorModalTitle.text = "Sorry.";
                errorModalDescription.text = "This feature isn't available yet on this platform.";
                ApplyStandardModalListeners();
                break;
        }

        errorModal.SetActive(true);
    }

    private void RemoveModalListeners()
    {
        errorModalOkButton.onClick.RemoveAllListeners();
        errorModalCloseButton.onClick.RemoveAllListeners();
    }

    private void ApplyStandardModalListeners()
    {
        errorModalOkButton.onClick.AddListener(delegate { errorModal.SetActive(false); });
        errorModalCloseButton.onClick.AddListener(delegate { errorModal.SetActive(false); });
    }
    #endregion

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        if(pauseMenu.activeInHierarchy)
        {
            EventManager.TriggerEvent("UIPause");
            Time.timeScale = 0f;
        }
        else
        {
            EventManager.TriggerEvent("UIResume");
            Time.timeScale = 1f;
        }
    }
}

public enum ErrorType { Unspecified, CantAfford, PlayerDeath, ReturnFromDeath, WrongInput, NoResults, InvalidProfileName, InactiveOnThisPlatform }