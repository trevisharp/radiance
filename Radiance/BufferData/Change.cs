/* Author:  Leonardo Trevisan Silio
 * Date:    27/12/2024
 */
namespace Radiance.BufferData;

/// <summary>
/// Represents a change on a buffer data region.
/// </summary>
public record Change(
    int Start, int End
);