using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class EnemyAgent : Agent
{
    public Transform target;
    public float moveSpeed = 5f;


    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0f);
        target.localPosition = new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions
        float moveX = actionBuffers.ContinuousActions[0];
        float moveY = actionBuffers.ContinuousActions[1];

        Vector3 velocity = new Vector3(moveX, moveY) * Time.deltaTime * moveSpeed;
        velocity = velocity.normalized * Time.deltaTime;

        transform.localPosition += velocity;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxisRaw("Horizontal");
        continuousActionsOut[1] = Input.GetAxisRaw("Vertical");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            AddReward(2f);
            EndEpisode();
        }
        if (collision.gameObject.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}
