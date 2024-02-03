/* Author:  Leonardo Trevisan Silio
 * Date:    03/02/2024
 */
namespace Radiance;

/// <summary>
/// A usefull class to manage timer between frames.
/// </summary>
public class Clock
{
    private float lastReset = Utils.Time;
    private float? holdedTime = null;
    private float? startFreezedTime = null;
    private float oldFreezedTime = 0;
    
    private float newFreezedTime => IsFreezed ? Utils.Time - startFreezedTime.Value : 0;
    private float totalFreezedTime => oldFreezedTime + newFreezedTime;
    private float unfreezedTime => Utils.Time - totalFreezedTime;
    private float now => holdedTime ?? unfreezedTime;
    
    /// <summary>
    /// Get current type between now or a holded time and last Reset in seconds.
    /// </summary>
    public float Time => now - lastReset;

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
        => holdedTime = unfreezedTime;

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
        startFreezedTime = Utils.Time;
    }

    /// <summary>
    /// Unstop application time for this clock.
    /// </summary>
    public void Unfreeze()
    {
        if (!IsFreezed)
            return;
        
        oldFreezedTime += Utils.Time - startFreezedTime.Value;
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
        lastReset = unfreezedTime;
        Release();
    }
}