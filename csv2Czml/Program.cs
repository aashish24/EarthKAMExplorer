﻿using CesiumLanguageWriter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace csv2Czml
{
    class Program
    {
        public static void writeCzml()
        {
            var files = new[]{
                @"Data\ISS11_07_image_data.csv",
                @"Data\ISS11_11_image_data.csv",
                @"Data\ISS12_01_image_data.csv",
                @"Data\ISS12_07_2_image_data.csv",
                @"Data\ISS12_11_image_data.csv",
                @"Data\ISS13_01_image_data.csv",
                @"Data\ISS11_04_image_data.csv"
                };

            foreach (var file in files)
            {
                StringWriter f = new StringWriter();
                var m_output = new CesiumOutputStream(f);
                m_output.PrettyFormatting = false;
                var m_writer = new CesiumStreamWriter();
                m_output.WriteStartSequence();

                string[] lines = File.ReadAllLines(file);
                var rng = new Random();

                GregorianDate start = new GregorianDate();
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string[] tokens = line.Split(new[] { ',' });
                    for (int q = 0; q < tokens.Length; q++)
                    {
                        tokens[q] = tokens[q].Trim('"').Trim();
                    }

                    if (i == 1)
                    {
                        start = GregorianDate.Parse(tokens[17]);
                    }
                    else if (i == lines.Length - 1)
                    {
                        Console.WriteLine(Path.GetFileNameWithoutExtension(file));
                        Console.WriteLine(start.ToJulianDate().TotalDays + " JDate");
                        var stop = GregorianDate.Parse(tokens[17]);
                        Console.WriteLine(stop.ToJulianDate().TotalDays + " JDate");
                        Console.WriteLine();
                        //Console.WriteLine((stop.ToJulianDate() - start.ToJulianDate()).TotalDays);
                    }
                    using (var packet = m_writer.OpenPacket(m_output))
                    {
                        packet.WriteId(tokens[0]);
                        using (var vertexPositions = packet.OpenVertexPositionsProperty())
                        {
                            var points = new Cartographic[]{
                        new Cartographic(double.Parse(tokens[5]), double.Parse(tokens[6]), 0),
                        new Cartographic(double.Parse(tokens[7]), double.Parse(tokens[8]), 0),
                        new Cartographic(double.Parse(tokens[9]), double.Parse(tokens[10]), 0),
                        new Cartographic(double.Parse(tokens[11]), double.Parse(tokens[12]), 0)
                        };
                            vertexPositions.WriteCartographicDegrees(points);
                        }
                        using (var polygon = packet.OpenPolygonProperty())
                        {
                            polygon.WriteShowProperty(true);
                            using (var material = polygon.OpenMaterialProperty())
                            {
                                using (var color = material.OpenSolidColorProperty())
                                {
                                    color.WriteColorProperty(Color.FromArgb(255, (int)(rng.NextDouble() * 255), (int)(rng.NextDouble() * 255), (int)(rng.NextDouble() * 255)));
                                }
                            }
                        }
                    }
                }
                m_output.WriteEndSequence();
                m_output.Dispose();
                File.WriteAllText(Path.GetFileNameWithoutExtension(file) + ".czml", f.ToString());
            }
        }

        static void writeJsonArray(StringBuilder output, string propertyName, List<string> items)
        {
            output.Append("\"");
            output.Append(propertyName);
            output.Append("\":");
            output.Append("[");
            for (int i = 0; i < items.Count; i++)
            {
                output.Append("\"");
                output.Append(items[i]);
                output.Append("\"");
                if (i < items.Count - 1)
                    output.Append(",");
            }
            output.Append("]");
        }

        static public void writeJson()
        {
            var files = new[]{
                @"Data\ISS11_07_image_data.csv",
                @"Data\ISS11_11_image_data.csv",
                @"Data\ISS12_01_image_data.csv",
                @"Data\ISS12_07_2_image_data.csv",
                @"Data\ISS12_11_image_data.csv",
                @"Data\ISS13_01_image_data.csv",
                @"Data\ISS11_04_image_data.csv"
                };

            List<string> ID = new List<string>();
            List<string> Time = new List<string>();
            List<string> Mission = new List<string>();
            List<string> School = new List<string>();
            List<string> ImageUrl = new List<string>();
            List<string> LensSize = new List<string>();
            List<string> OrbitNumber = new List<string>();
            List<string> FrameWidth = new List<string>();
            List<string> FrameHeight = new List<string>();
            List<string> Page = new List<string>();
            List<string> CZML = new List<string>();

            foreach (var file in files)
            {
                string[] lines = File.ReadAllLines(file);

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string[] tokens = line.Split(new[] { ',' });
                    for (int q = 0; q < tokens.Length; q++)
                    {
                        tokens[q] = tokens[q].Trim('"').Trim().Replace("\"", "\\\"");
                    }

                    ID.Add(tokens[0]);
                    Time.Add(GregorianDate.Parse(tokens[17]).ToIso8601String(Iso8601Format.Compact));
                    Mission.Add(tokens[18]);
                    School.Add(tokens[23]);
                    ImageUrl.Add(tokens[21].Split(new[] { '=' })[2]);
                    LensSize.Add(tokens[14]);
                    OrbitNumber.Add(tokens[19]);
                    FrameWidth.Add(tokens[15]);
                    FrameHeight.Add(tokens[16]);
                    Page.Add(tokens[20].Split(new[] { '=' })[1]);
                    CZML.Add(Path.GetFileNameWithoutExtension(file) + ".czml");
                }
            }

            StringBuilder output = new StringBuilder();
            output.AppendLine("{");
            writeJsonArray(output, "ID", ID);
            output.AppendLine(",");
            writeJsonArray(output, "Time", Time);
            output.AppendLine(",");
            writeJsonArray(output, "Mission", Mission);
            output.AppendLine(",");
            writeJsonArray(output, "School", School);
            output.AppendLine(",");
            writeJsonArray(output, "ImageUrl", ImageUrl);
            output.AppendLine(",");
            writeJsonArray(output, "LensSize", LensSize);
            output.AppendLine(",");
            writeJsonArray(output, "OrbitNumber", OrbitNumber);
            output.AppendLine(",");
            writeJsonArray(output, "FrameWidth", FrameWidth);
            output.AppendLine(",");
            writeJsonArray(output, "FrameHeight", FrameHeight);
            output.AppendLine(",");
            writeJsonArray(output, "Page", Page);
            output.AppendLine(",");
            writeJsonArray(output, "CZML", CZML);
            output.AppendLine();
            output.AppendLine("}");
            File.WriteAllText("missions.json", output.ToString());
        }

        static void Main(string[] args)
        {
            writeCzml();
            writeJson();
        }
    }
}
