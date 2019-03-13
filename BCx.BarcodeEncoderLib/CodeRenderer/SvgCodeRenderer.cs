using System;
using System.IO;
using System.Text;

namespace BCx
{
   public class SvgRenderOptions : CodeRenderOptions {

      public int                          m_iPixelPerModuleX=1;
      public int                          m_iPixelPerModuleY=1;

      public bool                         m_bHorizontalOptimization=true;  // best for 2D codes


      public                              SvgRenderOptions()
      {
      }

   }


   public class SvgRenderer : CodeRenderer {

      string                              m_sResult;

      public override void                Render(CodeData xData)
      {
         SvgRenderOptions xOpt=m_xOptions as SvgRenderOptions;
         //
         if( xOpt==null ) xOpt=new SvgRenderOptions();
         //
         SvgCode xSvg=new SvgCode(xData,xOpt.m_bHorizontalOptimization);
         //
         m_sResult=xSvg.GetGraphic( xOpt.m_iPixelPerModuleX , xOpt.m_iPixelPerModuleY );
      }

      public override byte[]              GetByteData()
      {
         return null;
      }

      public override string              GetStringData()
      {
         return m_sResult;
      }


   }


   public class SvgCode : IDisposable
   {
      protected CodeData m_xCodeData;

      bool m_bHorizontalOptimization=true;

      public class Size {
         public int                          Width;
         public int                          Height;

         public                              Size(int iWidth=0, int iHeight=0)
         {
            Width=iWidth;
            Height=iHeight;
         }

      }

      public class Color {
         static public Color                 Black=new Color( "#000000" );
         static public Color                 White=new Color( "#FFFFFF" );

         public string                       m_sColor;

         public                              Color(string sColor="#FFFFFF")
         {
            m_sColor=sColor;
         }

      }

      public void Dispose()
      {
         m_xCodeData = null;
      }


      /// <summary>
      /// Constructor without params to be used in COM Objects connections
      /// </summary>
      public SvgCode() { }
      public SvgCode(CodeData xCodeData, bool bHorizontalOptimization) 
      {
         m_xCodeData=xCodeData;
         m_bHorizontalOptimization=bHorizontalOptimization;
      }

      public string GetGraphic(int pixelsPerModuleX, int pixelsPerModuleY)
      {
         var viewBox = new Size( pixelsPerModuleX * m_xCodeData.ModuleMatrix[0].Count, 
                     pixelsPerModuleY * m_xCodeData.ModuleMatrix.Count);
         //
         return this.GetGraphic(viewBox,pixelsPerModuleX,pixelsPerModuleY, "#000000" , "#ffffff" );
      }

      public string GetGraphic(Size viewBox, int pixelsPerModuleX, int pixelsPerModuleY, string darkColorHex, string lightColorHex, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
      {
         var offset = drawQuietZones ? 0 : 4;
         var drawableModulesXCount = m_xCodeData.ModuleMatrix[0].Count - (drawQuietZones ? 0 : offset * 2);
         var drawableModulesYCount = m_xCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : offset * 2);
         //
         var SizeX = drawableModulesXCount * pixelsPerModuleX;
         var SizeY = drawableModulesYCount * pixelsPerModuleY;
         //
         var svgSizeAttributes = sizingMode.Equals(SizingMode.WidthHeightAttribute) ? $@"width=""{viewBox.Width}"" height=""{viewBox.Height}""" : $@"viewBox=""0 0 {viewBox.Width} {viewBox.Height}""";
         var svgFile = new StringBuilder($@"<svg version=""1.1"" baseProfile=""full"" shape-rendering=""crispEdges"" {svgSizeAttributes} xmlns=""http://www.w3.org/2000/svg"">");
         svgFile.AppendLine($@"<rect x=""0"" y=""0"" width=""{CleanSvgVal(SizeX)}"" height=""{CleanSvgVal(SizeY)}"" fill=""{lightColorHex}"" />");
         //
         if( m_bHorizontalOptimization )
         {
            for (int yi = offset; yi < offset + drawableModulesYCount; yi++)
            {
               for(int xi = offset; xi < offset + drawableModulesXCount; xi++)
               {
                  int iWidth=0;
                  while( xi+iWidth < offset + drawableModulesXCount && m_xCodeData.ModuleMatrix[yi][xi+iWidth] ) iWidth++;
                  //if( m_xCodeData.ModuleMatrix[yi][xi+iWidth] ) iWidth++;
                  //
                  if( iWidth > 0 )
                  {
                     var x = (xi - offset) * pixelsPerModuleX;
                     var y = (yi - offset) * pixelsPerModuleY;
                     //svgFile.AppendLine($@"<rect x=""{CleanSvgVal(x)}"" y=""{CleanSvgVal(y)}"" width=""{CleanSvgVal(pixelsPerModuleX)}"" height=""{CleanSvgVal(pixelsPerModuleY)}"" fill=""{darkColorHex}"" />");
                     if( y==0 )
                        svgFile.AppendLine($@"<rect x=""{CleanSvgVal(x)}"" width=""{CleanSvgVal(pixelsPerModuleX*iWidth)}"" height=""{CleanSvgVal(pixelsPerModuleY)}""/>");
                     else
                        svgFile.AppendLine($@"<rect x=""{CleanSvgVal(x)}"" y=""{CleanSvgVal(y)}"" width=""{CleanSvgVal(pixelsPerModuleX*iWidth)}"" height=""{CleanSvgVal(pixelsPerModuleY)}""/>");
                     //
                     xi+=iWidth-1;
                  }
               }
            }
         }
         else
         {
            for(int xi = offset; xi < offset + drawableModulesXCount; xi++)
            {
               for (int yi = offset; yi < offset + drawableModulesYCount; yi++)
               {
                  int iHeight=0;
                  while( yi+iHeight < offset + drawableModulesYCount && m_xCodeData.ModuleMatrix[yi+iHeight][xi] ) iHeight++;
                  //
                  if( iHeight > 0 )
                  {
                     var x = (xi - offset) * pixelsPerModuleX;
                     var y = (yi - offset) * pixelsPerModuleY;
                     //svgFile.AppendLine($@"<rect x=""{CleanSvgVal(x)}"" y=""{CleanSvgVal(y)}"" width=""{CleanSvgVal(pixelsPerModuleX)}"" height=""{CleanSvgVal(pixelsPerModuleY)}"" fill=""{darkColorHex}"" />");
                     if( y==0 )
                        svgFile.AppendLine($@"<rect x=""{CleanSvgVal(x)}"" width=""{CleanSvgVal(pixelsPerModuleX)}"" height=""{CleanSvgVal(pixelsPerModuleY*iHeight)}""/>");
                     else
                        svgFile.AppendLine($@"<rect x=""{CleanSvgVal(x)}"" y=""{CleanSvgVal(y)}"" width=""{CleanSvgVal(pixelsPerModuleX)}"" height=""{CleanSvgVal(pixelsPerModuleY*iHeight)}""/>");
                     //
                     yi+=iHeight-1;
                  }
               }
            }
         }
         svgFile.Append(@"</svg>");
         return svgFile.ToString();
      }

      private string CleanSvgVal(double input)
      {
         //Clean double values for international use/formats
         return input.ToString(System.Globalization.CultureInfo.InvariantCulture);
      }

      public enum SizingMode
      {
         WidthHeightAttribute,
         ViewBoxAttribute
      }
   }
}
