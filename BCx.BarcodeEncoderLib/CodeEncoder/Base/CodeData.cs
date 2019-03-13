using System;
using System.Collections;
using System.Collections.Generic;

namespace BCx
{

   public enum CodeType
   {
      Invalid,
      //
      QR,
      SwissQR,
      Code128,
      Pdf417,
      Aztec,
   }

   public class CodeData : IDisposable
   {
      public List<BitArray>               ModuleMatrix { get; set; }

      public                              CodeData(int version)
      {
         this.Version = version;

         var size = ModulesPerSideFromVersion(version);

         this.ModuleMatrix = new List<BitArray>();
         for (var i = 0; i < size; i++)
            this.ModuleMatrix.Add(new BitArray(size));
      }

      public                              CodeData(int iWidth, int iHeight, int iVersion=0)
      {
         this.Version = iVersion;
         this.ModuleMatrix = new List<BitArray>();
         //
         for (var i = 0; i < iHeight; i++)
            this.ModuleMatrix.Add(new BitArray(iWidth));
      }

      public int                          Version { get; private set; }

      private static int                  ModulesPerSideFromVersion(int version)
      {
         return 21 + (version - 1) * 4;
      }

      public void                         Dispose()
      {
         this.ModuleMatrix = null;
         this.Version = 0;
      }

   }
}
