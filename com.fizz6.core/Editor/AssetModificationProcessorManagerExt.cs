using System;

namespace Fizz6.Core.Editor
{
    public static class AssetModificationProcessorManagerExt
    {
        private class Handle : IDisposable
        {
            private event Action DisposeEvent;

            public Handle(Action onDispose) =>
                DisposeEvent = onDispose;

            public void Dispose() =>
                DisposeEvent?.Invoke();
        }

        public static IDisposable OnWillCreateAsset(AssetModificationProcessorManager.WillCreateAssetCallback willCreateAssetCallback)
        {
            void OnDispose() =>
                AssetModificationProcessorManager.RemoveWillCreateAssetCallback(willCreateAssetCallback);
            AssetModificationProcessorManager.AddWillCreateAssetCallback(willCreateAssetCallback);
            var handle = new Handle(OnDispose);
            return handle;
        }

        public static IDisposable OnWillCreateAssetAtPath(string assetPath, AssetModificationProcessorManager.WillCreateAssetCallback willCreateAssetCallback)
        {
            void OnDispose() =>
                AssetModificationProcessorManager.RemoveWillCreateAssetAtPathCallback(assetPath, willCreateAssetCallback);
            AssetModificationProcessorManager.AddWillCreateAssetAtPathCallback(assetPath, willCreateAssetCallback);
            var handle = new Handle(OnDispose);
            return handle;
        }

        public static IDisposable OnWillDeleteAsset(AssetModificationProcessorManager.WillDeleteAssetCallback willDeleteAssetCallback)
        {
            void OnDispose() =>
                AssetModificationProcessorManager.RemoveWillDeleteAssetCallback(willDeleteAssetCallback);
            AssetModificationProcessorManager.AddWillDeleteAssetCallback(willDeleteAssetCallback);
            var handle = new Handle(OnDispose);
            return handle;
        }

        public static IDisposable OnWillDeleteAssetAtPath(string assetPath, AssetModificationProcessorManager.WillDeleteAssetCallback willDeleteAssetCallback)
        {
            void OnDispose() =>
                AssetModificationProcessorManager.RemoveWillDeleteAssetAtPathCallback(assetPath, willDeleteAssetCallback);
            AssetModificationProcessorManager.AddWillDeleteAssetAtPathCallback(assetPath, willDeleteAssetCallback);
            var handle = new Handle(OnDispose);
            return handle;
        }

        public static IDisposable OnWillDeleteAssetOfType<T>(AssetModificationProcessorManager.WillDeleteAssetCallback willDeleteAssetCallback)
        {
            void OnDispose() =>
                AssetModificationProcessorManager.RemoveWillDeleteAssetOfTypeCallback<T>(willDeleteAssetCallback);
            AssetModificationProcessorManager.AddWillDeleteAssetOfTypeCallback<T>(willDeleteAssetCallback);
            var handle = new Handle(OnDispose);
            return handle;
        }
    }
}
