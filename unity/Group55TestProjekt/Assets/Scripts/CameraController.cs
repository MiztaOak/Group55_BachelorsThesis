using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float cameraSpeed = 1f;
    public float sensitivity = 2f;

    private float rotX = 0.0f;
    private float rotY = 55f; // REALLY BAD, SHOULD COME FROM transform.eulerAngle.x

    private Vector3 overView = new Vector3(0, 25, 0);
    Quaternion currentRotation;
    Vector3 currentEulerAngles = new Vector3(90, 0, 0);
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {

        Vector3 p = Vector3.zero;

        p = GetBaseInput() * cameraSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift)) {
            p *= 5.0f; // LeftShift for 5 times faster movement
        }
            
        // For movement, WASDEQ
        transform.Translate(p);

        // For camera view
        if (Input.GetMouseButtonDown(1)) {
            Cursor.lockState = CursorLockMode.Locked; // Hide & lock the cursor
        }
        if (Input.GetMouseButtonUp(1)) {
            // Show & unluck the cursor again
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButton(1)) {
            
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
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W)) {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A)) {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.Q)) {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.E)) {
            direction += Vector3.up;
        }
        return direction;
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