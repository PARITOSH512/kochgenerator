using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Renderer))]
public class ShadingInWorldSpace : MonoBehaviour
{
	public GameObject other;
	Renderer rend;
	void Start()
	{
		rend = GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update()
	{
		if (other != null)
		{
			rend.sharedMaterial.SetVector("_Point", other.transform.position);
		}
	}
}