using SharedKernel.Ports.Out;
using SharedKernel.Ports.Out.Shared;

namespace Shared.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime NowUtc => DateTime.UtcNow;
}