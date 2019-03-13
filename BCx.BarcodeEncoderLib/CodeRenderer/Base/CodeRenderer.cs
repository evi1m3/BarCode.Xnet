
namespace BCx
{

   public enum CodeRenderType {
      Svg,
      Png,
   }

   public class CodeRenderOptions {
      public int                          m_iWidth;
      public int                          m_iHeight;
   }

   public abstract class CodeRenderer {

      protected CodeRenderOptions         m_xOptions;

      static public CodeRenderer          Create(CodeRenderType eType, CodeRenderOptions xOptions=null)
      {
         CodeRenderer xRenderer=null;
         //
         switch( eType ){
         case CodeRenderType.Svg    : xRenderer=new SvgRenderer(); break;
         case CodeRenderType.Png    : xRenderer=new PngRenderer(); break;
         }
         //
         xRenderer.m_xOptions=xOptions;
         //
         return xRenderer;
      }

      public virtual void                 Render(CodeData xData)
      {
      }

      public virtual byte[]               GetByteData()
      {
         return null;
      }

      public virtual string               GetStringData()
      {
         return null;
      }

      public virtual T                    GetImage<T>() where T : class
      {
         return null;
      }

   }


}
