using ZXing;
using ZXing.QrCode;

namespace BCx
{

   public class QRDecoder : CodeDecoder {

      public                              QRDecoder(BinaryBitmap xBitmap)
      {
         QRCodeReader xReader=new QRCodeReader();
         //
         m_xResult=xReader.decode( xBitmap );
      }

   }
}
