using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject _actualMenu;

    [SerializeField] GameObject _spriteMuteSound;
    [SerializeField] GameObject _spriteMuteMusic;

    bool _isMusicMute = false;
    bool _isSoundMute = false;

    public void GoToNewMenu(GameObject newMenu)
    {
        _actualMenu.SetActive(false);
        _actualMenu = newMenu;
        _actualMenu.SetActive(true);
    }

    public void MuteSound()
    {
        _isSoundMute = !_isSoundMute;

        if (_isSoundMute)
        {
            SoundManager.instance.MuteAllSounds();
            _spriteMuteSound.gameObject.SetActive(true);
        }
        else
        {
            SoundManager.instance.UnmuteAllSounds();
            _spriteMuteSound.gameObject.SetActive(false);
        }
    }

    public void MuteMusic()
    {
        _isMusicMute = !_isMusicMute;

        if (_isMusicMute)
        {
            SoundManager.instance.MuteAllMusic();
            _spriteMuteMusic.gameObject.SetActive(true);
        }
        else
        {
            SoundManager.instance.UnmuteAllMusic();
            _spriteMuteMusic.gameObject.SetActive(false);
        }
    }

    public void ChangeSoundVolume(float value)
    {
        SoundManager.instance.ChangeVolumeSound(value);
    }

    public void ChangeMusicVolum(float value)
    {
        SoundManager.instance.ChangeVolumeMusic(value);
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
