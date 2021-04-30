using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("VectorSpace Method")]
    private Vector3 tempVector;

    [Header("Sliding")]
    public bool slideReady;

    [Header("Dashing")]
    private float dashLerpCounter;
    public bool isDashing;

    [Header("References")]
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private Vector2 VectorSpace(Vector2 directionVector)
    {
        tempVector = new Vector3(directionVector.x, 0.0f, directionVector.y);
        tempVector = transform.TransformDirection(tempVector);
        tempVector.Normalize();
        return new Vector2(tempVector.x, tempVector.z);
    }
    public void ApplyGravity(float amount)
    {
        rb.AddForce(Physics.gravity * (rb.mass * rb.mass) * amount);
    }
    public void ApplyFriction(float friction)
    {
        rb.AddForce(new Vector3(rb.velocity.x * -friction, 0, rb.velocity.z * -friction));
    }
    public void ApplyVerticalFriction(float friction)
    {
        rb.AddForce(new Vector3(0, rb.velocity.y * -friction, 0));
    }
    public void GroundedJump(float force)
    {
        rb.AddForce(new Vector3(0, force * 10, 0));
    }
    public void AirborneJump(float force, float dirForce, Vector3 dir)
    {
        rb.velocity = new Vector3(rb.velocity.x/4, 0.0f, rb.velocity.z/4);
        rb.AddForce(new Vector3(0, force * 10, 0) + dir * dirForce * 10);
    }
    public void WallJump(float upForce, float range, float wallPush, Vector3 dir, Vector3 wallDir)
    {
        rb.AddForce(dir * range * 10 + new Vector3(0, upForce * 10, 0) + wallDir * wallPush * 10);
    }
    public void SlideJump(float force, Vector3 dir)
    {
        rb.AddForce(new Vector3(0, force * 10, 0) + dir * 10);
    }
    public void Walk(Vector2 moveDir, float walkSpeed, float walkSpeedIncrease, ref float rampUpCounter, float rampUpTime)
    {
        if (rampUpCounter < rampUpTime)
        {
            rampUpCounter += 0.05f;
            if (rampUpCounter > rampUpTime) { rampUpCounter = rampUpTime; } //prevent overshoot
        }

        moveDir = VectorSpace(moveDir);
        
        rb.AddForce(new Vector3(moveDir.x * (walkSpeed + (walkSpeedIncrease * (rampUpCounter / rampUpTime))), 0, moveDir.y * (walkSpeed + (walkSpeedIncrease * (rampUpCounter / rampUpTime)))));
    }
    public void Slide(float strenght, bool grounded)
    {

        if (slideReady)
        {
            if (grounded) { rb.AddForce(new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized * strenght * 100); }
            else { rb.AddForce(new Vector3(rb.velocity.x, -0.8f, rb.velocity.z).normalized * strenght * 100); }
        }
        slideReady = false;
    }
    public void Wallrun(bool rightSide, float dir, float speed, Vector3 normal)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (rightSide)
        {
            rb.AddForce(Vector3.Cross(Vector3.up, normal) * dir * speed);
        }
        else
        {
            rb.AddForce(Vector3.Cross(normal, Vector3.up) * dir * speed);
        }
    }
    public void AirControl(Vector2 moveDir, float airSpeed)
    {
        moveDir = VectorSpace(moveDir);
        rb.AddForce(new Vector3(moveDir.x * airSpeed, 0, moveDir.y * airSpeed));
    }
    public void Climb(Vector2 moveDir, float climbSpeed)
    {
        rb.AddForce(Vector3.up * climbSpeed * moveDir.y);
    }
    public void Vault(Vector3 targetPos)
    {
        transform.position = targetPos;
    }
    public void Dashhold(Vector2 dir, float speed)
    {
        
    }
    public IEnumerator Dash(float dashSpeed, Vector3 currPos, Vector3 targetPos)
    {
        rb.velocity = Vector3.zero;
        isDashing = true;
        while (dashLerpCounter <= dashSpeed)
        {
            transform.position = Vector3.Slerp(
                currPos,
                targetPos,
                (dashLerpCounter / dashSpeed));
            dashLerpCounter += Time.deltaTime;

            yield return null;
        }
        transform.position = targetPos;
        dashLerpCounter = 0;
        rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        rb.AddForce((targetPos - currPos).normalized * 400);
        isDashing = false;
        yield return null;
    }
}
