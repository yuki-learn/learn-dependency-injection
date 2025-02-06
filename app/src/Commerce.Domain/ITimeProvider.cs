using System;

namespace Ploeh.Samples.Commerce.Domain
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}