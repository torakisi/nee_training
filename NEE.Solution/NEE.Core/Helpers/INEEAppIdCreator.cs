using System;

namespace NEE.Core.Helpers
{
    public interface INEEAppIdCreator
    {
        string CreateIdFromDateTime(DateTime dt);
    }
}
