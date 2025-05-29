namespace Trace;

/// <summary>
/// A PCG (Permuted Congruential Generator) class that implements a simple pseudo-random number generator.
/// </summary>
public class Pcg
{
    public ulong State = 42;
    public ulong Inc = (54 << 1) | 1;

    /// <summary>
    /// Initializes a new instance of the PCG class with the specified initial state and sequence value with default
    /// values <c>initState = 42</c> and <c>initSeq = 54</c> for the state and sequence values, respectively, and the
    /// default values for the increment value.
    /// </summary>
    public Pcg()
    {
        ulong initState = State;
        State = 0;
        Random();
        State += initState;
        Random();
    }

    /// <summary>
    /// Initializes a new instance of the PCG class with the specified initial state and sequence value.
    /// </summary>
    /// <param name="initState">The initial state value (default is 42).</param>
    /// <param name="initSeq">The sequence value used to perturb the state (default is 54).</param>
    public Pcg(ulong initState = 42, ulong initSeq = 54)
    {
        State = 0;
        Inc = (initSeq << 1) | 1;
        Random();
        State += initState;
        Random();
    }

    /// <summary>
    /// Generates the next pseudo-random 32-bit unsigned integer.
    /// </summary>
    /// <returns>A 32-bit unsigned integer pseudo-random number.</returns>
    public uint Random()
    {
        ulong oldState = State;

        State = 6364136223846793005 * oldState + Inc;

        uint xorShifted = (uint)(((oldState >> 18) ^ oldState) >> 27);
        uint rot = (uint)(oldState >> 59);

        return (xorShifted >> (int)rot) | (xorShifted << (int)((-rot) & 31));
    }

    /// <summary>
    /// Generates the next pseudo-random <c>float</c> in the range [0, 1].
    /// </summary>
    /// <returns>A pseudo-random <c>float</c> in the range [0, 1].</returns>
    public float RandomFloat() => Random() / (float)uint.MaxValue;
}