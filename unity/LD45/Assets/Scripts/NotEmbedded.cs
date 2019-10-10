using UnityEngine;

public class NotEmbedded : MonoBehaviour {
  void Start() {
#if UNITY_EDITOR || UNITY_WEBGL
    gameObject.SetActive(false);
#else
    gameObject.SetActive(true);
#endif
  }
}
