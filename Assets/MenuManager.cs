using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject modeSelectionCanvas;
    public GameObject songSelectionCanvas;
    public GameObject pianoCanvas;

    private void Start()
    {
        ShowMainMenu(); // Start by showing the main menu only
    }

    public void ShowMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        modeSelectionCanvas.SetActive(false);
        songSelectionCanvas.SetActive(false);
        pianoCanvas.SetActive(false);
    }

    public void ShowModeSelection()
    {
        mainMenuCanvas.SetActive(false);
        modeSelectionCanvas.SetActive(true);
        songSelectionCanvas.SetActive(false);
        pianoCanvas.SetActive(false);
    }

    public void ShowSongSelection()
    {
        mainMenuCanvas.SetActive(false);
        modeSelectionCanvas.SetActive(false);
        songSelectionCanvas.SetActive(true);
        pianoCanvas.SetActive(false);
    }

    public void ShowPianoCanvas()
    {
        mainMenuCanvas.SetActive(false);
        modeSelectionCanvas.SetActive(false);
        songSelectionCanvas.SetActive(false);
        pianoCanvas.SetActive(true);
    }
}