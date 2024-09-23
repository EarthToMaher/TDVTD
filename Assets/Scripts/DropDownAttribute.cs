using UnityEngine;
public class DropDownAttribute : PropertyAttribute
{
    public string DropdownName;

    public DropDownAttribute(string dropdownName)
    {
        DropdownName = dropdownName;
    }
}