using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace BCx
{
   public class XamlRenderOptions : CodeRenderOptions {

      public int                          m_iPixelPerModuleX=1;
      public int                          m_iPixelPerModuleY=1;

      public                              XamlRenderOptions(int iPixelPerModuleX=1, int iPixelPerModuleY=1)
      {
         m_iPixelPerModuleX=iPixelPerModuleX;
         m_iPixelPerModuleY=iPixelPerModuleY;
      }

   }


   public class XamlRenderer : CodeRenderer {

      CodeData                            m_xData;
                                                               
      DrawingGroup                        m_xDrawingGroup;

      public                              XamlRenderer() { }
      public                              XamlRenderer(XamlRenderOptions xOptions=null)
      {
         m_xOptions=xOptions;
      }

      public DrawingImage                 Image => new DrawingImage(this.m_xDrawingGroup);

      public override T                   GetImage<T>()
      {
         return this.m_xDrawingGroup  as T;
      }


      public override void                Render(CodeData xData)
      {
         XamlRenderOptions xOpt=m_xOptions as XamlRenderOptions;
         //
         if( xOpt==null ) xOpt=new XamlRenderOptions();
         //
         m_xData=xData;
         //
         this.m_xDrawingGroup=GetGraphic( xOpt.m_iPixelPerModuleX , xOpt.m_iPixelPerModuleY );
      }

      public DrawingGroup  GetGraphic(int pixelsPerModuleX , int pixelsPerModuleY )
      {
         var drawableModulesCountX = m_xData.ModuleMatrix[0].Count;
         var drawableModulesCountY = m_xData.ModuleMatrix.Count;
         var viewBox = new Size(pixelsPerModuleX * drawableModulesCountX, pixelsPerModuleY * drawableModulesCountY);
         return this.GetGraphic(pixelsPerModuleX , pixelsPerModuleY , viewBox, new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.White));
      }

      public DrawingGroup  GetGraphic(int pixelsPerModuleX, int pixelsPerModuleY, Size viewBox, Brush darkBrush, Brush lightBrush)
      {
         var drawableModulesCountX = m_xData.ModuleMatrix[0].Count;
         var drawableModulesCountY = m_xData.ModuleMatrix.Count;

         
         DrawingGroup   drawingGroup = new DrawingGroup();

         // draw directly
         DrawingContext drawingContext = drawingGroup.Open();

         double x = 0d, y = 0d;
         for (int yi = 0; yi < drawableModulesCountY; yi++, y += pixelsPerModuleY )
         {
            x = 0d;
            for(int xi = 0; xi < drawableModulesCountX; xi++, x += pixelsPerModuleX )
            {
               int iWidth=0;
               //
               while( xi + iWidth < drawableModulesCountX && m_xData.ModuleMatrix[yi][xi+iWidth] ) iWidth++;
               //
               if( iWidth > 0 )
               {
                  drawingContext.DrawRectangle(darkBrush, null, new Rect(x, y, pixelsPerModuleX * iWidth , pixelsPerModuleY));
                  xi+=(iWidth-1);
                  x+=(iWidth-1)*pixelsPerModuleX;
               }
            }
         }
         drawingContext.Close();

         // use geometry classes (looks better on 2D)
         /*
         var group = new GeometryGroup();
         //
         double x = 0d, y = 0d;
         //
         for (int yi = 0; yi < drawableModulesCountY; yi++, y += pixelsPerModuleY )
         {
            x = 0d;
            for(int xi = 0; xi < drawableModulesCountX; xi++, x += pixelsPerModuleX )
            {
               int iWidth=0;
               //
               while( xi + iWidth < drawableModulesCountX && m_xData.ModuleMatrix[yi][xi+iWidth] ) iWidth++;
               //
               if( iWidth > 0 )
               {
                  group.Children.Add(new RectangleGeometry(new Rect(x, y, pixelsPerModuleX * iWidth , pixelsPerModuleY)));
                  xi+=(iWidth-1);
                  x+=(iWidth-1)*pixelsPerModuleX;
               }
            }
         }
         drawing.Children.Add(new GeometryDrawing(darkBrush, null, group));
         */

         drawingGroup.Freeze();

         return drawingGroup;
      }
   }
}
