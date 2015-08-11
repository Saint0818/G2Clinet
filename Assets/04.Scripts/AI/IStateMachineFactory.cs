
using System;
using AI;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEnum"> 必須是 enum. </typeparam>
public interface IStateMachineFactory<in TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable
{
    IState CreateState(TEnum e);
}
