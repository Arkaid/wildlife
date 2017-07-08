using UnityEngine;

/// <summary> Allows to select flag enums in the inspector </summary>
public class EnumFlagAttribute : PropertyAttribute
{
    public string enumName;

    public EnumFlagAttribute() { }

    public EnumFlagAttribute(string name)
    {
        enumName = name;
    }
}