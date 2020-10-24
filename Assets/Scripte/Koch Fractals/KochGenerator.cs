using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochGenerator : MonoBehaviour
{
    protected enum _axis
    {
        XAxis,
        YAxis,
        ZAxis
    }

    protected enum _initinator
    {
        Triangle,
        Square,
        Pentagon,
        Hexagon,
        Heptagon,
        Octagon
    }

    public struct LineSegment
    {
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public Vector3 Direction { get; set; }
        public float Lenght { get; set; }

        public void MeasureDirectionAndLenght()
        {
            this.Direction = (this.EndPosition - this.StartPosition).normalized;
            this.Lenght = Vector3.Distance(this.EndPosition, this.StartPosition);
        }
    }

    [SerializeField]
    protected _initinator initinator = new _initinator();

    [SerializeField]
    protected AnimationCurve _generator;

    [Serializable]
    public struct StartGen
    {
        public bool outward;
        public float scale;
    }
    
    public StartGen[] startGen;

    protected Keyframe[] _keys;
    protected int _generationCount;

    [SerializeField]
    protected _axis _Axis = new _axis();

    [SerializeField]
    protected bool _useBezierCurves;

    [SerializeField]
    [Range(8,24)]
    protected int _bezierVertexCount;
    protected int _initinatorPointAmount;

    [SerializeField]
    private Vector3[] _initinatorPoint;
    private Vector3 _rotateVector;
    private Vector3 _rotateAxis;
    private float _initialRotation;

    [SerializeField]
    protected float _initinatorSize;

    [SerializeField]
    protected Vector3[] _position;
    protected Vector3[] _targetposition;
    protected Vector3[] _bezierPosition;

    private List<LineSegment> _linesegment;


    protected Vector3[] BezierCurve(Vector3[] points , int vertexCount)
    {
        var pointList = new List<Vector3>();
        for (int i = 0; i < points.Length; i+=2)
        {
             if(i+2 <= points.Length-1)
            {
                for(float ratio = 0f;ratio<=1f; ratio+= 1.0f/vertexCount)
                {
                    var tangetLineVertex1 = Vector3.Lerp(points[i], points[i + 1], ratio);
                    var tangetLineVertex2 = Vector3.Lerp(points[i+1], points[i + 2], ratio);
                    var bezierpoint = Vector3.Lerp(tangetLineVertex1, tangetLineVertex2, ratio);
                    pointList.Add(bezierpoint);
                }
            }
        }
        return pointList.ToArray();
    }

    private void Awake()
    {
        GetInitiatorPoints();

        //assign list & array
        _position = new Vector3[_initinatorPointAmount+1];
        _targetposition = new Vector3[_initinatorPointAmount + 1];
        _linesegment = new List<LineSegment>();
        _keys = _generator.keys;

        _rotateVector = Quaternion.AngleAxis(_initialRotation, _rotateAxis) * _rotateVector;
        for (int i = 0; i < _initinatorPointAmount; i++)
        {
            _position[i] = _rotateVector * _initinatorSize;
            _rotateVector = Quaternion.AngleAxis(360 / _initinatorPointAmount, _rotateAxis) * _rotateVector;
            print("_rotatevector " + _rotateVector + (Quaternion.AngleAxis(360 / _initinatorPointAmount, _rotateAxis)));
            // MultiQutVect(Quaternion.AngleAxis(360 / _initinatorPointAmount, _rotateAxis), _rotateVector);
        }
        _position[_initinatorPointAmount] = _position[0];
        _targetposition = _position;

        for (int i = 0; i < startGen.Length; i++)
        {
            KochGenerate(_targetposition, startGen[i].outward, startGen[i].scale);
        }

    }


    protected void KochGenerate(Vector3[] positions, bool outwards , float generatorMultiplier)
    {

        //Creating line Segment
        _linesegment.Clear();
        for (int i = 0; i < positions.Length-1; i++)
        {
            LineSegment line = new LineSegment();
            line.StartPosition = positions[i];
            if (i == positions.Length - 1)
            {
                line.EndPosition = positions[0];
            }
            else
            {
                line.EndPosition = positions[i + 1];
            }
            //line.MeasureDirectionAndLenght();
            line.Direction = (line.EndPosition - line.StartPosition).normalized;
            line.Lenght = Vector3.Distance(line.EndPosition, line.StartPosition);
            _linesegment.Add(line);
           
        }

        //add the line segment point to a point array
        List<Vector3> newPos = new List<Vector3>();
        List<Vector3> targetPos = new List<Vector3>();

        for (int i = 0; i < _linesegment.Count; i++)
        {
            newPos.Add(_linesegment[i].StartPosition);
            targetPos.Add(_linesegment[i].StartPosition);

            for (int j = 1; j < _keys.Length-1; j++)
            {
                float moveAmount = _linesegment[i].Lenght * _keys[j].time;
                float heightAmount = _linesegment[i].Lenght * _keys[j].value * generatorMultiplier;

                //getting the point position for fratcal
                Vector3 movePos = _linesegment[i].StartPosition + (_linesegment[i].Direction * moveAmount);
                //fractal Dir outward or inward
                Vector3 Dir;
                if(outwards)
                {
                    Dir = Quaternion.AngleAxis(-90, _rotateAxis) * _linesegment[i].Direction; 
                }
                else
                {
                    Dir = Quaternion.AngleAxis(90, _rotateAxis) * _linesegment[i].Direction;
                }
                newPos.Add(movePos);
                targetPos.Add(movePos + (Dir * heightAmount));
            }
        }
        newPos.Add(_linesegment[0].StartPosition);
        targetPos.Add(_linesegment[0].StartPosition);

        _position = new Vector3[newPos.Count];
        _targetposition = new Vector3[targetPos.Count];
        _bezierPosition = BezierCurve(_targetposition, _bezierVertexCount);

        _position = newPos.ToArray();
        _targetposition = targetPos.ToArray();

        _generationCount++;

       


    }


    private void OnDrawGizmos()
    {
        GetInitiatorPoints();
        _initinatorPoint = new Vector3[_initinatorPointAmount];


        _rotateVector = Quaternion.AngleAxis(_initialRotation, _rotateAxis) * _rotateVector;
        for (int i = 0; i < _initinatorPointAmount; i++)
        {
            _initinatorPoint[i] = _rotateVector * _initinatorSize;
            _rotateVector = Quaternion.AngleAxis(360 / _initinatorPointAmount, _rotateAxis) * _rotateVector;
            //print("_rotatevector " + _rotateVector + (Quaternion.AngleAxis(360 / _initinatorPointAmount, _rotateAxis)));
            // MultiQutVect(Quaternion.AngleAxis(360 / _initinatorPointAmount, _rotateAxis), _rotateVector);
        }

        for (int i = 0; i < _initinatorPointAmount; i++)
        {
            Gizmos.color = Color.white;

            Matrix4x4 rotationmatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            //print(rotationmatrix);
            Gizmos.matrix = rotationmatrix;


            if(i<_initinatorPointAmount-1)
            {
                Gizmos.DrawLine(_initinatorPoint[i], _initinatorPoint[i +1]);
            }
            else
            {
                Gizmos.DrawLine(_initinatorPoint[i], _initinatorPoint[0]);
            }
        }
        //KochGenerate(_position,true,1);
    }

    private void GetInitiatorPoints()
    {
        switch(initinator)
        {
            case _initinator.Triangle:
                _initinatorPointAmount = 3;
                _initialRotation = 0;
                break;

            case _initinator.Square:
                _initinatorPointAmount = 4;
                _initialRotation = 45;
                break;

            case _initinator.Pentagon:
                _initinatorPointAmount = 5;
                _initialRotation = 36;
                break;

            case _initinator.Hexagon:
                _initinatorPointAmount = 6;
                _initialRotation = 30;
                break;

            case _initinator.Heptagon:
                _initinatorPointAmount = 7;
                _initialRotation = 25.71f;
                break;
                 
            case _initinator.Octagon:
                _initinatorPointAmount = 8;
                _initialRotation = 22.5f;
                break;
        }

        switch(_Axis)
        {
            case _axis.XAxis:
                _rotateVector = new Vector3(1, 0, 0);
                _rotateAxis = new Vector3(0, 0, 1);
                break;

            case _axis.YAxis:
                _rotateVector = new Vector3(0, 1, 0);
                _rotateAxis = new Vector3(1, 0, 0);
                break;

            case _axis.ZAxis:
                _rotateVector = new Vector3(0, 0, 1);
                _rotateAxis = new Vector3(0, 1, 0);
                break;

            default:
                _rotateVector = new Vector3(0, 1, 0);
                _rotateAxis = new Vector3(1, 0, 0);
                break;
        }
    }

    void MultiQutVect(Quaternion quat , Vector3 vec)
    {
        {
            float num = quat.x * 2f;
            float num2 = quat.y * 2f;
            float num3 = quat.z * 2f;
            float num4 = quat.x * num;
            float num5 = quat.y * num2;
            float num6 = quat.z * num3;
            float num7 = quat.x * num2;
            float num8 = quat.x * num3;
            float num9 = quat.y * num3;
            float num10 = quat.w * num;
            float num11 = quat.w * num2;
            float num12 = quat.w * num3;
            Vector3 result;
            result.x = (1f - (num5 + num6)) * vec.x + (num7 - num12) * vec.y + (num8 + num11) * vec.z;
            result.y = (num7 + num12) * vec.x + (1f - (num4 + num6)) * vec.y + (num9 - num10) * vec.z;
            result.z = (num8 - num11) * vec.x + (num9 + num10) * vec.y + (1f - (num4 + num5)) * vec.z;
            print (result);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
