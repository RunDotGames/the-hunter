using System;
using UnityEngine;

namespace RDG.TheHunter {

  [Serializable]
  public class IndicationConfig {
    public GameObject forwardIndicatorPrefab;
  }
  [CreateAssetMenu(menuName = "RDG/The Hunter/Indication")]
  public class IndicationSo : ScriptableObject {

    public GameGridSo grid;
    public IndicationConfig config;

    private GameObject forwardIndicator;
    private Guid forwardId;

    public void Begin(Transform root) {
      forwardIndicator = Instantiate(config.forwardIndicatorPrefab, Vector3.zero, Quaternion.identity, root);
      forwardIndicator.SetActive(false);
      forwardId = Guid.Empty;
    }

    public void ForwardShow(Guid nodeId) {
      var node = grid.GetNode(nodeId);
      if (node == null) {
        return;
      }
      forwardId = nodeId;
      forwardIndicator.SetActive(true);
      forwardIndicator.transform.position = node.Object.transform.position;
    }

    public void ForwardHide() {
      forwardIndicator.SetActive(false);
      forwardId = Guid.Empty;
    }

    public Guid GetForwardNodeId() {
      return forwardId;
    }

    public void Release() {
      
    }
  }
}
