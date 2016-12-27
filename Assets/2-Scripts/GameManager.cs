﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Player startPlayer;
    public Canvas canvas;
    //private WeaponManager weaponManager;

    public Player currentPlayer { get; set; }
    //public Weapon playerCurrentWeapon { get; set; }
    private ControllersUI controllersUI;
    private GameUI gameUI;

    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

	void Awake()
    {
        //Set up UI
        canvas = FindObjectOfType<Canvas>();
        controllersUI = canvas.GetComponent<ControllersUI>();
        gameUI = canvas.GetComponent<GameUI>();

        //Find map, player, and equip weapon
        //currentPlayer = (Player) Instantiate(startPlayer, mapGen.CoordToPosition(map.mapSize.x / 2, 2), Quaternion.identity);

        //playerCurrentWeapon = weaponManager.allWeapons[weaponManager.startingWeaponIndex];

        //Setup audio
        Transform audioManager = FindObjectOfType<AudioManager>().transform;
        audioManager.position = currentPlayer.transform.position;
        audioManager.rotation = currentPlayer.transform.rotation;
        audioManager.parent = currentPlayer.transform;

        //Set up controllers
        SwipeDetector swipeDetect = currentPlayer.GetComponent<SwipeDetector>();
        swipeDetect.aimJoystickArea = controllersUI.AimJoystickArea;
        swipeDetect.aimJoystickRect = controllersUI.AimJoystickRect;
        swipeDetect.attackJoystickRect = controllersUI.AttackJoystickRect;
        swipeDetect.changeWeaponJoystickRect = controllersUI.ChangeWeaponRect;
        //Camera.main.GetComponent<CameraController>().SetTarget(currentPlayer.transform);
        //Camera.main.GetComponent<CameraFollow>().SetTarget(currentPlayer.transform);

    }

	
}
