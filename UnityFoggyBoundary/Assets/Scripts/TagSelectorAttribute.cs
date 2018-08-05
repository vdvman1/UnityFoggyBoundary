using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VDV.Utility
{
    public class TagSelectorAttribute : PropertyAttribute
    {
        public readonly bool Required;

        public TagSelectorAttribute(bool required = true)
        {
            Required = required;
        }
    } 
}
