using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;

public class ButtonClick : MonoBehaviour, IStateChange {

    
    public Material selectedMaterial;
    public Material unselectedMaterial;
    public Material errorMaterial;
    public Material lockedMaterial;
    public AudioClip bellSound;

    private readonly List<string> numbers = new List<string> { "n1_obj", "n2_obj", "n3_obj", "n4_obj", "n5_obj", "n6_obj", "n7_obj" };
    private readonly List<string> virtualButtons = new List<string> { "VB1", "VB2", "VB3", "VB4", "VB5", "VB6", "VB7" };
    private readonly List<VirtualClick> clickHandler = new List<VirtualClick>();

    private AudioSource audioSource;
    private VirtualClick currentSelectedButton = null;
    private bool vibrateEnabled = false;


    // Start is called before the first frame update
    void Start() {
        InitButtonClickHandlers();
        audioSource = GetComponent<AudioSource>();
    }

    void IStateChange.OnStateChanged() {
        List<VirtualClick> pressedHandlers = GetPressedVirtualButtonHandler();

        if (pressedHandlers.Count == 0) {
            currentSelectedButton.elapsedTime = VirtualClick.TIME_DELAY;
            currentSelectedButton = null;
            vibrateEnabled = false;

        }
        else if (pressedHandlers.Count == 1) {
            currentSelectedButton = pressedHandlers[0];
            UpdateMaterial(currentSelectedButton.buttonModel, selectedMaterial);

        }
        else if (pressedHandlers.Count > 1) {
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

    // Update is called once per frame
    void Update() {
        if(currentSelectedButton == null) {
            return;
        }

        var elapsedTime = Time.deltaTime;
        currentSelectedButton.elapsedTime -= elapsedTime;

        if(currentSelectedButton.elapsedTime <= 0 && !vibrateEnabled) {
            UpdateMaterial(currentSelectedButton.buttonModel, lockedMaterial);
            vibrateEnabled = true;
            audioSource.PlayOneShot(bellSound);
        }
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


    class VirtualClick : IVirtualButtonEventHandler {

        public static float TIME_DELAY = 1;

        public readonly GameObject buttonModel;

        public bool isPressed = false;
        public float elapsedTime = TIME_DELAY;

        private readonly IStateChange callback;
        

        public VirtualClick(IStateChange callback, GameObject buttonModel) {
            this.callback = callback;
            this.buttonModel = buttonModel;
        }

        public void OnButtonPressed(VirtualButtonBehaviour vb) {
            isPressed = true;
            callback.OnStateChanged();
        }

        public void OnButtonReleased(VirtualButtonBehaviour vb) {
            isPressed = false;
            callback.OnStateChanged();
        }
    }
}

interface IStateChange {
    void OnStateChanged();
}
