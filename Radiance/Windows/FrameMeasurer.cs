/* Author:  Leonardo Trevisan Silio
 * Date:    08/11/2024
 */
using System;
using System.Collections.Generic;

namespace Radiance.Windows;

/// <summary>
/// Measure the frequency of frames on a window.
/// Recives the quantity of frames used on fps calcules.
/// High values implies on more stable variations.
/// Low values implies on more precision.
/// </summary>
public class FrameMeasurer(int windowSize)
{
    DateTime newerFrame = DateTime.MinValue;
    DateTime olderFrame = DateTime.MinValue;
    readonly Queue<DateTime> frames = [];

    public void Reset()
        => frames.Clear();
    
    public void RegisterFrame()
    {
        newerFrame = DateTime.UtcNow;
        frames.Enqueue(newerFrame);

        if (frames.Count > windowSize)
            olderFrame = frames.Dequeue();
    }

    /// <summary>
    /// Get the average time between frames.
    /// </summary>
    public float DeltaTime
    {
        get
        {
            int winCount = frames.Count;
            if (winCount == 0)
                return 0f;

            var delta = newerFrame - olderFrame;
            var time = delta.TotalSeconds / winCount;
            return (float)time;
        }
    }

    /// <summary>
    /// Get the average frames per second.
    /// </summary>
    public float Fps
    {
        get
        {
            var delta = DeltaTime;
            if (delta == 0f)
                return 0f;
            
            return 1f / delta;
        }
    }

    public override string ToString() => Fps.ToString();
}