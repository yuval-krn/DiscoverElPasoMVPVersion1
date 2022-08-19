using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPosition : MonoBehaviour {

    public GameObject follow;

      void LateUpdate()
      {
            transform.position = follow.transform.position;
      }
}
