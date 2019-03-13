using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

using BCx;
using static BCx.SwissQrCodePayload;

namespace BCx.BarcodeLib.XAML
{
    public class BarcodeSwissQR : BcBaseCtrl<SwissQrCodePayload, CodeOptions, SwissQREncoder, WritableBitmapRenderer>
    {
        private static readonly Type ThisType = typeof(BarcodeSwissQR);


        internal static readonly    DependencyProperty CreditorProperty     = DependencyProperty.Register("Creditor",       typeof(Contact),    ThisType, new PropertyMetadata(null,            OnPropChanged));
        internal static readonly    DependencyProperty UltCreditorProperty  = DependencyProperty.Register("UltCreditor",    typeof(Contact),    ThisType, new PropertyMetadata(null,            OnPropChanged));
        internal static readonly    DependencyProperty DebitorProperty      = DependencyProperty.Register("Debitor",        typeof(Contact),    ThisType, new PropertyMetadata(null,            OnPropChanged));
        internal static readonly    DependencyProperty IbanProperty         = DependencyProperty.Register("Iban",           typeof(Iban),       ThisType, new PropertyMetadata(null,            OnPropChanged));
        internal static readonly    DependencyProperty ReferenceProperty    = DependencyProperty.Register("Reference",      typeof(Reference),  ThisType, new PropertyMetadata(null,            OnPropChanged));
        internal static readonly    DependencyProperty PayCurrencyProperty  = DependencyProperty.Register("PayCurrency",    typeof(Currency),   ThisType, new PropertyMetadata(Currency.CHF,    OnPropChanged));
        internal static readonly    DependencyProperty PayAmountProperty    = DependencyProperty.Register("PayAmount",      typeof(decimal),    ThisType, new PropertyMetadata(Decimal.Zero,    OnPropChanged));
        internal static readonly    DependencyProperty PayDateProperty      = DependencyProperty.Register("PayDate",        typeof(DateTime?),  ThisType, new PropertyMetadata(null,            OnPropChanged));
        internal static readonly    DependencyProperty Procedure1Property   = DependencyProperty.Register("Procedure1",     typeof(string),     ThisType, new PropertyMetadata(null,            OnPropChanged));
        internal static readonly    DependencyProperty Procedure2Property   = DependencyProperty.Register("Procedure2",     typeof(string),     ThisType, new PropertyMetadata(null,            OnPropChanged));

        public Contact Creditor
        {
            get => (Contact)    GetValue(CreditorProperty);
            set =>              SetValue(CreditorProperty, value);
        }
        public Contact UltCreditor
        {
            get => (Contact)    GetValue(UltCreditorProperty);
            set =>              SetValue(UltCreditorProperty, value);
        }
        public Contact Debitor
        {
            get => (Contact)    GetValue(DebitorProperty);
            set =>              SetValue(DebitorProperty, value);
        }
        public Iban Iban
        {
            get => (Iban)       GetValue(IbanProperty);
            set =>              SetValue(IbanProperty, value);
        }
        public Reference Reference
        {
            get => (Reference)  GetValue(ReferenceProperty);
            set =>              SetValue(ReferenceProperty, value);
        }
        public Currency PayCurrency
        {
            get => (Currency)   GetValue(PayCurrencyProperty);
            set =>              SetValue(PayCurrencyProperty, value);
        }
        public decimal PayAmount
        {
            get => (decimal)    GetValue(PayAmountProperty);
            set =>              SetValue(PayAmountProperty, value);
        }
        public DateTime? PayDate
        {
            get => (DateTime?)  GetValue(PayDateProperty);
            set =>              SetValue(PayDateProperty, value);
        }
        public string Procedure1
        {
            get => (string)     GetValue(Procedure1Property);
            set =>              SetValue(Procedure1Property, value);
        }
        public string Procedure2
        {
            get => (string)     GetValue(Procedure2Property);
            set =>              SetValue(Procedure2Property, value);
        }

        private static void OnPropChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs eArgs)
        {
            if (dpObj is BarcodeSwissQR bcCtrl)
            {
                if (bcCtrl.Iban != null)
                {
                    SwissQrCodePayload payLoad = new SwissQrCodePayload(bcCtrl.Iban, bcCtrl.PayCurrency, bcCtrl.Creditor, bcCtrl.Reference, bcCtrl.Debitor, 
                                                                        bcCtrl.PayAmount, bcCtrl.PayDate, bcCtrl.UltCreditor, bcCtrl.Procedure1, bcCtrl.Procedure2);

                    bcCtrl.SetPayLoad(payLoad, null);
                }

                if (bcCtrl.IsInitialized)
                {
                    bcCtrl.InvalidateVisual();
                }
            }
        }
    }
}
