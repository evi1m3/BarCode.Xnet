using System;
using System.Collections.Generic;
using System.Text;

namespace BCx
{

   public class SwissQREncoder : CodeEncoder {

   byte[,]                                abCrossData = new byte[,]{ { 0,0,0,0,0,0,0,0,0},
                                                                     { 0,1,1,1,1,1,1,1,0},
                                                                     { 0,1,1,1,0,1,1,1,0},
                                                                     { 0,1,1,1,0,1,1,1,0},
                                                                     { 0,1,0,0,0,0,0,1,0},
                                                                     { 0,1,1,1,0,1,1,1,0},
                                                                     { 0,1,1,1,0,1,1,1,0},
                                                                     { 0,1,1,1,1,1,1,1,0},
                                                                     { 0,0,0,0,0,0,0,0,0} };

      public override CodeData            CreateCodeData(Payload xPayload, CodeOptions xOptions)
      {
         QREncoder xQREncoder=new QREncoder();
         //
         CodeData xData=xQREncoder.CreateCodeData( xPayload , xOptions );

         // make cross
         int iMidX = xData.ModuleMatrix.Count / 2 - 4;
         int iMidY = xData.ModuleMatrix[0].Count / 2 - 4;
         for (int y = 0; y < 9; y++)
            for (int x = 0; x < 9; x++)
               xData.ModuleMatrix[iMidY + y][iMidX + x] = abCrossData[x, y] == 1;
         //
         return xData;
      }
   }
}
