using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCamera : MonoBehaviour {

    [Range(0, 0.5f)]
    public float moveSpeed = 0.1f;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float rotationSpeed = 0.5f;

    float yaw = 0.0f;
    float pitch = 0.0f;

    private void Update() {
        UpdatePosition();
        UpdateRotation();
        CheckHit();
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit objectHit;
        //if (Physics.Raycast(ray, out objectHit)) {
        //    Transform target = objectHit.transform;
        //    if (Input.GetKey(KeyCode.W)) {
        //        transform.Translate(Vector3.up * moveSpeed);
        //        transform.LookAt(target);
        //        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * moveSpeed);
        //    }
        //    if (Input.GetKey(KeyCode.S)) {
        //        transform.Translate(Vector3.down * moveSpeed);
        //        transform.LookAt(target);
        //        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * moveSpeed);
        //    }
        //    if (Input.GetKey(KeyCode.D)) {
        //        transform.Translate(Vector3.right * moveSpeed);
        //        transform.LookAt(target);
        //        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * moveSpeed);
        //    }
        //    if (Input.GetKey(KeyCode.A)) {
        //        transform.Translate(Vector3.left * moveSpeed);
        //        transform.LookAt(target);
        //        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * moveSpeed);
        //    }
        //    if (Input.GetKey(KeyCode.Mouse0)) {
        //        transform.LookAt(target);
        //        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * moveSpeed);
        //    }
        //}
    }

    private void UpdatePosition() {
        if (Input.GetKey(KeyCode.W)) { transform.Translate(Vector3.forward * moveSpeed); }
        if (Input.GetKey(KeyCode.S)) { transform.Translate(Vector3.back * moveSpeed); }
        if (Input.GetKey(KeyCode.D)) { transform.Translate(Vector3.right * moveSpeed); }
        if (Input.GetKey(KeyCode.A)) { transform.Translate(Vector3.left * moveSpeed); }
        if (Input.GetKey(KeyCode.Space)) { transform.Translate(Vector3.up * moveSpeed); }
    }

    private void UpdateRotation() {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    private void CheckHit() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Transform selectedObject;
        if (Physics.Raycast(ray, out hit)) {
            Debug.Log("Test");
            if (Input.GetKey(KeyCode.Mouse0)) {
                Debug.Log("Object Selected!");
                selectedObject = hit.transform;
                selectedObject.transform.Rotate(0, 0.5f, 0);
            }
        }
    }
}
