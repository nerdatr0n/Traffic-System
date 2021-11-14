using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Node : MonoBehaviour
{
	// each node its connected to
	[SerializeField] private List<Node> allNodes;
	[SerializeField] private List<Node> blockerNodes;

	[SerializeField] private bool canOnlyHaveOneFollower = true;

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 0f, 0.33f);

		// Draws connecting line to each node
		foreach (Node node in allNodes)
		{
			if (node != null)
			{
				Gizmos.DrawLine(transform.position, node.transform.position);


				// Draws moving sphere
				Gizmos.DrawSphere(Vector3.Lerp(transform.position, node.transform.position, Time.time % 1), 0.2f);
			}
		}

		// will make it red if it is blocked
		if (GetIsBlocked())
		{
			Gizmos.color = new Color(1f, 0f, 0f, 0.33f);
		}
		Gizmos.DrawSphere(transform.position, 0.5f);

		foreach (Node blockerNode in blockerNodes)
		{
			Gizmos.color = new Color(1f, 0f, 0f, 0.33f);

			Gizmos.DrawLine(transform.position, blockerNode.transform.position);
		}
	}


	[Button("Create And Attach Node")]
	private void CreateAndAttachNode()
	{
#if UNITY_EDITOR

		GameObject objectToCreate = Resources.Load<GameObject>("Node");

		GameObject instantiate = PrefabUtility.InstantiatePrefab(objectToCreate, transform.parent) as GameObject;

		// Checks that its not null
		if (instantiate == null)
		{
			return;
		}

		// Sets position
		instantiate.transform.position = transform.position;
		instantiate.transform.rotation = transform.rotation;

		allNodes.Add(instantiate.GetComponent<Node>());

		Selection.objects = new Object[] { instantiate };
#endif
	}

	// returns the next node
	public Node GetNode(int nodeNumber)
	{
		return nodeNumber >= 0 && allNodes.Count > nodeNumber ? allNodes[nodeNumber] : null;
	}

	public Node GetRandomNode()
	{
		if (allNodes.Count == 0)
		{
			return null;
		}
		if (allNodes.Count == 1)
		{
			return allNodes[0];
		}
		return allNodes[Random.Range(0, allNodes.Count)];
	}

	public bool GetIsBlocked()
	{
		if (canOnlyHaveOneFollower && GetHasFollowers())
		{
			return true;
		}


		foreach (Node blockerNode in blockerNodes)
		{
			if (blockerNode.GetHasFollowers())
			{
				return true;
			}
		}

		return false;
	}

	public bool GetHasFollowers() => allFollowers.Count > 0;


	[SerializeField] protected List<NodeFollower> allFollowers = new List<NodeFollower>();

	private void Update()
	{
		UpdateAllFollowers();
	}


	private void UpdateAllFollowers()
	{
		List<NodeFollower> followersToDelete = new List<NodeFollower>();

		foreach (NodeFollower follower in allFollowers)
		{
			Vector3 velocity;

			bool isCloseToNode = Vector3.Distance(follower.transform.position, transform.position) < follower.distanceToSwitchNodes;

			if (isCloseToNode)
			{
				velocity = Vector3.zero;
			}
			else
			{
				Vector3 directionToNextNode = transform.position - follower.transform.position;

				directionToNextNode.y = 0;
				directionToNextNode.Normalize();

				velocity = directionToNextNode;
			}

			follower.UpdateVelocity(velocity);

			// Updates the next node
			if (isCloseToNode)
			{
				// Selects the next node
				if (follower.nextNode == null)
				{
					Node nextNode = GetRandomNode();

					if (nextNode == null)
					{
						followersToDelete.Add(follower);
						Destroy(follower.gameObject);
						continue;
					}
					follower.nextNode = nextNode;
				}

				// Will start moving to the next node;
				if (!follower.nextNode.GetIsBlocked())
				{
					follower.nextNode.allFollowers.Add(follower);
					followersToDelete.Add(follower);
					follower.nextNode = null;
				}
			}

			// selects the next node
		}

		// clears all the nodes
		foreach (NodeFollower follower in followersToDelete)
		{
			allFollowers.Remove(follower);
		}
	}
}
