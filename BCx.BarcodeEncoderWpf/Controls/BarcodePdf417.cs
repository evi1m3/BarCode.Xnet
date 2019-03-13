using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

using BCx;

namespace BCx.BarcodeLib.XAML
{
    public class BarcodePdf417 : BcBaseCtrl<TextPayload, CodeOptions, Pdf417Encoder, WritableBitmapRenderer>
    {
        private static readonly Type ThisType = typeof(BarcodePdf417);

        internal static readonly    DependencyProperty CodeProperty         = DependencyProperty.Register("Code",           typeof(string),     ThisType, new PropertyMetadata(null,    OnPropChanged));
        internal static readonly    DependencyProperty IsMacroPdfProperty   = DependencyProperty.Register("IsMacroPdf",     typeof(bool),       ThisType, new PropertyMetadata(false,   OnPropChanged));
        internal static readonly    DependencyProperty FileIdProperty       = DependencyProperty.Register("FileId",         typeof(string),     ThisType, new PropertyMetadata(null,    OnPropChanged));
        internal static readonly    DependencyProperty SegIndexProperty     = DependencyProperty.Register("SegIndex",       typeof(int),        ThisType, new PropertyMetadata(0,       OnPropChanged));
        internal static readonly    DependencyProperty SegCountProperty     = DependencyProperty.Register("SegCount",       typeof(int),        ThisType, new PropertyMetadata(0,       OnPropChanged));
        internal static readonly    DependencyProperty DataColsProperty     = DependencyProperty.Register("DataCols",       typeof(int),        ThisType, new PropertyMetadata(14,      OnPropChanged));
        internal static readonly    DependencyProperty DataRowsProperty     = DependencyProperty.Register("DataRows",       typeof(int),        ThisType, new PropertyMetadata(0,       OnPropChanged));
        internal static readonly    DependencyProperty XYRatioProperty      = DependencyProperty.Register("XYRatio",        typeof(int),        ThisType, new PropertyMetadata(4,       OnPropChanged));

        public string Code
        {
            get => (string) GetValue(CodeProperty);
            set =>          SetValue(CodeProperty, value);
        }
        public bool IsMacroPdf
        {
            get => (bool)   GetValue(IsMacroPdfProperty);
            set =>          SetValue(IsMacroPdfProperty, value);
        }
        public string FileId
        {
            get => (string) GetValue(FileIdProperty);
            set =>          SetValue(FileIdProperty, value);
        }
        public int SegIndex
        {
            get => (int)    GetValue(SegIndexProperty);
            set =>          SetValue(SegIndexProperty, value);
        }
        public int SegCount
        {
            get => (int)    GetValue(SegCountProperty);
            set =>          SetValue(SegCountProperty, value);
        }
        public int DataCols
        {
            get => (int)    GetValue(DataColsProperty);
            set =>          SetValue(DataColsProperty, value);
        }
        public int DataRows
        {
            get => (int)    GetValue(DataRowsProperty);
            set =>          SetValue(DataRowsProperty, value);
        }
        public int XYRatio
        {
            get => (int)    GetValue(XYRatioProperty);
            set =>          SetValue(XYRatioProperty, value);
        }

        private static void OnPropChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs eArgs)
        {
            if (dpObj is BarcodePdf417 bcCtrl)
            {
                Pdf417CodeOptions   codeOptions = new Pdf417CodeOptions();
                                    codeOptions.m_bMacroPDF             = bcCtrl.IsMacroPdf;
                                    codeOptions.m_sMacroPDFFileID       = bcCtrl.FileId;
                                    codeOptions.m_iMacroPDFSegmentIdx   = bcCtrl.SegIndex;
                                    codeOptions.m_iMacroPDFSegmentCount = bcCtrl.SegCount;
                                    codeOptions.m_iDataColumns          = bcCtrl.DataCols;
                                    codeOptions.m_iDataRows             = bcCtrl.DataRows;
                                    codeOptions.m_fY2XRatio             = bcCtrl.XYRatio;

                bcCtrl.SetPayLoad(new TextPayload(bcCtrl.Code), codeOptions);

                if (bcCtrl.IsInitialized)
                {
                    bcCtrl.InvalidateVisual();
                }
            }
        }
    }
}
