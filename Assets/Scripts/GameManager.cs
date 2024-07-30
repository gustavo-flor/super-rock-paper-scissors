using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text resultMessage;
    [SerializeField] private Sprite rockCardSprite;
    [SerializeField] private Sprite paperCardSprite;
    [SerializeField] private Sprite scissorsCardSprite;
    [SerializeField] private Sprite unknownCardSprite;
    [SerializeField] private Image rockOpponentAction;
    [SerializeField] private Image paperOpponentAction;
    [SerializeField] private Image scissorsOpponentAction;
    [SerializeField] private Button rockButton;
    [SerializeField] private Button paperButton;
    [SerializeField] private Button scissorsButton;

    private readonly string[] _actions = { "Rock", "Paper", "Scissors" };
    private int _timer;
    private string _playerAction;
    private string _opponentAction;
    private int _plays;
    private bool _waitingToPlay;

    private void Start()
    {
        _waitingToPlay = true;
        StartGame();
    }

    private void Update()
    {
        if (_waitingToPlay && _playerAction != null && _opponentAction != null)
        {
            Play();
        }
    }

    public void SetRandomOpponentAction()
    {
        string action = GetRandomAction();
        _opponentAction = action;
    }

    public void SetPlayerAction(string action)
    {
        _playerAction = action;
    }

    private void Play()
    {
        _waitingToPlay = false;
        DisableButtons();
        switch (_opponentAction)
        {
            case "Rock":
                switch (_playerAction)
                {
                    case "Rock":
                        resultMessage.text = "Tie!";
                        break;
                    case "Paper":
                        GiveFirstWinAchievement();
                        resultMessage.text = "Paper covers the rock, you win!";
                        break;
                    case "Scissors":
                        resultMessage.text = "Rock breaks the scissors, you lose!";
                        break;
                }
                rockOpponentAction.sprite = rockCardSprite;
                break;
            case "Paper":
                switch (_playerAction)
                {
                    case "Rock":
                        resultMessage.text = "Paper covers the rock, you lose!";
                        break;
                    case "Paper":
                        resultMessage.text = "Tie!";
                        break;
                    case "Scissors":
                        GiveFirstWinAchievement();
                        resultMessage.text = "Scissors cuts the paper, you win!";
                        break;
                }
                paperOpponentAction.sprite = paperCardSprite;
                break;
            case "Scissors":
                switch (_playerAction)
                {
                    case "Rock":
                        GiveFirstWinAchievement();
                        resultMessage.text = "Rock breaks the scissors, you win!";
                        break;
                    case "Paper":
                        resultMessage.text = "Scissors cuts the paper, you lose!";
                        break;
                    case "Scissors":
                        resultMessage.text = "Tie!";
                        break;
                }
                scissorsOpponentAction.sprite = scissorsCardSprite;
                break;
        }
        _plays += 1;
        StartCoroutine(ResetGame());
    }

    private Button GetRandomActionButton()
    {
        string action = GetRandomAction();
        switch (action)
        {
            case "Rock":
                return rockButton;
            case "Paper":
                return paperButton;
            case "Scissors":
                return scissorsButton;
        }
        throw new Exception("Unknown action received (" + action + ")");
    }

    private string GetRandomAction()
    {
        return _actions[Random.Range(0, _actions.Length)];
    }

    private void DisableButtons()
    {
        rockButton.interactable = false;
        paperButton.interactable = false;
        scissorsButton.interactable = false;
    }

    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(3f);
        resultMessage.text = "Starting new game in 3s";
        yield return new WaitForSeconds(1f);
        resultMessage.text = "Starting new game in 2s";
        yield return new WaitForSeconds(1f);
        resultMessage.text = "Starting new game in 1s";
        yield return new WaitForSeconds(1f);
        StartGame();
    }

    private void StartGame()
    {
        resultMessage.text = "";
        rockOpponentAction.sprite = unknownCardSprite;
        paperOpponentAction.sprite = unknownCardSprite;
        scissorsOpponentAction.sprite = unknownCardSprite;
        rockButton.interactable = true;
        paperButton.interactable = true;
        scissorsButton.interactable = true;
        rockButton.enabled = true;
        paperButton.enabled = true;
        scissorsButton.enabled = true;
        _playerAction = null;
        _opponentAction = null;
        _waitingToPlay = true;
        _timer = 20;
        SetRandomOpponentAction();
        StartCoroutine(CountDownTimer());
    }

    private IEnumerator CountDownTimer()
    {
        if (_playerAction == null)
        {
            if (_timer == 0)
            {
                Button button = GetRandomActionButton();
                button.onClick.Invoke();
            }
            if (_timer > 0)
            {
                resultMessage.text = "Choose an action! " + _timer + "s";
                _timer -= 1;
                yield return new WaitForSeconds(1f);
                StartCoroutine(CountDownTimer());
            }
        } else if (_opponentAction == null)
        {
            resultMessage.text = "Waiting opponent! " + _timer + "s";
            _timer -= 1;
            yield return new WaitForSeconds(1f);
            StartCoroutine(CountDownTimer());
        }
    }

    private void GiveFirstWinAchievement()
    {
        if (_plays != 0 || !SteamManager.Initialized) { return; }

        SteamUserStats.SetAchievement("ACH_WIN_ONE_GAME");
        SteamUserStats.StoreStats();
    }
}
