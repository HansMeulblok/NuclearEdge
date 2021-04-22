using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private RoomListing room;

    private List<RoomListing> listOfRooms = new List<RoomListing>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Removed room from room list.
            if (info.RemovedFromList)
            {
                int index = listOfRooms.FindIndex(room => room.RoomInfo.Name == info.Name);

                if (index != -1)
                {
                    Destroy(listOfRooms[index].gameObject);
                    listOfRooms.RemoveAt(index);
                }
            }
            // Added room to room list.
            else
            {
                RoomListing listing = Instantiate(room, content);
                if (listing != null)
                {
                    listing.SetRoomInfo(info);
                    listOfRooms.Add(listing);
                }
            }

        }
    }
}
