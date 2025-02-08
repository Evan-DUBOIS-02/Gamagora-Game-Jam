using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]

    //Variables Colliders
    public PlayerMovementStats moveStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    private Rigidbody2D _rb;

    //Variable Movement
    private Vector2 _moveVelocity;
    private bool _isFacingRight;

    //Variable collision check
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _isGrounded;
    private bool _bumpedHead;

    //Variable Jump
    public float verticalVelocity {  get; private set; }
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    //Variable Apex
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    //Variable Jump Buffer
    private float _jumpBufferTimer;
    private bool _jumpReleaseDuringBuffer;

    //Variable Coyote time
    private float _coyoteTimer;

    private void Awake()
    {
        _isFacingRight = true;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CountTimers();
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if(_isGrounded)
        {
            Move(moveStats.groundAcceleration, moveStats.groundDecceleration, InputManager.movement);
        }
        else
        {
            Move(moveStats.airAcceleration, moveStats.airDecceleration, InputManager.movement);
        }
    }

    private void OnDrawGizmos()
    {
        if(moveStats.showWalkJumpArc)
        {
            DrawJumpArc(moveStats.maxWalkSpeed, Color.white);
        }

        if(moveStats.showRunJumpArc)
        {
            DrawJumpArc(moveStats.maxRunSpeed, Color.red);
        }
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.runIsHeld)
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * moveStats.maxRunSpeed;
            }
            else
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * moveStats.maxWalkSpeed;
            }

            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }

        else if (moveInput == Vector2.zero) 
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if(_isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if(!_isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if(turnRight)
        {
            _isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            _isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    #endregion

    #region Jump
    
    private void JumpChecks()
    {
        //WHEN WE PRESS JUMP BUTTON
        if(InputManager.jumpWasPressed)
        {
            _jumpBufferTimer = moveStats.jumpBufferTime;
            _jumpReleaseDuringBuffer = false;
        }

        //WHEN WE RELEASE JUMP BUTTON
        if(InputManager.jumpWasReleased)
        {
            if(_jumpBufferTimer > 0)
            {
                _jumpReleaseDuringBuffer = true;
            }

            if(_isJumping && verticalVelocity > 0f)
            {
                if(_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = moveStats.timeForUpwardsCancel;
                    verticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = verticalVelocity;
                }
            }
        }
        //ACTUAL JUMP WITH JUMP BUFFERING AND COYOTE TIME
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);

            //A verif
            if (_jumpReleaseDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = verticalVelocity;
            }
        }

        //ACTUAL JUMP WITH DOUBLE JUMP (RENTRE JAMAIS DEDANS CAR JUMP MAX AUTORISE = 1)
        else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < moveStats.numberOfJumpsAllowed)
        {
            _isFastFalling = false;
            InitiateJump(1);
        }

        //HANDLE AIR JUMP AFTER COYOTE TIME LAPSED (RENTRE JAMAIS DEDANS CAR JUMP MAX AUTORISE = 1) (enleve un extra jump pour pas en avoir en bonus)
        else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < moveStats.numberOfJumpsAllowed -1)
        {
            InitiateJump(2); //2 car saut en l'air pour ne pas qu'il lui reste des jumps
            _isFastFalling = false;
        }

        //LANDED
        if((_isJumping || _isFalling) && _isGrounded && verticalVelocity <= 0f)
        {
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _fastFallTime = 0f;
            _isPastApexThreshold = false;
            _numberOfJumpsUsed = 0;

            verticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int numberOfJumpsUsed)
    {
        if(!_isJumping)
        {
            _isJumping = true;
        }

        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberOfJumpsUsed;
        verticalVelocity = moveStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        //APPLY GRAVITY WHILE JUMPING 
        if( _isJumping )
        {
            //CHECK FOR HEAD BUMP
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }
            
            //GRAVITY ON ASCENDING
            if(verticalVelocity >= 0f)
            {
                //APEX CONTROLS
                _apexPoint = Mathf.InverseLerp(moveStats.InitialJumpVelocity, 0f, verticalVelocity);

                if(_apexPoint > moveStats.apexThreshold)
                {
                    if(!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if(_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;

                        if(_timePastApexThreshold < moveStats.apexHangTime)
                        {
                            verticalVelocity = 0f;
                        }
                        else
                        {
                            verticalVelocity -= 0.01f;
                        }
                    }
                }

                //GRAVITY ON ASCENDING BUT NOT PAST APEX THRESHOLD
                else
                {
                    verticalVelocity += moveStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;

                    }
                }
            }

            //GRAVITY ON DESCENDING
            else if (!_isFastFalling)
            {
                verticalVelocity += moveStats.Gravity * moveStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if(verticalVelocity < 0f)
            {
                if(!_isFalling)
                {
                    _isFalling = true;
                }
            }
        }

        //JUMP CUT
        if(_isFastFalling)
        {
            if(_fastFallTime >= moveStats.timeForUpwardsCancel)
            {
                verticalVelocity += moveStats.Gravity * moveStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if(_fastFallTime < moveStats.timeForUpwardsCancel)
            {
                verticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / moveStats.timeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }

        //NORMAL GRAVITY WHILE FALLING
        if(!_isGrounded && !_isJumping)
        {
            if(!_isFalling)
            {
                _isFalling = true;
            }

            verticalVelocity += moveStats.Gravity * Time.fixedDeltaTime;
        }

        //CLAMP FALL SPEED
        verticalVelocity = Mathf.Clamp(verticalVelocity, -moveStats.maxFallSpeed, 50f);

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, verticalVelocity);

    }
    
    #endregion

    #region Collision Checks

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x * moveStats.headWidth, moveStats.headDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, moveStats.headDetectionRayLength, moveStats.GroundLayer);
        if(_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else
        {
            _bumpedHead = false;
        }

        #region Debug Visualization
        if(moveStats.debugShowHeadBumpBox)
        {
            float headWidth = moveStats.headWidth;

            Color rayColor;
            if(_bumpedHead)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * moveStats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * moveStats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + moveStats.headDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }
        #endregion
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, moveStats.groundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, moveStats.groundDetectionRayLength, moveStats.GroundLayer);
        if(_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        #region Debug Visualization
        if(moveStats.debugShowIsGroundedBox)
        {
            Color raycolor;
            if(_isGrounded)
            {
                raycolor = Color.green;
            }
            else
            {
                raycolor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveStats.groundDetectionRayLength, raycolor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveStats.groundDetectionRayLength, raycolor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - moveStats.groundDetectionRayLength), Vector2.right * boxCastSize.x, raycolor);
        }

        #endregion
    }


    private void CollisionChecks()
    {
        IsGrounded();
    }
    #endregion

    #region Timers

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;

        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }

        else
        {
            _coyoteTimer = moveStats.jumpCoyoteTime;
        }
    }

    #endregion

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 previousPosition = startPosition;

        float speed = 0f;
        if(moveStats.drawRight)
        {
            speed = moveSpeed;
        }
        else
        {
            speed = -moveSpeed;
        }

        Vector2 velocity = new Vector2(speed, moveStats.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * moveStats.timeTillJumpApex / moveStats.arcResolution; //time step for the simulation
        //float totaltime = (2 * moveStats.timeTillJumpApex) + moveStats.apexHangTime; //total time of the arc including hang time

        for (int i = 0; i < moveStats.visualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if(simulationTime < moveStats.timeTillJumpApex) //ASCENDING
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, moveStats.Gravity) * simulationTime * simulationTime;
            }

            else if (simulationTime < moveStats.timeTillJumpApex + moveStats.apexHangTime) //Apex hang time
            {
                float apexTime = simulationTime - moveStats.timeTillJumpApex;
                displacement = velocity * moveStats.timeTillJumpApex + 0.5f * new Vector2(0, moveStats.Gravity) * moveStats.timeTillJumpApex * moveStats.timeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime; //No vertical movement during hang time
            }
            else //Descending
            {
                float descendTime = simulationTime - (moveStats.timeTillJumpApex + moveStats.apexHangTime);
                displacement = velocity * moveStats.timeTillJumpApex + 0.5f * new Vector2(0, moveStats.Gravity) * moveStats.timeTillJumpApex * moveStats.timeTillJumpApex;
                displacement += new Vector2(speed, 0) * moveStats.apexHangTime; //Horizontal movement during hang time
                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, moveStats.Gravity) * descendTime * descendTime;
            }

            drawPoint = startPosition + displacement;

            if (moveStats.stopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), moveStats.GroundLayer);
                if (hit.collider != null)
                {
                    //If a hit is detected, stop drawing the arc at the hit point
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }
}
