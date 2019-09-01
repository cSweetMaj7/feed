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
    int framePass = 1;
    SpriteRenderer sprite;
    Vector2 lastCalculatedPoint;
    GameObject hitTarget = null;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        lastOffsetWidth = sprite.size.x;
        circleOffset = lastOffsetWidth / 2;
        lastCalculatedPoint = transform.position;
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
            //transform.position = new Vector3(caculatedPosition.x, caculatedPosition.y, transform.position.z);
            lastCalculatedPoint = new Vector3(caculatedPosition.x, caculatedPosition.y, transform.position.z);
            Destroy(hitTarget);
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

        //Debug.DrawRay(originX, vecDirX, Color.green);
        Debug.DrawLine(originX, endX, Color.green);
        //Debug.DrawRay(originY, vecDirY, Color.green);
        Debug.DrawLine(originY, endY, Color.green);
    }

    protected Vector2 calculatNextPoint(Vector2 origin, float xLess=0f, float yLess=0f)
    {
        //Debug.Log("Calculating next point");
        Vector2 nextCalculatedPoint = origin;
        // move ball in direction by rate of change
        nextCalculatedPoint.x = xDir ? origin.x + (xRoc * v) : origin.x - (xRoc * v);
        nextCalculatedPoint.y = yDir ? origin.y + (yRoc * v) : origin.y - (yRoc * v);
        //Vector2 dest = new Vector2(newX, newY);

        float xDif = (xRoc * v) - xLess; // Mathf.Abs(result.x - origin.x) - xLess;
        float yDif = (yRoc * v) - yLess; // Mathf.Abs(result.y - origin.y) - yLess;

        //drawDebugRays(result, xDif);
        //drawDebugRays(result, yDif);

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

            hitTarget = verticalHitCheck.collider.gameObject.name == "Target" ? verticalHitCheck.collider.gameObject : null;
            hitTarget = horizontalHitCheck.collider.gameObject.name == "Target" ? horizontalHitCheck.collider.gameObject : null;

            nextCalculatedPoint = reflectY(verticalHitCheck.point, nextCalculatedPoint, true);
            //result = reflectX(horizontalHitCheck.point, result);
        }
        else if (verticalHitCheck.collider)
        {
            //Debug.Log("Y Collide");
            // invert y
            yDir = !yDir;

            hitTarget = verticalHitCheck.collider.gameObject.name == "Target" ? verticalHitCheck.collider.gameObject : null;

            nextCalculatedPoint = reflectY(verticalHitCheck.point, nextCalculatedPoint);
        }
        else if (horizontalHitCheck.collider)
        {
            //Debug.Log("X Collide");
            // invert x
            xDir = !xDir;

            hitTarget = horizontalHitCheck.collider.gameObject.name == "Target" ? horizontalHitCheck.collider.gameObject : null;

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


}
