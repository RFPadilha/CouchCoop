using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject[] spawnPoints;

    public List<PlayerInput> playerList = new List<PlayerInput>();

    [SerializeField] InputAction joinAction;
    [SerializeField] InputAction leaveAction;

    //Instances
    public static GameManager instance = null;

    //Events
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null) Destroy(gameObject);

        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        joinAction.Enable();
        joinAction.performed += context => JoinAction(context);

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);
    }
    private void Start()
    {
        //adds player 1 on game start with ID = 0
        PlayerInputManager.instance.JoinPlayer(0,-1,null);

    }
    void OnPlayerJoined(PlayerInput playerInput)
    {
        playerList.Add(playerInput);
        if(PlayerJoinedGame != null)
        {
            PlayerJoinedGame(playerInput);
        }
        //Debug.Log("Player joined");

    }
    void JoinAction(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }
    void LeaveAction(InputAction.CallbackContext context)
    {
        if(playerList.Count > 1)
        {
            //for each player in the list
            foreach(var player in playerList)
            {
                //find the device they're using
                foreach(var device in player.devices)
                {
                    //if found, drop out
                    if(device != null && context.control.device == device)
                    {
                        UnregisterPlayer(player);
                        return;
                    }
                }
            }
        }
    }
    void UnregisterPlayer(PlayerInput playerInput)
    {
        playerList.Remove(playerInput);
        if(PlayerLeftGame != null)
        {
            PlayerLeftGame(playerInput);
        }
        Destroy(playerInput.transform.parent.gameObject);
        Debug.Log("Player left");
    }
}
