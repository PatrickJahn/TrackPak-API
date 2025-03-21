using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OrderService.Domain.Enums;


public enum OrderType
{
    [EnumMember(Value = "Service")]
    Service,
    
    [EnumMember(Value = "Package")]
    Package
}