﻿using TMPro;
using UnityEngine;

public class NetworkRoomUI : MonoBehaviour
{
    #region Delegates
    public delegate void OnRoomSelectedDelegate(RoomInfo roomInfo);
    #endregion

    private OnRoomSelectedDelegate OnRoomSelectedCallback;
    public RoomInfo roomInfo;

    [SerializeField] private TMP_Text roomNameText;

    public void Setup(RoomInfo roomInfo, OnRoomSelectedDelegate OnRoomSelectedCallback)
    {
        this.roomInfo = roomInfo;
        this.OnRoomSelectedCallback = OnRoomSelectedCallback;
        roomNameText.text = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
    }

    public void SelectRoom()
    {
        if (OnRoomSelectedCallback != null)
        {
            OnRoomSelectedCallback.Invoke(roomInfo);
        }
    }
}