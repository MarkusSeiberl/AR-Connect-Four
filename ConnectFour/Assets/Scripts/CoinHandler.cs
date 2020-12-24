using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Vuforia;

public class CoinHandler : MonoBehaviour, ITrackableEventHandler {

    public GameObject coin1;
    public GameObject coin2;
    public ObjectPooler pooler;

    private static readonly Quaternion COIN_ROTATION = Quaternion.Euler(0, 90, 0);
    private static readonly Vector3 COIN_SCALE = new Vector3(1.3f, 1.3f, 1.3f);
    private static readonly Vector3 COIN_POSITION_1 = new Vector3(-0.5341f, 0.9938f, -0.4682f); 
    private static readonly Vector3 COIN_POSITION_2 = new Vector3(-0.5341f, 0.9938f, -0.3121f);
    private static readonly Vector3 COIN_POSITION_3 = new Vector3(-0.5341f, 0.9938f, -0.1560f);
    private static readonly Vector3 COIN_POSITION_4 = new Vector3(-0.5341f, 0.9938f, 0f);
    //private static readonly Vector3 COIN_POSITION_5 = new Vector3(-0.5341f, 0.9938f, 0.1560f);
    private static readonly Vector3 COIN_POSITION_5 = new Vector3(-0.5341f, 1.9938f, 0.1960f);
    private static readonly Vector3 COIN_POSITION_6 = new Vector3(-0.5341f, 0.9938f, 0.3121f);
    private static readonly Vector3 COIN_POSITION_7 = new Vector3(-0.5341f, 0.9938f, 0.4682f);

    private readonly List<GameObject> placedCoins = new List<GameObject>();
    private readonly int[,] gameField = new int[6,7];

    private int currPlayerNr = -1; //player 1 = 1; player 2 = 2
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
            Debug.Log("IF +++++++++++++++++++++++++");

        } else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                   newStatus == TrackableBehaviour.Status.NO_POSE) {

            EnableKinematic();
            Debug.Log("ELSE IF ##############");
        } else {

            EnableKinematic();
            Debug.Log("ELSE -----------------");
        }
    }



    public void SetCoinPosition(int playerNr, int column) {
        GameObject correctCoin = FetchCorrectCoin(playerNr);
        Vector3 correctPostion = FetchCorrectPosition(column);

        if (null == currentCoin) {
            InstantiateCoin(correctCoin, correctPostion);
            currPlayerNr = playerNr;
            currColumn = column;
            return;
        }

        if (currPlayerNr == playerNr) {
            UpdateCoinPostion(correctPostion);
        } else {
            InstantiateCoin(correctCoin, correctPostion);
        }

        currPlayerNr = playerNr;
        currColumn = column;    
    }

    public void PlaceCoin() {
        //currentCoin.AddComponent<Rigidbody>();
        //var mesh = currentCoin.AddComponent<MeshCollider>();
        //mesh.convex = true;
        GameObject duplicate = currentCoin;
        placedCoins.Add(duplicate);
        currentCoin = null;
        int rowIndex = RowNumberOfNextFreeSlot(currColumn);
        gameField[rowIndex, currColumn] = currPlayerNr;

    }

    public void RemoveCoin() {
        Destroy(currentCoin);
        currentCoin = null;
        currPlayerNr = -1;
        currColumn = -1;
    }

    private int RowNumberOfNextFreeSlot(int column) {
        int nrOfCoins = 0;

        for (int row = 0;  row < 6; row++) {
            if (gameField[row, column] != 0) {
                nrOfCoins += 1;
                
            }
        }

        return 5- nrOfCoins;
    }

    private void UpdateCoinPostion(Vector3 correctPostion) {
        currentCoin.transform.localPosition = correctPostion;
    }

    private GameObject FetchCorrectCoin(int playerNr) {
        if (playerNr == 1) {
            return coin1;
        }
        return coin2;
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

    private void InstantiateCoin(GameObject coin, Vector3 coinPosition) {
        pooler.SpawnFromPool("coin1", coinPosition, COIN_ROTATION);

        //currentCoin = Instantiate(coin, transform, false);
        currentCoin.transform.parent = this.transform;
        //currentCoin.transform.localRotation = COIN_ROTATION;
        //currentCoin.transform.localScale = COIN_SCALE;
        currentCoin.transform.localPosition = coinPosition;
    }
}
