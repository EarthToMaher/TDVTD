using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Rattles : CharacterFoundation
{
    protected override void GroundedDash(Vector2 dashVelocity)
    {
            weaponObject.transform.eulerAngles = new Vector3(0f,0f,0f);
            if (dashVelocity.x < 0)
            {
                weaponObject.transform.localScale = new Vector3(-1f, 1f, 1f);
                rb.velocity = new Vector2((dashDistance.x+moveSpeed) *-1.15f, rb.velocity.y);
            }
            else
            {
                weaponObject.transform.localScale = new Vector3(1f, 1f, 1f);
                rb.velocity = new Vector2((dashDistance.x+moveSpeed) *1.15f, rb.velocity.y);
            }
    }
}
