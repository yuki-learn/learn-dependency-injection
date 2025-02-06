using System;
using Ploeh.Samples.Commerce.Domain;

namespace Ploeh.Samples.Commerce.Web.Presentation
{
    public sealed class DefaultTimeProvider : ITimeProvider, IDisposable
    {
        public DateTime Now => DateTime.Now;

        public void Dispose()
        {
        }
    }
}