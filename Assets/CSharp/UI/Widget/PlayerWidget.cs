﻿using UnityEngine;
using UnityEngine.UI;

public class PlayerWidget : UIWidget
{
    [SerializeField] private Text playerName;
    [SerializeField] private Text scoreNumber;
    [SerializeField] private Transform pillowIconList;

    private GameObject[] pillowIcons;

    private Player player;

    public void Initialize(Player player)
    {
        if (player)
        {
            this.player = player;

            player.OnScoreChange.AddListener(UpdateScore);
            player.OnNumPillowsHeldChange.AddListener(UpdatePillows);

            UpdateAll();
        }
    }

    private void UpdateAll()
    {
        playerName.text = player.Name;

        UpdateScore(player.Score);
        UpdatePillows(player.NumPillowsHeld);
    }

    private void UpdateScore(int score)
    {
        scoreNumber.text = string.Format("Score: {0:D3}", score);
    }

    private void UpdatePillows(int numPillowsHeld)
    {
        int i = 0;

        while (i < numPillowsHeld)
            pillowIcons[i++].SetActive(true);

        while (i < 2)
            pillowIcons[i++].SetActive(false);
    }

    private void Awake()
    {
        int maxNumPillowsHeld = pillowIconList.childCount;

        pillowIcons = new GameObject[maxNumPillowsHeld];
        for (int i = 0; i < maxNumPillowsHeld; i++)
            pillowIcons[i] = pillowIconList.GetChild(i).gameObject;
    }

    private void OnDestroy()
    {
        if (player)
        {
            player.OnScoreChange.RemoveListener(UpdateScore);
            player.OnNumPillowsHeldChange.RemoveListener(UpdatePillows);
        }
    }
}
