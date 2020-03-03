using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSimpleFollow : MonoBehaviour
{
    const int zDistFromPlayer = -10;
    public Transform target;

    // Update is called once per frame
    void Update()
    {
     if(target!=null)
        {
            CenterOnTarget();
        }
    }

    void CenterOnTarget()
    {
        Vector3 position = new Vector3(target.position.x,target.position.y,zDistFromPlayer);
        transform.position = position;
    }
}
