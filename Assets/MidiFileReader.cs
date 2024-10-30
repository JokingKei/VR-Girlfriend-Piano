using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using System.Collections.Generic;

public class MidiFileReader : MonoBehaviour
{
    public GameObject notePrefab;
    public string midiFilePath;
    public float noteSpeed = 20f;
    public float spawnHeight = 10f;
    public float bpm = 90f;
    public AudioSource audioSource;

    private Dictionary<int, Transform> pianoKeys = new Dictionary<int, Transform>();
    private TempoMap tempoMap;
    private bool isPlaying = false;
    private bool playAudio = true;

    void Start()
    {
        for (int i = 21; i <= 108; i++)
        {
            string keyName = "Key_" + i;
            Transform keyTransform = GameObject.Find(keyName)?.transform;
            if (keyTransform != null)
            {
                pianoKeys.Add(i, keyTransform);
            }
        }
    }

    public void PlayMidiFile()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            playAudio = true;
            LoadMidiFile(midiFilePath);
        }
    }

    public void PlayMidiFileWithoutAudio()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            playAudio = false;
            LoadMidiFile(midiFilePath);
        }
    }

    public void StopMidiPlayback()
    {
        isPlaying = false;
        StopAllCoroutines();
    }

    void LoadMidiFile(string filePath)
    {
        MidiFile midiFile = MidiFile.Read(filePath);
        tempoMap = midiFile.GetTempoMap();
        var notes = midiFile.GetNotes();
        StartCoroutine(SpawnNotesFromMidi(notes));
    }

    IEnumerator SpawnNotesFromMidi(IEnumerable<Note> notes)
    {
        double lastNoteTime = -1;
        List<Note> currentTimeGroup = new List<Note>();

        foreach (var midiNote in notes)
        {
            var noteTime = midiNote.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds;
            var noteLength = midiNote.LengthAs<MetricTimeSpan>(tempoMap).TotalSeconds;

            if (lastNoteTime == -1 || Mathf.Abs((float)(noteTime - lastNoteTime)) < 0.01f)
            {
                currentTimeGroup.Add(midiNote);
            }
            else
            {
                SpawnNoteGroup(currentTimeGroup);

                float waitTime = Mathf.Max(0.01f, (float)(noteTime - lastNoteTime));
                yield return new WaitForSeconds(waitTime);

                currentTimeGroup.Clear();
                currentTimeGroup.Add(midiNote);
            }

            lastNoteTime = noteTime;
        }

        if (currentTimeGroup.Count > 0)
        {
            SpawnNoteGroup(currentTimeGroup);
        }

        isPlaying = false;
    }

    void SpawnNoteGroup(List<Note> noteGroup)
    {
        foreach (var midiNote in noteGroup)
        {
            int midiNoteNumber = midiNote.NoteNumber;
            double noteLength = midiNote.LengthAs<MetricTimeSpan>(tempoMap).TotalSeconds;

            if (pianoKeys.ContainsKey(midiNoteNumber))
            {
                Vector3 spawnPosition = pianoKeys[midiNoteNumber].position;
                spawnPosition.y += spawnHeight;

                GameObject newNote = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
                newNote.name = "Note_" + midiNoteNumber;

                FallingNote fallingNote = newNote.GetComponent<FallingNote>();
                if (fallingNote != null)
                {
                    fallingNote.SetFallSpeed(noteSpeed);
                    fallingNote.noteLength = noteLength;
                    fallingNote.playAudioOnHit = playAudio;
                    fallingNote.audioSourceClip = audioSource.clip;
                    fallingNote.midiNoteNumber = midiNoteNumber;
                }
            }
        }
    }

    // Original audio playback logic for each note
    public void PlayAudioForNoteOnDestruction(int midiNoteNumber, double noteDuration)
    {
        GameObject noteSoundObject = new GameObject("Note_" + midiNoteNumber);
        AudioSource noteAudioSource = noteSoundObject.AddComponent<AudioSource>();
        noteAudioSource.clip = audioSource.clip;

        float pitch = Mathf.Pow(2f, (midiNoteNumber - 60) / 12f);
        noteAudioSource.pitch = pitch;

        noteAudioSource.volume = 0f;
        noteAudioSource.Play();
        StartCoroutine(FadeIn(noteAudioSource, 0.05f));

        float playDuration = Mathf.Min((float)noteDuration, noteAudioSource.clip.length / noteAudioSource.pitch);
        StartCoroutine(StopNoteWithFadeOut(noteAudioSource, playDuration, 0.05f));
    }

    IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float startVolume = 0f;
        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.volume = 1f;
    }

    IEnumerator StopNoteWithFadeOut(AudioSource audioSource, float playDuration, float fadeOutTime)
    {
        yield return new WaitForSeconds(playDuration);
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeOutTime;
            yield return null;
        }
        audioSource.Stop();
        Destroy(audioSource.gameObject);
    }
}
