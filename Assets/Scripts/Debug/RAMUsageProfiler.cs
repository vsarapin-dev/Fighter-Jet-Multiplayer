using Unity.Profiling;
using UnityEngine;

public class RAMUsageProfiler : MonoBehaviour
{
    [SerializeField] private PlayerInfoDebug _ramInfoDebugGo;

    private ProfilerRecorder _profilerRecorder;
    private float _memoryUsageMB;
    private string _statsText;

    void Start()
    {
        // Get the profiler recorder for the memory usage
        _profilerRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
    }

    void Update()
    {
        // Get the last value of the profiler recorder
        _memoryUsageMB = _profilerRecorder.LastValue / (1024f * 1024f);

        _statsText = _memoryUsageMB.ToString("F2") + " MB";
        _ramInfoDebugGo.SetUserRamValues(_statsText);
    }

    void OnDisable()
    {
        // Stop recording the profiler data
        _profilerRecorder.Dispose();
    }
}
