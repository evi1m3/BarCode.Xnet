
using System;
using System.Collections.Generic;

namespace BCx
{

   public class Code128CodeOptions : CodeOptions {
      public int                          m_iHeight=1;
   }

   public class Code128Encoder : CodeEncoder
	{
		private const int CODE_START_B = 104;
		private const int CODE_START_C = 105;
		private const int CODE_CODE_B = 100;
		private const int CODE_CODE_C = 99;
		private const int CODE_STOP = 106;

		// Dummy characters used to specify control characters in input
		private const char ESCAPE_FNC_1 = '\u00f1';
		private const char ESCAPE_FNC_2 = '\u00f2';
		private const char ESCAPE_FNC_3 = '\u00f3';
		private const char ESCAPE_FNC_4 = '\u00f4';

		private const int CODE_FNC_1 = 102;   // Code A, Code B, Code C
		private const int CODE_FNC_2 = 97;    // Code A, Code B
		private const int CODE_FNC_3 = 96;    // Code A, Code B
		private const int CODE_FNC_4_B = 100; // Code B

		private bool forceCodesetB=false;
		private bool manualCodesets=false;

      internal static int[][] CODE_PATTERNS = {
                                                new[] {2, 1, 2, 2, 2, 2}, // 0
                                                new[] {2, 2, 2, 1, 2, 2},
                                                new[] {2, 2, 2, 2, 2, 1},
                                                new[] {1, 2, 1, 2, 2, 3},
                                                new[] {1, 2, 1, 3, 2, 2},
                                                new[] {1, 3, 1, 2, 2, 2}, // 5
                                                new[] {1, 2, 2, 2, 1, 3},
                                                new[] {1, 2, 2, 3, 1, 2},
                                                new[] {1, 3, 2, 2, 1, 2},
                                                new[] {2, 2, 1, 2, 1, 3},
                                                new[] {2, 2, 1, 3, 1, 2}, // 10
                                                new[] {2, 3, 1, 2, 1, 2},
                                                new[] {1, 1, 2, 2, 3, 2},
                                                new[] {1, 2, 2, 1, 3, 2},
                                                new[] {1, 2, 2, 2, 3, 1},
                                                new[] {1, 1, 3, 2, 2, 2}, // 15
                                                new[] {1, 2, 3, 1, 2, 2},
                                                new[] {1, 2, 3, 2, 2, 1},
                                                new[] {2, 2, 3, 2, 1, 1},
                                                new[] {2, 2, 1, 1, 3, 2},
                                                new[] {2, 2, 1, 2, 3, 1}, // 20
                                                new[] {2, 1, 3, 2, 1, 2},
                                                new[] {2, 2, 3, 1, 1, 2},
                                                new[] {3, 1, 2, 1, 3, 1},
                                                new[] {3, 1, 1, 2, 2, 2},
                                                new[] {3, 2, 1, 1, 2, 2}, // 25
                                                new[] {3, 2, 1, 2, 2, 1},
                                                new[] {3, 1, 2, 2, 1, 2},
                                                new[] {3, 2, 2, 1, 1, 2},
                                                new[] {3, 2, 2, 2, 1, 1},
                                                new[] {2, 1, 2, 1, 2, 3}, // 30
                                                new[] {2, 1, 2, 3, 2, 1},
                                                new[] {2, 3, 2, 1, 2, 1},
                                                new[] {1, 1, 1, 3, 2, 3},
                                                new[] {1, 3, 1, 1, 2, 3},
                                                new[] {1, 3, 1, 3, 2, 1}, // 35
                                                new[] {1, 1, 2, 3, 1, 3},
                                                new[] {1, 3, 2, 1, 1, 3},
                                                new[] {1, 3, 2, 3, 1, 1},
                                                new[] {2, 1, 1, 3, 1, 3},
                                                new[] {2, 3, 1, 1, 1, 3}, // 40
                                                new[] {2, 3, 1, 3, 1, 1},
                                                new[] {1, 1, 2, 1, 3, 3},
                                                new[] {1, 1, 2, 3, 3, 1},
                                                new[] {1, 3, 2, 1, 3, 1},
                                                new[] {1, 1, 3, 1, 2, 3}, // 45
                                                new[] {1, 1, 3, 3, 2, 1},
                                                new[] {1, 3, 3, 1, 2, 1},
                                                new[] {3, 1, 3, 1, 2, 1},
                                                new[] {2, 1, 1, 3, 3, 1},
                                                new[] {2, 3, 1, 1, 3, 1}, // 50
                                                new[] {2, 1, 3, 1, 1, 3},
                                                new[] {2, 1, 3, 3, 1, 1},
                                                new[] {2, 1, 3, 1, 3, 1},
                                                new[] {3, 1, 1, 1, 2, 3},
                                                new[] {3, 1, 1, 3, 2, 1}, // 55
                                                new[] {3, 3, 1, 1, 2, 1},
                                                new[] {3, 1, 2, 1, 1, 3},
                                                new[] {3, 1, 2, 3, 1, 1},
                                                new[] {3, 3, 2, 1, 1, 1},
                                                new[] {3, 1, 4, 1, 1, 1}, // 60
                                                new[] {2, 2, 1, 4, 1, 1},
                                                new[] {4, 3, 1, 1, 1, 1},
                                                new[] {1, 1, 1, 2, 2, 4},
                                                new[] {1, 1, 1, 4, 2, 2},
                                                new[] {1, 2, 1, 1, 2, 4}, // 65
                                                new[] {1, 2, 1, 4, 2, 1},
                                                new[] {1, 4, 1, 1, 2, 2},
                                                new[] {1, 4, 1, 2, 2, 1},
                                                new[] {1, 1, 2, 2, 1, 4},
                                                new[] {1, 1, 2, 4, 1, 2}, // 70
                                                new[] {1, 2, 2, 1, 1, 4},
                                                new[] {1, 2, 2, 4, 1, 1},
                                                new[] {1, 4, 2, 1, 1, 2},
                                                new[] {1, 4, 2, 2, 1, 1},
                                                new[] {2, 4, 1, 2, 1, 1}, // 75
                                                new[] {2, 2, 1, 1, 1, 4},
                                                new[] {4, 1, 3, 1, 1, 1},
                                                new[] {2, 4, 1, 1, 1, 2},
                                                new[] {1, 3, 4, 1, 1, 1},
                                                new[] {1, 1, 1, 2, 4, 2}, // 80
                                                new[] {1, 2, 1, 1, 4, 2},
                                                new[] {1, 2, 1, 2, 4, 1},
                                                new[] {1, 1, 4, 2, 1, 2},
                                                new[] {1, 2, 4, 1, 1, 2},
                                                new[] {1, 2, 4, 2, 1, 1}, // 85
                                                new[] {4, 1, 1, 2, 1, 2},
                                                new[] {4, 2, 1, 1, 1, 2},
                                                new[] {4, 2, 1, 2, 1, 1},
                                                new[] {2, 1, 2, 1, 4, 1},
                                                new[] {2, 1, 4, 1, 2, 1}, // 90
                                                new[] {4, 1, 2, 1, 2, 1},
                                                new[] {1, 1, 1, 1, 4, 3},
                                                new[] {1, 1, 1, 3, 4, 1},
                                                new[] {1, 3, 1, 1, 4, 1},
                                                new[] {1, 1, 4, 1, 1, 3}, // 95
                                                new[] {1, 1, 4, 3, 1, 1},
                                                new[] {4, 1, 1, 1, 1, 3},
                                                new[] {4, 1, 1, 3, 1, 1},
                                                new[] {1, 1, 3, 1, 4, 1},
                                                new[] {1, 1, 4, 1, 3, 1}, // 100
                                                new[] {3, 1, 1, 1, 4, 1},
                                                new[] {4, 1, 1, 1, 3, 1},
                                                new[] {2, 1, 1, 4, 1, 2},
                                                new[] {2, 1, 1, 2, 1, 4},
                                                new[] {2, 1, 1, 2, 3, 2}, // 105
                                                new[] {2, 3, 3, 1, 1, 1, 2}
                                             };

      public override CodeData            CreateCodeData(Payload xPayload, CodeOptions xOptions)
      {
         Code128CodeOptions xOpt=xOptions as Code128CodeOptions;
         if( xOpt==null ) xOpt=new Code128CodeOptions();
         //
         return CreateCode( xPayload.ToString() , xOpt.m_iHeight );
      }

      int appendPattern(bool[] target, int pos, int[] pattern, bool startColor)
      {
         bool color = startColor;
         int numAdded = 0;
         foreach (int len in pattern)
         {
            for (int j = 0; j < len; j++)
            {
               target[pos++] = color;
            }
            numAdded += len;
            color = !color; // flip color after each segment
         }
         return numAdded;
      }

      public CodeData CreateCode(String contents, int iHeight)
		{
			int length = contents.Length;
			// Check length
			if (length < 1 || length > 80)
			{
				throw new ArgumentException(
						"Contents length should be between 1 and 80 characters, but got " + length);
			}
			// Check content
			for (int i = 0; i < length; i++)
			{
				char c = contents[i];
				if (c < ' ' || c > '~')
				{
					switch (c)
					{
						case ESCAPE_FNC_1:
						case ESCAPE_FNC_2:
						case ESCAPE_FNC_3:
						case ESCAPE_FNC_4:
							break;
						default:
							throw new ArgumentException("Bad character in input: " + c);
					}
				}
			}

			var patterns = new List<int[]>(); // temporary storage for patterns
			int checkSum = 0;
			int checkWeight = 1;
			int codeSet = 0; // selected code (CODE_CODE_B or CODE_CODE_C)
			int position = 0; // position in contents

			int newCodeSet = CODE_CODE_B;

			while (position < length)
			{
				// If we're not manually controlling subsets
				if (!manualCodesets)
				{
					//Select code to use
					int requiredDigitCount = codeSet == CODE_CODE_C ? 2 : 4;
					
					if (isDigits(contents, position, requiredDigitCount))
					{
						newCodeSet = forceCodesetB ? CODE_CODE_B : CODE_CODE_C;
					}
					else
					{
						newCodeSet = CODE_CODE_B;
					}
				}

				//Get the pattern index
				int patternIndex;
				if (newCodeSet == codeSet)
				{
					// Encode the current character
					// First handle escapes
					switch (contents[position])
					{
						case ESCAPE_FNC_1:
							patternIndex = CODE_FNC_1;
							break;
						case ESCAPE_FNC_2:
							patternIndex = CODE_FNC_2;
							break;
						case ESCAPE_FNC_3:
							patternIndex = CODE_FNC_3;
							break;
						case ESCAPE_FNC_4:
							patternIndex = CODE_FNC_4_B; // FIXME if this ever outputs Code A
							break;
						default:

							if (manualCodesets && contents[position] == '~')
							{

								switch( contents[position + 1] )
								{
									case 'B':
										newCodeSet = CODE_CODE_B;
										codeSet = newCodeSet;
										patternIndex = newCodeSet;
										break;
									case 'C':
										newCodeSet = CODE_CODE_C;
										codeSet = newCodeSet;
										patternIndex = newCodeSet;
										break;
									default:
										patternIndex = contents[position + 1] - ' ';
										break;
								}
								position++;
							}
							else
							{

								// Then handle normal characters otherwise
								if (codeSet == CODE_CODE_B)
								{
									patternIndex = contents[position] - ' ';
								}
								else
								{ // CODE_CODE_C
									patternIndex = Int32.Parse(contents.Substring(position, 2));
									position++; // Also incremented below
								}

							}
							break;
					}
					position++;
				}
				else
				{
					// Should we change the current code?
					// Do we have a code set?
					if (codeSet == 0)
					{
						// No, we don't have a code set
						if (newCodeSet == CODE_CODE_B)
						{
							patternIndex = CODE_START_B;
						}
						else
						{
							// CODE_CODE_C
							patternIndex = CODE_START_C;
						}
					}
					else
					{
						// Yes, we have a code set
						patternIndex = newCodeSet;
					}
					codeSet = newCodeSet;
				}

				// Get the pattern
				patterns.Add(CODE_PATTERNS[patternIndex]);

				// Compute checksum
				checkSum += patternIndex * checkWeight;
				if (position != 0)
				{
					checkWeight++;
				}
			}

			// Compute and append checksum
			checkSum %= 103;
			patterns.Add(CODE_PATTERNS[checkSum]);

			// Append stop code
			patterns.Add(CODE_PATTERNS[CODE_STOP]);

			// Compute code width
			int codeWidth = 0;
			foreach (int[] pattern in patterns)
			{
				foreach (int width in pattern)
				{
					codeWidth += width;
				}
			}

			// Compute result
			var result = new bool[codeWidth];
			int pos = 0;
			foreach (int[] pattern in patterns)
			{
				pos += appendPattern(result, pos, pattern, true);
			}


         CodeData xData = new CodeData( result.Length , iHeight );
         //
         for(int y=0; y<iHeight; y++)
            for(int x = 0; x < result.Length; x++)
               xData.ModuleMatrix[y][x] = result[x];


         return xData;
		}

		private static bool isDigits(String value, int start, int length)
		{
			int end = start + length;
			int last = value.Length;
			for (int i = start; i < end && i < last; i++)
			{
				char c = value[i];
				if (c < '0' || c > '9')
				{
					if (c != ESCAPE_FNC_1)
					{
						return false;
					}
					end++; // ignore FNC_1
				}
			}
			return end <= last; // end > last if we've run out of string
		}
	}
}