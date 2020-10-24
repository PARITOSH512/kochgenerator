using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseGlowThing : MonoBehaviour
{
    private void Update()
    {
        Plane p = new Plane(Vector3.up, Vector3.zero);

        Vector2 mousepos = Input.mousePosition;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(mousepos);
        if(p.Raycast(ray,out float enterdis))
        {
            Vector3 worldmousepos = ray.GetPoint(enterdis);
            Shader.SetGlobalVector("_Mousepos", worldmousepos);
        }
    }
}
