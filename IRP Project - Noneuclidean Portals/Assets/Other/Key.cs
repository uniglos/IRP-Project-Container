using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private List<GameObject> walls = new List<GameObject>();
    [SerializeField] private float wallMoveTime = 2f;
    [SerializeField] private float wallMoveDistance = 5f;

    private bool pickedUp = false;
    float currentMoveProgress = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = transform.eulerAngles + new Vector3(0, Time.deltaTime * 40f, 0);

        if (pickedUp)
        {
            currentMoveProgress += Time.deltaTime;

            if (currentMoveProgress < wallMoveTime)
            {
                foreach (GameObject wall in walls)
                {
                    wall.transform.position -= new Vector3(0, (wallMoveDistance * Time.deltaTime / wallMoveTime), 0);
                }
            }
            else
            {
                pickedUp = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnPickup();
        }
    }

    private void OnPickup()
    {
        pickedUp = true;
    }
}
