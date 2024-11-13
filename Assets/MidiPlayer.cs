using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using System.Collections.Generic;

public class MidiPlayer : MonoBehaviour
{
    public string midiFilePath;   // path to .mid file, hope its correct lol
    public AudioSource audioSource; // this is the audio source for playing sounds

    // Start is called before the first frame update
    void Start()
    {
        PlayMidiFile(midiFilePath); // plays the midi file when i start the game
    }

    void PlayMidiFile(string filePath)
    {
        // read the midi file from the file path
        MidiFile midiFile = MidiFile.Read(filePath);

        // get the tempo map, this helps with timing stuff
        TempoMap tempoMap = midiFile.GetTempoMap();

        // grab all the notes from the midi file
        var notes = midiFile.GetNotes();

        // start a coroutine to play the notes
        StartCoroutine(PlayNotesFromMidi(notes, tempoMap));
    }

    IEnumerator PlayNotesFromMidi(IEnumerable<Note> notes, TempoMap tempoMap)
    {
        double lastNoteTime = -1;  // keeps track of the last note’s time, super important for timing

        foreach (var midiNote in notes)
        {
            // turn the note time into real-world time (using MetricTimeSpan)
            var metricTimeSpan = midiNote.TimeAs<MetricTimeSpan>(tempoMap);
            double noteTime = metricTimeSpan.TotalSeconds; // note's time in seconds

            // if the note isnt at the same time as the last one, me waits
            if (lastNoteTime != -1 && noteTime != lastNoteTime)
            {
                // wait before me plays the next note
                yield return new WaitForSeconds((float)(noteTime - lastNoteTime));
            }

            // update the last note’s time to this note's time
            lastNoteTime = noteTime;

            // play the sound for this note
            PlaySoundForNoteAndSpawn(midiNote.NoteNumber); // spawn sound for this note
        }
    }

    void PlaySoundForNoteAndSpawn(int midiNoteNumber)
    {
        // create a new GameObject with an AudioSource to play the sound
        GameObject noteSoundObject = new GameObject("Note_" + midiNoteNumber);
        AudioSource noteAudioSource = noteSoundObject.AddComponent<AudioSource>();

        // assign the same audio clip and adjust pitch based on the midi note
        noteAudioSource.clip = audioSource.clip;  // use the audio clip
        float pitch = Mathf.Pow(2f, (midiNoteNumber - 60) / 12f);  // adjust pitch for each note
        noteAudioSource.pitch = pitch; // set the pitch

        // play the sound for this note
        noteAudioSource.Play();

        // destroy the sound object after the sound finishes so it doesnt pile up
        Destroy(noteSoundObject, noteAudioSource.clip.length / noteAudioSource.pitch);
    }
}