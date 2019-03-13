using System;
using System.IO;

using BCx;
using static BCx.SwissQrCodePayload;

namespace BarCodeLibTest
{
   class Program
   {
      static void Main(string[] args)
      {
         // quick and easy code encode

         string dummyText     = @"A barcode (also bar code) is an optical, machine-readable representation of data; the data usually describes something about the object that carries the barcode. Traditional barcodes systematically represent data by varying the widths and spacings of parallel lines, and may be referred to as linear or one-dimensional (1D). Later, two-dimensional (2D) variants were developed, using rectangles, dots, hexagons and other geometric patterns, called matrix codes or 2D barcodes, although they do not use bars as such. Initially, barcodes were only scanned by special optical scanners called barcode readers.";

         CodeEncoder bc128    = CodeEncoder.CreateCode( CodeType.Code128 , new TextPayload("7560001000234"), null, 
               CodeRenderer.Create( CodeRenderType.Svg , new SvgRenderOptions(){m_iPixelPerModuleY=20 } ) );
         //CodeRenderer.Create( CodeRenderType.Xaml) );

         File.WriteAllText("test_code128.svg", bc128.Renderer.GetStringData());
         //
         //string sTest128 = CodeDecoder.Decode(bc128.Data).Text;


         // quick and easy code encode
         CodeEncoder bcAztec  = CodeEncoder.CreateCode( CodeType.Aztec , new TextPayload(dummyText), null, 
                  CodeRenderer.Create( CodeRenderType.Png , new PngRenderOptions(128, 128) ) );

         File.WriteAllBytes("test_aztec.png", bcAztec.Renderer.GetByteData());
         //
         //string sTestAztec = CodeDecoder.Decode(bcAztec.Data).Text;
         string sTestAztecPng = CodeDecoder.Decode(File.ReadAllBytes("test_aztec.png")).Text;



         // multiple code render and data acquire
         CodeEncoder bcQRCode = CodeEncoder.CreateCode( CodeType.QR , new TextPayload(dummyText) , null, 
               CodeRenderer.Create( CodeRenderType.Png , new PngRenderOptions(256, 256) ) );
         File.WriteAllBytes("test_qr.png", bcQRCode.Renderer.GetByteData());
         //
         //string sTestsQR=CodeDecoder.Decode( bcQRCode.Data ).Text;
         string sTestQRPng = CodeDecoder.Decode(File.ReadAllBytes("test_qr.png")).Text;

         // custom render
         CodeRenderer xSvgOutput = CodeRenderer.Create( CodeRenderType.Svg , new SvgRenderOptions() );
         xSvgOutput.Render(bcQRCode.Data);
         File.WriteAllText("test.svg", xSvgOutput.GetStringData());

         // acquire data
         byte[] sPngData=bcQRCode.Renderer.GetByteData();
         string sSvgData=xSvgOutput.GetStringData();



         // SwissQR - payload
         Contact     _contact    = new Contact("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
         Iban        _iban       = new Iban("CH2609000000857666015", Iban.IbanType.Iban);
         Reference   _reference  = new Reference(Reference.ReferenceType.QRR, "990005000000000320071012303", Reference.ReferenceTextType.QrReference);
         Currency    _currency   = Currency.CHF;
         decimal     _amount     = 100.25m;

         SwissQrCodePayload  dataQR      = new SwissQrCodePayload(_iban, _currency, _contact, _reference, null, _amount, null, null);
         CodeEncoder         bcQRSwiss   = CodeEncoder.CreateCode( CodeType.SwissQR , dataQR , null , 
                           CodeRenderer.Create( CodeRenderType.Png , new PngRenderOptions(256, 256) ) );
         File.WriteAllBytes("test_swissqr.png", bcQRSwiss.Renderer.GetByteData());
         //
         //string sQRSwiss = CodeDecoder.Decode(bcQRSwiss.Data).Text;
         string sQRSwissPng = CodeDecoder.Decode(File.ReadAllBytes("test_swissqr.png")).Text;


         // MacroPDF417 
         SplitFile( "~pdf417dump.cab.txt" , "NetCore.seg" , "001899017" );
      }

      static void SplitFile(string sFilename, string sOutpuFilename, string sFileId)
      {
         try {
            //
            string   sData=File.ReadAllText( sFilename );
            //
            int      iMaxBytes=796;
            int      iDataCols=14;
            //
            if( sData.Length >= 4776 )
            {
               iMaxBytes=1192;
               iDataCols=21;
            }
            //
            int      iSegmentCount=0;
            //
            for(int iLen=sData.Length; iLen>0; iLen-=iMaxBytes, iSegmentCount++);
            //
            for(int a=0, iPos=0; a<iSegmentCount; a++, iPos+=iMaxBytes)
            {
               int iSize=iMaxBytes;
               if( iPos + iSize > sData.Length ) iSize=sData.Length-iPos;
               //
               CreateFileString( sData.Substring( iPos , iSize ) , $"{sOutpuFilename}{a}", sFileId , a , iSegmentCount , iDataCols );
            }
         }
         catch(Exception ex )
         {
            Console.WriteLine(ex.Message);
         }
      }

      static void CreateFileString(string sData, string sOutpuFilename, string sFileId, int iSegmentIndex, int iSegmentCount, int iDataCols=14)
      {
         Pdf417CodeOptions xOptions=new Pdf417CodeOptions();
         xOptions.m_bMacroPDF=true;
         xOptions.m_sMacroPDFFileID=sFileId;
         xOptions.m_iMacroPDFSegmentIdx=iSegmentIndex;
         xOptions.m_iMacroPDFSegmentCount=iSegmentCount;
         xOptions.m_iDataColumns=iDataCols;
         xOptions.m_iDataRows=0;
         xOptions.m_fY2XRatio=4;

         CodeEncoder bcPdf417 = CodeEncoder.CreateCode( CodeType.Pdf417 , new TextPayload( sData ) , xOptions , 
               //CodeRenderer.Create( CodeRenderType.Png , new PngRenderOptions( 128 , 128 , sOutpuFilename + ".png" ) ) );
                  CodeRenderer.Create( CodeRenderType.Svg , new SvgRenderOptions(){m_bHorizontalOptimization=false } ) );
         File.WriteAllText( sOutpuFilename + ".svg" , bcPdf417.Renderer.GetStringData());

      }
   }
}
