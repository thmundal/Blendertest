using UnityEngine;
using System.Collections;

public class playerAnimation : MonoBehaviour {

    private Animator anim;
    private CharacterController controller;
    public float speed = 6.0f;
    public float turnSpeed = 60.0f;
    private Vector3 moveDirection = Vector3.zero;
    public float gravity = 20.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private Camera camera;

	// Use this for initialization
	void Start () {
        anim = gameObject.GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        camera = GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetAxis("Vertical") != 0)
        {
            anim.SetInteger("AnimParam", 1);
        } else
        {
            anim.SetInteger("AnimParam", 0);
        }

        /*if(controller.isGrounded)
        {
            moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
        }*/
        /*
        yaw = turnSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        pitch = turnSpeed * -Input.GetAxis("Mouse Y") * Time.deltaTime;
        Vector3 charPos = transform.position + new Vector3(0, 5f, 0);
        camera.transform.LookAt(charPos);

        pitch = Mathf.Clamp(pitch, -45f, 45f);
        //camera.transform.RotateAround(charPos, camera.transform.right, pitch);
        */

        //transform.Rotate(0, yaw, 0);
        //controller.Move(moveDirection * Time.deltaTime);
        //moveDirection.y -= gravity * Time.deltaTime;
	}
}
