using UnityEngine;
using System.Collections;

public class FilteredObject : MonoBehaviour
{
	[SerializeField]
	float filterFrequency = 120.0f;

	OneEuroFilter<Vector3> vector3Filter;
	OneEuroFilter<Quaternion> quaternionFilter;

	public float FilterFrequency
    {
		get { return filterFrequency; }
		set { filterFrequency = value; }
    }

	void OnValidate()
	{
		if(vector3Filter != null)
		{
			vector3Filter.UpdateParams(filterFrequency);
		}

		if(quaternionFilter != null)
		{
			quaternionFilter.UpdateParams(filterFrequency);
		}
	}

	void OnEnable()
	{
		vector3Filter = new OneEuroFilter<Vector3>(filterFrequency);
		quaternionFilter = new OneEuroFilter<Quaternion>(filterFrequency);
	}

	void LateUpdate()
	{
		transform.position = vector3Filter.Filter(transform.position);
		transform.rotation = quaternionFilter.Filter(transform.rotation);
	}
}