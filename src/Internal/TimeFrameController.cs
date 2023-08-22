/* Author:  Leonardo Trevisan Silio
 * Date:    22/08/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// A internal class to compute fps and Delta time.
/// </summary>
internal class TimeFrameController
{
    DateTime newer = DateTime.Now;
    DateTime older = DateTime.Now;

    public void RegisterFrame()
    {
        older = newer;
        newer = DateTime.Now;
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