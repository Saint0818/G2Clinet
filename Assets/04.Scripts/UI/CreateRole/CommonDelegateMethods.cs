
using GameStruct;

public class CommonDelegateMethods
{
    public delegate void Action();
    public delegate void Int1(int value);
    public delegate void Int2(int value1, int value2);
    public delegate void Object1(object obj);

    public delegate int RInt1Int1Kind1(int value, EAttributeKind kind);
}
