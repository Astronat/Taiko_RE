using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Taiko {
    public enum TaikoNoteType {
        None = 0,
        Don = 1, //Actually 1-3
        Katsu = 4, //Actually 4 or 5
        Drum_Roll = 6,
        Don_Large = 7,
        Katsu_Large = 8,
        Drum_Roll_Large = 9,
        Balloon = 0xA,
        //Dumpling @ 0xB? 
        Party_Ball = 0xC
        //Probably Spirit Explosion Taiko at 0xD
    }

    public enum TaikoSheetVersion {
        //DS,  //FIND OUT IF THESE
        //PS2, //ARE EVEN PLAUSIBLE
        //PSP, 3DS?!?!?!? no probably not
        One,
        Two,
        Three
    }


    /* BarHeader
     *  [int32 containing notes in bar]4200 3F8[6 byte value for speed change]
     *      [note type] [note offset] OffsetFooter F4FA1200 something CCFA1200
     * [current speed] [position of next bar] [01011200 or something similar, the second byte seems to change occasionally]
     */

    
    
    //TODO Find out which version Taiko 2 uses

    //Note sheet v1 Constants
    //Taiko Wii 1
    public static class TaikoConstants1 {
    }
    //Note sheet v2 constants
    //Taiko Wii 3
    public static class TaikoConstants2 {

    }
    //note sheet v3 constants
    //Taiko Wii 4 and 5
    public static class TaikoConstants3 {
        public const string BarHeader = "0CFD1200";
        public const string OffsetFooter = "00FB1200";
        public const string NoteFooter = "CCFA1200";
        public const string BarFooter0 = "00011200";
        public const string BarFooter1 = "01011200";
        public const string BarFooter2 = "00001200";
        public const string BarSeperator = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
    }

    public class SongData {
        string fileHex = "";
        List<BarData> bars = new List<BarData>();
        public List<BarData> Bars { get { return bars; } }


        List<string> headerChunks = new List<string>();
        public List<string> HeaderChunks { get { return headerChunks; } }


        int notes = 0;
        public int NoteCount { get { return notes; } }

        uint startOffset = 0x0;
        public uint StartOffset { get { return startOffset; } }

        public SongData(string fileName, bool writetofile = false) {
            //Read the entire sheet file as hex into a string
            fileHex = HelperFunctions.readFileAsHex(fileName);

            //replace all of the bar headers with a newline, effectively making each line a new bar
            fileHex = fileHex.Replace(TaikoConstants3.BarSeperator, Environment.NewLine);

            //Console.WriteLine(long.Parse("FFFFFFFF", System.Globalization.NumberStyles.HexNumber));

            StringReader sreader = new StringReader(fileHex);
            int count = 0;

            StreamWriter fs = null;
            string head = sreader.ReadLine();

            for (int i = 0; i < head.Length; i += 8) {
                headerChunks.Add(head.Substring(i, 8));
            }

            while (sreader.Peek() > -1) {
                string line = sreader.ReadLine();

                if (startOffset == 0x0) {
                    startOffset = uint.Parse(line.Substring(5 * 8, 8), System.Globalization.NumberStyles.HexNumber);
                }

                //Console.WriteLine(sreader.ReadLine());        
                if (writetofile) {
                    if (fs == null)
                        fs = new StreamWriter("out.txt");

                    fs.Write(line + Environment.NewLine);
                }
                bars.Add(new BarData(line));

                //if (bars[bars.Count - 1].
                bars[count].BarIndex = count;
                notes += bars[count].NoteCount;
                count++;
            }

            if (writetofile)
                fs.Close();
        }
    }

    public class BarData {
        string barAsHex = "";
        public string BarHex { get { return barAsHex; } }

        int noteCount = 0;
        public int NoteCount { get { return noteCount; } set { noteCount = value; } }

        int barIndex = 0;
        public int BarIndex { get { return barIndex; } set { barIndex = value; } }

        float speed = 0;
        public float Speed { get { return speed; } }

        float speedChangeAfterBar = 0;
        public float SpeedAfterBar { get { return speedChangeAfterBar; } }

        float offset = 0;
        public float Offset { get { return offset; } }

        int intEndOffset = 0;
        public int EndOffsetInt { get { return intEndOffset; } }

        string strEndOffset = "";
        public string EndOffsetString { get { return strEndOffset; } }

        string barFooter = "";
        public string BarFooter { get { return barFooter; } }

        List<List<NoteData>> noteGroups = new List<List<NoteData>>();
        public List<List<NoteData>> NoteGroups { get { return noteGroups; } }

        List<string> chunks = new List<string>();
        public List<string> Chunks { get { return chunks; } }

        public BarData(string bar, bool dbug = false) {

            for (int i = 0; i < bar.Length; i += 8) {
                chunks.Add(bar.Substring(i, 8));
            }

            barAsHex = bar;

            barFooter = chunks[chunks.Count - 1];

            if (chunks[0] != TaikoConstants3.BarHeader || (barFooter != TaikoConstants3.BarFooter0 && barFooter != TaikoConstants3.BarFooter1 && barFooter != TaikoConstants3.BarFooter2)) {
                return;
            }
            else {
                string lastThree = "";

                strEndOffset = chunks[chunks.Count - 2];
                intEndOffset = HelperFunctions.ParseInt(chunks[chunks.Count - 2]);
                offset = HelperFunctions.ParseFloat(chunks[chunks.Count - 2]);

                speed = HelperFunctions.ParseFloat(chunks[chunks.Count - 3]);

                speedChangeAfterBar = HelperFunctions.ParseFloat(chunks[2]);

                //Console.WriteLine(chunks[chunks.Count - 2] + " " + HelperFunctions.ParseUint(chunks[chunks.Count - 2]) + " " + speed);
                
                for (int i = 0; i < 3; i++) { lastThree += chunks[(chunks.Count - 3) + i]; }
                lastThree = lastThree.Trim();

                //Strip header/footer
                string pattern = string.Format(@"(?<={0})(?<Notes>.*)(?={1})", TaikoConstants3.BarHeader, lastThree);
                string trimmed = Regex.Match(bar, pattern).Groups["Notes"].Value;

                //pattern = "(?<NoteGroup>[0-9A-F]{4}42003F[0-9A-F]{6}(?:.*?(?=[0-9A-F]{4}4200)|))";
                pattern = "(?<NoteGroup>[0-9A-F]{4}4200[0-9A-F]{6,12}(?:.*?(?=[0-9A-F]{4}4200)|))";
                MatchCollection NoteGroupMatches = Regex.Matches(trimmed, pattern);

                //Console.WriteLine(trimmed);

                List<NoteData> ndListTmp = new List<NoteData>();
                //Match m = NoteGroupMatches[0];
                foreach (Match m in NoteGroupMatches) {
                    if (m.Groups["NoteGroup"].Value.Length > 16) {
                        pattern = "(?<Note>(?<NoteType>[0]{7}[1-9A-F]{1})(?<Offset>[0-9A-F]{8})[0-9A-F]{24}(?<ExtraData>(?:[0-9A-F]{16}|))" + TaikoConstants3.NoteFooter + ")";

                        MatchCollection noteMatches = Regex.Matches(m.Groups["NoteGroup"].Value, pattern);

                        ndListTmp = new List<NoteData>();

                        foreach (Match note in noteMatches) {
                            int type = HelperFunctions.ParseInt(note.Groups["NoteType"].Value);

                            if (type > 0 && type < 4) //Don
                                type = 1;

                            if (type == 5) //Katsu
                                type = 4;

                            noteCount++;
                            ndListTmp.Add(new NoteData((TaikoNoteType)type, note.Groups["Offset"].Value.Substring(0, 8), (note.Groups["ExtraData"].Value.Length > 7 ? HelperFunctions.ParseUint(note.Groups["ExtraData"].Value.Substring(0, 8)) : 0x0)));
                        }

                        noteGroups.Add(ndListTmp);
                    }
                }
            }
        }
    }

    public class NoteData {
        float noteOffset = 0;
        public float NoteOffset { get { return noteOffset; } }
        string noteOffsetString = "";
        public string NoteOffsetString { get { return noteOffsetString; } }

        uint notelength = 0;
        public uint NoteLength { get { return notelength; } }

        TaikoNoteType type = TaikoNoteType.Don;
        public TaikoNoteType NoteType { get { return type; } }

        public NoteData(TaikoNoteType tnt, string offset, uint noteLength = 0x00000000) {
            noteOffsetString = offset;
            noteOffset = HelperFunctions.ParseFloat(offset);
            notelength = noteLength;
            type = tnt;
        }
    }

    class HelperFunctions {
        public static uint ParseUint(object input) {
            try {
                return uint.Parse(input.ToString(), System.Globalization.NumberStyles.HexNumber);
            }
            catch { return 0x0; }
        }
        public static float ParseFloat(object input)
        {
            try
            {
                uint num = uint.Parse(input.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                byte[] bytes = BitConverter.GetBytes(num);
                //float myFloat = BitConverter.ToSingle(bytes, 0);
                return BitConverter.ToSingle(bytes,0);
            }
            catch { return 0x0; }
        }
        public static int ParseInt(object input) {
            try {
                return int.Parse(input.ToString(), System.Globalization.NumberStyles.HexNumber);
            }
            catch { return 0; }
        }
        public static string ConvertStringToHex(string asciiString) {
            string hex = "";
            foreach (char c in asciiString) {
                if (c != '\r' && c != '\n' && c != Environment.NewLine[0]) {
                    int tmp = c;
                    hex += String.Format("{0:X}", (uint)System.Convert.ToUInt32(tmp.ToString()));
                }
            }
            return hex;
        }

        public static string readFileAsHex(string fn) {
            string outStr = "";
            using (var file = File.Open(fn, FileMode.Open)) {
                int b;
                while ((b = file.ReadByte()) >= 0) {
                    outStr += b.ToString("X2");
                }
            }

            return outStr;
        }

        public static bool needsLength(TaikoNoteType type) {
            switch ((int)type) {
                case 6:
                case 9:
                case 10:
                case 0xC:
                    return true;

                default:
                    return false;
            }
        }
    }
}
