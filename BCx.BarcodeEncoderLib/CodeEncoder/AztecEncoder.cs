using System;
using System.Collections.Generic;

using ZXing;
using ZXing.Common;
using ZXing.Aztec;

namespace BCx
{
    public enum AztecCompactionMode 
    {
        cmAuto,
        cmText,
        cmNumeric,
        cmBinary,
    }


   public class AztecCodeOptions : CodeOptions {
   }


    public class AztecEncoder : CodeEncoder {

        static                            AztecEncoder()
        {
            CodePageIBM437.Register();
        }


         public override CodeData         CreateCodeData(Payload xPayload, CodeOptions xOptions)
        {
            AztecCodeOptions xOpt=xOptions as AztecCodeOptions;
            //           
            if( xOpt==null ) xOpt=new AztecCodeOptions();

            //
            int width=1024;
            int height=1024;
            AztecWriter _bcWriter = new AztecWriter();
            Dictionary<EncodeHintType, object> Hints=new Dictionary<EncodeHintType, object>();

            Hints[EncodeHintType.WIDTH]   = width;
            Hints[EncodeHintType.HEIGHT]  = height;

            //Hints[EncodeHintType.PDF417_COMPACTION]   = (ZXing.PDF417.Internal.Compaction)(int)xOpt.m_eCompactionMode;
            //Hints[EncodeHintType.ERROR_CORRECTION]    = (ZXing.PDF417.Internal.PDF417ErrorCorrectionLevel)(xOpt.ErrorCorrm_iectionLevel);
            //Hints[EncodeHintType.PDF417_DIMENSIONS]   = new ZXing.PDF417.Internal.Dimensions(xOpt.m_iDataColumns,xOpt.m_iDataColumns,xOpt.m_iDataRows,100);

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
