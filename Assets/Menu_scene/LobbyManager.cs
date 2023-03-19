using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private ListItem roomPrefab;
    [SerializeField] private Transform content;
    
    public Text LogText;
    public InputField inputField;

    private string _roomName;
    void Start()
    {
        PhotonNetwork.NickName = "Player " + Random.Range(1, 100);
        MyLog("Player's name is set to " + PhotonNetwork.NickName);

        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.GameVersion = "1.00";
        PhotonNetwork.ConnectUsingSettings();

        _roomName = "Room of " + PhotonNetwork.NickName;
        PhotonNetwork.JoinLobby();
    }

    public override void OnConnectedToMaster()
    {
        MyLog("Connected to master!");

        PhotonNetwork.JoinLobby();
    }

    private void MyLog(string message)
    {
        Debug.Log(message);
        LogText.text += "\n";
        LogText.text += message;
    }

    public void SetRoomName()
    {
        _roomName = inputField.text;
        MyLog("Room name: " + _roomName);
    }
    
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(_roomName, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("Game");
    }
    
    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    
    public override void OnJoinedRoom()
    {
        MyLog("Joined the room!");
        PhotonNetwork.LoadLevel("Game");
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // while (content.childCount > 0) {
        //     DestroyImmediate(content.GetChild(0).gameObject);
        // }

        bool toCreate = true;
        
        MyLog("ListUpdated!");
        foreach (var roomInfo in roomList)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                var currentChild = content.GetChild(i).gameObject;
                if (currentChild.GetComponent<ListItem>().GetTextName() == roomInfo.Name)
                {
                    toCreate = false;
                    
                    if (roomInfo.MaxPlayers == 0)
                    {
                        DestroyImmediate(currentChild);
                        break;
                    }
                    
                    currentChild.GetComponent<ListItem>().SetInfo(roomInfo);
                }
            }

            if (toCreate && roomInfo.MaxPlayers != 0)
            {
                ListItem item = Instantiate(roomPrefab, content);
                if (item)
                    item.SetInfo(roomInfo);
            }
        }
    }
}
