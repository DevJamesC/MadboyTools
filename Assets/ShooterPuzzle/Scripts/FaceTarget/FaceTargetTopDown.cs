using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTargetTopDown : MonoBehaviour
{
    public float turnSpeed = 100;
    public bool snapToAngle;
    public Transform target;
    Transform trans;
    Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        FaceTarget(target.position);
    }

    void FaceTarget(Vector2 target)
    {
        
        Vector2 subjectPosition = trans.position;
        target -= subjectPosition;

        float targetAngle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        Vector3 rot = transform.rotation.eulerAngles;

        rot.z = (snapToAngle) ? targetAngle : Mathf.MoveTowardsAngle(rot.z, targetAngle, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(rot);

    }
}
