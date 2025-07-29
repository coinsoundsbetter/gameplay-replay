using System;
using System.Collections.Generic;
using UnityEngine;

namespace KillCam {
    public class DelayUtils : MonoBehaviour {
        private static DelayUtils _instance;

        private class DelayAction {
            public float countDown;
            public Action trigger;
        }

        private List<DelayAction> delayActions = new List<DelayAction>();
        private Queue<int> pendingRemoves = new Queue<int>();

        private void Awake() {
            _instance = this;
        }

        private void Update() {
            for (int i = 0; i < delayActions.Count; i++) {
                var task = delayActions[i];
                task.countDown -= Time.deltaTime;
                if (task.countDown <= 0) {
                    task.trigger?.Invoke();
                    pendingRemoves.Enqueue(i);
                }
            }

            for (int i = 0; i < pendingRemoves.Count; i++) {
                delayActions.RemoveAt(pendingRemoves.Dequeue());
            }
        }

        public static void SetDelay(float delay, Action action) {
            if (_instance == null) {
                Debug.LogError("[DelayUtils] There is no instance of DelayUtils");
                return;
            }

            _instance.delayActions.Add(new DelayAction() {
                countDown = delay,
                trigger = action,
            });
        }
    }
}