using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModiferProvider
    {
        IEnumerable<float> GetAdditiveModifers(Stat stat);
        IEnumerable<float> GetPercentageModifers(Stat stat);
    }
}