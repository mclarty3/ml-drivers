using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class MLDriverAgent : Agent
{
	CarPercepts carPercepts;
	CarController carController;
	PathCrawler pathCrawler;
	CarRuleEnforcer carRuleEnforcer;
	MLTrainingScene scene;

	public float maxEpisodeTime = 15f;
	public float maxNegativeReward = -100f;
	public float accelerationOutput;
	public float steeringOutput;
	public float brakeOutput;
	private float _elapsedTime = 0f;

	public void Start()
	{
		scene = transform.GetComponentInParent<MLTrainingScene>();
	}

	public override void Initialize()
	{
		carPercepts = GetComponent<CarPercepts>();
		carController = GetComponent<CarController>();
		pathCrawler = GetComponent<PathCrawler>();
		carRuleEnforcer = GetComponent<CarRuleEnforcer>();
	}

	public void FixedUpdate()
	{
		_elapsedTime += Time.fixedDeltaTime;
		if (_elapsedTime > maxEpisodeTime)
		{
			EndEpisode();
			_elapsedTime = 0f;
		}

		if (GetCumulativeReward() < maxNegativeReward)
		{
			EndEpisode();
		}
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		// 3 + 8 + 1 + 1 = 13

		// Track self position and rotation
		sensor.AddObservation(transform.position.x);
		sensor.AddObservation(transform.position.z);
		sensor.AddObservation(transform.rotation.y);

		// Track current node position, and subsequent three node's positions
		sensor.AddObservation(pathCrawler.currentNodePosition.x);
		sensor.AddObservation(pathCrawler.currentNodePosition.z);
		foreach (Vector3 node in pathCrawler.nextThreeNodes)
		{
			sensor.AddObservation(node.x);
			sensor.AddObservation(node.z);
		}

		// Track status of approaching traffic signal, if applicable
		// -1 -> None; 0 -> Red; 1 -> Yellow; 2 -> Green; 3 -> Stop Sign
		sensor.AddObservation(carPercepts.approachingTrafficSignalType);

		// Track speed limit of current path
		sensor.AddObservation(pathCrawler.maxVelocity);

		// Track status of raycast percepts coming from the front of the car
		// By default, car casts 31 rays
		// carPercepts.GetCollisions(out List<float> distances, "Car");
		// foreach (float distance in distances)
		// {
		// 	sensor.AddObservation(distance);
		// }

	}

	public override void OnActionReceived(ActionBuffers actionBuffers)
	{
		float verticalAxis = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
		float horizontalAxis = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
		float brakeValue = Mathf.Clamp(actionBuffers.ContinuousActions[2], 0f, 1f);

		accelerationOutput = verticalAxis;
		steeringOutput = horizontalAxis;
		brakeOutput = brakeValue;

		// Debug.Log("Vertical: " + verticalAxis + " Horizontal: " + horizontalAxis + " Brake: " + brakeValue);
		carController.SetInput(verticalAxis, horizontalAxis, brakeValue);

		if (Vector3.Angle(transform.up, Vector3.up) > 45f)
		{
			Debug.Log("Car is upside down");
			AddReward(-1f);
			EndEpisode();
		}

		if (carPercepts.CollidedWithObject(out string tag, clear: true))
		{
			if (tag == "Car" || tag == "TrafficSignal" || tag == "Sidewalk")
			{
				Debug.Log("Collided with " + tag + "! Resetting...");
				AddReward(-1f);
				EndEpisode();
			}
		}
		if (carRuleEnforcer.CheckRanTrafficSignal(clear: true))
		{
			Debug.Log("Ran traffic signal");
			AddReward(-0.5f);
		}
		if (pathCrawler.IsInOtherLane())
		{
			// Debug.Log("Went into other lane");
			AddReward(-1f);
		}
		if (carController.velocity > pathCrawler.maxVelocity)
		{
			// Debug.Log("Exceeded max velocity");
			AddReward(-0.2f);
		}

		// This is triggering when the level is reset, which I think is throwing off the rewards
		if (pathCrawler.CheckChangedNodes(clear: true))
		{
			Debug.Log("Changed nodes");
			AddReward(1f);
		}

		AddReward(-0.01f);
	}

	public override void OnEpisodeBegin()
	{
		scene.InitializeScene(this);
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var continuousActionsOut = actionsOut.ContinuousActions;
		continuousActionsOut[0] = Input.GetAxis("Vertical");
		continuousActionsOut[1] = Input.GetAxis("Horizontal");
		continuousActionsOut[2] = Input.GetAxis("Jump");
	}
}