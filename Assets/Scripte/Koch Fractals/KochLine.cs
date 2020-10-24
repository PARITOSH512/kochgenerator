 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KochLine : KochGenerator
{
    
    public int GeneratorMultiplier;
    //List<GameObject> cubs = new List<GameObject>();
    //public GameObject lol;

    LineRenderer _lineRenderer;
    [Range(0,1)]
    public float _lerpAmount; // For test
    Vector3[] _lerpPosition;

    bool lolbool = false;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.enabled = true;
        _lineRenderer.loop = true;
        _lineRenderer.positionCount = _position.Length;
        _lineRenderer.SetPositions(_position);
        _lerpPosition = new Vector3[_position.Length];

    }

    // Update is called once per frame
    void Update()
    {
      if(_generationCount!= 0)
        {
           
            for (int i = 0; i < _position.Length; i++)
            {
                _lerpPosition[i] = Vector3.Lerp(_position[i], _targetposition[i], _lerpAmount);
                 print("lololoolololoolo");
            }
            if(_useBezierCurves)
            {
                _bezierPosition = BezierCurve(_lerpPosition, _bezierVertexCount);
                _lineRenderer.positionCount = _bezierPosition.Length;
                _lineRenderer.SetPositions(_bezierPosition);
            }
            else
            {
                _lineRenderer.positionCount = _lerpPosition.Length;
                _lineRenderer.SetPositions(_lerpPosition);
            }
            //if(!lolbool)
            //{
            //    for (int i = 0; i < _position.Length; i++)
            //    {
            //        GameObject game = Instantiate(lol);
            //        game.transform.position = _position[i];
            //    }
            //    lolbool = true;
            //}

            
            //for (int i = 0; i < _lerpPosition.Length; i++)
            //{
            //    cubs[i].transform.position = _lerpPosition[i];
            //}
            
        }



      //if(Input.GetKeyUp(KeyCode.O))
      //  {
      //      KochGenerate(_targetposition, true, GeneratorMultiplier);
      //      //for (int i = 0; i < _position.Length; i++)
      //      //{
      //      //    GameObject cube = Instantiate(lol);
      //      //    cubs.Add(cube);
      //      //    lol.transform.position = _position[i];
      //      //}
      //      _lerpPosition = new Vector3[_position.Length];
      //      _lineRenderer.positionCount = _position.Length;
      //      _lineRenderer.SetPositions(_position);
      //      _lerpAmount = 0;
      //  }
      //if(Input.GetKeyUp(KeyCode.I))
      //  {
      //      KochGenerate(_targetposition, false, GeneratorMultiplier);
      //      _lerpPosition = new Vector3[_position.Length];
      //      _lineRenderer.positionCount = _position.Length;
      //      _lineRenderer.SetPositions(_position);
      //      _lerpAmount = 0;
      //  }
    }
}
