namespace Shared.Enums;

public enum RouteStatusEnum
{
    Created = 0,     // Route has been created but not yet assigned
    Assigned = 1,    // Route has been assigned to an employee
    InProgress = 2,  // Route is actively being followed
    Delayed = 3,     // Route is delayed due to external factors
    Completed = 4,   // Route has been successfully completed
    Cancelled = 5,  // Route was cancelled before completion
    Pending = 6,
}