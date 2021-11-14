using UnityEngine;

public class NodeFollower : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] internal float distanceToSwitchNodes;
	[SerializeField, Range(0, 1)] private float speedLerpAmount = 0.2f;
	public Rigidbody rb;

	public Node nextNode;

	//public void SetCurrentNode(Node newNode) => currentNode = newNode;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	internal void UpdateVelocity(Vector3 velocity)
	{
		// Makes it look in the direction its moving
		if (velocity .magnitude > 0.1f)
		{
			Vector3 lookAtPoint = rb.velocity;
			lookAtPoint.y = 0;
			transform.LookAt(transform.position + lookAtPoint.normalized);
		}

		Vector3 currentVelocity = rb.velocity;
		Vector3 newVelocity;
		newVelocity.x = Mathf.Lerp(currentVelocity.x, velocity.x * speed, speedLerpAmount);
		newVelocity.y = currentVelocity.y;
		newVelocity.z = Mathf.Lerp(currentVelocity.z, velocity.z * speed, speedLerpAmount);

		rb.velocity = newVelocity;
	}
}
