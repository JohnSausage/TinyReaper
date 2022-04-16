using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Vector2 InputVelocity;
    protected Vector2 lastInputVelocity;

    public Vector2 velocity;// { get { return velocity; } }
    public Vector2 OutputVelocity { get => velocity * 60; }
    protected Vector2 oldVelocity;

    public Vector2 Position { get { return transform.TransformPoint(col.offset); } }

    protected float faceDirection;
    public float FaceDirection { set { faceDirection = value; } }

    protected BoxCollider2D col;
    protected Bounds bounds;
    protected static float skin = 0.01f;

    /* can be changed during runtime*/
    public float Gravity = -1;// { get; set; }
    public float Movespeed = 10;// { get; set; }
    public float Airspeed = 8;
    public float MaxSlopeAngle = 50; //{get;set;}
    public float aerialAcceleration = 0.2f;
    public float aerialDeceleration = 0.05f;
    public float maxAerialSpeed = 30;
    public float DIStrength = 2;
    public float maxFallSpeed = -15;
    public float fastFallSpeed = -20;
    public bool fastFall = false;
    public float WallslideSpeed = 5;

    [Space]

    public bool canFly;


    /* checks */
    public bool IsGrounded { get; protected set; }
    public bool WasGrounded { get; protected set; }
    public bool HasCollided { get; protected set; }
    public bool OnWall { get; protected set; }
    public bool OnWallTimed { get; protected set; }
    public bool OnLedge { get; protected set; }
    public bool IsJumping { get; protected set; }

    public int WallDirection { get; protected set; }
    public float CollisionAngle { get; protected set; }
    public Vector2 ReflectedVelocity { get; protected set; }


    //Can be set in SCS
    public bool IsInTumble { get; set; }
    public bool InControl { get; set; }
    public bool Frozen { get; set; }
    public bool ResetVelocity { get; set; }
    public bool FallThroughPlatforms { get; set; }

    public float JumpVelocity { get; set; }
    public Vector2 ForceMovement { get; set; }
    public Vector2 AddMovement { get; set; }

    //---local variables ---
    protected int freezeCounter = 0;

    protected float slopeUpAngle = 0f;
    protected float movingSlopeUpAngle = 0f;
    protected float slopeDownAngle = 0f;
    protected float movingSlopeDownAngle = 0f;

    private int onWallTimer = 0;

    protected EGroundMoveState moveState = EGroundMoveState.Idle;
    protected List<ColliderAndLayer> ignoredPlatforms;


    /* Masks used for collisions */
    [SerializeField]
    protected LayerMask collisionMask;

    [SerializeField]
    protected LayerMask platformMask;

    [SerializeField]
    protected LayerMask transporterMask;

    public LayerMask groundMask;


    /* initialization */
    protected void Start()
    {
        col = GetComponent<BoxCollider2D>();

        velocity = Vector2.zero;

        ignoredPlatforms = new List<ColliderAndLayer>();
    }

    /* Used to move the object in the objects FixedUpdate cycle */
    public virtual void UpdatePositionFixed()
    {
        bounds = col.bounds;
        bounds.Expand(-2 * skin);

        HasCollided = false;
        OnWall = false;
        OnLedge = false;
        IsGrounded = false;
        IsJumping = false;
        CollisionAngle = 0f;

        if (onWallTimer > 0)
        {
            OnWallTimed = true;
            onWallTimer--;
        }
        else
        {
            OnWallTimed = false;
        }

        SetGroundMask();
        CheckIfInsidePlatforms();

        GiantUpdate();

        ClearIgnoredPlatforms();

        transform.Translate(velocity);

        WasGrounded = IsGrounded;
        lastInputVelocity = InputVelocity;
    }

    public enum EGroundMoveState { None, Idle, Moving, SlopeUp, SlopeDown };

    protected void GiantUpdate()
    {
        if (Frozen)
        {
            FrozenUpdate();
            return;
        }

        if (ResetVelocity == true)
        {
            velocity = Vector2.zero;
        }

        //check if jumpvelocity was set, changes velocity and sets IsJumping
        CheckJumpVelocity();


        if (IsJumping == false)
        {
            //Sets IsGrounded
            CheckIfGrounded();
        }

        if (ChangedInputDirection())
        {
            moveState = EGroundMoveState.Idle;
        }



        if (IsGrounded == true)
        {
            if (InputVelocity.x == 0)
            {
                moveState = EGroundMoveState.Idle;
            }


            //grounded movement
            switch (moveState)
            {
                case EGroundMoveState.Idle:
                    {
                        ResetSlopeAngles();

                        velocity.y = 0;
                        velocity.x = 0;

                        ApplyAddMovementToVelocity();

                        if (InputVelocity.x != 0)
                        {
                            CheckForSlopeDown();

                            if (slopeDownAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeDownAngle <= MaxSlopeAngle)
                            {
                                movingSlopeDownAngle = slopeDownAngle;
                                moveState = EGroundMoveState.SlopeDown;
                            }
                        }

                        CheckForOnLedge();

                        break;
                    }
                case EGroundMoveState.Moving:
                    {
                        ResetSlopeAngles();

                        //normal sideways movement
                        velocity.y = 0;
                        velocity.x = InputVelocity.x * Movespeed / 60;

                        ApplyAddMovementToVelocity();

                        if (CheckForNewSlopeDown() == true)
                        {
                            //no collisioncheck
                        }
                        else
                        {
                            CheckForCollision();
                        }

                        if (CheckForSlopeUp() == true)
                        {
                            if (slopeUpAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeUpAngle <= MaxSlopeAngle)
                            {
                                movingSlopeUpAngle = slopeUpAngle;
                                moveState = EGroundMoveState.SlopeUp;
                            }
                            else
                            {
                                HasCollided = true;
                            }
                        }

                        CheckForOnLedge();


                        break;
                    }
                case EGroundMoveState.SlopeUp:
                    {
                        //moving slope up
                        velocity = new Vector2(Mathf.Sign(InputVelocity.x) * Mathf.Cos(Mathf.Deg2Rad * movingSlopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * movingSlopeUpAngle)).normalized / 60 * Movespeed;
                        velocity *= Mathf.Abs(InputVelocity.x);

                        ApplyAddMovementToVelocity();

                        if (CheckForNewSlopeUp() == true)
                        {
                            if (slopeUpAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeUpAngle <= MaxSlopeAngle)
                            {
                                movingSlopeUpAngle = slopeUpAngle;
                                moveState = EGroundMoveState.SlopeUp;
                            }
                            else
                            {
                                HasCollided = true;
                            }
                        }

                        if (CheckForSlopeDown() == true)
                        {
                            if (slopeDownAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeDownAngle <= MaxSlopeAngle)
                            {
                                movingSlopeDownAngle = slopeDownAngle;
                                moveState = EGroundMoveState.SlopeDown;
                            }
                        }

                        if (CheckForNewSlopeDown() == true)
                        {
                            //no collisioncheck
                        }
                        else
                        {
                            CheckForCollision();
                        }

                        CheckForOnLedge();

                        break;
                    }

                case EGroundMoveState.SlopeDown:
                    {
                        velocity = new Vector2(Mathf.Sign(InputVelocity.x) * Mathf.Cos(Mathf.Deg2Rad * movingSlopeDownAngle), -Mathf.Sin(Mathf.Deg2Rad * movingSlopeDownAngle)).normalized / 60 * Movespeed;
                        velocity *= Mathf.Abs(InputVelocity.x);

                        ApplyAddMovementToVelocity();

                        if (CheckForSlopeUp() == true)
                        {
                            if (slopeUpAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeUpAngle <= MaxSlopeAngle)
                            {
                                movingSlopeUpAngle = slopeUpAngle;
                                moveState = EGroundMoveState.SlopeUp;
                            }
                            else
                            {
                                HasCollided = true;
                            }
                        }

                        if (CheckForNewSlopeUp() == true)
                        {
                            if (slopeUpAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeUpAngle <= MaxSlopeAngle)
                            {
                                movingSlopeUpAngle = slopeUpAngle;
                                moveState = EGroundMoveState.SlopeUp;
                            }
                            else
                            {
                                HasCollided = true;
                            }
                        }

                        if (CheckForNewSlopeDown() == true)
                        {
                            if (slopeDownAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                        }
                        else
                        {
                            CheckForCollision();
                        }

                        CheckForOnLedge();

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            ApplyForceMovementToVelocity();

        }
        else //IsGrounded == false
        {
            //reset ground move state
            moveState = EGroundMoveState.Moving;

            if (IsInTumble == true)
            {
                //Tumble movement in air

                CalculateTumbleVelocityX();
                ApplyTumbleGravity();

                ApplyAddMovementToVelocity();
                ApplyForceMovementToVelocity();


                //check if landing on platform
                CheckForCollision();

                //no wallcheck, velocity can be reflected

                CheckForNewSlopeUp();
            }
            else //IsInTumble == false
            {
                //Normal movement in air
                CalculateAerialVelocityX();
                ApplyGravity();

                ApplyAddMovementToVelocity();
                ApplyForceMovementToVelocity();

                CheckForOnWall();

                //check if landing on platform
                CheckForCollision();

                CheckForNewSlopeUp();
            }
        }
    }

    protected void FrozenUpdate()
    {
        velocity = Vector2.zero;

        CheckForCollision();
    }

    public bool ChangedInputDirection()
    {
        bool retVal = false;

        if (InputVelocity.x != 0)
        {
            if (Mathf.Sign(InputVelocity.x) != Mathf.Sign(lastInputVelocity.x))
            {
                retVal = true;
            }
        }

        return retVal;
    }

    public void ApplyAddMovementToVelocity()
    {
        velocity += AddMovement / 60f;
        AddMovement = Vector2.zero;
    }

    public void ApplyForceMovementToVelocity()
    {
        if (ForceMovement != Vector2.zero)
        {
            velocity = ForceMovement / 60f;
            ForceMovement = Vector2.zero;

            CheckForCollision();
        }
    }

    public void ApplyGravity()
    {
        if (canFly == true)
        {
            // no gravity
        }
        else
        {
            /* slow down gravity before going down at a certain speed */
            if (velocity.y > 2 * Gravity / 60f)
            {
                velocity.y += Gravity / 60f * 0.75f;
            }
            else
            {
                velocity.y += Gravity / 60f;
            }

            if (velocity.y >= 0)
            {
                fastFall = false;
            }

            /* adjust fallspeed */
            if (fastFall)
            {
                velocity.y = fastFallSpeed / 60f;
            }
            else
            {
                if (velocity.y < maxFallSpeed / 60f)
                {
                    velocity.y = maxFallSpeed / 60f;
                }
            }
        }
    }

    public void ApplyTumbleGravity()
    {
        float velYAfterGravity = velocity.y + Gravity / 60f * 0.5f;

        //only apply gravity when slower than fastFallSpeed
        if (velYAfterGravity > fastFallSpeed)
        {
            velocity.y = velYAfterGravity;
        }
        else
        { //faster than fastfallspeed
            //increase or decrease fallspeed woth di;
            velocity.y += InputVelocity.y * DIStrength / 60 * aerialAcceleration;
        }
    }

    public void CheckJumpVelocity()
    {
        if (JumpVelocity != 0)
        {
            velocity.y = JumpVelocity / 60;
            JumpVelocity = 0;

            IsJumping = true;
        }
        else
        {
            IsJumping = false;
        }
    }

    public bool CheckForOnWall()
    {
        RaycastHit2D onWallCheck = RayCastXY(Vector2.right * Mathf.Sign(InputVelocity.x), Mathf.Abs(velocity.x) + skin, collisionMask);

        if (onWallCheck)
        {
            OnWall = true;
            OnWallTimed = true;
            onWallTimer = 3;
            WallDirection = (int)Mathf.Sign(InputVelocity.x);

            velocity.x = (onWallCheck.distance - skin) * Mathf.Sign(InputVelocity.x);

            if (velocity.y < -WallslideSpeed / 60f)
            {
                velocity.y = -WallslideSpeed / 60f;
            }
        }

        return onWallCheck;
    }

    public bool CheckForOnLedge()
    {
        RaycastHit2D ledgeCheck = RayCastLine(Vector2.down, 0.75f + skin, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(InputVelocity.x), -bounds.extents.y), groundMask);

        if (ledgeCheck)
        {
            OnLedge = false;
        }
        else
        {
            OnLedge = true;
        }

        return ledgeCheck;
    }

    public bool CheckForCollision()
    {
        LayerMask checkCollisionsLayer = collisionMask;
        if (IsInTumble == true && velocity.y < 0)
        {
            checkCollisionsLayer += platformMask;
        }

        RaycastHit2D collisionCheck = RayCastXY(velocity, velocity.magnitude + skin, checkCollisionsLayer);

        if (collisionCheck)
        {
            if (velocity != Vector2.zero)
            {
                ReflectedVelocity = Vector2.Reflect(velocity, collisionCheck.normal);
            }

            HasCollided = true;

            CollisionAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);

            velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
        }
        else
        {
            HasCollided = false;
        }

        return collisionCheck;
    }


    /// <summary>
    /// checks if the ctr is touching the ground by raycasting
    /// </summary>
    /// <returns> the RaycastHit2D if true and false otherwise </returns>
    public bool CheckIfGrounded()
    {

        // don't check the same frame that the ctr starts jumping 
        if (IsJumping)
        {
            return false;
        }

        // don't check if the ctr is in tumble and is getting launched upwards
        if (IsInTumble)
        {
            if (velocity.y > 0)
            {
                return false;
            }
        }

        // dont check if in the air and moving up
        if (WasGrounded == false)
        {
            if (velocity.y > 0)
            {
                return false;
            }
        }

        // cast a ctr-wide and skin high box at the bottom of the ctr
        RaycastHit2D groundCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2f),
            new Vector2(bounds.size.x, skin), 0, Vector2.down, skin * 2f, groundMask);

        if (groundCheck)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }

        return groundCheck;
    }

    public bool CheckForSlopeUp()
    {
        RaycastHit2D slopeUpCheck = RayCastLine(velocity, velocity.magnitude + skin, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(InputVelocity.x), -bounds.extents.y), groundMask);

        if (slopeUpCheck)
        {
            velocity = Vector2.ClampMagnitude(velocity, HitDistance(slopeUpCheck));

            slopeUpAngle = Vector2.Angle(Vector2.up, slopeUpCheck.normal);

            if (slopeUpAngle == 90)
            {
                OnWall = true;
            }
        }

        return slopeUpCheck;
    }

    public bool CheckForNewSlopeUp()
    {
        RaycastHit2D newSlopeUpCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2f),
           new Vector2(bounds.size.x, skin), 0, velocity, velocity.magnitude + skin, groundMask);

        if (newSlopeUpCheck)
        {
            velocity = Vector2.ClampMagnitude(velocity, HitDistance(newSlopeUpCheck));

            slopeUpAngle = Vector2.Angle(Vector2.up, newSlopeUpCheck.normal);
        }
        return newSlopeUpCheck;
    }

    public bool CheckForSlopeDown()
    {
        RaycastHit2D slopeDownCheck = RayCastLine(Vector2.down, skin * 2, (Vector2)bounds.center + new Vector2(-bounds.extents.x * Mathf.Sign(InputVelocity.x), -bounds.extents.y), groundMask);

        if (slopeDownCheck)
        {
            slopeDownAngle = Vector2.Angle(Vector2.up, slopeDownCheck.normal);
        }

        return slopeDownCheck;
    }

    public bool CheckForNewSlopeDown()
    {
        RaycastHit2D newSlopeDownCast = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y + skin / 2) + velocity,
                       new Vector2(bounds.size.x, skin), 0, Vector2.down, 4, groundMask);

        if (newSlopeDownCast)
        {

            if (newSlopeDownCast.distance > skin && newSlopeDownCast.distance < Mathf.Abs(velocity.x) * 2 + skin)
            {
                velocity.y += -(newSlopeDownCast.distance - skin);

                slopeDownAngle = Vector2.Angle(Vector2.up, newSlopeDownCast.normal);
            }
        }

        return newSlopeDownCast;
    }

    public RaycastHit2D RayCastXY(Vector2 direction, float distance, LayerMask layerMask)
    {
        float angle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        float skinDistance = skin;

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        if (cos > skin) skinDistance = skin / cos;

        return Physics2D.BoxCast(bounds.center, bounds.size, 0, direction, distance + skinDistance, layerMask);
    }

    public RaycastHit2D RayCastLine(Vector2 direction, float distance, Vector2 center, LayerMask layerMask)
    {
        float angle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        float skinDistance = skin;

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        if (cos > skin)
            skinDistance = skin / cos;

        return Physics2D.Raycast(center, direction, distance + skinDistance, layerMask);
    }

    public float HitDistance(RaycastHit2D collision)
    {
        Vector2 hitDirection;

        hitDirection = (Vector2)bounds.center - collision.point;
        hitDirection = new Vector2(Mathf.Sign(hitDirection.x), Mathf.Sign(hitDirection.y));

        float gamma = Mathf.Abs(90 - Vector2.Angle(collision.normal, hitDirection));
        float alpha = Mathf.Abs(90 - Vector2.Angle(collision.normal, velocity));

        float triangleDistance = skin;

        triangleDistance = skin * Mathf.Sqrt(2) * Mathf.Sin(gamma * Mathf.Deg2Rad) / Mathf.Sin(alpha * Mathf.Deg2Rad);

        float moveDistance = (collision.distance - triangleDistance);

        //to stop backwards movemenet
        if (moveDistance < 0) moveDistance = 0;

        return moveDistance;
    }

    public void SetGroundMask()
    {
        groundMask = collisionMask;

        if (FallThroughPlatforms == false)
        {
            groundMask += platformMask;
        }
    }

    public void ResetSlopeAngles()
    {
        slopeDownAngle = 0;
        slopeUpAngle = 0;
        movingSlopeDownAngle = 0;
        movingSlopeUpAngle = 0;
    }

    /* checks if the player is inside a platform and adds those to a list of ignored platforms */
    public void CheckIfInsidePlatforms()
    {
        ignoredPlatforms.Clear();

        /* repeatedly check for platforms and add them to the list of ignored platforms*/
        RaycastHit2D platformCheck = RayCastXY(Vector2.up, 0, platformMask);

        while (platformCheck == true)
        {
            ColliderAndLayer storeCollider = new ColliderAndLayer(platformCheck.collider, platformCheck.collider.gameObject.layer);
            ignoredPlatforms.Add(storeCollider);

            storeCollider.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            platformCheck = RayCastXY(Vector2.up, 0, platformMask);
        }
    }

    public void SetIgnoredPlatforms()
    {
        foreach (ColliderAndLayer storedCollider in ignoredPlatforms)
        {
            storedCollider.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    public void ClearIgnoredPlatforms()
    {
        foreach (ColliderAndLayer storedCollider in ignoredPlatforms)
        {
            storedCollider.collider.gameObject.layer = storedCollider.layerMask;
        }
    }

    public void CalculateAerialVelocityX()
    {
        //float dirX = Mathf.Sign(velocity.x);
        //velocity.x -= Airspeed / 60 * aerialDeceleration * Mathf.Sign(velocity.x);

        //if (dirX != Mathf.Sign(velocity.x))
        //{
        //    velocity.x = 0;
        //}

        //velocity.x += InputVelocity.x * Airspeed / 60 * aerialAcceleration;

        //velocity.x = Mathf.Clamp(velocity.x, -Airspeed / 60, Airspeed / 60);

        velocity.x = InputVelocity.x / 60;

        if (canFly == true)
        {
            //velocity.y -= Gravity / 60f;

            velocity.y -= Airspeed / 60 * aerialDeceleration * Mathf.Sign(velocity.y);

            velocity.y += InputVelocity.y * Airspeed / 60 * aerialAcceleration;

            velocity.y = Mathf.Clamp(velocity.y, -Airspeed / 60, Airspeed / 60);
        }
    }

    public void CalculateTumbleVelocityX()
    {
        float dirX = Mathf.Sign(velocity.x);

        //natural deceleration
        velocity.x -= 1 / 60 * aerialDeceleration * Mathf.Sign(velocity.x);

        //stop wehn changing directions
        if (dirX != Mathf.Sign(velocity.x))
        {
            velocity.x = 0;
        }


        //allow DI away only if slower than maxairspeed
        float velXAfterDI = velocity.x + InputVelocity.x * DIStrength / 60 * aerialAcceleration;

        if (Mathf.Abs(velXAfterDI) <= maxAerialSpeed)
        {
            velocity.x = velXAfterDI;
        }

        //allow vectoring away only if slower than maxairspeed
        if (velocity.y > 0)
        {
            Vector2 velAfterVectoring = velocity.normalized * (velocity.magnitude + InputVelocity.y * DIStrength / 60 * aerialAcceleration);

            if (Mathf.Abs(velAfterVectoring.x) <= maxAerialSpeed)
            {
                velocity.x = velAfterVectoring.x;
            }

            velocity.y = velAfterVectoring.y;
        }
    }


    [System.Serializable]
    public struct ColliderAndLayer
    {
        public Collider2D collider;
        public LayerMask layerMask;

        public ColliderAndLayer(Collider2D collider, LayerMask layerMask)
        {
            this.collider = collider;
            this.layerMask = layerMask;
        }
    }
}
