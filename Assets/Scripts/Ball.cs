using UnityEngine;

public class Ball : MonoBehaviour
{
    bool locked = true; // locks ball movement, set false to launch
    public bool xDir; // false = left, true = right
    public bool yDir; // false = down, true, up
    public float m = 1.0f; // slope
    public float v = 1.0f; // velocity
    float circleOffset = 0.55f;
    float lastOffsetWidth;
    float xRoc;
    float yRoc;
    float lastX;
    float lastY;
    float nextX;
    float nextY;
    float lastM;
    SpriteRenderer sprite;


    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        lastOffsetWidth = sprite.size.x;
        circleOffset = lastOffsetWidth / 2;
        launch();
    }

    // Update is called once per frame
    // y = m(x)
    void FixedUpdate()
    {
        // don't ever let these get negative
        if(m < Mathf.Epsilon)
        {
            m = Mathf.Epsilon;
        }
        if(v < Mathf.Epsilon)
        {
            v = Mathf.Epsilon;
        }
        // move thyself
        if(!locked)
        {
            // see if size needs updating
            if(Mathf.Abs(System.Math.Abs(sprite.size.x - lastOffsetWidth)) > Mathf.Epsilon)
            {
                lastOffsetWidth = lastOffsetWidth = sprite.size.x;
                circleOffset = sprite.size.x / 2;
            }

            // see if slope/roc needs updating
            if (System.Math.Abs(m - lastM) > Mathf.Epsilon)
            {
                setRoc();
            }

            // move ball in direction by rate of change
            float newX = xDir ? transform.position.x + (xRoc * v) : transform.position.x - (xRoc * v);
            float newY = yDir ? transform.position.y + (yRoc * v) : transform.position.y - (yRoc * v);
            //Vector2 dest = new Vector2(newX, newY);

            float xDif = Mathf.Abs(newX - transform.position.x) + 0.2f;
            float yDif = Mathf.Abs(newY - transform.position.y) + 0.2f;
            //float distance = Mathf.Sqrt((xDif * xDif) + (yDif * yDif)); // make sure this distance is traveled in total

            // raycast out the dest and see what we hit
            Vector2 vecDirY = yDir ? Vector2.up : -Vector2.up;
            Vector2 vecDirX = xDir ? Vector2.right : -Vector2.right;
            Vector2 originX = transform.position;
            Vector2 originY = transform.position;
            originX.x = xDir ? originX.x + circleOffset : originX.x - circleOffset;
            originY.y = yDir ? originY.y + circleOffset : originY.y - circleOffset;

            
            RaycastHit2D horizontalHitCheck = Physics2D.Raycast(originX, vecDirX, xDif); // h collider
            RaycastHit2D verticalHitCheck = Physics2D.Raycast(originY, vecDirY, yDif); // v collider
            
            if (horizontalHitCheck.collider)
            {
                //Debug.Log("X Collide");
                // invert x
                xDir = !xDir;
                newX = xDir ? horizontalHitCheck.point.x + (circleOffset) : horizontalHitCheck.point.x - (circleOffset);
            }
            if (verticalHitCheck.collider)
            {
                //Debug.Log("Y Collide");
                // invert y
                yDir = !yDir;
                newY = yDir ? verticalHitCheck.point.y + (circleOffset) : verticalHitCheck.point.y - (circleOffset);
            }

            // place at new calculated position
            Vector3 newPosition = new Vector3(newX, newY, transform.position.z);
            transform.position = newPosition;

            if(transform.position.y > 5.0f ||
               transform.position.y < -5.0f ||
               transform.position.x > 9.28f ||
               transform.position.x < -9.28f)
            {
                Debug.LogError("OOB");
                Debug.Break();
            }

            vecDirY = yDir ? Vector2.up : -Vector2.up;
            vecDirX = xDir ? Vector2.right : -Vector2.right;

            originX = transform.position;
            originY = transform.position;

            originX.x = xDir ? originX.x + circleOffset : originX.x - circleOffset;
            originY.y = yDir ? originY.y + circleOffset : originY.y - circleOffset;

            Debug.DrawRay(originX, vecDirX, Color.green);
            Debug.DrawRay(originY, vecDirY, Color.green);
        }
    }

    void launch() // launching from bottom of screen
    {
        xDir = true;
        yDir = true;

        setRoc();
        
        locked = false;
        Debug.Log("Lauched.");
    }

    void setRoc()
    {
        lastM = m;
        xRoc = xDir ? transform.position.x + 0.1f : transform.position.x - 0.1f;
        xRoc = Mathf.Abs(xRoc);

        // results in smallest y rate of change
        yRoc = Mathf.Abs(m * xRoc);

        // normalize y rate of change to ~1
        if (yRoc > 1.0f)
        {
            xRoc /= yRoc;
            yRoc = 1.0f;
        }
        else if (yRoc < 1.0f)
        {
            while(yRoc < 1.0f)
            {
                yRoc *= 1.1f;
                xRoc *= 1.1f; // increment by 10% at a time
            }
        }
        if(yRoc > 1.25)
        {
            Debug.LogError("Y RATE OF CHANGE TOO HIGH: " + yRoc);
        }
    }


}
