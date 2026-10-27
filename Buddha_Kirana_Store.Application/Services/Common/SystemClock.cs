using Buddha_Kirana_Store.Abstractions;

namespace Buddha_Kirana_Store.Services;

internal sealed class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}
