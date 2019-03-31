using UnityEngine;

public class PlayerMovementFixed : MonoBehaviour
{
    private struct State
    {
        public float time;
        public Vector3 position;
    }

    [SerializeField]
    private LayerMask groundCollision;

    // ScriptableObjs
    [SerializeField]
    private MovementValues MoveSettings;

    // Internals for class control
    private float playerHeight;
    private Vector3 playerVelocity = Vector3.zero;
    private Vector3 playerDireccion = Vector3.zero;
    private float groundSmooth = 0f; //Extra distance to smooth out rough terrain
    private CharacterController chController; //will be used for caracter collision
    private IPlayerInput reader;
    private bool wishJump = false;
    private Vector3 moveDirNorm;
    private Transform parentTransform;
    private float wishSpeed;
    private PlayerEnviromentChecker checker;

    private State next;
    private State current;
    private float lastState = float.NegativeInfinity;
    private NetworkTag networkTag;

    // Awake is called when object is enabled
    private void Awake()
    {
        current = next = new State { time = Time.time, position = transform.position };

        reader = GetComponent<IPlayerInput>();
        checker = GetComponent<PlayerEnviromentChecker>();
        chController = GetComponentInParent<CharacterController>();
        playerHeight = chController.bounds.extents.y;
        networkTag = GetComponent<NetworkTag>();
        parentTransform = GetComponentInParent<Transform>();

    }

    // Update is called once per frame
    private void Update()
    {
        if (current.time != next.time)
        {
            float t = (Time.time - current.time) / (next.time - current.time);
            transform.position = Vector3.Lerp(current.position, next.position, t);
        }
    }

    private void FixedUpdate()
    {
        /* Debug part of the code */
        Debug.DrawRay(transform.position, -transform.up * (playerHeight + groundSmooth), Color.red);

        // State setup
        transform.position = next.position;
        current = next;

        // Movement part 
        QueueJump();
        if (IsGrounded())
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }
        if (playerVelocity.y < 0)
        {
            playerVelocity.y -= MoveSettings.gravity * Time.fixedDeltaTime * MoveSettings.downGravityMultiplier;
        }
        else
        {
            playerVelocity.y -= MoveSettings.gravity * Time.fixedDeltaTime;
        }


        chController.Move(playerVelocity * Time.fixedDeltaTime);

        // State gather
        next.time = Time.fixedTime + Time.fixedDeltaTime;
        next.position = transform.position;

        // Network part
        //networkTag.Call(NetworkMove, NetworkTarget.Others, new BitStream().Write(Time.fixedTime).Write(transform.position).Write(transform.rotation), SendType.Unreliable);

    }

    [NetworkCall]
    private void NetworkMove(BitStream stream, SteamPlayer sender)
    {
        // Is this the actual owner sending the state?
        if (sender == networkTag.Owner)
        {
            float time = stream.ReadFloat();
            Vector3 position = stream.ReadVector3();
            Quaternion rotation = stream.ReadQuaternion();

            // Is this state outdated?
            if (lastState < time)
            {
                lastState = time;
                transform.position = position;
                transform.rotation = rotation;
            }
        }
    }

    private void AirMove()
    {
        float wishVel = MoveSettings.airAcceleration;
        float accel;
        Vector3 wishDir;

        wishDir = DesireDirection();

        float wishSpeed = wishDir.magnitude;
        wishSpeed *= MoveSettings.baseSpeed;

        wishDir.Normalize();
        moveDirNorm = wishDir;

        //start AirControl part
        if (Vector3.Dot(playerVelocity, wishDir) < 0)
            accel = MoveSettings.airDecceleration;
        else
            accel = MoveSettings.airAcceleration;

        float wishSpeed2 = wishSpeed;

        //if the player is only strafing
        if (!(reader.Forward || reader.Backwards))
        {
            if (wishSpeed > MoveSettings.sideStrafeSpeed) wishSpeed = MoveSettings.sideStrafeSpeed;

            accel = MoveSettings.sideStrafeAcceleration;
        }

        Accelerate(wishDir, wishSpeed, accel);
        if (MoveSettings.airControl > 0)
            AirControl(wishDir, wishSpeed2);
        

    }

    /// <summary>
    /// Straight up copied from https://github.com/WiggleWizard/quake3-movement-unity3d/blob/master/CPMPlayer.cs#L149 not feeling that sure for it
    /// </summary>

    private void AirControl(Vector3 wishdir, float wishspeed)
    {
        float zspeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backwards
        if (reader.Forward || reader.Backwards || Mathf.Abs(wishspeed) < 0.001)
            return;
        zspeed = playerVelocity.y;
        playerVelocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dot = Vector3.Dot(playerVelocity, wishdir);
        k = 32;
        k *= MoveSettings.airControl * dot * dot * Time.fixedDeltaTime;

        // Change direction while slowing down
        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

            playerVelocity.Normalize();
            moveDirNorm = playerVelocity;
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zspeed; // Note this line
        playerVelocity.z *= speed;
    }

    private void GroundMove()
    {
        // only apply friction if the player is not queuing a jump
        if (!wishJump)
        {
            ApplyFriction(1f);
        }

        moveDirNorm = DesireDirection();
        moveDirNorm.Normalize();
        wishSpeed = moveDirNorm.magnitude;
        wishSpeed *= MoveSettings.baseSpeed;
        Accelerate(moveDirNorm, wishSpeed, MoveSettings.runAcceleration);


        //Resets gravity velocity if grounded
        playerVelocity.y = -MoveSettings.gravity * Time.fixedDeltaTime;

        //This may make jumping weird on slopes
        if (checker.OnSlope)
        {
            //this will single out what parts are perpendicular to the plane, the 
            //the substraction will make it so only the parts paralel to the plane stay.
            Vector3 temp = Vector3.Project(playerVelocity, checker.Hit.normal);
            playerVelocity -= temp;
        }


        if (wishJump)
        {
            playerVelocity.y = MoveSettings.jumpSpeed;
            wishJump = false;
        }

    }



    private void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
    {

        float addSpeed, accelSpeed, currentSpeed;

        currentSpeed = Vector3.Dot(playerVelocity, wishDir);
        addSpeed = wishSpeed - currentSpeed;
        if (addSpeed <= 0)
            return;

        accelSpeed = accel * Time.fixedDeltaTime * wishSpeed;
        if (accelSpeed >= addSpeed)
            accelSpeed = addSpeed;

        playerVelocity.x += wishDir.x * accelSpeed;
        playerVelocity.z += wishDir.z * accelSpeed;

    }


    /// <summary>
    /// Transforms input booleans to a vector3 used for horizontal plane movement
    /// </summary>

    private Vector3 DesireDirection()
    {

        Vector3 result = new Vector3((reader.Right ? 1 : 0) + (reader.Left ? -1 : 0), 0, (reader.Forward ? 1 : 0) + (reader.Backwards ? -1 : 0));
        result = parentTransform.TransformDirection(result);
        return result;

    }


    /// <summary>
    /// Apply friction function, not that sure about its implementation, got 2 ask 4 input
    /// </summary>
    /// <param name="fMultiplier"></param>
    private void ApplyFriction(float fMultiplier)
    {
        Vector3 vec = playerVelocity; // Equivalent to: VectorCopy();
        float speed;
        float newspeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        /* Only if the player is on the ground then apply friction */
        if (IsGrounded())
        {
            control = speed < MoveSettings.runDeacceleration ? MoveSettings.runDeacceleration : speed;
            drop = control * MoveSettings.friction * Time.fixedDeltaTime * fMultiplier;
        }

        newspeed = speed - drop;

        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;

        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;

    }


    /// <summary>
    /// Queues Jump 
    /// </summary>
    private void QueueJump()
    {
        wishJump = false;

        if (MoveSettings.holdJumpToBhop)
        {
            wishJump = reader.JumpingHeld;
        }

        if (reader.JumpingPressed)
        {
            wishJump = true;
            reader.JumpingPressed = false;
        }
    }


    /// <returns>
    /// Retruns true if anything is under the player's feet
    /// </returns>
    private bool IsGrounded()
    {
        return checker.IsGrounded;
    }

}
