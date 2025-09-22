using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireDamageScript : MonoBehaviour
{
    public float Speed;
    public SpeedCalculator Player;

    public GameObject TireMesh;
    public GameObject TireRigid;

    public WheelCollider TheWheelCollider;

    public GameObject PopedTireSpawnPos;

    public float TirePopSpeed;


    public void OnTriggerEnter(Collider other)
    {
        Speed = Player.Speed;

        if(Speed > TirePopSpeed)
        {
            if (other.tag != "Player" && other.tag != "Trigger")
            {
                TireMesh.SetActive(false);
                Instantiate(TireRigid, PopedTireSpawnPos.transform.position, PopedTireSpawnPos.transform.rotation);
                TheWheelCollider.radius = 0;
                gameObject.SetActive(false);
            }
        }

    }

}
