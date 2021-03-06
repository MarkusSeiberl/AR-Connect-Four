﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    public GameObject player1Object;
    public GameObject player2Object;
    public GameObject player1Wins;
    public GameObject player2Wins;
    public GameObject leftParticleSystem;
    public GameObject rightParticleSystem;
    public AudioClip gameWinAudioClip;

    private AudioSource audioSource;
    private int[,] gameField = new int[6, 7];
    private int playerNr;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        playerNr = 1;
    }

    public bool PlaceCoin(int column) {
       
        int rowIndex = RowNumberOfNextFreeSlot(column);
        gameField[rowIndex, column] = playerNr;
        
        var hasConnectedFour = ConnectedFour(rowIndex, column, playerNr); //TODO maybe let coinhandler check it to act upon it
        if (hasConnectedFour) {
            ActivateVisualFeedback();
            ActivateAuditiveFeedback();
            return hasConnectedFour;
        } else {
            ChangePlayer();
            return hasConnectedFour;
        }
    }
     
    public bool ConnectedFour(int row, int column, int playerNr) {
        if (CheckHorizonal(row, column, playerNr) ||
            CheckVertical(row, column, playerNr) ||
            CheckDiagonal(row, column, playerNr)) {

            return true;
        }
        return false;
    }
     
    public int GetCurrentPlayerNumber() {
        return playerNr;
    }

    public void ResetGame() {
        playerNr = 1;
        gameField = new int[6, 7];
        player1Object.SetActive(true);
        player2Object.SetActive(false);
        player1Wins.SetActive(false);
        player2Wins.SetActive(false);
        leftParticleSystem.SetActive(false);
        rightParticleSystem.SetActive(false);
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


    private bool CheckHorizonal(int row, int column, int playerNr) {
        var left = CountLeftSide(row, column, playerNr);
        var right = CountRightSide(row, column, playerNr);
        if ((left + 1 + right) >= 4) {
            return true;
        }
        return false;
    }

    private bool CheckVertical(int row, int column, int playerNr) {
        if (row > 2) { // not high enough to get a 4 in a row
            return false;
        }

        var count = 0;
        for (int r = row; r < 6; r++) {
            if (gameField[r, column] == playerNr) {
                count += 1;
            } else if (count < 4) {
                return false;  // Did not reach 4 in a row of the same coin
            } else {
                return true; // Did reach 4 in a row of the same coin with the 5th = other coin
            }
        }
        return true;
    }

    private bool CheckDiagonal(int row, int column, int playerNr) {
        var leftUpperDiagonal = CountLeftUpperDiagonal(row, column, playerNr);
        var rightUpperDiagonal = CountRightUpperDiagonal(row, column, playerNr);
        var leftLowerDiagonal = CountLeftLowerDiagonal(row, column, playerNr);
        var rightLowerDiagonal = CountRightLowerDiagonal(row, column, playerNr);
        var diagonal1 = 1 + leftLowerDiagonal + rightUpperDiagonal;
        var diagonal2 = 1 + rightLowerDiagonal + leftUpperDiagonal;
        if (diagonal1 >= 4 || diagonal2 >= 4) { 
            return true;
        }
        return false;
    }


    private int CountLeftSide(int row, int column, int playerNr) {
        var count = -1; // -1 because the placed coin gets counted too
        for (int col = column; col >= 0; col--) {
            if (gameField[row, col] != playerNr) {
                return count;
            }
            count += 1;
        }
        return count;
    }

    private int CountRightSide(int row, int column, int playerNr) {
        var count = -1; // -1 because the placed coin gets counted too
        for (int col = column; col < 7; col++) {
            if (gameField[row, col] != playerNr) {
                return count;
            }
            count += 1;
        }
        return count;
    }

    private int CountLeftUpperDiagonal(int row, int column, int playerNr) {
        var count = -1; // -1 because the placed coin gets counted too
        var r = row;
        for (int col = column; col >= 0 && r >= 0; col--) {
            if (gameField[r, col] != playerNr) {
                Debug.Log("Count: " + count);
                return count;
            }
            r -= 1;
            count += 1;
        }

        Debug.Log("Count: " + count);
        return count;
    }
    private int CountRightUpperDiagonal(int row, int column, int playerNr) {
        var count = -1; // -1 because the placed coin gets counted too
        var r = row;
        for (int col = column; col < 7 && r >= 0; col++) {
            if (gameField[r, col] != playerNr) {
                Debug.Log("Count: " + count);
                return count;
            }
            r -= 1;
            count += 1;
        }

        Debug.Log("Count: " + count);
        return count;
    }
    private int CountLeftLowerDiagonal(int row, int column, int playerNr) {
        var count = -1; // -1 because the placed coin gets counted too
        var r = row;
        for (int col = column; col >= 0 && r < 6; col--) {
            if (gameField[r, col] != playerNr) {
                Debug.Log("Count: " + count);
                return count;
            }
            r += 1;
            count += 1;
        }

        Debug.Log("Count: " + count);
        return count;
    }
    private int CountRightLowerDiagonal(int row, int column, int playerNr) {
        var count = -1; // -1 because the placed coin gets counted too
        var r = row;
        for (int col = column; col < 7 && r < 6; col++) {
            if (gameField[r, col] != playerNr) {
                Debug.Log("Count: " + count);
                return count;
            }
            r += 1;
            count += 1;
        }

        Debug.Log("Count: " + count);
        return count;
    }

    private void ChangePlayer() {
        if (playerNr == 1) {
            playerNr = 2;
            player1Object.SetActive(false);
            player2Object.SetActive(true);
        } else {
            playerNr = 1; 
            player1Object.SetActive(true);
            player2Object.SetActive(false);
        }
    }

    private void ActivateVisualFeedback() {
        // Activate Particle System
        leftParticleSystem.SetActive(true);
        rightParticleSystem.SetActive(true);
        
        // Show which user won
        if(playerNr == 1) {
            player1Wins.SetActive(true);
        } else {
            player2Wins.SetActive(true);
        }

        //Display Reset Button
    }

    private void ActivateAuditiveFeedback() {
        audioSource.PlayOneShot(gameWinAudioClip);
    }
}
