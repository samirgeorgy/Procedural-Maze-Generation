using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavigation : MonoBehaviour
{
    #region Private Variables

    private NavMeshAgent agent;     //The navmesh agent

    #endregion

    #region Unity Functions

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check for left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            //Getting the mouse position and converting it to a ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 500))
            {
                if (hitInfo.collider.tag.Equals("Floor"))
                {
                    agent.destination = hitInfo.point;
                }
            }
        }
    }

    #endregion
}
