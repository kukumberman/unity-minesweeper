using Unity.Collections;
using Random = System.Random;

public static class UnityCollectionsExtensions
{
    public static void ShuffleList<T>(this NativeList<T> list, Random random)
        where T : unmanaged
    {
        int n = list.Length;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
