namespace OrionLag.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;

    public static class InputFileParser
    {
        public static List<InputData> ParseFile(string path, string filename)
        {
            var restunVal = new List<InputData>();
            try
            {
                string fullPath = Path.Combine(path, filename);
                string[] text = System.IO.File.ReadAllLines(fullPath, Encoding.UTF7);

                foreach (var line in text)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            var elements = line.Split(new char[] { ';' }, StringSplitOptions.None);
                            InputData data = new InputData();

                            if (elements.Length >= 6)
                            {
                                data.Order = Convert.ToInt32(elements[0]);
                                data.Name = elements[1];
                                if(!string.IsNullOrEmpty(data.Name))
                                {
                                    data.Name = data.Name.Trim();
                                }
                                if (!string.IsNullOrEmpty(elements[2]))
                                {
                                    int nr = 0;
                                    if (int.TryParse(elements[2], out nr))
                                    {
                                        data.SkytterNr = nr;
                                    }
                                }
                                data.Skytterlag = elements[3];
                                if (!string.IsNullOrEmpty(data.Skytterlag))
                                {
                                    data.Skytterlag = data.Skytterlag.Trim();
                                }
                                if (!string.IsNullOrEmpty(elements[4]))
                                {
                                    int nr = 0;
                                    if (int.TryParse(elements[4], out nr))
                                    {
                                        data.SkytterlagNr = nr;
                                    }
                                }
                                data.Klasse = elements[5];
                                if (!string.IsNullOrEmpty(data.Klasse))
                                {
                                    data.Klasse = data.Klasse.Trim();
                                }
                                restunVal.Add(data);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, line);
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                throw;
            }

            return restunVal;
        }
    }
}