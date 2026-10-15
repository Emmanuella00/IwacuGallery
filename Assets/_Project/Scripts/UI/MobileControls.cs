using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Shows the on-screen touch controls (sticks and Inspect button) only when the player is
/// really on a touch device, so desktop players see a clean screen.
/// - Native Android/iOS builds: shown from the start.
/// - WebGL: hidden until the first real touch (WebGL reports a touchscreen device even on
///   desktop browsers, so its mere presence cannot be trusted).
/// </summary>
public class MobileControls : MonoBehaviour
{
    [SerializeField] private GameObject controlsRoot;
    [SerializeField] private bool showInEditor = true;

    private void Awake()
    {
        bool nativeMobile = Application.platform == RuntimePlatform.Android
                            || Application.platform == RuntimePlatform.IPhonePlayer;
        bool editorTest = Application.isEditor && showInEditor;

        controlsRoot.SetActive(nativeMobile || editorTest);
    }

    private void Update()
    {
        if (controlsRoot.activeSelf) return;

        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame)
        {
            controlsRoot.SetActive(true);
        }
    }
}
