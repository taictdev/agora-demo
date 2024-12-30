﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Skywatch.AssetManagement
{
    //Special thanks to TextusGames for their forum post: https://forum.unity.com/threads/how-to-get-asset-and-its-guid-from-known-lable.756560/
    public class LoadAssetsByLabelOperation : AsyncOperationBase<List<object>>
    {
        string _label;
        Dictionary<object, AsyncOperationHandle> _loadedDictionary;
        Dictionary<object, AsyncOperationHandle> _loadingDictionary;
        Dictionary<string, List<object>> _loadedLableAssets;
        Action<object, AsyncOperationHandle> _loadedCallback;

        public LoadAssetsByLabelOperation(Dictionary<object, AsyncOperationHandle> loadedDictionary, Dictionary<object, AsyncOperationHandle> loadingDictionary,
            Dictionary<string, List<object>> loadedLableAssets,
            string label, Action<object, AsyncOperationHandle> loadedCallback)
        {
            _loadedDictionary = loadedDictionary;
            if (_loadedDictionary == null)
                _loadedDictionary = new Dictionary<object, AsyncOperationHandle>();
            _loadingDictionary = loadingDictionary;
            if (_loadingDictionary == null)
                _loadingDictionary = new Dictionary<object, AsyncOperationHandle>();
            _loadedLableAssets = loadedLableAssets;
            if (_loadedLableAssets == null)
            {
                _loadedLableAssets = new Dictionary<string, List<object>>();
            }
            if (!_loadedLableAssets.ContainsKey(label))
            {
                _loadedLableAssets.Add(label, new List<object>());
            }
            _loadedCallback = loadedCallback;

            _label = label;
        }

        protected override void Execute()
        {
            #pragma warning disable CS4014
            DoTask();
            #pragma warning restore CS4014
        }

        public async UniTask DoTask()
        {
            var locationsHandle = Addressables.LoadResourceLocationsAsync(_label);
            var locations = await locationsHandle.ToUniTask();

            var loadingInternalIdDic = new Dictionary<string, AsyncOperationHandle<Object>>();
            var loadedInternalIdDic = new Dictionary<string, AsyncOperationHandle<Object>>();

            var operationHandles = new List<AsyncOperationHandle<Object>>();
            foreach (var resourceLocation in locations)
            {
                AsyncOperationHandle<Object> loadingHandle = Addressables.LoadAssetAsync<Object>(resourceLocation.PrimaryKey);

                operationHandles.Add(loadingHandle);

                if (!loadingInternalIdDic.ContainsKey(resourceLocation.InternalId))
                    loadingInternalIdDic.Add(resourceLocation.InternalId, loadingHandle);

                loadingHandle.Completed += assetOp =>
                {
                    if (!loadedInternalIdDic.ContainsKey(resourceLocation.InternalId))
                        loadedInternalIdDic.Add(resourceLocation.InternalId, assetOp);
                };
            }
            
            foreach (var locator in Addressables.ResourceLocators)
            {
                foreach (var key in locator.Keys)
                {
                    bool isGUID = Guid.TryParse(key.ToString(), out var guid);
                    if (!isGUID)
                        continue;
                    
                    if (!TryGetKeyLocationID(locator, key, out var keyLocationID))
                        continue;

                    var locationMatched = loadingInternalIdDic.TryGetValue(keyLocationID, out var loadingHandle);
                    if (!locationMatched)
                        continue;

                    if (!_loadingDictionary.ContainsKey(key))
                        _loadingDictionary.Add(key, loadingHandle);
                }
            }

            foreach (var handle in operationHandles)
                await handle.ToUniTask();

            foreach (var locator in Addressables.ResourceLocators)
            {
                foreach (var key in locator.Keys)
                {
                    bool isGUID = Guid.TryParse(key.ToString(), out var guid);
                    if (!isGUID)
                        continue;
                    
                    if (!TryGetKeyLocationID(locator, key, out var keyLocationID))
                        continue;

                    var locationMatched = loadedInternalIdDic.TryGetValue(keyLocationID, out var loadedHandle);
                    if (!locationMatched)
                        continue;

                    if (_loadingDictionary.ContainsKey(key))
                        _loadingDictionary.Remove(key);
                    if (!_loadedDictionary.ContainsKey(key))
                    {
                        _loadedDictionary.Add(key, loadedHandle);
                        _loadedCallback?.Invoke(key, loadedHandle);
                    }
                }
            }

            List<object> result = new List<object>();
            foreach(var op in operationHandles)
            {
                result.Add(op.Result);
            }
            Complete(result, true, string.Empty);
        }

        bool TryGetKeyLocationID(IResourceLocator locator, object key, out string internalID)
        {
            internalID = string.Empty;
            var hasLocation = locator.Locate(key, typeof(Object), out var keyLocations);
            if (!hasLocation)
                return false;
            if (keyLocations.Count == 0)
                return false;
            if (keyLocations.Count > 1)
                return false;

            internalID = keyLocations[0].InternalId;
            return true;
        }
    }
}