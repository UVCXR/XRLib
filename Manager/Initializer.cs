using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WIFramework
{
    public partial class Initializer : MonoBehaviour
    {
        Dictionary<MonoBehaviour, bool> activeTable;
        public List<MonoBehaviour> targets = new List<MonoBehaviour>();
        public void GetAllInitializer()
        {
            var sceneRootObjs = SceneManager.GetActiveScene().GetRootGameObjects();

            targets.Clear();
            foreach (var obj in sceneRootObjs)
            {
                var inits = obj.transform.FindAll<MonoBehaviour>();
                targets.AddRange(inits);
            }
        }
        void Awake()
        {
            Debug.Log("Initializing");
            GetAllInitializer();
            activeTable = new Dictionary<MonoBehaviour, bool>();
            foreach (var target in targets)
            {
                activeTable.Add(target, target.gameObject.activeSelf);
                target.gameObject.SetActive(true);
            }

            foreach (var a in activeTable)
            {
                if (a.Key.gameObject != null && a.Value == false)
                {
                    a.Key.gameObject.SetActive(false);
                }
            }
        }

    }
}