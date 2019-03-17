using DarkRift;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Since <see cref="ObjectCacheSettings"/> uses properties, Unity can't serialize it to we clone it here and provide conversion methods.
/// </summary>
[Serializable]
public sealed class SerializableObjectCacheSettings
{
    [Tooltip("")]
    [SerializeField]
    int maxWriters = 2;

    [Tooltip("")]
    [SerializeField]
    int maxReaders = 2;

    [Tooltip("")]
    [SerializeField]
    int maxMessages = 4;

    [Tooltip("")]
    [SerializeField]
    int maxMessageBuffers = 4;

    [Tooltip("")]
    [SerializeField]
    int maxSocketAsyncEventArgs = 32;

    [Tooltip("")]
    [SerializeField]
    int maxActionDispatcherTasks = 16;

    [Tooltip("")]
    [SerializeField]
    int maxAutoRecyclingArrays = 4;

    [Tooltip("")]
    [SerializeField]
    int extraSmallMemoryBlockSize = 16;

    [Tooltip("")]
    [SerializeField]
    int maxExtraSmallMemoryBlocks = 2;

    [Tooltip("")]
    [SerializeField]
    int smallMemoryBlockSize = 64;

    [Tooltip("")]
    [SerializeField]
    int maxSmallMemoryBlocks = 2;

    [Tooltip("")]
    [SerializeField]
    int mediumMemoryBlockSize = 256;

    [Tooltip("")]
    [SerializeField]
    int maxMediumMemoryBlocks = 2;

    [Tooltip("")]
    [SerializeField]
    int largeMemoryBlockSize = 1024;

    [Tooltip("")]
    [SerializeField]
    int maxLargeMemoryBlocks = 2;

    [Tooltip("")]
    [SerializeField]
    int extraLargeMemoryBlockSize = 4096;

    [Tooltip("")]
    [SerializeField]
    int maxExtraLargeMemoryBlocks = 2;

    public ObjectCacheSettings ToObjectCacheSettings()
    {
        ObjectCacheSettings objectCacheSettings = new ObjectCacheSettings();
        objectCacheSettings.MaxWriters = maxWriters;
        objectCacheSettings.MaxReaders = maxReaders;
        objectCacheSettings.MaxMessages = maxMessages;
        objectCacheSettings.MaxMessageBuffers = maxMessageBuffers;
        objectCacheSettings.MaxSocketAsyncEventArgs = maxSocketAsyncEventArgs;
        objectCacheSettings.MaxActionDispatcherTasks = maxActionDispatcherTasks;
        objectCacheSettings.MaxAutoRecyclingArrays = maxAutoRecyclingArrays;

        objectCacheSettings.ExtraSmallMemoryBlockSize = extraSmallMemoryBlockSize;
        objectCacheSettings.MaxExtraSmallMemoryBlocks = maxExtraSmallMemoryBlocks;
        objectCacheSettings.SmallMemoryBlockSize = smallMemoryBlockSize;
        objectCacheSettings.MaxSmallMemoryBlocks = maxSmallMemoryBlocks;
        objectCacheSettings.MediumMemoryBlockSize = mediumMemoryBlockSize;
        objectCacheSettings.MaxMediumMemoryBlocks = maxMediumMemoryBlocks;
        objectCacheSettings.LargeMemoryBlockSize = largeMemoryBlockSize;
        objectCacheSettings.MaxLargeMemoryBlocks = maxLargeMemoryBlocks;
        objectCacheSettings.ExtraLargeMemoryBlockSize = extraLargeMemoryBlockSize;
        objectCacheSettings.MaxExtraLargeMemoryBlocks = maxExtraLargeMemoryBlocks;
        return objectCacheSettings;
    }
}
