using UnityEngine;
using UnityEngine.Events;

namespace Connect4.UI
{
    public class MouseClickHandler : MonoBehaviour
    {
        public UnityEvent onMouseClick;

        private void OnMouseDown()
        {
            this.onMouseClick?.Invoke();
        }
    }
}