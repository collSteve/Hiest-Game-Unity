using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnPlayerWin;

    public float moveSpeed = 7f;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8f;

    private float angle = 0;
    private float smoothInputMagnitude = 0;
    private float smoothMoveVolecity = 0;
    private Vector3 velocity;
    new Rigidbody rigidbody;
    private bool spotted;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardSpottedPlayer += OnSpotted; // static
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float inputMag = inputDirection.magnitude;

        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMag, ref smoothMoveVolecity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMag);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "finish point")
        {
            if (OnPlayerWin != null)
            {
                OnPlayerWin();
            }
        }
    }

    void OnSpotted()
    {
        spotted = true;
        transform.GetComponent<Renderer>().material.color = Color.red;
    }

    private void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    private void OnDestroy()
    {
        Guard.OnGuardSpottedPlayer -= OnSpotted;
    }
}
