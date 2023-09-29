using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instace;    

    [Space]
    [Header("Touch Inputs")]
    [SerializeField]
    public float touchSensitivity = 0.1f;    

    [SerializeField]
    private TMP_Text display_Text;

    private void Awake()
    {
        if (Instace == null)
        {
            Instace = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 120;
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    private void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        var avgFrameRate = (int)current;
        if (display_Text != null)
            display_Text.text = avgFrameRate.ToString() + " FPS";
    }

    public TMP_Text FPSText()
    {
        return display_Text;
    }
}
