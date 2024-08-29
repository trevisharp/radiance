/* Author:  Leonardo Trevisan Silio
 * Date:    16/08/2024
 */
using System;

namespace Radiance;

/// <summary>
/// A usefull class to manage timer between frames.
/// </summary>
public struct Clock
{
    /// <summary>
    /// Get the initial Utc Time from application.
    /// </summary>
    public static readonly DateTime ZeroTime = DateTime.UtcNow;

    /// <summary>
    /// Get the total seconds of running of application.
    /// </summary>
    public static float Seconds => (float)(DateTime.UtcNow - ZeroTime).TotalSeconds;

    /// <summary>
    /// A global shared reference of a clock object.
    /// </summary>
    public readonly static Clock Shared = new();

    private float lastResetStart;
    private float? holdedTime;
    private float? startFreezedTime;
    private float oldFreezedTime;

    public Clock()
    {
        lastResetStart = Seconds;
        holdedTime = null;
        startFreezedTime = null;
        oldFreezedTime = 0;
    }

    /// <summary>
    /// Get the total time passed from the last Freeze() call.
    /// </summary>
    private float LastFreezedTime => IsFreezed ? Seconds - startFreezedTime.Value : 0;

    /// <summary>
    /// Get the total time that this clock has freezed.
    /// </summary>
    private float TotalFreezedTime => oldFreezedTime + LastFreezedTime;

    /// <summary>
    /// Get the current time considering only unfreezed times.
    /// </summary>
    private float UnfreezedTime => Seconds - TotalFreezedTime;

    /// <summary>
    /// Get the current time. If the clock is holded, return the last hold time.
    /// </summary>
    private float Now => holdedTime ?? UnfreezedTime;
    
    /// <summary>
    /// Get current type between now or a holded time and last Reset in seconds.
    /// </summary>
    public float Time => Now - lastResetStart;

    /// <summary>
    /// Return if the Clock is freezed.
    /// </summary>
    public bool IsFreezed => startFreezedTime is not null;

    /// <summary>
    /// Return if the Clock is holding a time.
    /// </summary>
    public bool IsHolded => holdedTime is not null;

    /// <summary>
    /// Hold the current time has a now time.
    /// </summary>
    public void Hold()
        => holdedTime = UnfreezedTime;

    /// <summary>
    /// Release the holded time and return to normal time.
    /// </summary>
    public void Release()
        => holdedTime = null;
    
    /// <summary>
    /// Stop aplication time for this clock.
    /// </summary>
    public void Freeze()
    {
        Unfreeze();
        startFreezedTime = Seconds;
    }

    /// <summary>
    /// Unstop application time for this clock.
    /// </summary>
    public void Unfreeze()
    {
        if (!IsFreezed)
            return;
        
        oldFreezedTime += Seconds - startFreezedTime.Value;
        startFreezedTime = null;
    }

    /// <summary>
    /// Toogle the freeze state of the clock. If freezed: unfreeze. Else freeze.
    /// </summary>
    public void ToogleFreeze()
    {
        if (IsFreezed)
            Unfreeze();
        else Freeze();
    }

    /// <summary>
    /// Reset and Release the clock.
    /// </summary>
    public void Reset()
    {
        lastResetStart = UnfreezedTime;
        Release();
    }
}