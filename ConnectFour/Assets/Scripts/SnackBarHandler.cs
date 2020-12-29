using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackBarHandler : MonoBehaviour {

    public GameObject restartButton;
    public ButtonClick buttonClick;

    private Translation translation;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public void OnClick() {
        // Reset everything from here
        buttonClick.ResetGame();

        // User restarts game
        // Hide Snackbars
        StartTranslation(Translation.DOWN);
    }

    public void StartTranslation(Translation direction) {
        translation = direction;

        targetPosition = transform.position;
        
        switch (translation) {
            case Translation.UP:
                targetPosition.y = 0;
                StartCoroutine(TranslateCoroutine());
                break;
            case Translation.DOWN:
                targetPosition.y = -GetComponent<RectTransform>().sizeDelta.y;
                StartCoroutine(TranslateCoroutine());
                break;
            default:
                // DO NOTHING
                break;
        }
    }

    private IEnumerator TranslateCoroutine() {
        while (Vector3.Distance(transform.position, targetPosition) > 1f) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f);
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(1f);
    }
}

public enum Translation {
    NONE,
    UP,
    DOWN
}
