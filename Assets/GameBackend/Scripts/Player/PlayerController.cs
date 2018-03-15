using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BehaviourLibraryLinker
{

    public float speed = 10f;       // The speed at which I run
    public float jumpSpeed = 6f;    // How much power we put into our jump. Change this to jump higher.
    public IShootable currentWeapon;

    // Booking variables
    Vector3 direction = Vector3.zero;   // forward/back & left/right
    float verticalVelocity = 0;     // up/down

    CharacterController cc;

    // Use this for initialization
    void Start()
    {
        base.Start();

        cc = GetComponent<CharacterController>();
    }


    void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        // WASD forward/back & left/right movement is stored in "direction"
        direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        if (cc.isGrounded && Input.GetButton("Jump"))
        {
            verticalVelocity = jumpSpeed;
            if (Input.GetButton("Jump"))
                verticalVelocity = jumpSpeed;

        }

        if (Input.GetMouseButton(0))
        {
            GetComponent<Agent>().CurrentWeapon.Shoot();
        }

        if (direction.magnitude > 1f)
        {
            direction = direction.normalized;
        }


    }

    // FixedUpdate is called once per physics loop
    // Do all MOVEMENT and other physics stuff here.
    void FixedUpdate()
    {
        // "direction" is the desired movement direction, based on our player's input
        Vector3 dist = direction * speed * Time.deltaTime;

        if (cc.isGrounded && verticalVelocity < 0)
        {

            // Set our vertical velocity to *almost* zero. This ensures that:
            //   a) We don't start falling at warp speed if we fall off a cliff (by being close to zero)
            //   b) cc.isGrounded returns true every frame (by still being slightly negative, as opposed to zero)
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        // Add our verticalVelocity to our actual movement for this frame
        dist.y = verticalVelocity * Time.deltaTime;
        cc.Move(dist);
    }
}
