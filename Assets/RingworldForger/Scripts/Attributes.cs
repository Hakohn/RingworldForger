using System;
using UnityEngine;

namespace ChironPE
{
    #region Conditional Hide
    /// <summary>
    /// The  ConditionalHideAttribute inherits from the PropertyAttribute class and is nothing more than a simple data class with some constructors. 
    /// The main purpose of this class is to provide additional data that will be used within the PropertyDrawer. If you want to add extra options / 
    /// parameters to this attribute, this is where you would add them.
    ///
    /// The AttributeUsage attributes used at the top of the ConditionalHideAttribute class control where you will be able to use this attribute.
    /// Source: http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        //The name of the bool field that will be in control
        public string ConditionalSourceField = "";
        //TRUE = Hide in inspector / FALSE = Disable in inspector 
        public bool HideInInspector = false;
        public bool Reversed = false;

        public ConditionalHideAttribute(string conditionalSourceField)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = false;
        }

        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = hideInInspector;
        }

        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector, bool reversed)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = hideInInspector;
            Reversed = reversed;
        }
    }
	#endregion

	#region Disable Field
	/// <summary>
	/// Marks the property as disabled, and thus making it visible in the inspector but unmodifiable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = true)]
    public class DisableFieldAttribute : PropertyAttribute { }
    #endregion

    #region Limited Value 
    #endregion

    #region Sprites & Textures
    /// <summary>
    /// Displays a texture preview for the sprite property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class SpritePreviewAttribute : PropertyAttribute 
    {
        public int PreviewSize = 70;
        public bool IncludeLabel = true;
        public bool ForceSpriteSize = false;

        public SpritePreviewAttribute(int previewSize = 70, bool includeLabel = true, bool forceSpriteSize = false)
		{
            PreviewSize = previewSize;
            IncludeLabel = includeLabel;
            ForceSpriteSize = forceSpriteSize;
        }
    }

    /// <summary>
    /// Displays a texture preview for the sprite property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class TexturePreviewAttribute : PropertyAttribute
    {
        public int PreviewSize = 70;
        public bool IncludeLabel = true;
        public bool ForceTextureSize = false;

        public TexturePreviewAttribute(int previewSize = 70, bool includeLabel = true, bool forceSpriteSize = false)
        {
            PreviewSize = previewSize;
            IncludeLabel = includeLabel;
            ForceTextureSize = forceSpriteSize;
        }
    }
    #endregion
}