using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Presentation
{
    public sealed class ErrorOverlayView : IErrorPresenter, IDisposable
    {
        private const string BuiltinFontName = "LegacyRuntime.ttf";
        private const string OverlayObjectName = "ErrorOverlay";
        private const string MessageObjectName = "ErrorMessage";
        private const string EmptyMessageFallback = "Ошибка без текста.";
        private const int OverlaySortingOrder = 30000;
        private const int MessageFontSize = 24;
        private const float MessagePadding = 32f;
        private const float ReferenceWidth = 1920f;
        private const float ReferenceHeight = 1080f;

        private GameObject _root;
        private Text _message;
        private bool _disposed;

        public void Show(string message)
        {
            var text = string.IsNullOrWhiteSpace(message) ? EmptyMessageFallback : message;

            Debug.LogError(text);

            if (_disposed)
            {
                return;
            }

            if (_root == null)
            {
                Create();
            }

            if (_message != null)
            {
                _message.text = text;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_root != null)
            {
                UnityEngine.Object.Destroy(_root);
            }

            _root = null;
            _message = null;
        }

        private void Create()
        {
            var root = new GameObject(OverlayObjectName, typeof(Canvas), typeof(CanvasScaler));

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = OverlaySortingOrder;

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var messageObject = new GameObject(MessageObjectName, typeof(Text));
            messageObject.transform.SetParent(root.transform, false);

            var rect = messageObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(MessagePadding, MessagePadding);
            rect.offsetMax = new Vector2(-MessagePadding, -MessagePadding);

            var message = messageObject.GetComponent<Text>();
            var font = Resources.GetBuiltinResource<Font>(BuiltinFontName);

            if (font == null)
            {
                Debug.LogError($"Нет встроенного шрифта {BuiltinFontName}, текст пойдёт без него.");
            }

            message.font = font;
            message.fontSize = MessageFontSize;
            message.alignment = TextAnchor.MiddleCenter;
            message.horizontalOverflow = HorizontalWrapMode.Wrap;
            message.verticalOverflow = VerticalWrapMode.Overflow;
            message.color = Color.white;

            var outline = messageObject.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2f, -2f);

            _root = root;
            _message = message;
        }
    }
}
