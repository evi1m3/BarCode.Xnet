using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BCx;

namespace BCx.BarcodeLib.XAML
{
    public abstract class BcBaseCtrl<TData, TOption, TEncode, TRender> : FrameworkElement
        where TData: Payload 
        where TOption: CodeOptions
        where TEncode: CodeEncoder, new()
        where TRender: CodeRenderer, new()
    {
        private static readonly Type ThisType = typeof(BcBaseCtrl<TData, TOption, TEncode, TRender>);

        private TData           m_payLoad           = default;
        private TOption         m_options           = default;
        private TRender         m_renderer          = new TRender();
        private TEncode         m_encoder           = new TEncode();

        private bool            m_isOneDimensional  = false;
        private bool            m_isXamlRenderer    = false;
        private bool            m_isBitmapRenderer  = false;
        private DrawingGroup    m_drawingGroup      = null;
        private WriteableBitmap m_writeableBitmap   = null;
        private bool            m_isRenderCache     = false;

        public BcBaseCtrl()
        {
            this.SetValue(SnapsToDevicePixelsProperty, true);

            this.m_isOneDimensional = typeof(TEncode) == typeof(Code128Encoder);
            this.m_isXamlRenderer   = typeof(TRender) == typeof(XamlRenderer);
            this.m_isBitmapRenderer = typeof(TRender) == typeof(WritableBitmapRenderer);
        }

        public void SetPayLoad(TData payload, TOption options)
        {
            //  changing data/options will force recreation of the barcode
            this.m_payLoad       = payload;
            this.m_options       = options;
            this.m_isRenderCache = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect ctrlRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

            if (ctrlRect.IsEmpty)
            {
                return;
            }

            if (!this.m_isRenderCache)
            {
                this.CreateEncoder();
            }

            drawingContext.DrawRectangle(Brushes.White, null, ctrlRect);

            if (this.m_isXamlRenderer)
            {
                if (this.m_drawingGroup != null && !this.m_drawingGroup.Bounds.IsEmpty)
                {
                    double[] scale = this.GetScaleSize(this.m_drawingGroup.Bounds.Width, this.m_drawingGroup.Bounds.Height);

                    drawingContext.PushTransform(new ScaleTransform(scale[0], scale[1]));
                    drawingContext.DrawDrawing(this.m_drawingGroup);
                }

            }
            else if (this.m_isBitmapRenderer)
            {
                if (this.m_writeableBitmap != null && this.m_writeableBitmap.Width > 0 && this.m_writeableBitmap.Height > 0)
                {
                    double[] size = this.GetScaleSize(this.m_writeableBitmap.Width, this.m_writeableBitmap.Height);

                    Rect         imageRect = new Rect(0, 0, size[0], size[1]);
                    DrawingGroup drawingGroup = new DrawingGroup();
                                 drawingGroup.Children.Add(new ImageDrawing(this.m_writeableBitmap, imageRect));

                    RenderOptions.SetBitmapScalingMode(drawingGroup, BitmapScalingMode.NearestNeighbor);
                    drawingContext.DrawDrawing(drawingGroup);
                }
            }
        }

        private double[] GetScaleSize(double barcodeWidth, double barcodeHeight)
        {
            double scaleX = this.ActualWidth  / barcodeWidth;
            double scaleY = this.ActualHeight / barcodeHeight;

            if (!this.m_isOneDimensional)
            {
                //  1D Barcodes stretch.fill, 2D Barcodes stretch.uniform
                scaleX = scaleY = Math.Min(scaleX, scaleY);
            }
             
            if (this.m_isBitmapRenderer)
            {
                scaleX *= barcodeWidth;
                scaleY *= barcodeHeight;
            }

            //  only allow scale close to device pixel
            scaleX = ((int)(scaleX * 96d) / 96d);
            scaleY = ((int)(scaleY * 96d) / 96d);

            return new double[] { scaleX, scaleY };
        }
        
        private void CreateEncoder()
        {
            if (this.m_payLoad != null)
            {
                CodeData codeData = this.m_encoder.CreateCodeData(this.m_payLoad, this.m_options);

                this.m_renderer.Render(codeData);

                this.m_drawingGroup = this.m_renderer.GetImage<DrawingGroup>();

                if (this.m_isXamlRenderer)
                {
                    this.m_drawingGroup = this.m_renderer.GetImage<DrawingGroup>();
                }
                else if (this.m_isBitmapRenderer)
                {
                    this.m_writeableBitmap = this.m_renderer.GetImage<WriteableBitmap>();
                }
                this.m_isRenderCache = true;
            }
        }
    }
}
