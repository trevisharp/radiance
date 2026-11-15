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
    readonly Queue<DateTime> frames = [];
    DateTime newest = DateTime.MinValue;

    public void Reset()
        => frames.Clear();

    public void RegisterFrame()
    {
        newest = DateTime.UtcNow;
        frames.Enqueue(newest);

        while (frames.Count > windowSize + 1)
            frames.Dequeue();
    }

    /// <summary>
    /// Get the average time between frames.
    /// </summary>
    public float DeltaTime
    {
        get
        {
            int intervals = frames.Count - 1;
            if (intervals < 1)
                return 0f;

            var delta = newest - frames.Peek();
            var time = delta.TotalSeconds / intervals;
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