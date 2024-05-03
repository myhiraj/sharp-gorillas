using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameState
{
    START, PLAYER_ONE, PLAYER_TWO, WIN_P1, WIN_P2, END
}

enum GenerationMode { UPWARDS, DOWNWARDS, V_SLOPE, V_SLOPE_INV }

public enum WindDirection { NONE, LEFT, RIGHT}

public class GameLogic : MonoBehaviour
{
    public GameState state;
    public WindDirection wind;
    public int windSpeed;

    public GameObject playerPrefab;

    public GameObject projectilePrefab;
    public GameObject buildingPrefab;

    List<GameObject> buildings_list;
    List<GameObject> projectiles_list;

    public GameObject playerOne;
    public GameObject playerTwo;
    GameObject currentPlayer;

    public TMP_InputField velocityField;
    public TMP_InputField angleField;

    public Button restartButton;
    public Image windArrow;
    public TextMeshProUGUI gameOverText;

    public TextMeshProUGUI scoreText;
    public int playerOneScore;
    public int playerTwoScore;

    public float angleInput = 80;
    public float velocityInput = 1;

    public InputAction launchAction;

    void Start()
    {
        launchAction.Enable();
        launchAction.performed += Launch;

        playerOneScore = 0;
        playerTwoScore = 0;
        displayScore();

        state = GameState.START;
        StartGame();

    }

    private void StartGame()
    {
        //MakeSun();

        MakeWind();

        MakePlayers();

        MakeBuildings();

        Turn();
    }

    private void MakeWind()
    {
        int wind_type = UnityEngine.Random.Range(-1, 1);
        switch (wind_type)
        {
            case -1: wind = WindDirection.LEFT; break;
            case 0: wind = WindDirection.NONE; break;
            case 1: wind = WindDirection.RIGHT; break;
            default: wind = WindDirection.NONE; break;
        }

        windSpeed = UnityEngine.Random.Range(1, 5);

        windArrow.rectTransform.localScale = new Vector3(wind_type * windSpeed, 1, 1);
    }

    private void NextRound()
    {
        ClearScreen();

        CheckWin();

        if (state != GameState.END)
        {
            MakeWind();
            MakePlayers();
            MakeBuildings();

            Turn();
        }
        else if (state == GameState.END)
        {
            gameOverText.gameObject.SetActive(true);
            launchAction.Disable();
            restartButton.gameObject.SetActive(true);
        }
    }

    private void ClearScreen()
    {
        Destroy(playerOne);
        Destroy(playerTwo);
        foreach (GameObject b in buildings_list)
        {
            Destroy(b);
        }
    }

    private void MakeBuildings()
    {
        GenerationMode getMode()
        {
            GenerationMode mode_return;
            int mode_select = UnityEngine.Random.Range(1, 6);
            switch (mode_select)
            {
                case 1: mode_return = GenerationMode.UPWARDS; break;
                case 2: mode_return = GenerationMode.DOWNWARDS; break;
                case 3: mode_return = GenerationMode.V_SLOPE; break;
                case 4: mode_return = GenerationMode.V_SLOPE; break;
                case 5: mode_return = GenerationMode.V_SLOPE; break;
                case 6: mode_return = GenerationMode.V_SLOPE_INV; break;
                default: mode_return = GenerationMode.V_SLOPE; break;
            }
            return mode_return;
        }
        int getHeight(GenerationMode mode) { throw new NotImplementedException(); };

        GenerationMode mode = getMode();

        int min_height = 3;
        int max_height = 7;
        int min_width = 2;
        int max_width = 3;

        int total_width = 0;
        int screen_wdith = 20;

        buildings_list = new(0);

        int prev_width = 0;
        float x = -8.6f;
        float y = -5;

        while (total_width < screen_wdith)
        {
            int width = UnityEngine.Random.Range(min_width, max_width);
            int height = UnityEngine.Random.Range(min_height, max_height);
            //height = getHeight(mode);

            GameObject building = Instantiate(buildingPrefab, new Vector3(x, y, 0), Quaternion.identity);
            building.transform.localScale = new Vector3(width, height, 0);


            int offset = prev_width / 2 + width / 2;
            if (buildings_list.Count == 0) { offset = width; }
            x += offset;

            prev_width = width;
            total_width += width;

            buildings_list.Add(building);
        }


        playerOne.transform.position = buildings_list[1].transform.position;

        Vector3 temp = playerOne.transform.position;
        float y_scale = buildings_list[1].transform.localScale.y;
        temp = new Vector3(temp.x, temp.y + y_scale, temp.z);

        playerOne.transform.position = temp;

        playerTwo.transform.position = buildings_list[buildings_list.Count - 3].transform.position;

        temp = playerTwo.transform.position;
        y_scale = buildings_list[buildings_list.Count - 3].transform.localScale.y;
        temp = new Vector3(temp.x, temp.y + y_scale, temp.z);

        playerTwo.transform.position = temp;

    }

    private void MakeSun()
    {
        throw new NotImplementedException();
    }

    private void MakePlayers()
    {
        playerOne = Instantiate(playerPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
        playerOne.name = "GorillaOne";

        playerTwo = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        playerTwo.name = "GorillaTwo";
    }

    void Launch(InputAction.CallbackContext context)
    {
        GameObject projectile = Instantiate(projectilePrefab, currentPlayer.transform.position, currentPlayer.transform.rotation);
        float angle = angleInput;
        float velocity = velocityInput;
        if (state == GameState.PLAYER_TWO) { angle = 180-angle; }
        projectile.GetComponent<ProjectileScript>().Launch(angle, velocity);
        Turn();
    }

    private void Turn()
    {
        if (state == GameState.START)
        {
            state = GameState.PLAYER_ONE;
            currentPlayer = playerOne;
        }

        else if (state == GameState.PLAYER_ONE)
        {
            state = GameState.PLAYER_TWO;
            currentPlayer = playerTwo;
        }

        else if (state == GameState.PLAYER_TWO)
        {
            state = GameState.PLAYER_ONE;
            currentPlayer = playerOne;
        }
    }

    public void doScore(GameObject hit_gorilla)
    {
        if (hit_gorilla.name == "GorillaOne") { playerTwoScore += 1; }
        else if (hit_gorilla.name == "GorillaTwo") { playerOneScore += 1; }
        displayScore();
    }

    public void displayScore()
    {
        string sign = "=";
        if (playerOneScore > playerTwoScore) { sign = ">"; }
        else if (playerOneScore < playerTwoScore) { sign = "<"; }

        scoreText.text = playerOneScore.ToString() + "    " + sign + "    " + playerTwoScore.ToString();
    }

    public void Reset()
    {
        NextRound();
    }

    private void CheckWin()
    {
        if (playerOneScore > 2 || playerTwoScore > 2) {
            state = GameState.END;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VelocityInput()
    {
        velocityInput = float.Parse(velocityField.text);
    }

    public void AngleInput()
    {
        angleInput = float.Parse(angleField.text);
    }
}
