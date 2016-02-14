using UnityEngine;
using System.Collections;

public class mmoCharacterController : MonoBehaviour
{
    // Public varaibles
    public bool firstPerson = false;

    private Transform cameraPosition;
    private GameObject _camera;
    private Transform _myTransform;

    private float _zoomDistance = 5.0f;
    private Vector2 _zoomSpeeds = new Vector2(120.0f, 120.0f);  // Get these from configuration
    public float _zoomMin = 1.5f;
    public float _zoomMax = 15f;

    private Vector2 _zoomAngle = new Vector2(0.0f, 0.0f);
    private Vector2 _zoomLimits = new Vector2(-20f, 80f);

    private Vector3 position;
    public Vector3 positionOffset = new Vector3(0, 0, 0);

    private Rigidbody rBody;

    // Movement data
    private Vector3 _moveVector = Vector3.zero;
    private float _moveSpeed = 10.0f;
    private float _jumpSpeed = 500f;
    public bool isGrounded = false;

    public bool useGravity = false;

    private BoxCollider bCollider;
    private Renderer cRenderer;

    private bool _canFly = false;

    private Rigidbody[] rigids;

    void Start()
    {
        _myTransform = transform;
        _camera = GameObject.FindGameObjectWithTag("Player");

        cRenderer = GetComponentInChildren<Renderer>();

        if(!cRenderer)
        {
            Debug.Log("No renderer found, quitting");
            return;
        }

        bCollider = GetComponent<BoxCollider>();
        if(bCollider == null)
        {
            Debug.Log("No box collider found");
            bCollider = gameObject.AddComponent<BoxCollider>();
            bCollider.center = cRenderer.bounds.center;
            bCollider.size = new Vector3(bCollider.bounds.size.x, cRenderer.bounds.size.y, bCollider.bounds.size.z);
        }

        if (_camera == null)
        {
            Debug.Log("No main camera found, looking in children");
            _camera = GetComponentInChildren<Camera>().gameObject;
            if (_camera == null)
            {
                Debug.LogError("Cannot find camera, aborting");
                return;
            }
        }

        _camera.transform.position = _myTransform.position;

        rBody = GetComponent("Rigidbody") as Rigidbody;
        if (rBody == null)
        {
            Debug.Log("No RigidBody found, adding component");
            rBody = gameObject.AddComponent<Rigidbody>() as Rigidbody;
        }

        rBody.useGravity = false;
        rBody.freezeRotation = true;

        rigids = GetComponentsInChildren<Rigidbody>();
        ToggleKinematic(false);
    }

    // Update is called once per frame
    void Update()
    {
        // This one will create the distance float variable within the minimum and maximum values
        float _mouseScrollZoom = Mathf.Clamp(_zoomDistance - Input.GetAxis("Mouse ScrollWheel") * 5, _zoomMin, _zoomMax);
        if (!firstPerson)
            _zoomDistance = _mouseScrollZoom;
        else
            _zoomDistance = 0.0f;

        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            // These lines calculates the positions of the camera along the x and y axis
            _zoomAngle.x += Input.GetAxis("Mouse X") * _zoomSpeeds.x * 0.02f;
            _zoomAngle.y -= Input.GetAxis("Mouse Y") * _zoomSpeeds.y * 0.02f;
        }

        // This will ensure we do not exceed 80 degrees on the Y axis looking down, and 20 deg looking up
        _zoomAngle.y = ClampAngle(_zoomAngle.y, _zoomLimits.x, _zoomLimits.y);

        // This one will rotate the camera along the y and x axis according to the angles we calculated earlier
        // It will not move the camera, just change the rotation / lookrotation
        // In combination with the position movement this will make an orbit camera. Should be the same as using
        // Quaternation.LookRotation or transform.LookAt
        Quaternion rotation = Quaternion.Euler(_zoomAngle.y, _zoomAngle.x, 0);

        // Check for objects in the line-of-sights and adjust the zoomDistance
        RaycastHit hit;
        if (Physics.Linecast(_myTransform.position, _camera.transform.position, out hit))
        {
            if (hit.collider != rBody.GetComponent<Collider>())
                _zoomDistance -= hit.distance;
        }

        // Not sure why this is here and what it does
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -_zoomDistance);


        Vector3 meshPosition = cRenderer.bounds.center;

        position = rotation * negDistance + meshPosition + positionOffset;
        _camera.transform.rotation = rotation;
        _camera.transform.position = position;

        // Rotate the object while holding down the right mouse button
        if (Input.GetMouseButton(1))
        {
            _myTransform.rotation = Quaternion.Euler(0, _zoomAngle.x, 0);
        }

        UpdateMovement();
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    private void UpdateMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !_canFly)
            rBody.AddRelativeForce(0, _jumpSpeed, 0);

        if (isGrounded)
        {
            _moveVector = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                _moveVector += rBody.transform.forward * _moveSpeed;
            if (Input.GetKey(KeyCode.S))
                _moveVector += -rBody.transform.forward * _moveSpeed;
            if (Input.GetKey(KeyCode.A))
                _moveVector += -rBody.transform.right * _moveSpeed;
            if (Input.GetKey(KeyCode.D))
                _moveVector += rBody.transform.right * _moveSpeed;

            //Debug.Log(_moveVector);
        }

        if (useGravity && !isGrounded)
        {
            rBody.AddForce(Physics.gravity);
        }

        rBody.MovePosition(rBody.position + _moveVector * Time.deltaTime);
    }

    void OnCollisionStay(Collision c)
    {
        Vector3 hitDir = c.contacts[0].point - transform.position;
        Vector3 bottom = transform.TransformDirection(-Vector3.up);


        Debug.Log("Hitting comething");

        if (Vector3.Dot(bottom, hitDir) > 0)
        {
            isGrounded = true;
            //rBody.MoveRotation (Quaternion.Euler(rBody.transform.up + c.contacts[0].normal));
            Vector3 hitNormal = c.contacts[0].normal;

            if (hitNormal != _myTransform.up)
            {
                /***
				 * TODO: Make the character rotate with the ground in slopes, and not slide down hills when standing still
				 * */
                //Quaternion toRotation = Quaternion.FromToRotation(_myTransform.up, c.contacts[0].normal);
                //_myTransform.rotation = Quaternion.Slerp(_myTransform.rotation, toRotation, 1.0f);
                //Debug.Log (c.contacts[0].normal);
            }
        }
    }

    void OnCollisionExit(Collision c)
    {
        Vector3 hitDir = c.contacts[0].point - transform.position;
        Vector3 bottom = transform.TransformDirection(-Vector3.up);

        if (Vector3.Dot(bottom, hitDir) > 0)
        {
            isGrounded = false;
        }
    }

    public void ToggleKinematic(bool toggle)
    {
        foreach (Rigidbody rb in rigids)
        {
            rb.isKinematic = toggle;
        }
    }
}