using System.Collections.Generic;

public static class ListWithKeyValuePairsExtension
{
    public static bool ContainsKey<K, V>(this List<KeyValuePair<K, V>> listWithKeyValuePairs, K keyToBeChecked)
    {
        foreach (KeyValuePair<K, V> keyValuePair in listWithKeyValuePairs)
        {
            if (keyValuePair.Key.Equals(keyToBeChecked))
            {
                return true;
            }
        }
        return false;
    }
}