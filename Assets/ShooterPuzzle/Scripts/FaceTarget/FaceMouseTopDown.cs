using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMouseTopDown : MonoBehaviour
{
    public float turnSpeed=100;
    public bool snapToAngle;
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
        FaceMouse();
    }

    void FaceMouse()
    {
        Vector2 targetPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 subjectPosition = trans.position;
        targetPosition -= subjectPosition;

        float targetAngle = Mathf.Atan2(targetPosition.y,targetPosition.x)*Mathf.Rad2Deg;
        Vector3 rot = transform.rotation.eulerAngles;
      
            rot.z = (snapToAngle)?targetAngle:Mathf.MoveTowardsAngle(rot.z, targetAngle, turnSpeed * Time.deltaTime);
      
        transform.rotation = Quaternion.Euler(rot);

    }
}
