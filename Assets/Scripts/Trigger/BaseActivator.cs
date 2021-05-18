using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseActivator : MonoBehaviourPun, IPunObservable
{
    //The base activate fuction
    public abstract void Activate();

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
