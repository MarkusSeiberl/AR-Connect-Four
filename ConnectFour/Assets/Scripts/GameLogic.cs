using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    private readonly int[,] gameField = new int[6, 7];

    private int playerNr;

    // Start is called before the first frame update
    void Start() {
        playerNr = 1;
    }

    public void PlaceCoin(int column) {
       
        int rowIndex = RowNumberOfNextFreeSlot(column);
        gameField[rowIndex, column] = playerNr;

        //TODO check if four are connected (or let the coinhanler do it)
        ChangePlayer();
    }
     
    public bool ConnectedFour() {
        //TODO implement algo
        return false;
    }
     
    public int GetCurrentPlayerNumber() {
        return playerNr;
    }

    private int RowNumberOfNextFreeSlot(int column) {
        int nrOfCoins = 0;

        for (int row = 0; row < 6; row++) {
            if (gameField[row, column] != 0) {
                nrOfCoins += 1;

            }
        }

        return 5 - nrOfCoins;
    }

    private void ChangePlayer() {
        if (playerNr == 1) {
            playerNr = 2;
        } else {
            playerNr = 1;
        }
    }
}
