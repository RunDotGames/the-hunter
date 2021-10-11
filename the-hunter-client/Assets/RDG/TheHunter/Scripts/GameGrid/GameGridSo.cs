using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDG.TheHunter {

  [Serializable]
  public class GameGridConfig {
    public Vector2Int gridSize;
    public Vector2 cellSize;
    public GameObject cellPrefab;

  }

  public class GridNode {
    private readonly List<Guid> items;
    
    public readonly GameObject Object;
    public readonly Guid Guid;
    public readonly Vector2Int Position;
    public IEnumerable<Guid> Items => items;
    
    public GridNode(GameObject obj, Guid guid, Vector2Int pos, List<Guid> items) {
      Object = obj;
      Guid = guid;
      Position = pos;
      this.items = items;
    }
  }

  
  [CreateAssetMenu(menuName = "RDG/The Hunter/Game Grid")]
  public class GameGridSo : ScriptableObject {
    private static readonly Vector2Int[]  Directionals = {
      Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up,
    };

    public GameGridConfig config;

    private Dictionary<Guid, GridNode> guidIndex = new Dictionary<Guid, GridNode>();
    private Dictionary<Vector2Int, GridNode> posIndex = new Dictionary<Vector2Int, GridNode>();
    private Dictionary<Guid,  Guid> itemIndex = new Dictionary<Guid, Guid>();
    private Dictionary<Guid, List<Guid>> reverseItemIndex = new Dictionary<Guid, List<Guid>>();
    public void Begin(Transform transform) {
      guidIndex = new Dictionary<Guid, GridNode>();
      posIndex = new Dictionary<Vector2Int, GridNode>();
      itemIndex = new Dictionary<Guid, Guid>();
      reverseItemIndex = new Dictionary<Guid, List<Guid>>();
      for (var x = 0; x < config.gridSize.x; x++) {
        for (var y = 0; y < config.gridSize.y; y++) {
          var position = new Vector2Int(x, y);
          var guid = Guid.NewGuid();
          var worldPosition = Vector3.right + Vector3.forward;
          worldPosition.Scale(new Vector3(position.x, 0, position.y));
          worldPosition.Scale(new Vector3(config.cellSize.x, 0, config.cellSize.y));
          var nodeObject = Instantiate(config.cellPrefab, worldPosition, Quaternion.identity, transform);
          nodeObject.name = $"GridCell|{position.x}.{position.y}";
          var items = new List<Guid>();
          var node = new GridNode(nodeObject, guid, position, items);
          guidIndex[guid] = node;
          posIndex[position] = node;
          reverseItemIndex[guid] = items;
        }
      }
    }

    public void PlaceItemAt(Guid item, Guid at) {
      var wasAt = WhereIsItem(item);
      if (wasAt != Guid.Empty) {
        reverseItemIndex[wasAt].Remove(item);
      }
      
      itemIndex[item] = at;
      reverseItemIndex[at].Add(item);
    }

    public Guid WhereIsItem(Guid item) {
      return itemIndex.TryGetValue(item, out var at) ? at : Guid.Empty;
    }

    public GridNode GetNode(int x, int y) {
      return GetNode(new Vector2Int(x, y));
    }

    public GridNode GetNode(Vector2Int position) {
      return posIndex.TryGetValue(position, out var node) ? node : null;
    }

    public GridNode GetNode(Guid guid) {
      return guidIndex.TryGetValue(guid, out var node) ? node : null;
    }

    public List<GridNode> GetAdjacent(Guid guid) {
      var adj = new List<GridNode>();
      var node = GetNode(guid);
      if (node == null) {
        return adj;
      }

      foreach (var dir in Directionals) {
        var pos = node.Position + dir;
        var adjNode = GetNode(pos);
        if (adjNode == null) {
          continue;
        }
        adj.Add(adjNode);
      }
      return adj;
    }

    public Vector2Int Size => config.gridSize;

    public void Release() {
      //Free Any Action Refs
    }
  }
}
