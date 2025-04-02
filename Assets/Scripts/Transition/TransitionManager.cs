using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Transition
{
        

public class TransitionManager : MonoBehaviour
{
        [SceneName]
        public string startSceneName = string.Empty;

        private CanvasGroup fadeCanvasGroup;

        private bool isFade;
        private IEnumerator Start()
        { 
                fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
                yield return LoadSceneSetActive(startSceneName);
                EventHandler.CallAfterSceneLoadEvent();
        }

        private void OnEnable()
        {
                EventHandler.TransitionEvent+= OnTransitionEvent;
        }
        private void OnDisable()
        {
                EventHandler.TransitionEvent-= OnTransitionEvent;
        }

        /// <summary>
        /// 加载场景并设置激活
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
                yield return SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);

                Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                SceneManager.SetActiveScene(newScene);
        }

        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="targetPosition">传送坐标</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName,Vector3 targetPosition)
        {
                EventHandler.CallBeforeSceneUnloadEvent();

                yield return Fade(1);
                
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

                
                yield return LoadSceneSetActive(sceneName);
                EventHandler.CallMoveToPosition(targetPosition);
                EventHandler.CallAfterSceneLoadEvent();
                yield return Fade(0);

        }
        private void OnTransitionEvent(string sceneName,Vector3 targetPosition)
        {
                if(!isFade)
                        StartCoroutine(Transition(sceneName,targetPosition));
        }

        /// <summary>
        /// 淡入淡出场景
        /// </summary>
        /// <param name="targetAlpha">1为黑0为透明</param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
                isFade = true;
                fadeCanvasGroup.blocksRaycasts = true;
                float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha)/Settings.fadeDuration;
                while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
                {
                        fadeCanvasGroup.alpha =
                                Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, Time.deltaTime * speed);
                        yield return null;
                }
                fadeCanvasGroup.blocksRaycasts = false;
                isFade = false;
        }
}
}