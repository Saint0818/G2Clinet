
using System;
using AI;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEnum"> 必須是 enum. </typeparam>
public interface IStateMachineFactory<TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable
{
    State<TEnum> CreateState(TEnum e);
}
