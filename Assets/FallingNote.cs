using UnityEngine;

public class FallingNote : MonoBehaviour
{
    private float fallSpeed = 20.0f;
    public double noteLength;
    public bool playAudioOnHit;
    public AudioClip audioSourceClip;
    public int midiNoteNumber;

    public void SetFallSpeed(float speed)
    {
        fallSpeed = speed;
    }

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -2)
        {
            if (playAudioOnHit)
            {
                FindObjectOfType<MidiFileReader>().PlayAudioForNoteOnDestruction(midiNoteNumber, noteLength);
            }
            Destroy(gameObject);
        }
    }
}
