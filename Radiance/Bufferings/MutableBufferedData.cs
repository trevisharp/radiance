/* Author:  Leonardo Trevisan Silio
 * Date:    27/12/2024
 */
namespace Radiance.Bufferings;

public abstract class MutableBufferedData(float[] initial) : IBufferedData
{
    protected float[] data = initial;
    protected Buffer? buffer = null;

    int? currentChangeStart = null;
    int? currentChangeEnd = null;
    readonly Changes changes = [];
    void TryAddChange()
    {
        if (!currentChangeStart.HasValue)
            return;
        
        if (!currentChangeEnd.HasValue)
            return;
        
        changes.Add(new(
            currentChangeStart.Value,
            currentChangeEnd.Value
        ));
        currentChangeStart = currentChangeEnd = null;

    }

    public Changes Changes
    {
        get
        {
            TryAddChange();
            return changes;
        }
    }

    public float this[int index]
    {
        get => data[index];
        set
        {
            data[index] = value;
            if (!currentChangeStart.HasValue)
            {
                currentChangeStart = currentChangeEnd = index;
                return;
            }

            if (currentChangeEnd < index && index < currentChangeEnd + 5)
            {
                currentChangeEnd = index;
                return;
            }

            TryAddChange();
        }
    }

    public abstract int Rows { get; }

    public abstract int Columns { get; }

    public abstract int Instances { get; }

    public abstract int InstanceLength { get; }

    public abstract bool IsGeometry { get; }

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public abstract float[] GetBufferData();
}