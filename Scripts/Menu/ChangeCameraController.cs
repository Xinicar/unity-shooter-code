using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _virtual;

    void Start()
    {
        _virtual.Priority++;
    }

    // Update is called once per frame
    public void UpdateCamera(CinemachineVirtualCamera _cam)
    {
        _virtual.Priority--;

        _virtual = _cam;

        _virtual.Priority++;
    }
}
