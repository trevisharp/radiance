/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
using System;

namespace Radiance.Internal;

/// <summary>
/// A internal class to compute fps and Delta time.
/// </summary>
internal class TimeFrameController
{
    DateTime newer = DateTime.UtcNow;
    DateTime older = DateTime.UtcNow;

    public void RegisterFrame()
    {
        older = newer;
        newer = DateTime.UtcNow;
    }

    public float DeltaTime
    {
        get
        {
            var delta = newer - older;
            var time = delta.TotalSeconds;
            return (float)time;
        }
    }

    public float Fps => 1.0f / DeltaTime;
}