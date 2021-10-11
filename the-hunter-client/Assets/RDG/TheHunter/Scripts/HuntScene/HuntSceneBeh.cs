using System;
using UnityEngine;

namespace RDG.TheHunter {
  public class HuntSceneBeh : MonoBehaviour {

    public HuntSceneSo huntScene;
    
    
    public void Start() {
      huntScene.Begin(transform);  
    }

    public void OnDestroy() {
      huntScene.Release();
    }
  }
}
