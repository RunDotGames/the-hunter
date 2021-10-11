using System;
using UnityEngine;

namespace RDG.TheHunter {
  
  public class SpawnerBeh : MonoBehaviour {
    public GameObject toSpawn;

    public void Start() {
      Instantiate(toSpawn, Vector3.zero, Quaternion.identity, transform.parent);
    }
  }
}
