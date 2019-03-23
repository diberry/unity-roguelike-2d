using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// must be finished off in derived class
public abstract class MovingObject : MonoBehaviour
{
    // time to move in seconds
    public float moveTime = 0.1f;

    // check collision as we are moving to check 
    // space to move into
    public LayerMask blockingLayer;
        
    private BoxCollider2D boxCollider;

    // unit we are moving 
    private Rigidbody2D rb2d;

    // make movement calcs more effecient
    private float inverseMoveTime;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime; // use multiple instead of div - more efficient computationally
    }

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        // discard z axis data
        Vector2 start = transform.position;

        // calc end position based on direction params
        Vector2 end = start + new Vector2(xDir, yDir);

        // disable attached boxCollider - won't hit our own collider
        boxCollider.enabled = false;

        // cast lint from start to end, checking collision on blocking layer
        hit = Physics2D.Linecast(start, end, blockingLayer);

        // re-enable box collider
        boxCollider.enabled = true;

        // check if anything was hit
        // space we are moving into is open and available
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));

            // move was successful
            return true;
        }

        // move was unsuccessful
        return false;
    }

    /// <summary>
    /// Both player and enemy with inherit from this object so need to use generics to represent that. 
    /// </summary>
    /// <typeparam name="T">Type of component we expect to interact with if blocked. For enemy, this is player. For player, this is walls, can attack and destroy walls. </typeparam>
    /// <param name="xDir"></param>
    /// <param name="yDir"></param>
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T: Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        // nothing was hit
        if (hit.transform == null)
            return;

        // something was hit - get object that was hit
        T hitComponent = hit.transform.GetComponent<T>();

        // object is blocked - hit something it can interact with
        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistrance = (transform.position - end).sqrMagnitude;

        while(sqrRemainingDistrance > float.Epsilon)
        {
            // move from current position toward end /
            // in straight line
            // closer to end destination
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition);

            // recalc distance for next loop
            sqrRemainingDistrance = (transform.position - end).sqrMagnitude;

            // wait for a frame before re-evaluating condition of the loop
            yield return null;
        }
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
