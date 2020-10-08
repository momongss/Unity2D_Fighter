using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MainCamera_Action : MonoBehaviour
{
    public GameObject player;
    public float followSpeed;
    public float MapX, MapY;

    Vector3 cameraPosition;
    float cameraH, cameraW;

    private void Awake()
    {
        cameraH = Camera.main.orthographicSize;
        cameraW = cameraH * Camera.main.aspect;
    }

    void LateUpdate()
    {
        // 카메라 움직임 o
        cameraPosition.x = player.transform.position.x;
        cameraPosition.y = player.transform.position.y;
        cameraPosition.z = transform.position.z;

        if (MovableXY())
        {
            transform.position = Vector3.Lerp(transform.position, cameraPosition, followSpeed * Time.deltaTime);
        }
        else if (MovableX())
        {
            int ty = (int)(player.transform.position.y / Mathf.Abs(player.transform.position.y));

            cameraPosition.x = player.transform.position.x;
            cameraPosition.y = ty * (MapY - cameraH);
            transform.position = Vector3.Lerp(transform.position, cameraPosition, followSpeed * Time.deltaTime);
        }
        else if (MovableY())
        {
            int tx = (int)(player.transform.position.x / Mathf.Abs(player.transform.position.x));

            cameraPosition.x = tx * (MapX - cameraW);
            cameraPosition.y = player.transform.position.y;
            transform.position = Vector3.Lerp(transform.position, cameraPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            int tx = (int)(player.transform.position.x / Mathf.Abs(player.transform.position.x));
            int ty = (int)(player.transform.position.y / Mathf.Abs(player.transform.position.y));

            cameraPosition.x = tx * (MapX - cameraW);
            cameraPosition.y = ty * (MapY - cameraH);
            transform.position = Vector3.Lerp(transform.position, cameraPosition, followSpeed * Time.deltaTime);
        }
    }

    bool MovableXY()
    {
        return -MapX <= cameraPosition.x - cameraW && cameraPosition.x + cameraW <= MapX && -MapY <= cameraPosition.y - cameraH && cameraPosition.y + cameraH <= MapY;
    }

    bool MovableX()
    {
        return -MapX <= cameraPosition.x - cameraW && cameraPosition.x + cameraW <= MapX;
    }

    bool MovableY()
    {
        return -MapY <= cameraPosition.y - cameraH && cameraPosition.y + cameraH <= MapY;
    }

}
