using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Frank : CharacterFoundation
{
    private bool canAirJump;

    public override void OnGroundState()
    {
        canAirJump = true;
        base.OnGroundState();
    }
    public override void InAirState()
    {
        if (canAirJump && jump)
        {
            rb.gravityScale=fallSpeed;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            Jump();
            canAirJump = false;
        }
        base.InAirState();
    }

    protected override void GroundedDash(Vector2 dashVelocity)
    {
            if (movementDirection.x < 0)
            {
                rb.velocity = new Vector2((dashDistance.x+moveSpeed) *-1.3f, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2((dashDistance.x+moveSpeed) *1.3f, rb.velocity.y);
            }
    }
}
