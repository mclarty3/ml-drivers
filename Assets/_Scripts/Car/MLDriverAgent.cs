using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class MLDriverAgent : Agent
{
	public CarPercepts carPercepts;
	public CarController carController;
	public PathCrawler pathCrawler;

	public override void Initialize()
	{
		carPercepts = GetComponent<CarPercepts>();
		carController = GetComponent<CarController>();
		pathCrawler = GetComponent<PathCrawler>();
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(transform.position.x);
		sensor.AddObservation(transform.position.z);
		sensor.AddObservation(transform.rotation.y);
		sensor.AddObservation(pathCrawler.currentNodePosition.x);
		sensor.AddObservation(pathCrawler.currentNodePosition.z);
		sensor.AddObservation(carPercepts.approachingTrafficSignalType);
		carPercepts.GetCollisions(out List<float> distances, "Car");
		foreach (float distance in distances)
		{
			sensor.AddObservation(distance);
		}
	}

	public override void OnActionReceived(ActionBuffers actionBuffers)
	{
		float verticalAxis = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
		float horizontalAxis = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

		carController.SetInput(verticalAxis, horizontalAxis);

		/* INSERT REWARDS HERE */
	}

	public override void OnEpisodeBegin()
	{

	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var continuousActionsOut = actionsOut.ContinuousActions;
		continuousActionsOut[0] = Input.GetAxis("Vertical");
		continuousActionsOut[1] = Input.GetAxis("Horizontal");
	}
}