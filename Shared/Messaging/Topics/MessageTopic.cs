namespace Shared.Messaging.Topics;

public enum MessageTopic
{
    
    // User events
    UserCreated = 10,

    
    // Location events
    UserLocationCreated = 20,
    OrderLocationCreated = 21,
    CompanyLocationCreated = 22,
    EmployeeLocationCreated = 23,
    
    // Employee events
    EmployeeCreated = 30,
    EmployeeDeleted = 31,
    EmployeeLocationUpdated = 32,


    // Order Events
    OrderCreated = 40,
    OrderUpdated = 41,
    OrderCancelled = 42,

    
    // Company Events
    CompanyCreated,
    CompanyUpdated,
    CompanyLocationUpdated,



}