namespace Matrix_Elementary.Scripts
{
    public class LaTeXExport
    {
        string output, end, new_line;
        int index = 0, entries_count = 0, sep_len;
        const int MaxInd = 20;
        int matrix_width;
        bool start = true;
        bool double_sep;
        string separator;

        public LaTeXExport(int matrix_width, string separator = "\\to", int sep_len = 2,
            bool double_sep = true, string begin = "$$", string end = "$$", int begin_width = 0, string new_line = "$$\n$$")
        {
            this.matrix_width = matrix_width;
            this.separator = separator;
            this.sep_len = sep_len;
            output = begin;
            index += begin_width;
            this.end = end;
            this.double_sep = double_sep;
            this.new_line = new_line;
        }

        public void Add(string data)
        {
            if (start)
            {
                start = false;
                index = matrix_width;
            }
            else
            {
                index += sep_len + matrix_width;
                if (index > MaxInd)
                {
                    output += separator + new_line + (double_sep ? separator : "");
                    index = sep_len + matrix_width;
                }
                else
                    output += separator + "\n";
            }
            output += data;
            entries_count++;
        }

        public void Add(string data, string sep, int sep_length = 2)
        {
            string old_sep = separator;
            int old_len = sep_len;
            separator = sep;
            sep_len = sep_length;
            Add(data);
            separator = old_sep;
            sep_len = old_len;
        }

        public string Result => output + end;
        public bool Empty => start;
        public int EntriesCount => entries_count;
    }
}
