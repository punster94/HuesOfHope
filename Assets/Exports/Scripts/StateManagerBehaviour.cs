using UnityEngine;
using System.Collections;
using System;

public enum GameState
{
    DISCLAIMER,
    BIO,
    START,
    GAME,
    END,
    CREDITS
};

public class StateManagerBehaviour : MonoBehaviour
{
    public Camera startCamera;
    public Camera gameCamera;
    public Camera endCamera;

    public MeshRenderer startCanvas;
    public MeshRenderer endCanvas;

    public Material disclaimer;
    public Material bio;
    public Material startScreen;
    public Material winScreen;
    public Material lossScreen;
    public Material creditsScreen;

    public PlayerController player;
    public CellPhoneBehavior cellPhone;

    public float startingDepressionPercentage;
    public float drainPerTick;

    public float timeBetweenCalls;

    public UIBarManager uibarManager;
    public AudioSource negativeVibe;
    public AudioSource maybeIShouldTrySomethingElse;
    public string maybeIShouldTrySomethingElseText;
    public DialogueManager dialogueManager;
    public float vibeWarningDropInterval;

    private float depressionIntensity;
    private float lastMaxIntensity;
    private bool gameOver;
    private bool endingOver;
    private bool playerCanInput;
    private bool receivingCalls;
    private GameState currentGameState;
    private float startTime;
    private bool won;

    // Use this for initialization
    void Start ()
    {
        toggleTextOnCamera(startCamera, true);
        toggleTextOnCamera(endCamera, false);

        gameCamera.enabled = false;
        endCamera.enabled = false;
        startCamera.enabled = true;

        currentGameState = GameState.START;
        startCanvas.material = startScreen;

        playerCanInput = true;
        gameOver = false;
        endingOver = false;
        receivingCalls = true;

        depressionIntensity = startingDepressionPercentage;
        lastMaxIntensity = depressionIntensity;

        startTime = Time.time;

        won = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        switch(currentGameState)
        {
            case GameState.BIO :
                bioUpdate();
                break;
            case GameState.DISCLAIMER :
                disclaimerUpdate();
                break;
            case GameState.START :
                startUpdate();
                break;
            case GameState.GAME:
                gameUpdate();
                break;
            case GameState.END:
                endUpdate();
                break;
            case GameState.CREDITS:
                creditsUpdate();
                break;   
        }
	}

    private void switchCameras(Camera off, Camera on)
    {
        toggleTextOnCamera(off, false);
        off.enabled = false;

        toggleTextOnCamera(on, true);
        on.enabled = true;
    }

    private void toggleTextOnCamera(Camera camera, bool enableText)
    {
        foreach (TextMesh text in camera.transform.parent.GetComponentsInChildren<TextMesh>())
            text.transform.parent.gameObject.SetActive(enableText);
    }

    private void bioUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            startGame();
        }
    }

    private void disclaimerUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            startCanvas.material = bio;
            currentGameState = GameState.BIO;
        }
    }

    private void startUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            startCanvas.material = disclaimer;
            currentGameState = GameState.DISCLAIMER;
        }
    }

    private void endUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (won)
            {
                endCanvas.material = creditsScreen;
                currentGameState = GameState.CREDITS;
            }
            else
            {
                resetGame();
                startGame();
            }
        }
    }

    private void creditsUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            resetGame();
        }
    }

    private void gameUpdate()
    {
        float currentTime = Time.time;

        if (currentTime - startTime >= timeBetweenCalls && receivingCalls)
        {
            cellPhone.moveCellPhoneUp();
            startTime = Time.time;
            playerCanInput = false;
        }

        if (gameOver && endingOver)
        {
            cellPhone.moveCellPhoneDown();
            switchCameras(gameCamera, endCamera);
            currentGameState = GameState.END;
            return;
        }

        if (depressionIntensity >= 100f && playerCanInput)
        {
            cellPhone.moveCellPhoneUp(true);
            receivingCalls = false;
            playerCanInput = false;
        }
        else if (depressionIntensity <= 0f && playerCanInput)
        {
            cellPhone.moveCellPhoneUp(false, true);
            receivingCalls = false;
            playerCanInput = false;
        }
        else if (playerCanInput)
        {
            depressionIntensity = Math.Max(depressionIntensity - drainPerTick, 0f);

            if (Input.GetKeyDown(KeyCode.Q))
                depressionIntensity = Math.Max(depressionIntensity - 10f, 1f);
            if (Input.GetKeyDown(KeyCode.F))
                depressionIntensity = Math.Min(depressionIntensity + 10f, 99f);
            if (Input.GetKeyDown(KeyCode.R))
                resetGame();
        }

        if (depressionIntensity > lastMaxIntensity)
        {
            lastMaxIntensity = depressionIntensity;
        }

        if (lastMaxIntensity - depressionIntensity >= vibeWarningDropInterval)
        {
            negativeVibe.Play();
            dialogueManager.PlayDialogue(maybeIShouldTrySomethingElse, maybeIShouldTrySomethingElseText);
            lastMaxIntensity = depressionIntensity;
        }
    }

    public float currentDepressionPercentage()
    {
        return depressionIntensity / 100f;
    }

    public void changeDepression(float delta)
    {
        depressionIntensity = Math.Min(depressionIntensity + delta, 100f);
    }

    public bool isGameOver()
    {
        return gameOver;
    }

    public void setTakingInput()
    {
        playerCanInput = true;
    }

    public void notTakingCalls()
    {
        receivingCalls = false;
    }

    public bool takingInput()
    {
        return playerCanInput;
    }

    public bool inGameState()
    {
        return currentGameState == GameState.GAME;
    }

    public void setEndingOver(bool ending)
    {
        endingOver = ending;
    }

    public void setGameOver()
    {
        gameOver = true;
    }
    public void executeEndState(bool won)
    {
        gameOver = true;
        receivingCalls = false;

        uibarManager.Reset();

        if (won)
            executeWinState();
        else
            executeLossState();
    }

    private void executeWinState()
    {
        endCanvas.material = winScreen;
        won = true;
    }
    private void executeLossState()
    {
        endCanvas.material = lossScreen;
        won = false;
    }

    private void resetGame()
    {
        player.reset();
        cellPhone.reset();
        gameCamera.GetComponent<CameraController>().reset();
        
        foreach (InteractionObject item in GameObject.Find("DormRoom").GetComponentsInChildren<InteractionObject>())
        {
            item.reset();
        }

        Start();
    }

    private void startGame()
    {
        currentGameState = GameState.GAME;
        switchCameras(startCamera, gameCamera);
        player.getOutOfBed();
    }
}
