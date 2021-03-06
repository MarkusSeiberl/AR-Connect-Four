﻿using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ButtonClick : MonoBehaviour, IStateChange {
    
    public Material selectedMaterial;
    public Material unselectedMaterial;
    public Material errorMaterial;
    public Material lockedMaterial;
    public AudioClip bellSound;
    public GameLogic gameLogic;
    public SnackBarHandler snackBarHandler;
    public CoinHandler coinHandler;

    private readonly List<string> numbers = new List<string> { "n1_obj", "n2_obj", "n3_obj", "n4_obj", "n5_obj", "n6_obj", "n7_obj" };
    private readonly List<string> virtualButtons = new List<string> { "VB1", "VB2", "VB3", "VB4", "VB5", "VB6", "VB7" };
    private readonly List<VirtualClick> clickHandler = new List<VirtualClick>();

    
    //private CoinHandler coinHandler;
    private AudioSource audioSource;
    private VirtualClick currentSelectedButton = null;
    private bool vibrateEnabled = false;
    private bool isGameFinished = false;
    private float coolDown = 0;


    // Start is called before the first frame update
    void Start() {
        InitButtonClickHandlers();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

        var elapsedTime = Time.deltaTime;
        if (coolDown > 0) {
            coolDown -= elapsedTime;
        }

        // if no button is selected return - no need to countdown
        if (currentSelectedButton != null) {

            // One Button is selected -> Countdown till the coin is placed
            if (coolDown <= 0) {
                currentSelectedButton.elapsedTime -= elapsedTime;
            }

            // Button is selected and countdown is at 0 --> place the coin
            if (currentSelectedButton.elapsedTime <= 0 && !vibrateEnabled) {
                PlaceCoin();
            }
        }
    }

    void IStateChange.OnStateChanged() {
        List<VirtualClick> pressedHandlers = GetPressedVirtualButtonHandler();

        // Nothing is selected --> reset currentselectedButton
        if (pressedHandlers.Count == 0) {
            if (currentSelectedButton != null) {
                currentSelectedButton.elapsedTime = VirtualClick.TIME_DELAY;
                currentSelectedButton = null;
            }
            vibrateEnabled = false;
            coinHandler.RemoveCoin();

        }
        // If one is selected and the cooldown is over
        else if (pressedHandlers.Count == 1 && coolDown <= 0 && !isGameFinished) {
            currentSelectedButton = pressedHandlers[0];
            UpdateMaterial(currentSelectedButton.buttonModel, selectedMaterial);
            coinHandler.SetCoinPosition(clickHandler.IndexOf(currentSelectedButton));

        }
        // More than one is selected --> indicate error
        else if (pressedHandlers.Count > 1 && !isGameFinished) {
            coinHandler.RemoveCoin();
            if (currentSelectedButton != null) {
                currentSelectedButton.elapsedTime = VirtualClick.TIME_DELAY;
                currentSelectedButton = null;
                vibrateEnabled = false;
            }

            foreach (var vc in pressedHandlers) {
                UpdateMaterial(vc.buttonModel, errorMaterial);
            }
        }
    }

    public void ResetGame() {
        clickHandler.Clear();
        InitButtonClickHandlers();

        currentSelectedButton = null;
        vibrateEnabled = false;
        isGameFinished = false;
        coolDown = 0;

        coinHandler.ResetGame();
    }

    private void PlaceCoin() {
        UpdateMaterial(currentSelectedButton.buttonModel, lockedMaterial);
        vibrateEnabled = true;
        audioSource.PlayOneShot(bellSound);

        isGameFinished = coinHandler.PlaceCoin(); // Set flag for clickhandler to not interact
        coolDown = 1; // Set time to 1 sec - to not be able to select other buttons

        if (isGameFinished) {
            // Show Snackbar
            snackBarHandler.StartTranslation(Translation.UP);
        }


        //TODO check if if column is full
        //  if so than deactivate VB of this column
        //  MB no need for it - as an IndexOutOfArrayException will occur and the coin will not be placed
        //var currentColumn = clickHandler.IndexOf(currentSelectedButton);
        //if (gameLogic.isColumnFull(currentColumn)) {
        //    clickHandler[currentColumn] = null;
        //}

    }

    private List<VirtualClick> GetPressedVirtualButtonHandler() {
        List<VirtualClick> pressedHandlers = new List<VirtualClick>();
        for (var i = 0; i < numbers.Count; i++) {
            if (clickHandler[i].isPressed) {
                pressedHandlers.Add(clickHandler[i]);

            }
            else {
                UpdateMaterial(clickHandler[i].buttonModel, unselectedMaterial);
            }
        }
        return pressedHandlers;
    }

    private void InitButtonClickHandlers() {
        for (var i = 0; i < numbers.Count; i++) {
            GameObject numberObj = GameObject.Find(numbers[i]);
            VirtualClick virtualButton = new VirtualClick(this, numberObj);
            GameObject.Find(virtualButtons[i]).GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(virtualButton);
            clickHandler.Add(virtualButton);
        }
    }

    private void UpdateMaterial(GameObject gameObject, Material material) {
        Renderer rend = gameObject.GetComponent<Renderer>();
        rend.sharedMaterial = material;
    }
}
