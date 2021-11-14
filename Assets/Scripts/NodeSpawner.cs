using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class NodeSpawner : Node
{
	[SerializeField] private GameObject objectToSpawn;
	[SerializeField] private bool autoSpawnObjects = true;

	[SerializeField, MinMaxSlider(0.2f, 10f)]
	private Vector2 timeBetweenSpawning = new Vector2(1, 3);

	private void Start()
	{
		if (autoSpawnObjects)
		{
			StartCoroutine(ConstantlySpawnFollowers());
		}
	}

	[Button("Spawn A new Follower")]
	public void SpawnFollower()
	{
		GameObject newFollower = Instantiate(objectToSpawn, transform.position, Quaternion.identity);

		allFollowers.Add(newFollower.GetComponent<NodeFollower>());
	}

	private IEnumerator ConstantlySpawnFollowers()
	{
		while (autoSpawnObjects)
		{
			yield return new WaitForSeconds(Random.Range(timeBetweenSpawning.x, timeBetweenSpawning.y));

			if (!GetIsBlocked())
			{
				SpawnFollower();
			}
		}
	}
}
