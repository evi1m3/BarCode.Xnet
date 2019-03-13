using System;
using System.Collections.Generic;
using System.Text;

public class CodePageIBM437 : EncodingProvider {

   static CodePageIBM437 xIBMEncodingProvider = null;

   static public CP437Encoding CP437 = new CP437Encoding();

   public override Encoding GetEncoding(int codepage)
   {
      if (codepage == 437)
         return CP437;
      //
      return null;
   }
   public override Encoding GetEncoding(int codepage, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
   {
      if (codepage == 437)
         return CP437;
      //
      return null;
   }
   public override Encoding GetEncoding(string name)
   {
      if (name == "CP437")
         return CP437;
      //
      return null;
   }
   public override Encoding GetEncoding(string name, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
   {
      if (name == "CP437")
         return CP437;
      //
      return null;
   }

   static public void Register()
   {
      if (xIBMEncodingProvider == null)
      {
         xIBMEncodingProvider = new CodePageIBM437();
         Encoding.RegisterProvider(xIBMEncodingProvider);
      }
   }

}



public class CP437Encoding : Encoding {

   public string _GetString(byte[] bytes, int byteIndex)
   {
      byte[] abData = new byte[(bytes.Length - byteIndex) * 4];
      //
      for (int a = byteIndex, i=0; a < bytes.Length; a++)
      {
         int iCode = CP437toUniCode[bytes[a]];
         //
         abData[i++] = (byte)((iCode >> 0) & 0xff);
         abData[i++] = (byte)((iCode >> 8) & 0xff);
         abData[i++] = 0;
         abData[i++] = 0;
      }
      //
      return Encoding.UTF32.GetString(abData);
   }

   public string _GetString(byte[] bytes, int byteIndex, int byteCount)
   {
      byte[] abData = new byte[byteCount * 4];
      for (int a = 0, i = 0; a < byteCount; a++)
      {
         int iCode = CP437toUniCode[bytes[byteIndex + a]];
         //
         abData[i++] = (byte)((iCode >> 0) & 0xff);
         abData[i++] = (byte)((iCode >> 8) & 0xff);
         abData[i++] = 0;
         abData[i++] = 0;
      }
      //
      return Encoding.UTF32.GetString(abData);
   }


   // convert string to UTF32 then use table to encode to CP437
   public byte[] _GetBytes(char[] chars, int charIndex, int charCount)
   {
      byte[] abUtfData=Encoding.UTF32.GetBytes(chars,charIndex,charCount);
      //
      byte[] abOutData = new byte[charCount];
      //
      for(int a=0,b=0; a<abUtfData.Length;a+=4,b++)
      {
         int iUtf32 = abUtfData[a] + abUtfData[a + 1] * 256;
         //
         bool bFound = false;
         //
         for(int i=0; i<CP437toUniCode.Length; i++)
            if( CP437toUniCode[i]==iUtf32 )
            {
               abOutData[b] = (byte)i;
               bFound = true;
               break;
            }
         //
         if( !bFound )
         {
            abOutData[b] = (byte)'?';
         }
      }
      //
      return abOutData;
   }



   public override int GetByteCount(char[] chars, int index, int count)
   {
      return count;
   }
   public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
   {
      byte[] abData=_GetBytes(chars, charIndex, charCount);
      //
      for (int a = 0; a < abData.Length; a++)
         bytes[a + byteIndex] = abData[a];
      //
      return abData.Length;
   }
   public override int GetCharCount(byte[] bytes, int index, int count)
   {
      return count;
   }
   public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
   {
      string sData = _GetString(bytes, byteIndex, byteCount);
      //
      for (int a = 0; a < sData.Length; a++)
         chars[charIndex + a] = sData[a];
      //
      return sData.Length;
   }
   public override int GetMaxByteCount(int charCount)
   {
      return charCount;
   }
   public override int GetMaxCharCount(int byteCount)
   {
      return byteCount;
   }


   int[] CP437toUniCode = new int[256]
   {
          0x0000 , 0x263A , 0x263B , 0x2665 , 0x2666 , 0x2663 , 0x2660 , 0x2022 ,
          0x25D8 , 0x25CB , 0x25D9 , 0x2642 , 0x2640 , 0x266A , 0x266B , 0x263C ,
                                                               
          0x25BA , 0x25C4 , 0x2195 , 0x203C , 0x00B6 , 0x00A7 , 0x25AC , 0x21A8 ,
          0x2191 , 0x2193 , 0x2192 , 0x2190 , 0x221F , 0x2194 , 0x25B2 , 0x25BC ,

          0x0020 , 0x0021 , 0x0022 , 0x0023 , 0x0024 , 0x0025 , 0x0026 , 0x0027 ,
          0x0028 , 0x0029 , 0x002a , 0x002b , 0x002c , 0x002d , 0x002e , 0x002f ,
          0x0030 , 0x0031 , 0x0032 , 0x0033 , 0x0034 , 0x0035 , 0x0036 , 0x0037 ,
          0x0038 , 0x0039 , 0x003a , 0x003b , 0x003c , 0x003d , 0x003e , 0x003f ,
          0x0040 , 0x0041 , 0x0042 , 0x0043 , 0x0044 , 0x0045 , 0x0046 , 0x0047 ,
          0x0048 , 0x0049 , 0x004a , 0x004b , 0x004c , 0x004d , 0x004e , 0x004f ,
          0x0050 , 0x0051 , 0x0052 , 0x0053 , 0x0054 , 0x0055 , 0x0056 , 0x0057 ,
          0x0058 , 0x0059 , 0x005a , 0x005b , 0x005c , 0x005d , 0x005e , 0x005f ,
          0x0060 , 0x0061 , 0x0062 , 0x0063 , 0x0064 , 0x0065 , 0x0066 , 0x0067 ,
          0x0068 , 0x0069 , 0x006a , 0x006b , 0x006c , 0x006d , 0x006e , 0x006f ,
          0x0070 , 0x0071 , 0x0072 , 0x0073 , 0x0074 , 0x0075 , 0x0076 , 0x0077 ,
          0x0078 , 0x0079 , 0x007a , 0x007b , 0x007c , 0x007d , 0x007e , 0x007f ,
          0x00c7 , 0x00fc , 0x00e9 , 0x00e2 , 0x00e4 , 0x00e0 , 0x00e5 , 0x00e7 ,
          0x00ea , 0x00eb , 0x00e8 , 0x00ef , 0x00ee , 0x00ec , 0x00c4 , 0x00c5 ,
          0x00c9 , 0x00e6 , 0x00c6 , 0x00f4 , 0x00f6 , 0x00f2 , 0x00fb , 0x00f9 ,
          0x00ff , 0x00d6 , 0x00dc , 0x00a2 , 0x00a3 , 0x00a5 , 0x20a7 , 0x0192 ,
          0x00e1 , 0x00ed , 0x00f3 , 0x00fa , 0x00f1 , 0x00d1 , 0x00aa , 0x00ba ,
          0x00bf , 0x2310 , 0x00ac , 0x00bd , 0x00bc , 0x00a1 , 0x00ab , 0x00bb ,

          0x2591 , 0x2592 , 0x2593 , 0x2502 , 0x2524 , 0x2561 , 0x2562 , 0x2556 ,
          0x2555 , 0x2563 , 0x2551 , 0x2557 , 0x255d , 0x255c , 0x255b , 0x2510 ,

          0x2514 , 0x2534 , 0x252c , 0x251c , 0x2500 , 0x253c , 0x255e , 0x255f ,
          0x255a , 0x2554 , 0x2569 , 0x2566 , 0x2560 , 0x2550 , 0x256c , 0x2567 ,

          0x2568 , 0x2564 , 0x2565 , 0x2559 , 0x2558 , 0x2552 , 0x2553 , 0x256b ,
          0x256a , 0x2518 , 0x250c , 0x2588 , 0x2584 , 0x258c , 0x2590 , 0x2580 ,

          0x03b1 , 0x00df , 0x0393 , 0x03c0 , 0x03a3 , 0x03c3 , 0x00b5 , 0x03c4 ,
          0x03a6 , 0x0398 , 0x03a9 , 0x03b4 , 0x221e , 0x03c6 , 0x03b5 , 0x2229 ,

          0x2261 , 0x00b1 , 0x2265 , 0x2264 , 0x2320 , 0x2321 , 0x00f7 , 0x2248 ,
          0x00b0 , 0x2219 , 0x00b7 , 0x221a , 0x207f , 0x00b2 , 0x25a0 , 0x00a0
   };
}


