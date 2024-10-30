using UnityEngine;
using System.Collections;

public class PianoKey : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource component
    public AudioClip longC4NoteClip; // Assign the Long C4 clip here
    private int midiNoteNumber; // MIDI note number for this key
    private bool isPlaying = false; // Tracks if the note is currently playing

    private void Start()
    {
        audioSource.clip = longC4NoteClip; // Set the audio clip
        AssignMidiNoteBasedOnKeyName();    // Assign MIDI note based on the key's name
    }

    private void AssignMidiNoteBasedOnKeyName()
    {
        string keyName = gameObject.name;
        if (keyName.StartsWith("Key_"))
        {
            if (int.TryParse(keyName.Substring(4), out midiNoteNumber))
            {
                Debug.Log("Assigned MIDI note " + midiNoteNumber + " to " + keyName);
            }
            else
            {
                Debug.LogWarning("Failed to parse MIDI note number from key name: " + keyName);
            }
        }
        else
        {
            Debug.LogWarning("Key name format incorrect. Expected format 'Key_XX'. Got: " + keyName);
        }
    }

    private void OnMouseDown()
    {
        if (!PianoModeController.Instance.IsAutoMode() && !isPlaying)
        {
            audioSource.pitch = Mathf.Pow(2f, (midiNoteNumber - 60) / 12f); // Adjust pitch relative to C4
            audioSource.volume = 0f; // Start volume at 0 for fade-in
            audioSource.loop = true; // Enable looping for continuous playback
            audioSource.Play();
            StartCoroutine(FadeIn(audioSource, 0.05f)); // Fade in over 0.05 seconds
            isPlaying = true;
        }
    }

    private void OnMouseUp()
    {
        if (isPlaying)
        {
            StartCoroutine(FadeOutAndStop(audioSource, 0.05f)); // Fade out over 0.05 seconds
            isPlaying = false;
        }
    }

    private IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float startVolume = 0f;
        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.volume = 1f;
    }

    private IEnumerator FadeOutAndStop(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.Stop(); // Stop audio after fade-out
        audioSource.volume = 1f; // Reset volume for next play
    }
}
