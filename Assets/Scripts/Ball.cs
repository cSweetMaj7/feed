using UnityEngine;
using UnityEngine.EventSystems;

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
    int framePass = 1;
    SpriteRenderer sprite;
    Vector2 lastCalculatedPoint;
    GameObject hitBarrier;
    GameResource ballResources;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        lastOffsetWidth = sprite.size.x;
        circleOffset = lastOffsetWidth / 2;
        lastCalculatedPoint = transform.position;

        // setup ball resource to just be 1 of everything for now
        ballResources = new GameResource();
        ballResources.SetResourceQuantity(GameResource.ResourceType.Cheese, 1);
        ballResources.SetResourceQuantity(GameResource.ResourceType.Pepperoni, 1);
        ballResources.SetResourceQuantity(GameResource.ResourceType.Canadian_Bacon, 1);
        ballResources.SetResourceQuantity(GameResource.ResourceType.Pineapple, 1);

        // launch();
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked");
    }

    public GameResource getBallResources()
    {
        return ballResources;
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
            transform.position = lastCalculatedPoint;
            // see if size needs updating
            if(Mathf.Abs(System.Math.Abs(sprite.size.x - lastOffsetWidth)) > Mathf.Epsilon)
            {
                lastOffsetWidth = lastOffsetWidth = sprite.size.x;
                circleOffset = sprite.size.x / 2;
            }

            // see if slope/roc needs updating
            if (System.Math.Abs(m - lastM) > Mathf.Epsilon)
            {
                transform.position = new Vector2(0, 0);
                setRoc();
            }

            Vector2 caculatedPosition = calculatNextPoint(transform.position);

           if (transform.position.y > 5.0f ||
           transform.position.y < -5.0f ||
           transform.position.x > 8.89f ||
           transform.position.x < -8.89f)
            {
                Debug.LogError("OOB");
                Debug.Break();
            }

            // place at new calculated position
            lastCalculatedPoint = new Vector3(caculatedPosition.x, caculatedPosition.y, transform.position.z);

            if(hitBarrier)
            {
                // handle special events for hitting objects
                CollisionHandler.HandleCollision(hitBarrier, this);
                hitBarrier = null;
            }
        }
        framePass = 1;
    }

    void drawDebugRaysDist(Vector2 originIn, float distance)
    {
        // draw debug rays
        Vector2 originX = originIn;
        Vector2 originY = originIn;

        originX.x = xDir ? originX.x + circleOffset : originX.x - circleOffset;
        originY.y = yDir ? originY.y + circleOffset : originY.y - circleOffset;

        Vector2 endX = originX;
        Vector2 endY = originY;

        endX.x = xDir ? endX.x + distance : endX.x - distance;
        endY.y = yDir ? endY.y + distance : endY.y - distance;

        Debug.DrawLine(originX, endX, Color.green);
        Debug.DrawLine(originY, endY, Color.green);
    }

    protected Vector2 calculatNextPoint(Vector2 origin, float xLess=0f, float yLess=0f)
    {
        //Debug.Log("Calculating next point");
        Vector2 nextCalculatedPoint = origin;
        // move ball in direction by rate of change
        nextCalculatedPoint.x = xDir ? origin.x + (xRoc * v) : origin.x - (xRoc * v);
        nextCalculatedPoint.y = yDir ? origin.y + (yRoc * v) : origin.y - (yRoc * v);

        float xDif = (xRoc * v) - xLess;
        float yDif = (yRoc * v) - yLess;

        if (xDif < Mathf.Epsilon || yDif < Mathf.Epsilon)
        {
            // trying to move too far
            xDif = 0;
            yDif = 0;
        }
        //float distance = Mathf.Sqrt((xDif * xDif) + (yDif * yDif)); // make sure this distance is traveled in total

        // raycast out the dest and see what we hit
        Vector2 vecDirY = yDir ? Vector2.up : -Vector2.up;
        Vector2 vecDirX = xDir ? Vector2.right : -Vector2.right;
        Vector2 originX = origin;
        Vector2 originY = origin;
        originX.x = xDir ? originX.x + circleOffset : originX.x - circleOffset;
        originY.y = yDir ? originY.y + circleOffset : originY.y - circleOffset;

        Color lineColor;

        switch (framePass)
        {
            case 1:
                lineColor = Color.green;
                break;

            case 2:
                lineColor = Color.yellow;
                break;

            case 3:
                lineColor = Color.red;
                break;

            default:
                lineColor = Color.black;
                break;
        }

        Debug.DrawLine(originX, new Vector2(xDir ? originX.x + xDif : originX.x - xDif, originX.y), lineColor);
        Debug.DrawLine(originY, new Vector2(originY.x, yDir ? originY.y + yDif : originY.y - yDif), lineColor);

        framePass++;

        RaycastHit2D horizontalHitCheck = Physics2D.Raycast(originX, vecDirX, xDif); // h collider
        RaycastHit2D verticalHitCheck = Physics2D.Raycast(originY, vecDirY, yDif); // v collider

        // this is where collision logic starts happening
        if(verticalHitCheck.collider && horizontalHitCheck.collider)
        {
            yDir = !yDir;
            xDir = !xDir;

            hitBarrier = verticalHitCheck.collider.gameObject;
            hitBarrier = hitBarrier ? hitBarrier : horizontalHitCheck.collider.gameObject;

            nextCalculatedPoint = reflectY(verticalHitCheck.point, nextCalculatedPoint, true);
            //result = reflectX(horizontalHitCheck.point, result);
        }
        else if (verticalHitCheck.collider)
        {
            //Debug.Log("Y Collide");
            // invert y
            yDir = !yDir;

            hitBarrier = verticalHitCheck.collider.gameObject;

            nextCalculatedPoint = reflectY(verticalHitCheck.point, nextCalculatedPoint);
        }
        else if (horizontalHitCheck.collider)
        {
            //Debug.Log("X Collide");
            // invert x
            xDir = !xDir;

            hitBarrier = horizontalHitCheck.collider.gameObject;

            nextCalculatedPoint = reflectX(horizontalHitCheck.point, nextCalculatedPoint);
        }

        return nextCalculatedPoint;
    }

    Vector2 reflectY(Vector2 hitPoint, Vector2 currentPoint, bool twoHit = false, bool calcNextReflect = false)
    {
        float yTraveled = Mathf.Abs(currentPoint.y - hitPoint.y) - circleOffset;
        float currYRoc = yRoc * v;
        float reflectDistance = Mathf.Abs(currYRoc - yTraveled);
        currentPoint.y = yDir ? hitPoint.y + reflectDistance : hitPoint.y - reflectDistance;
        if(twoHit)
        {
            // x also needs to be calculated before recursing
            currentPoint = reflectX(hitPoint, currentPoint, false, false);
        }
        Debug.DrawLine(hitPoint, currentPoint, twoHit ? Color.blue : Color.magenta);

        // adjust for circle offset
        currentPoint.y = yDir ? currentPoint.y + circleOffset : currentPoint.y - circleOffset;

        return calcNextReflect ? calculatNextPoint(currentPoint, 0f, yTraveled) : currentPoint;
    }

    Vector2 reflectX(Vector2 hitPoint, Vector2 currentPoint, bool twoHit = false, bool calcNextReflect = false)
    {
        float xTraveled = Mathf.Abs(currentPoint.x - hitPoint.x) - circleOffset ;
        float currXRoc = xRoc * v;
        float reflectDistance = Mathf.Abs(currXRoc - xTraveled);
        currentPoint.x = xDir ? hitPoint.x + reflectDistance : hitPoint.x - reflectDistance;
        if(twoHit)
        {
            // y also needs to be calculated before recursing
            currentPoint = reflectY(hitPoint, currentPoint, false, false);
        }
        Debug.DrawLine(hitPoint, currentPoint, Color.cyan);

        // adjust for circle offset
        currentPoint.x = xDir ? currentPoint.x + circleOffset : currentPoint.x - circleOffset;

        return calcNextReflect ? calculatNextPoint(currentPoint, xTraveled, 0f) : currentPoint;
    }

    public void launch()
    {
        yDir = true; // always launching from bottom of screen

        setRoc();
        
        locked = false;
        Debug.Log("Lauched.");
    }

    void setRoc()
    {
        lastM = m;
        xRoc = xDir ? transform.position.x + 0.01f : transform.position.x - 0.01f;
        xRoc = Mathf.Abs(xRoc);

        // results in smallest y rate of change
        yRoc = Mathf.Abs(m * xRoc);

        normalizeX();
        normalizeY();

        Debug.Log("setRoc calc: xRoc-" + xRoc + " yRoc-" + yRoc);
    }

    void normalizeY()
    {
        // normalize y rate of change to ~1
        if (yRoc > 1.0f)
        {
            while(yRoc > 1.0f)
            {
                xRoc *= 0.99f;
                yRoc *= 0.99f;
            }
        }
        else if (yRoc < 1.0f)
        {
            while (yRoc < 1.0f)
            {
                yRoc *= 1.1f;
                xRoc *= 1.1f; // increment by 10% at a time
            }
        }
        if (yRoc > 1.25)
        {
            Debug.LogError("Y RATE OF CHANGE TOO HIGH: " + yRoc);
        }
    }

    void normalizeX()
    {
        // normalize x rate of change to ~1
        if (xRoc > 1.0f)
        {
            while(xRoc > 1.0f)
            {
                xRoc *= 0.99f;
                yRoc *= 0.99f;
            }
            
        } else {
            while (xRoc < 1.0f)
            {
                yRoc *= 1.01f;
                xRoc *= 1.01f; // increment by 1% at a time
            }
        }
        if (xRoc > 1.25)
        {
            Debug.LogError("X RATE OF CHANGE TOO HIGH: " + xRoc);
        }
    }

    public void setSlopeByInputPoint(Vector2 inputPoint)
    {
        float x = Mathf.Abs(transform.position.x - inputPoint.x);
        float y = Mathf.Abs(transform.position.y - inputPoint.y);

        m = y/x;
        setRoc();

        // set x polarity
        xDir = inputPoint.x > transform.position.x;
    }

    public void setLock(bool itIs)
    {
        locked = itIs;
        lastCalculatedPoint = transform.position;
    }


}
