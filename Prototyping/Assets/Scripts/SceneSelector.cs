using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private Button _nextScene;
    [SerializeField] private Canvas _canvas;
    private int _sceneIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(_eventSystem);
        DontDestroyOnLoad(_canvas);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_nextScene.gameObject);
        _nextScene.onClick.AddListener(LoadNextScene);
    }

    void LoadNextScene()
    {
        _sceneIndex++;
        SceneManager.LoadScene(_sceneIndex, LoadSceneMode.Single);
    }
}
