
using System;
using AI;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEnumState"> 必須是 enum. </typeparam>
public interface IStateMachineFactory<TEnumState, TEnumMsg> 
    where TEnumState : struct, IConvertible, IComparable, IFormattable
    where TEnumMsg : struct, IConvertible, IComparable, IFormattable
{
    State<TEnumState, TEnumMsg> CreateState(TEnumState e);
}
