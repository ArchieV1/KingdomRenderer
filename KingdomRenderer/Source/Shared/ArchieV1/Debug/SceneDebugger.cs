using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HalfHeightTowers.Shared.ArchieV1.Debug
{
    public class SceneDebugger
    {
        /// <summary>
        /// Gets all of the GameObjects active in the hierarchy.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GameObject> GetActiveGameObjects()
        {
            return ActiveInHierarchy(Object.FindObjectsOfType<GameObject>());
        }

        /// <summary>
        /// Get all game objects with given tag active in the hierarchy
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static IEnumerable<GameObject> GetActiveGameObjectsByTag(string tag)
        {
            return ActiveInHierarchy(GameObject.FindGameObjectsWithTag(tag));
        }
        
        public static IEnumerable<AudioSource> GetAudioSources()
        {
            return Object.FindObjectsOfType<AudioSource>();
        }

        public static IEnumerable<AudioSource> GetActiveAudioSources()
        {
            foreach (GameObject gameObject in GetActiveGameObjects()){
                foreach (AudioSource component in gameObject.GetComponentsInChildren<AudioSource>(true)){
                    yield return component;
                }
            }
        }

        /// <summary>
        /// Gets the root level GameObjects in the current scene
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GameObject> GetRootGameObjectsInScene()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        }

        public static string GetSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        private static IEnumerable<GameObject> ActiveInHierarchy(IEnumerable<GameObject> objects)
        {
            foreach (GameObject obj in objects)
            {
                if (obj.activeInHierarchy)
                {
                    yield return obj;
                }
            }
        }
    }
}