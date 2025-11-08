using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    private Transform target;
    public NavMeshAgent navMeshAgent;

    public void SetUp(Transform player)
    {
        //this.target = target;
        target = player;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }
    //private void Awake()
    //{
    //    SetUp();
    //}

    //private void Update()
    //{
    //    navMeshAgent.SetDestination(target.position);
    //}
}
