﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Chunker
{
    public abstract class Dechunker : IDechunker
    {
        public abstract DechunkOperation[] GetDechunkerOperations(string[] chunks);

        public string Dechunk(string[] chunks, string splitMarker = "")
        {
            DechunkOperation[] operations = GetDechunkerOperations(chunks);

            if (chunks.Length != operations.Length)
            {
                throw new ArgumentException("chunks and operations array must have same length: chunks=" +
                                            chunks.Length + ", operations=" + operations.Length + "!");
            }

            var untokenizedString = new StringBuilder();
            for (int i = 0; i < chunks.Length; i++)
            {
                // attach token to string buffer
                untokenizedString.Append(chunks[i]);

                bool isAppendSpace;
                bool isAppendSplitMarker;

                // if this token is the last token do not attach a space
                if (i + 1 == operations.Length)
                {
                    isAppendSpace = false;
                    isAppendSplitMarker = false;
                }
                // if next token move left, no space after this token,
                // its safe to access next token
                else if (operations[i + 1] == DechunkOperation.MERGE_TO_LEFT
                            || operations[i + 1] == DechunkOperation.MERGE_BOTH)
                {
                    isAppendSpace = false;
                    isAppendSplitMarker = true;
                }
                // if this token is move right, no space
                else if (operations[i] == DechunkOperation.MERGE_TO_RIGHT
                            || operations[i] == DechunkOperation.MERGE_BOTH)
                {
                    isAppendSpace = false;
                    isAppendSplitMarker = true;
                }
                else
                {
                    isAppendSpace = true;
                    isAppendSplitMarker = false;
                }

                if (isAppendSpace)
                {
                    untokenizedString.Append(' ');
                }

                if (isAppendSplitMarker && splitMarker != null)
                {
                    untokenizedString.Append(splitMarker);
                }
            }

            return untokenizedString.ToString();
        }
    }
}
