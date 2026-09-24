using UnityEngine;

public static class Rnd
{
    /// <summary>
    /// Return a random number influenced by Schlick's bias. Bias values closer to 0 provide lower random values, while values closer to 1 make higher values more likely. x = r/((1/b-2)(1-r)+1)
    /// </summary>
    /// <param name="r">A random number.</param>
    /// <param name="b">The bias argument. Should be between 0 and 1.</param>
    /// <returns>A random value between 0 and 1, influenced by Schlick's bias.</returns>
    public static float bias(float r, float b)
    {
        return r/((1/b-2)*(1-r)+1);
    }

    /// <summary>
    /// Returns a random number influenced by Schlick's gain. Gain values closer to 0 produce values biased to 0.5, while gains closer to 1 produce values biased towards extremes.
    /// </summary>
    /// <param name="r">A random number.</param>
    /// <param name="g">The gain argument. Should be between 0 and 1.</param>
    /// <returns>A random value between 0 and 1, influenced by Schlick's gain.</returns>
    public static float gain(float r, float g)
    {
        if (r < 0.5f)
        {
            return bias(2*r, 1-g)/2f;
        } 
        else
        {
            return 1 - bias(2-2*r, 1-g)/2f;
        }
    }
}
