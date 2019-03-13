using System;
using System.Collections.Generic;
using ZXing;
using ZXing.Aztec;
using ZXing.Common;

namespace BCx
{

   public class AztecDecoder : CodeDecoder {

      public                              AztecDecoder(BinaryBitmap xBitmap)
      {
         AztecReader xAztec=new AztecReader();
         //
         m_xResult=xAztec.decode( xBitmap );
      }

   }
}
