using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class CustomLabel: Label
    {
        public static readonly BindableProperty LetterSpacingProperty = BindableProperty.Create(
                                                                    propertyName: "LetterSpacing",
                                                                    returnType: typeof(float),
                                                                    declaringType: typeof(CustomLabel),
                                                                    defaultValue: 0f);

        public float LetterSpacing
        {
            get
            {
                return (float)GetValue(LetterSpacingProperty);
            }
            set
            {
                SetValue(LetterSpacingProperty, value);
            }
        }
    }
}
