using System.Reflection;

namespace SmartEnums; 

public abstract class Enumeration<TEnum>
    : IEquatable<Enumeration<TEnum>> 
    where TEnum : Enumeration<TEnum> {
    private static readonly Dictionary<int, TEnum> Enumerations = CreateEnumerations();
    public int Value { get; protected init; }
    public string Name { get; protected init; }

    protected Enumeration(int value, string name) {
        Value = value;
        Name = name;
    }

    public static TEnum? FromValue(int value) => Enumerations.TryGetValue(value, out var enumeration)
            ? enumeration
            : null;

    public static TEnum? FromName(string name) => Enumerations
            .Values.FirstOrDefault(e => e.Name == name);
    public bool Equals(Enumeration<TEnum>? other) {
        if (other is null)
            return false;
        return GetType() == other.GetType()
               && Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is Enumeration<TEnum> other &&
                                                Equals(other);
    
    public static bool operator ==(Enumeration<TEnum> enumeration1, Enumeration<TEnum> enumeration2) => 
         enumeration1.Equals(enumeration2);

    public static bool operator !=(Enumeration<TEnum> enumeration1, Enumeration<TEnum> enumeration2) => 
         !enumeration1.Equals(enumeration2);
    
    public override int GetHashCode() =>  
         Value.GetHashCode();

    public override string ToString() => Name;
    private static Dictionary<int, TEnum> CreateEnumerations() => 
        typeof(TEnum)
            .GetFields(
                BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => typeof(TEnum).IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!)
            .ToDictionary(x => x.Value);
}