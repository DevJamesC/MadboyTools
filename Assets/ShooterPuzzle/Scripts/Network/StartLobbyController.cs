using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject startBtn;
    [SerializeField]
    private GameObject cancelBtn;
    [SerializeField]
    private int roomSize = 1000;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        startBtn.SetActive(true);
    }

    public void StartRoom()
    {
        startBtn.SetActive(false);
        cancelBtn.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("quick start");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Creating a new room");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        Debug.Log(randomRoomNumber);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... trying again");
        CreateRoom();
    }

    public void CancelRoom()
    {
        cancelBtn.SetActive(false);
        startBtn.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
