using System;
using System.Collections.Generic;

namespace BCx
{

   public class CodeOptions {
   }

   public class CodeEncoder {

      CodeData                            m_xData=null;
      CodeRenderer                        m_xRenderer=null;         

      static public CodeEncoder           CreateCode(CodeType eType, Payload xPayload, CodeOptions xOptions, CodeRenderer xRenderer=null)
      {
         CodeEncoder xGenerator=null;
         //
         switch( eType ){
         case CodeType.QR        : xGenerator=new QREncoder();       break;
         case CodeType.SwissQR   : xGenerator=new SwissQREncoder();  break;
         case CodeType.Code128   : xGenerator=new Code128Encoder();  break;
         case CodeType.Pdf417    : xGenerator=new Pdf417Encoder();   break;
         case CodeType.Aztec     : xGenerator=new AztecEncoder();    break;
         }
         //
         xGenerator.m_xData=xGenerator.CreateCodeData( xPayload , xOptions );
         //
         if( xRenderer!=null )
         {
            xGenerator.m_xRenderer=xRenderer;
            //
            xRenderer.Render( xGenerator.m_xData );
         }
         //
         return xGenerator;
      }

      public CodeData                     Data { get { return m_xData;} }
      public CodeRenderer                 Renderer { get { return m_xRenderer;} }

      public virtual CodeData             CreateCodeData(Payload xPayload, CodeOptions xOptions)
      {
         return null;
      }

   }
}
