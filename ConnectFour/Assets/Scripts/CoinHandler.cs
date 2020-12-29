using System.Collections.Generic;
using UnityEngine;
using Utility;
using Vuforia;

public class CoinHandler : MonoBehaviour, ITrackableEventHandler {

    public ObjectPooler pooler;
    public GameLogic gameLogic;

    private static readonly Vector3 COIN_SCALE = new Vector3(1.3f, 1.3f, 1.3f);
    private static readonly Vector3 COIN_POSITION_1 = new Vector3(-0.5341f, 0.9938f, -0.4682f); 
    private static readonly Vector3 COIN_POSITION_2 = new Vector3(-0.5341f, 0.9938f, -0.3121f);
    private static readonly Vector3 COIN_POSITION_3 = new Vector3(-0.5341f, 0.9938f, -0.1560f);
    private static readonly Vector3 COIN_POSITION_4 = new Vector3(-0.5341f, 0.9938f, 0f);
    private static readonly Vector3 COIN_POSITION_5 = new Vector3(-0.5341f, 0.9938f, 0.1560f);
    private static readonly Vector3 COIN_POSITION_6 = new Vector3(-0.5341f, 0.9938f, 0.3121f);
    private static readonly Vector3 COIN_POSITION_7 = new Vector3(-0.5341f, 0.9938f, 0.4682f);

    private readonly List<GameObject> placedCoins = new List<GameObject>();
    
    private int currColumn = -1; //column starts at 0
    private GameObject currentCoin = null;
    private TrackableBehaviour marker;

    // Start is called before the first frame update
    void Start() {
        marker = GetComponent<TrackableBehaviour>();
        if (marker) {
            marker.RegisterTrackableEventHandler(this);
        }
    }

    void OnDestroy() {
        if (marker) {
            marker.UnregisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus) {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
            DisableKinematic();

        } else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                   newStatus == TrackableBehaviour.Status.NO_POSE) {

            EnableKinematic();
        } else {

            EnableKinematic();
        }
    }



    public void SetCoinPosition(int column) {
        Vector3 correctPostion = FetchCorrectPosition(column);
        int correctPlayerNr = gameLogic.GetCurrentPlayerNumber();

        //if null:
        //   player placed coin --> new players turn --> create new
        //if not null:
        //   still same player --> udpate position + enable
        if (null == currentCoin) {
            currentCoin = FetchCorrectCoin(correctPlayerNr, correctPostion);

        } else { 
            UpdateCoinPostion(correctPostion);
        } 
        currColumn = column;   
    }

    public bool PlaceCoin() {
        currentCoin.GetComponent<Rigidbody>().isKinematic = false;
        placedCoins.Add(currentCoin);
        currentCoin = null;
        return gameLogic.PlaceCoin(currColumn);

    }

    public void RemoveCoin() {
        if (currentCoin != null) {
            currentCoin.SetActive(false);
        }
    }

    public void ResetGame() {
        while (placedCoins.Count > 0) {
            GameObject coin = placedCoins[0];
            coin.GetComponent<Rigidbody>().isKinematic = true;
            coin.SetActive(false);
            placedCoins.RemoveAt(0);
        }

        currentCoin = null;
        currColumn = -1;

        gameLogic.ResetGame();
    }


    private void UpdateCoinPostion(Vector3 correctPostion) {
        currentCoin.transform.localPosition = correctPostion;
        currentCoin.SetActive(true);
    }

    private GameObject FetchCorrectCoin(int playerNr, Vector3 position) {
        var objectTag = "coin" + playerNr;
        GameObject coin = pooler.SpawnFromPool(objectTag, position, Quaternion.identity);
        coin.transform.parent = transform;
        coin.transform.localPosition = position;
        coin.transform.localScale = COIN_SCALE;

        return coin;
    }

    private Vector3 FetchCorrectPosition(int column) { 
        switch(column) {
            case 0: return COIN_POSITION_1;
            case 1: return COIN_POSITION_2;
            case 2: return COIN_POSITION_3;
            case 3: return COIN_POSITION_4;
            case 4: return COIN_POSITION_5;
            case 5: return COIN_POSITION_6;
            case 6: return COIN_POSITION_7;
            default: return COIN_POSITION_1;

        }
    }

    private void EnableKinematic() {
        placedCoins.ForEach(coin => {
            coin.GetComponent<Rigidbody>().isKinematic = true;
        });
    }

    private void DisableKinematic() {
        placedCoins.ForEach(coin => {
            coin.GetComponent<Rigidbody>().isKinematic = false;
        });
    }
}
