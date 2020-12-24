using UnityEngine;
using Vuforia;

public class VirtualClick : IVirtualButtonEventHandler {

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
