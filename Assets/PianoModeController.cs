using UnityEngine;

public class PianoModeController : MonoBehaviour
{
    public static PianoModeController Instance { get; private set; } // Singleton instance
    public MidiFileReader midiFileReader; // Reference to your MidiFileReader GameObject
    public enum Mode { Freestyle, Auto, Practice }
    public Mode currentMode = Mode.Freestyle;

    private void Awake()
    {
        // Ensure only one instance of PianoModeController exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }
        Instance = this; // Set this instance as the singleton
        DontDestroyOnLoad(gameObject); // Optional: keeps this instance across scenes
    }

    public void SetFreestyleMode()
    {
        currentMode = Mode.Freestyle;
        midiFileReader.StopMidiPlayback(); // Stops any playback
    }

    public void SetAutoMode()
    {
        currentMode = Mode.Auto;
        midiFileReader.PlayMidiFile(); // Starts auto playback with audio
    }

    public void SetPracticeMode()
    {
        currentMode = Mode.Practice;
        midiFileReader.PlayMidiFileWithoutAudio(); // Starts playback without audio
    }

    public bool IsAutoMode() => currentMode == Mode.Auto;
    public bool IsPracticeMode() => currentMode == Mode.Practice;
}
