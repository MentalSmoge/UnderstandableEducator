using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3[] Direction = new Vector3[] { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    public Vector3 CurrentDirection = Vector3.up;
    bool Wall_Front = false;
    bool Wall_Behind = false;
    public bool WallInFront
	{
        get
		{
            Wall_Front = CheckForWall();
            return Wall_Front;
		}
	}
    public GameObject WhoIsItInFront
	{
        get
		{
            return CheckForAnything();
		}
	}
    public GameObject WhoIsItBehind
    {
        get
        {
            return CheckForAnythingBehind();
        }
    }
    public bool WallBehind
    {
        get
        {
            Wall_Behind = CheckForBehindWall();
            return Wall_Behind;
        }
    }
    
    private GameObject CheckForAnything()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, CurrentDirection, 1);
        if (raycastHit)
        {
            return raycastHit.transform.gameObject;
        }
        return null;
    }
    private GameObject CheckForAnythingBehind()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, GetBehindDirection(CurrentDirection), 1);
        if (raycastHit)
        {
            return raycastHit.transform.gameObject;
        }
        return null;
    }
    private bool CheckForWall()
    {
        LayerMask mask = LayerMask.GetMask("Wall");
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, CurrentDirection, 1, mask);
        if (raycastHit)
		{
            if (raycastHit.transform.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
    private bool CheckForBehindWall()
    {
        LayerMask mask = LayerMask.GetMask("Wall");
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, GetBehindDirection(CurrentDirection), 1, mask);
        if (raycastHit)
        {
            if (raycastHit.transform.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
    private Vector3 GetNextDirection(Vector3 origDir, bool turningRight)
    {
        if (turningRight)
        {
            int i = Array.IndexOf(Direction, origDir) + 1;
            if (i >= Direction.Length)
            {
                i = 0;
            }
            return Direction[i];
        }
        else
        {
            int i = Array.IndexOf(Direction, origDir) - 1;
            if (i < 0)
            {
                i = Direction.Length - 1;
            }
            return Direction[i];
        }
    }
    private Vector3 GetBehindDirection(Vector3 origDir)
    {
        int i = Array.IndexOf(Direction, origDir) + 2;
        if (i >= Direction.Length)
        {
            i = i - 4;
        }
        if (i < 0 || i > 3)
		{
            Debug.LogError("Задняя стенка багует");
		}
        return Direction[i];
    }
    public IEnumerator RotatePlayer(int angle, float timeToMove)
	{
        if (angle > 0)
        {
            CurrentDirection = GetNextDirection(CurrentDirection, false);
        }
        else
        {
            CurrentDirection = GetNextDirection(CurrentDirection, true);
        }
        float elapsedTime = 0f;
        Vector3 origRotation = transform.eulerAngles;
        Vector3 targetRotation = origRotation + (new Vector3(0,0,angle));
        while (elapsedTime < timeToMove)
        {
            float zRotation = Mathf.Lerp(origRotation.z, targetRotation.z, elapsedTime / timeToMove) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = targetRotation;
    }
    //public IEnumerator MovePlayer(bool forward, float timeToMove)
    //{
    //    float elapsedTime = 0f;
    //    Vector3 targetPosition;
    //    Vector3 origPosition = transform.position;
    //    if (forward)
    //        targetPosition = origPosition + CurrentDirection;
    //    else
    //        targetPosition = origPosition - CurrentDirection;
    //    while (elapsedTime < timeToMove)
    //    {
    //        transform.position = Vector3.Lerp(origPosition, targetPosition, (elapsedTime / timeToMove));
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    transform.position = targetPosition;
    //}
    public IEnumerator MovePlayer(bool forward, float timeToMove)
    {
        float elapsedTime = 0f;
        Vector3 targetPosition;
        Vector3 origPosition = transform.position;
        if (forward)
            targetPosition = origPosition + CurrentDirection;
        else
            targetPosition = origPosition - CurrentDirection;
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPosition, targetPosition, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
