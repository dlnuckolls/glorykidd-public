using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
    public class TextTermStore : TermStore
    {
        private TextTermFormat _textTermFormat;

        public TextTermStore(string name, TextTermFormat textTermFormat, int length)
            : base(name, TermType.Text)
        {
            _textTermFormat = textTermFormat;
            Length = length;
        }
    }
}
