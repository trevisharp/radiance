/* Author:  Leonardo Trevisan Silio
 * Date:    25/12/2024
 */
using System;
using System.Collections.Generic;

namespace Radiance;

using Bufferings;
using Primitives;

/// <summary>
/// Class with utils to work with buffers and
/// stream of data.
/// </summary>
public class Buffers
{
    /// <summary>
    /// Create a buffer based on a function.
    /// </summary>
    public static Bufferings.BufferData Create(int size, Func<int, float> factory)
    {
        var stream = new Bufferings.BufferData(1, 1, false);

        stream.PrepareSize(size);
        for (int i = 0; i < size; i++)
            stream.Add(factory(i));

        return stream;
    }

    /// <summary>
    /// Create a buffer based on a function.
    /// </summary>
    public static BufferedDataArray Create(int size, Func<int, Vec2> factory)
        => FillBuffer(size, 2, factory);

    /// <summary>
    /// Create a buffer based on a function.
    /// </summary>
    public static BufferedDataArray Create(int size, Func<int, Vec3> factory)
        => FillBuffer(size, 3, factory);

    /// <summary>
    /// Create a buffer based on a function.
    /// </summary>
    public static BufferedDataArray Create(int size, Func<int, Vec4> factory)
        => FillBuffer(size, 4, factory);

    /// <summary>
    /// Create a buffer from a array.
    /// </summary>
    public static Bufferings.BufferData Create(params float[] data)
    {
        var stream = new Bufferings.BufferData(1, 1, false);

        stream.PrepareSize(data.Length);
        stream.AddRange(data);

        return stream;
    }
    
    static BufferedDataArray FillBuffer<T>(int rows, int columns, Func<int, T> factory)
        where T : IBufferizable
    {
        List<Bufferings.BufferData> streams = [];

        for (int i = 0; i < columns; i++)
        {
            var stream = new Bufferings.BufferData(1, 1, false);
            streams.Add(stream);
            stream.PrepareSize(rows);
        }

        var buffer = new float[columns];
        for (int i = 0; i < rows; i++)
        {
            var bufferizable = factory(i);
            bufferizable.Bufferize(buffer, 0);

            for (int j = 0; j < columns; j++)
                streams[j].Add(buffer[j]);
        }

        return new BufferedDataArray(streams);
    }
    
    /// <summary>
    /// Get factories for use to create buffers.
    /// </summary>
    public static readonly StreamFactory Factories = new();
    public class StreamFactory
    {
        /// <summary>
        /// factory for rand values. Uniform between 0 and 1.
        /// </summary>
        public Func<int, float> Urand => Rand(0, 1f);

        /// <summary>
        /// factory for rand values. Uniform between 0 and 1.
        /// </summary>
        public Func<int, Vec2> Urand2 => Rand2(0, 1f);

        /// <summary>
        /// factory for rand values. Uniform between 0 and 1.
        /// </summary>
        public Func<int, Vec3> Urand3 => Rand3(0, 1f);

        /// <summary>
        /// factory for rand values. Uniform between 0 and 1.
        /// </summary>
        public Func<int, Vec4> Urand4 => Rand4(0, 1f);

        /// <summary>
        /// factory for rand values.
        /// </summary>
        /// <param name="max">Max value generated.</param>
        /// <param name="min">Min value generated.</param>
        /// <param name="seed">The seed of random algorithm. If null create a seed based on time.</param>
        public Func<int, float> Rand(float min, float max, int? seed = null)
        {
            var rand = GetRand(min, max, seed);
            return _ => rand();
        }

        /// <summary>
        /// factory for rand values on vec2(x, y) format.
        /// </summary>
        /// <param name="max">Max value generated.</param>
        /// <param name="min">Min value generated.</param>
        /// <param name="seed">The seed of random algorithm. If null create a seed based on time.</param>
        public Func<int, Vec2> Rand2(float min, float max, int? seed = null)
        {
            var rand = GetRand(min, max, seed);
            return _ => (rand(), rand());
        }

        /// <summary>
        /// factory for rand values on vec3(x, y, z) format.
        /// </summary>
        /// <param name="max">Max value generated.</param>
        /// <param name="min">Min value generated.</param>
        /// <param name="seed">The seed of random algorithm. If null create a seed based on time.</param>
        public Func<int, Vec3> Rand3(float min, float max, int? seed = null)
        {
            var rand = GetRand(min, max, seed);
            return _ => (rand(), rand(), rand());
        }

        /// <summary>
        /// factory for rand values on vec3(x, y, z) format.
        /// </summary>
        /// <param name="max">Max value generated.</param>
        /// <param name="min">Min value generated.</param>
        /// <param name="seed">The seed of random algorithm. If null create a seed based on time.</param>
        public Func<int, Vec4> Rand4(float min, float max, int? seed = null)
        {
            var rand = GetRand(min, max, seed);
            return _ => (rand(), rand(), rand(), rand());
        }

        /// <summary>
        /// Build a function that generates random values between min and max using a seed.
        /// </summary>
        private Func<float> GetRand(float min, float max, int? seed)
        {
            seed ??= (int)(DateTime.UtcNow.Ticks % int.MaxValue);
            var random = new Random(seed.Value);
            var band = max - min;

            return () => band * random.NextSingle() + min;
        }

        /// <summary>
        /// Generate a repetitive sequence of values.
        /// </summary>
        public Func<int, float> Mod(params float[] values)
            => i => values[i % values.Length];
            
        /// <summary>
        /// Generate a repetitive sequence of values.
        /// </summary>
        public Func<int, T> Mod<T>(params T[] values)
            where T : IBufferizable
            => i => values[i % values.Length];
    }
}
