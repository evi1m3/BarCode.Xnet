using Hjg.Pngcs;
using System;
using System.Collections.Generic;
using System.IO;
using ZXing;
using ZXing.Aztec;
using ZXing.Common;

namespace BCx
{

   public class CodeDecoder {

      protected Result                    m_xResult;

      public string                       Text { get {  return m_xResult==null ? "" : m_xResult.Text; } }

      static public CodeDecoder           Decode(byte[] abImage)
      {
         PngReader xPng=new PngReader( new MemoryStream( abImage ) );
         //
         ImageLines xLines=xPng.ReadRowsByte();
         //
         byte[,] abData=new byte[xLines.ImgInfo.Cols, xLines.ImgInfo.Rows];
         //
         int iBits = xLines.ImgInfo.BitspPixel;
         int iChannels = xLines.ImgInfo.Channels;
         //
         if ( iBits==1 )
         {
            for (int y = 0; y < xLines.ImgInfo.Rows; y++)
               for (int x = 0; x < xLines.ImgInfo.Cols/8; x++)
               {
                  byte b= xLines.ScanlinesB[y][x];
                  //
                  for (int a=0; a<8; a++)
                     abData[x*8+a, y] = (byte)( ( (b & (1<<(7-a))) !=0 ) ? 255 : 0 );
               }
         }
         else if( iBits==8 )
         {
            int iC=0;
            for (int y = 0; y < xLines.ImgInfo.Rows; y++)
               for (int x = 0; x < xLines.ImgInfo.Cols; x++, iC+=iChannels)
                  abData[x,y]=xLines.ScanlinesB[y][iC];
         }
         //
         return Decode( abData , xLines.ImgInfo.Cols , xLines.ImgInfo.Rows);
      }

      static public CodeDecoder           Decode(byte[,] aabImage, int iWidth, int iHeight)
      {
         byte[] abImage = new byte[ iWidth * iHeight ];
         //
         for (int y = 0, i = 0; y < iHeight; y++)
            for (int x = 0; x < iWidth; x++)
               abImage[i++] = (byte)((aabImage[x, y] > 128) ? 255 : 0);
         //
         ZXing.LuminanceSource xSource = new RGBLuminanceSource(abImage, iWidth, iHeight, RGBLuminanceSource.BitmapFormat.Gray8);
         //
         var xBinarizer = new HybridBinarizer(xSource);
         var xBinBitmap = new BinaryBitmap(xBinarizer);
         //
         return Decode(xBinBitmap);
      }

      static public CodeDecoder           Decode(CodeData xBitmap)
      {
         BitMatrix      xMatrix=new BitMatrix(xBitmap.ModuleMatrix[0].Count , xBitmap.ModuleMatrix.Count );
         //
         for(int y=0; y<xBitmap.ModuleMatrix.Count; y++)
            for(int x=0; x<xBitmap.ModuleMatrix[0].Count; x++)
               xMatrix[x,y]=xBitmap.ModuleMatrix[y][x];
         //
         BinaryBitmap xBin=new BinaryBitmap( xMatrix );
         //
         return Decode( xBin );
      }

      static CodeDecoder                  Decode(BinaryBitmap xBin)
      {
         Code128Decoder xCode128Decoder = new Code128Decoder(xBin);
         if (xCode128Decoder.isValid()) return xCode128Decoder;
         //
         QRDecoder xQRDecoder = new QRDecoder(xBin);
         if (xQRDecoder.isValid()) return xQRDecoder;
         //
         CodeDecoder xAztecDecoder = new AztecDecoder(xBin);
         if (xAztecDecoder.isValid()) return xAztecDecoder;
         //
         return new CodeDecoder();
      }

      public virtual bool                 isValid()
      { 
         return m_xResult!=null;
      }

   }
}
