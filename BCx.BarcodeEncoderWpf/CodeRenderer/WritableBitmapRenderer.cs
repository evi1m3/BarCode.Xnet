using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BCx
{
   public class WritableBitmapRenderOptions : CodeRenderOptions {

      public int                          m_iPixelPerModuleX=1;
      public int                          m_iPixelPerModuleY=1;

      public                              WritableBitmapRenderOptions(int iPixelPerModuleX=1, int iPixelPerModuleY=1)
      {
         m_iPixelPerModuleX=iPixelPerModuleX;
         m_iPixelPerModuleY=iPixelPerModuleY;
      }

   }


   public class WritableBitmapRenderer : CodeRenderer {

      CodeData                            m_xData;
                                                               
      WriteableBitmap                     m_xBitmap;

      public                              WritableBitmapRenderer() { }
      public                              WritableBitmapRenderer(WritableBitmapRenderOptions xOptions=null)
      {
         m_xOptions=xOptions;
      }

      public WriteableBitmap              Image => new WriteableBitmap(this.m_xBitmap);

      public override T                   GetImage<T>()
      {
         return this.m_xBitmap as T;
      }


      public override void                Render(CodeData xData)
      {
         XamlRenderOptions xOpt=m_xOptions as XamlRenderOptions;
         //
         if( xOpt==null ) xOpt=new XamlRenderOptions();
         //
         m_xData=xData;
         //
         this.m_xBitmap=GetGraphic();
      }

      public WriteableBitmap              GetGraphic()
      {
         var iWidth = m_xData.ModuleMatrix[0].Count;
         var iHeight = m_xData.ModuleMatrix.Count;
         //
         WriteableBitmap xBitmap=new WriteableBitmap( iWidth , iHeight , 96, 96, PixelFormats.Gray8, null);
        
         //
         byte[] abPixel = new byte[iWidth * iHeight ];
         //
         for (int yi = 0, i=0; yi < iHeight; yi++ )
         {
            for(int xi = 0; xi < iWidth; xi++ , i++ )
            {
               if( m_xData.ModuleMatrix[yi][xi] )
                  abPixel[i]=0;
               else
                  abPixel[i]=255;
            }
         }
         //
         Int32Rect xRect = new Int32Rect(0, 0, iWidth, iHeight);
         xBitmap.WritePixels(xRect, abPixel, iWidth, 0);
         //

         xBitmap.Freeze();

         return xBitmap;
      }
   }

}
