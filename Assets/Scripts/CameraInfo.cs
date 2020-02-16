using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInfo : MonoBehaviour
{

    public const int FACING_POS_Z = 1;

    public const int FACING_NEG_Z = 2;

    public const int FACING_POS_X = 3;

    public const int FACING_NEG_X = 4;

    public const float Y_FLOAT = 0.35f;

    public const int STAGNANT = 0;

    public const int FORWARD_MOVE = 1;

    public const int ROTATE_MOVE = 2;

    private float desiredEndAngle;

    private int directionFacing;

    private int mode;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetDirectionFacing()
    {
        return directionFacing;
    }

    public void SetDirectionFacing(int directionFacing)
    {
        this.directionFacing = directionFacing;
    }

    public float GetDesiredEndAngle()
    {
        return desiredEndAngle;
    }

    public void SetDesiredEndAngle(float desiredEndAngle)
    {
        this.desiredEndAngle = desiredEndAngle;
    }

    public int GetMode()
    {
        return mode;
    }

    public void SetMode(int mode)
    {
        this.mode = mode;
    }

}
