using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float cameraSpeed = 1f;
    public float sensitivity = 2f;

    private float rotX = 0.0f;
    private float rotY = 55f; // REALLY BAD, SHOULD COME FROM transform.eulerAngle.x

    private Vector3 overView = new Vector3(0, 15, 0);
    Quaternion currentRotation;
    Vector3 currentEulerAngles = new Vector3(90, 0, 0);
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {

        // For movement, WASDEQ
        Vector3 p = GetBaseInput() * cameraSpeed * Time.deltaTime;
        transform.Translate(p);

        // For camera view
        if (Input.GetMouseButton(0)) {
            rotX += sensitivity * Input.GetAxis("Mouse X");
            rotY -= sensitivity * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(rotY, rotX, 0.0f);
        }
        // Nice animation towards a nice overview when pressing G 
        if (Input.GetKeyDown(KeyCode.G)) {
            StartCoroutine(animCamera(overView)); 
        }
    }
    private Vector3 GetBaseInput() {
        if (Input.GetKey(KeyCode.W)) {
            return Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            return Vector3.back;
        }
        if (Input.GetKey(KeyCode.A)) {
            return Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) {
            return Vector3.right;
        }
        if (Input.GetKey(KeyCode.Q)) {
            return Vector3.down;
        }
        if (Input.GetKey(KeyCode.E)) {
            return Vector3.up;
        } else return Vector3.zero;
    }
    IEnumerator animCamera(Vector3 dest) {
        while (transform.position != overView || transform.rotation != currentRotation) {
            transform.position = Vector3.MoveTowards(transform.position, overView, cameraSpeed * Time.deltaTime);
            currentRotation.eulerAngles = currentEulerAngles;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, currentRotation, cameraSpeed*3 * Time.deltaTime);
            yield return null;
        }
    }
}