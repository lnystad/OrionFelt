namespace OrionLag.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using OrionLag.Common.DataModel;

    public static class InputFileParser
    {

        public static List<InputData> ParseFile(string path, string filename)
        {
            var restunVal = new List<InputData>();
            string fullPath = Path.Combine(path, filename);
            string[] text = System.IO.File.ReadAllLines(fullPath,Encoding.UTF7);

            foreach (var line in text)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var elements = line.Split(new char[] { ';' },StringSplitOptions.None);
                    InputData data = new InputData();

                    if (elements.Length >= 4)
                    {
                        data.SkytterNr = Convert.ToInt32(elements[0]);
                        data.Name = elements[1];
                        data.Skytterlag= elements[2];
                        data.Klasse = elements[3];
                        restunVal.Add(data);
                    }
                }
            }


            return restunVal;
        }
    }
}
