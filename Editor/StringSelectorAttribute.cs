using UnityEngine;

public class StringSelectorAttribute : PropertyAttribute
{
    public string[] array;
    public bool isEnumOrButton;
    public bool isShowProperty;

    public StringSelectorAttribute(string[] array, bool isEnumOrButton = false, bool isShowProperty = true)
    {
        this.array = array;
        this.isEnumOrButton = isEnumOrButton;
        this.isShowProperty = isShowProperty;
    }
}
