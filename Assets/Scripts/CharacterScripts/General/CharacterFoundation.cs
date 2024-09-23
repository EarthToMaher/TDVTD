using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEditor.Rendering;
using UnityEngine.TextCore.Text;
using System;

/*
This script is the foundation for all the characters in the game. Changes to this script will affect all characters.
This script should NOT be put on any object, rather it should be something each individual character script inherits from.
*/
public class CharacterFoundation : CharacterStateController
{

    private int hp = 3;
    public enum WeaponTypes
    {
        Melee,
        Ranged,
        Other
    }
    [SerializeField] public WeaponTypes weaponType;
    public Weapon weapon;
    protected CharacterInputController characterInput;
    public Vector2 aimDirection;
    public Rigidbody2D rb;
    #region Weapon Variables
        [SerializeField] public GameObject weaponObject;
        [SerializeField] public float weaponStartLag;
        [SerializeField] public float weaponEndLag;
        [SerializeField] public bool usePrefabDefaults;
        #region Melee Variables
            [SerializeField] public Collider2D weaponHitbox;
            [SerializeField] public float weaponLifespan;
        #endregion
        #region Ranged Variables
            [SerializeField] public GameObject bulletPrefab;
            [SerializeField] public Transform bulletSpawnPoint;
            [SerializeField] public float bulletLifespan;
            [SerializeField] public float bulletSpeed;
            [SerializeField] public float pierceValue;
            [SerializeField] public bool ignoreWalls;
        #endregion
    #endregion
    #region PlayerInput
        public Vector2 movementDirection;
        public bool dash = false;
        public bool attack = false;
        public bool active = false;
        public bool parry = false;
        public bool jump = false;
    #endregion
    #region PlayerMovementVariables
        private float currentSpeed = 0f;
        public float friction = 1f;
        public float inertia = 1f;
        [SerializeField]public float moveSpeed;
        [SerializeField]public Vector2 dashDistance;
        [SerializeField]public float jumpSpeed;
        [SerializeField]public float fallSpeed;
        [SerializeField]public float dashEndLag;
        [SerializeField]public float gravityScaling;
        [SerializeField] public float maxFallSpeed;
    #endregion
    #region Cooldowns
        public bool dashOnCooldown = false;
        [SerializeField] public float dashCooldown = 0.3f;
        public bool activeOnCooldown = false;
        [SerializeField] public float activeCooldown = 10f;
        public bool attackOnCooldown = false;
        [SerializeField] public float attackCooldown = 1f;
        public bool jumpOnCooldown = false;
        public float jumpCooldown = 0.3f;
        public bool invincibile = false;
        public float invincibilityLength = 1.5f;
        public bool parrying = false;
        public float parryLength = 1f;
    #endregion
    #region Animations
        [SerializeField] public AnimationClip animIdle;
        [SerializeField] public AnimationClip animAirborne;
        [SerializeField] public AnimationClip animGroundedDash;
        [SerializeField] public AnimationClip animAirDash;
        [SerializeField] public AnimationClip animRunning;
        [SerializeField] public AnimationClip animParry;
        [SerializeField] public AnimationClip animActive;
        [SerializeField] public AnimationClip animTurn;
        [SerializeField] public AnimationClip animDead;
    #endregion
    public virtual void Start()
    {
        TransitionToState(entryState);
        characterInput = this.gameObject.GetComponent<CharacterInputController>();
        weapon = this.GetComponentInChildren<Weapon>();
        weaponObject = Instantiate(weaponObject, this.gameObject.transform);
        weapon = weaponObject.GetComponent<Weapon>();
        rb = this.GetComponent<Rigidbody2D>();
        rb.gravityScale = fallSpeed;
        characterInput = GetComponent<CharacterInputController>();
    }
    public virtual void Update()
    {
        Debug.Log(currentState);
        GetInput();
        try
        {
            currentState.Update(this);  
        }
        catch (NullReferenceException)
        {
            Debug.Log("State Missing, setting to grounded");
            TransitionToState(groundedState);
        }
        if (rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        }
    }
#region Custom Functions
    protected virtual void GetInput()
    {
        movementDirection = characterInput.moveInput;
        dash = characterInput.dashInput;
        attack = characterInput.attackInput;
        active = characterInput.activeInput;
        parry = characterInput.parryInput;
        jump = characterInput.jumpInput;
        aimDirection = characterInput.aimInput;
    }

    public virtual void UpdateCollider()
    {
        Destroy(gameObject.GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }

    public virtual void MovePlayer()
    {
        Debug.Log("moving");
        float desiredMove = movementDirection.x * moveSpeed;
        if ((currentSpeed <= 0 && desiredMove <= currentSpeed)||(currentSpeed>= 0 && desiredMove >=currentSpeed))
        {
            transform.position += transform.right * desiredMove  * Time.deltaTime;
            currentSpeed = desiredMove;
        }
        else
        {
            float newSpeed = currentSpeed + (desiredMove/inertia);
            if (Mathf.Abs(newSpeed)>moveSpeed)
            {
                if (newSpeed < 0)
                {
                    newSpeed = -moveSpeed;
                }
                else
                {
                    newSpeed = moveSpeed;
                }
            }
            transform.position += transform.right * newSpeed  * Time.deltaTime;
            if (newSpeed < 0)
            {
                if (newSpeed + friction > 0)
                {
                    newSpeed = 0;
                }
                else
                {
                    newSpeed += friction;
                }
            }
            else if (newSpeed > 0)
            {
                if (newSpeed - friction < 0)
                {
                    newSpeed = 0;
                }
                else
                {
                    newSpeed -= friction;
                }
            }
            currentSpeed = newSpeed;
        }
    }

    public virtual void Jump()
    {
        rb.AddForce(transform.up * jumpSpeed, ForceMode2D.Impulse);
        TransitionToState(airborneState);
    }

    public virtual IEnumerator Parry()
    {
        bool parrying = true;
        TransitionToState(parryState);
        StartCoroutine(Invincibility(parrying));
        yield return new WaitForSeconds(parryLength);
        parrying = false;
    }

    public void ReduceHP(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            StopAllCoroutines();
            TransitionToState(deadState);
        }
    }

    public virtual void AimWeapon()
    {
        if(aimDirection!=Vector2.zero && movementDirection == Vector2.zero)
        {
            //Vector3 weaponScale = weaponObject.transform.localScale;
            if(aimDirection.x < 0)
            {
                weaponObject.transform.localScale = new Vector3 (-1f, 1f, 1f);
                this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                weaponObject.transform.eulerAngles = new Vector3(0f, 0f, (aimDirection.y * -90f));
            }
            else if (aimDirection.x > 0)
            {
                weaponObject.transform.localScale = new Vector3 (1f, 1f, 1f);
                this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                weaponObject.transform.eulerAngles = new Vector3(0f, 0f, (aimDirection.y * 90f));
            }
        }
        else if (aimDirection != Vector2.zero && movementDirection != Vector2.zero)
        {
            if(aimDirection.x < 0)
            {
                weaponObject.transform.localScale = new Vector3 (-1f, 1f, 1f);
                weaponObject.transform.eulerAngles = new Vector3(0f, 0f, (aimDirection.y * -90f));
                this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (aimDirection.x > 0)
            {
                weaponObject.transform.localScale = new Vector3 (1f, 1f, 1f);
                weaponObject.transform.eulerAngles = new Vector3(0f, 0f, (aimDirection.y * 90f));
                this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        else if (aimDirection == Vector2.zero && movementDirection != Vector2.zero)
        {
            if (movementDirection.x < 0)
            {
                weaponObject.transform.localScale = new Vector3 (-1f, 1f, 1f);
                this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if  (movementDirection.x > 0)
            {
                weaponObject.transform.localScale = new Vector3 (1f, 1f, 1f);
                this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            weaponObject.transform.eulerAngles = Vector3.zero;
        }
    }

    public virtual IEnumerator Dash()
    {
        rb.gravityScale = fallSpeed;
        TransitionToState(dashState);
        Vector2 dashVelocity = rb.velocity;
        if      (movementDirection.x<0)   dashVelocity.x = (dashDistance.x+moveSpeed) *-1f;
        else if (movementDirection.x>0)   dashVelocity.x = dashDistance.x+moveSpeed;
        if      (movementDirection.y<0)   dashVelocity.y = dashDistance.y * -1f;
        else if (movementDirection.y>0)   dashVelocity.y = dashDistance.y;
        if (currentState == groundedState && dashVelocity.y <= 0 && dashVelocity.x != 0)
        {
            GroundedDash(dashVelocity);
        }
        else
        {
            AirDash(dashVelocity);
        }
        yield return new WaitForSeconds(dashEndLag);
        rb.velocity = new Vector2(0f, rb.velocity.y);
        if (rb.velocity.y == 0)
        {
            TransitionToState(groundedState);
            yield return new WaitForSeconds(dashCooldown-dashEndLag);
            dashOnCooldown = false;
        }
        else
        {
            TransitionToState(airborneState);
            yield return new WaitForSeconds(dashCooldown-dashEndLag);
            yield return new WaitUntil(() => rb.velocity.y == 0);
            dashOnCooldown = false;
        }
    }

    protected virtual void GroundedDash(Vector2 dashVelocity)
    {
        DashMovement(dashVelocity);
    }

    protected virtual void AirDash(Vector2 dashVelocity)
    {
        DashMovement(dashVelocity);
    }

    private void DashMovement(Vector2 dashVelocity)
    {
        rb.velocity = dashVelocity;
        dashOnCooldown = true;
    }

    public virtual IEnumerator Invincibility(float invincibilityLength)
    {
        invincibile = true;
        yield return new WaitForSeconds(invincibilityLength);
        invincibile = false;
    }
    public virtual IEnumerator Invincibility(bool isInvincible)
    {
        invincibile = true;
        yield return new WaitUntil(()=>!isInvincible);
        invincibile = false;
    }

    public virtual IEnumerator Stunned(float stunLength)
    {
        TransitionToState(stunState);
        yield return new WaitForSeconds(stunLength);
        if (rb.velocity.y == 0)
        {
            TransitionToState(groundedState);
        }
        else
        {
            TransitionToState(airborneState);
        }
    }

    public virtual void TakeDamage(float stunLength)
    {
        this.StopAllCoroutines();
        StartCoroutine(Invincibility(invincibilityLength));
        StartCoroutine(Stunned(stunLength));
    }
    public void TransitionToState(CharacterBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
    
    public void Dead()
    {
        Destroy(this);
    }

    public virtual void InAirState()
    {
        MovePlayer();
        AimWeapon();
        if (dash && !dashOnCooldown)
        {
            StartCoroutine(Dash());
            return;
        }
        else if (rb.velocity.y == 0f)
        {
            TransitionToState(groundedState);
            return;
        }
        else
        {
            rb.gravityScale += gravityScaling;
        }
    }

    public virtual void OnGroundState()
    {
        MovePlayer();
        AimWeapon();
        if (jump)
        {
            Jump();
            return;
        }
        if (dash && !dashOnCooldown)
        {
            StartCoroutine(Dash());
            return;
        }
        if (rb.velocity.y < -0.05f || rb.velocity.y > 0.05f)
        {
            TransitionToState(airborneState);
            return;
        }
    }
#endregion
}
#region State Machine
public class CharacterStateController : MonoBehaviour
{
    #region State Variables
        public CharacterBaseState currentState;
        public readonly EntryState entryState = new EntryState();
        public readonly GroundedState groundedState = new GroundedState();
        public readonly AirborneState airborneState = new AirborneState();
        public readonly DashState dashState = new DashState();
        public readonly AttackState attackState = new AttackState();
        public readonly StunState stunState = new StunState();
        public readonly ParryState parryState = new ParryState();
        public readonly DeadState deadState = new DeadState();
    #endregion
}
#region States
public abstract class CharacterBaseState
{
    public abstract void Update(CharacterFoundation character);
    public abstract void EnterState(CharacterFoundation character);
}

public class EntryState : CharacterBaseState
{
    public override void EnterState(CharacterFoundation character)
    {
        character.StartCoroutine(WaitToStart(1f, character));
    }
    public override void Update(CharacterFoundation character){} //NOTHING EVER HAPPENS

    public IEnumerator WaitToStart(float time, CharacterFoundation character)
    {
        yield return new WaitForSeconds(time);
        character.TransitionToState(character.groundedState);
    }
}
public class GroundedState : CharacterBaseState
{
    public override void EnterState(CharacterFoundation character)
    {
        //Change the animation that is playing
        character.rb.gravityScale = character.fallSpeed;
        character.UpdateCollider();
    }
    public override void Update(CharacterFoundation character)
    {
        character.OnGroundState();
    }
}
public class AirborneState : CharacterBaseState
{
    public override void EnterState(CharacterFoundation character)
    {
        //Change the animation that is playing
        character.UpdateCollider();
    }
    
    public override void Update(CharacterFoundation character)
    {
        character.InAirState();
    }
}
public class DashState : CharacterBaseState
{
    public override void EnterState(CharacterFoundation character)
    {
        character.UpdateCollider();
    }
    public override void Update(CharacterFoundation character)
    {
        //character.MovePlayer();
    }
}

public class StunState : CharacterBaseState
{
    public override void EnterState(CharacterFoundation character)
    {
        character.UpdateCollider();
    }
    public override void Update(CharacterFoundation character)
    {
        //is literally unable to act because they are stunned duh
    }
}

public class AttackState : CharacterBaseState
{
    public override void EnterState(CharacterFoundation character)
    {
        character.UpdateCollider();
    }
    public override void Update(CharacterFoundation character)
    {
        
    }
}
public class ParryState : CharacterBaseState
{
    public override void EnterState(CharacterFoundation character)
    {
        character.UpdateCollider();
    }
    public override void Update(CharacterFoundation character)
    {
        
    }
}
public class DeadState : CharacterBaseState
{
    public override void EnterState(CharacterFoundation character)
    {
        character.Dead();
    }
    public override void Update(CharacterFoundation character)
    {
        
    }
}
#endregion
#endregion