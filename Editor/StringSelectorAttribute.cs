using UnityEngine;

public class StringSelectorAttribute : PropertyAttribute
{
    public string[] array;
    public bool isEnumOrButton;

    public StringSelectorAttribute(string[] array, bool isEnumOrButton = false)
    {
        this.array = array;
    }
}
