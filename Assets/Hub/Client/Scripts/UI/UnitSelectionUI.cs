using System;
using Hub.Client.Scripts.MonoBehaviours;
using UnityEngine;

namespace Hub.Client.Scripts.UI
{
    public class UnitSelectionUI : MonoBehaviour
    {
        [SerializeField] RectTransform _selectionAreaRectTransform;
        [SerializeField] Canvas _canvas;

        void Start()
        {
            var unitSelection = UnitSelectionManager.Instance;
            unitSelection.OnSelectionAreaStart += OnSelectionStart;
            unitSelection.OnSelectionAreaEnd += OnSelectionEnd;

            _selectionAreaRectTransform.gameObject.SetActive(false);
        }

        void OnSelectionStart(object sender, EventArgs e)
        {
            _selectionAreaRectTransform.gameObject.SetActive(true);
        }

        void OnSelectionEnd(object sender, EventArgs e)
        {
            _selectionAreaRectTransform.gameObject.SetActive(false);
            _selectionAreaRectTransform.sizeDelta = Vector2.zero;
        }

        void Update()
        {
            if (_selectionAreaRectTransform.gameObject.activeSelf)
                UpdateVisual();
        }

        void UpdateVisual()
        {
            Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();

            float canvasScale = _canvas.transform.localScale.x;
            _selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y) / canvasScale;
            _selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height) / canvasScale;
        }
    }
}