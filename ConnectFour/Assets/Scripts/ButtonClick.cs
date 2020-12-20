using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ButtonClick : MonoBehaviour, StateChange {

    ArrayList clickHandler = new ArrayList();
    GameObject VB1;
    Color color;

    // Start is called before the first frame update
    void Start() {

        VirtulClick clickVB1 = new VirtulClick(this, GameObject.Find("BTN1"));
        GameObject.Find("VB1").GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(clickVB1);
        clickHandler.Add(clickVB1);

        VirtulClick clickVB2 = new VirtulClick(this, GameObject.Find("BTN2"));
        GameObject.Find("VB2").GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(clickVB2);
        clickHandler.Add(clickVB2);

        VirtulClick clickVB3 = new VirtulClick(this, GameObject.Find("BTN3"));
        GameObject.Find("VB3").GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(clickVB3);
        clickHandler.Add(clickVB3);
    }

    void StateChange.onStateChanged() {
        Debug.Log("++++++++++++++++++++++++++++++");
        //TODO one button got pressed or released
    }

    // Update is called once per frame
    void Update() {
        
    }


    class VirtulClick : IVirtualButtonEventHandler {

        public bool isPressed = false;

        private readonly StateChange callback;
        private readonly GameObject buttonModel;

        public VirtulClick(StateChange stateChange, GameObject buttonModel) {
            this.callback = stateChange;
            this.buttonModel = buttonModel;
        }

        public void highlightButton() {
            Debug.Log("pressed");
            var renderer = buttonModel.GetComponent<Renderer>();
            Debug.Log(renderer.material.color.ToString());
            renderer.material.SetColor("_Color", Color.red);
        }

        public void resetButton() {
            Debug.Log("pressed");
            var renderer = buttonModel.GetComponent<Renderer>();
            Debug.Log(renderer.material.color.ToString());
            renderer.material.SetColor("_Color", Color.white);
        }

        public void OnButtonPressed(VirtualButtonBehaviour vb) {
            isPressed = true;
            highlightButton();
            callback.onStateChanged();
        }

        public void OnButtonReleased(VirtualButtonBehaviour vb) {
            isPressed = false;
            resetButton();
            callback.onStateChanged();
        }
    }
}

interface StateChange {
    void onStateChanged();
}
