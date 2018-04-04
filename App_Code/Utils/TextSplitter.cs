using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Utils
{
    /// <summary>
    /// Summary description for TextSplitter
    /// </summary>
    public class TextSplitter
    {
        private int _blockSize;
        public int BlockSize
        {
            get { return _blockSize; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Block size must be positive integer.");
                _blockSize = value;
                _isSplitContentUpToDate = false;
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _isSplitContentUpToDate = false;
            }
        }

        public int BlocksCount
        {
            get { return (Text.Length / BlockSize) + (Text.Length % BlockSize == 0 ? 0 : 1); }
        }


        #region Constructors

        public TextSplitter(int blockSize, string content)
            : this(blockSize)
        {
            Text = content;
        }

        public TextSplitter(int blockSize)
        {
            BlockSize = blockSize;
            Text = string.Empty;
        }

        #endregion Constructors


        private bool _isSplitContentUpToDate = false;

        private List<string> _splitContentCache;
        public List<string> SplitText
        {
            get
            {
                if (!_isSplitContentUpToDate)
                {
                    _splitContentCache = new List<string>(BlocksCount);

                    for (int i = 0; i < Text.Length; i += BlockSize)
                    {
                        int end = Math.Min(BlockSize, Text.Length - i);
                        _splitContentCache.Add(Text.Substring(i, end));
                    }
                }
                return _splitContentCache.Clone();
            }
        }
    }
}