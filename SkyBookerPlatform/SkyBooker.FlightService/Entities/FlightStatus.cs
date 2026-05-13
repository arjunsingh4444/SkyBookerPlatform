namespace SkyBooker.FlightService.Entities;

// Flight status options
public enum FlightStatus
{
    Scheduled = 0,    // Flight is scheduled (default)
    Delayed = 1,      // Flight is delayed
    Cancelled = 2,    // Flight is cancelled
    Completed = 3     // Flight has completed
}
