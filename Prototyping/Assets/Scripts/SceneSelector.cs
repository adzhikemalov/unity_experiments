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
    
    [SerializeField] private List<Animator> _animators;
    private Animator _animator;
    private int _sceneIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(_eventSystem);
        DontDestroyOnLoad(_canvas);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_nextScene.gameObject);
        _nextScene.onClick.AddListener(LoadNextScene);
        SceneManager.sceneLoaded += OnSceneLoaded;
        _animator = GetCurrentAnimator();
        foreach (var animator in _animators)
        {
            animator.Play("Close");
        }
    }

    private Animator GetCurrentAnimator()
    {
        var val = Random.Range(0, _animators.Count);
        var currAnimator = _animators[val];
        return currAnimator;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(ShowCurrentScene());
    }

    public void LoadNextScene()
    {
        _animator.Play("Expand");
        StartCoroutine(LoadCurrentScene());
    }

    private IEnumerator ShowCurrentScene()
    {
        yield return new WaitForSeconds(1);
        _animator.Play("Close");
        
        _animator = GetCurrentAnimator();
    }
    private IEnumerator LoadCurrentScene()
    {
        yield return new WaitForSeconds(1);
        _sceneIndex++;
        SceneManager.LoadScene(_sceneIndex, LoadSceneMode.Single);
    }
}
