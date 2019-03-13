using System;
using System.Collections.Generic;

using ZXing;
using ZXing.Common;
using ZXing.PDF417;

namespace BCx
{
    public enum Pdf417CompactionMode 
    {
        cmAuto,
        cmText,
        cmNumeric,
        cmBinary,
    }


   public class Pdf417CodeOptions : CodeOptions {
        public Pdf417CompactionMode     m_eCompactionMode=Pdf417CompactionMode.cmAuto;          
        public int                      m_iDataColumns=14;
        public int                      m_iDataRows=0;                
        public float                    m_fY2XRatio=4;            
        public int                      ErrorCorrm_iectionLevel=3;
        public bool                     m_bMacroPDF=false;
        public string                   m_sMacroPDFFileID=""; // all numbers! all 3 pairs must be lover than 900!          
        public int                      m_iMacroPDFSegmentIdx=0;      
        public int                      m_iMacroPDFSegmentCount=1;    
   }


    public class Pdf417Encoder : CodeEncoder {

        static                            Pdf417Encoder()
        {
            CodePageIBM437.Register();
        }


         public override CodeData         CreateCodeData(Payload xPayload, CodeOptions xOptions)
        {
            Pdf417CodeOptions xOpt=xOptions as Pdf417CodeOptions;
            //           
            if( xOpt==null ) xOpt=new Pdf417CodeOptions();

            //
            int width=1024;
            int height=1024;
            PDF417Writer _bcWriter = new PDF417Writer();
            Dictionary<EncodeHintType, object> Hints=new Dictionary<EncodeHintType, object>();

            Hints[EncodeHintType.WIDTH]   = width;
            Hints[EncodeHintType.HEIGHT]  = height;

            Hints[EncodeHintType.MARGIN            ]  = 0;
            Hints[EncodeHintType.PDF417_COMPACTION]   = (ZXing.PDF417.Internal.Compaction)(int)xOpt.m_eCompactionMode;
            Hints[EncodeHintType.ERROR_CORRECTION]    = (ZXing.PDF417.Internal.PDF417ErrorCorrectionLevel)(xOpt.ErrorCorrm_iectionLevel);
            Hints[EncodeHintType.PDF417_DIMENSIONS]   = new ZXing.PDF417.Internal.Dimensions(xOpt.m_iDataColumns,xOpt.m_iDataColumns,xOpt.m_iDataRows,100);

            if( xOpt.m_bMacroPDF )
            {
                Hints[EncodeHintType.PDF417_MACROENABLE]        = true;
                Hints[EncodeHintType.PDF417_MACROFileID]        = xOpt.m_sMacroPDFFileID;
                Hints[EncodeHintType.PDF417_MACROSegmentIndex]  = xOpt.m_iMacroPDFSegmentIdx;
                Hints[EncodeHintType.PDF417_MACROSegmentCount]  = xOpt.m_iMacroPDFSegmentCount;
            }

            Hints[EncodeHintType.PDF417_Y2XRatio] = xOpt.m_fY2XRatio;

            BitMatrix xData=_bcWriter.encode( xPayload.ToString() , width , height , Hints);
            //
            CodeData xResData=new CodeData( xData.Width , xData.Height );
         //
            for(int y=0; y<xData.Height; y++)
               for(int x=0; x<xData.Width; x++)
                  xResData.ModuleMatrix[y][x]=xData[x,y];
            //
            return xResData;
        }

    }
}
