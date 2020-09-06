using System;
using System.Collections.Generic;
using System.Text;

namespace Rewrite
{
    public struct History
    {
        private string title;
        private string text;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }
}
