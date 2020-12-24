using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ButtonClick : MonoBehaviour, IStateChange {

    
    public Material selectedMaterial;
    public Material unselectedMaterial;
    public Material errorMaterial;
    public Material lockedMaterial;
    public AudioClip bellSound;
    public GameObject coin1;
    public GameObject coin2;

    public readonly List<GameObject> coins = new List<GameObject>();

    private readonly List<string> numbers = new List<string> { "n1_obj", "n2_obj", "n3_obj", "n4_obj", "n5_obj", "n6_obj", "n7_obj" };
    private readonly List<string> virtualButtons = new List<string> { "VB1", "VB2", "VB3", "VB4", "VB5", "VB6", "VB7" };
    private readonly List<VirtualClick> clickHandler = new List<VirtualClick>();

    private CoinHandler coinHandler;
    private AudioSource audioSource;
    private VirtualClick currentSelectedButton = null;
    private bool vibrateEnabled = false;


    // Start is called before the first frame update
    void Start() {
        InitButtonClickHandlers();
        audioSource = GetComponent<AudioSource>();
        GameObject imageTarget = GameObject.Find("ImageTarget");
        coinHandler = imageTarget.GetComponent<CoinHandler>();
    }

    // Update is called once per frame
    void Update() {
        if (currentSelectedButton == null) {
            return;
        }

        var elapsedTime = Time.deltaTime;
        currentSelectedButton.elapsedTime -= elapsedTime;

        if (currentSelectedButton.elapsedTime <= 0 && !vibrateEnabled) {
            UpdateMaterial(currentSelectedButton.buttonModel, lockedMaterial);
            vibrateEnabled = true;
            audioSource.PlayOneShot(bellSound);

            coinHandler.PlaceCoin();
        }
    }

    void IStateChange.OnStateChanged() {
        List<VirtualClick> pressedHandlers = GetPressedVirtualButtonHandler();

        if (pressedHandlers.Count == 0) {
            currentSelectedButton.elapsedTime = VirtualClick.TIME_DELAY;
            currentSelectedButton = null;
            vibrateEnabled = false;
            coinHandler.RemoveCoin();

        }
        else if (pressedHandlers.Count == 1) {
            currentSelectedButton = pressedHandlers[0];
            UpdateMaterial(currentSelectedButton.buttonModel, selectedMaterial);
            coinHandler.SetCoinPosition(1, clickHandler.IndexOf(currentSelectedButton));

        }
        else if (pressedHandlers.Count > 1) {
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
