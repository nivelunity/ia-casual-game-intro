using UnityEngine;

public class BackgroundMusic : MonoBehaviour {
    public static BackgroundMusic instance;
    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
