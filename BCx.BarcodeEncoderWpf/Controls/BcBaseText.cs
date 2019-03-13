using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

using BCx;

namespace BCx.BarcodeLib.XAML
{
    public abstract class BcBaseText<TEncode, TRender> : BcBaseCtrl<TextPayload, CodeOptions, TEncode, TRender>
        where TEncode: CodeEncoder, new()
        where TRender: CodeRenderer, new()
    {
        private static readonly Type ThisType = typeof(BcBaseText<TEncode, TRender>);


        internal static readonly    DependencyProperty CodeProperty = 
                                    DependencyProperty.Register("Code", typeof(string), ThisType, new PropertyMetadata(null, OnPropChanged));
        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set =>         SetValue(CodeProperty, value);
        }

        private static void OnPropChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs eArgs)
        {
            if (dpObj is BcBaseText<TEncode, TRender> bcCtrl)
            {
                bcCtrl.SetPayLoad(new TextPayload(bcCtrl.Code), null);

                if (bcCtrl.IsInitialized)
                {
                    bcCtrl.InvalidateVisual();
                }
            }
        }
    }
}
