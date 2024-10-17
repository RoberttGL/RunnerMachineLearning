using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEditor.Presets;
using TMPro;

public class PlayerML : Agent
{
    private int score = 0;
    public int jumpForce;
    private bool jumpAllowed;
    Vector2 posicioIncial;
    public TextMeshProUGUI scoreText;
    public List<GameObject> chemicals;
    public List<GameObject> spikes;

    public delegate void RestartDelegate();
    public event RestartDelegate Restart;

    void Start()
    {
        StartCoroutine(RewardPoints());

    }


    void Update()
    {
        scoreText.text = score.ToString();
    }

    private void FixedUpdate()
    {
        if (jumpAllowed)
        {
            RequestDecision();
        }
    }
    public override void Initialize()
    {
        base.Initialize();
        posicioIncial = this.transform.position;

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        if (actions.DiscreteActions[0] == 1)
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (jumpAllowed)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
            jumpAllowed = false;
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        score = 0;
        jumpAllowed = true;
        transform.position = posicioIncial;
        Restart?.Invoke();

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Obstacle")
        {
            AddReward(-10f);
            Debug.Log("rewards: " + GetCumulativeReward());
            Debug.Log("score: " + score);
            foreach (GameObject s in spikes)
            {
                Destroy(s.gameObject);
            }
            spikes.Clear();
            foreach (GameObject s in chemicals)
            {
                Destroy(s.gameObject);
            }
            chemicals.Clear();
            EndEpisode();
        }
        if (collision.transform.tag == "Chemicals")
        {
            Destroy(collision.gameObject);
            AddReward(1f);
            score++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Floor")
        {
            jumpAllowed = true;
        }
    }

    IEnumerator RewardPoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            AddReward(0.1f);

        }
    }
}
