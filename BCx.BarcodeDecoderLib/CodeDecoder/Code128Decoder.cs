using System;
using ZXing;
using ZXing.OneD;

namespace BCx
{

   public class Code128Decoder : CodeDecoder {

      public                              Code128Decoder(BinaryBitmap xBitmap)
      {
         Code128Reader xDecoder=new Code128Reader();
         //
         m_xResult=xDecoder.decode( xBitmap );
      }

   }
}
