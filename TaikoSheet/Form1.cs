using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Taiko;
namespace TaikoSheet {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            SongData sr = new SongData(@"J:\Taiko\wii4 sheets\sheet\newsht\solo\linda_m.bin", true); //Linda Linda
            //SongData sr = new SongData(@"J:\Taiko\wii5 sheets\sheet\newsht\solo\gumima_h.bin"); //Matryoshka
            //SongData sr = new SongData(@"J:\Taiko\wii5 sheets\sheet\newsht\solo\sidos_h.bin"); //Sid - S
            //SongData sr = new SongData(@"J:\Taiko\wii5 sheets\angel21p_h.bin"); //wii1 angel dream
            //SongData sr = new SongData(@"J:\Taiko\wii4 sheets\sheet\newsht\solo\marumo_m.bin");
            //SongData sr = new SongData(@"J:\Taiko\wii5 sheets\brs_h.bin"); //wii3 black rock shooter
            //SongData sr = new SongData(@"J:\Taiko\wii4 sheets\sheet\newsht\solo\ekiben_m.bin", true);
            //SongData sr = new SongData(@"J:\Taiko\wii4 sheets\sheet\newsht\solo\trance_h.bin", false); 
            //SongData sr = new SongData(@"J:\Taiko\wii4 sheets\sheet\newsht\solo\hatara_m.bin", true);
            //SongData sr = new SongData(@"J:\Taiko\wii4 sheets\sheet\newsht\solo\tktime_n.bin", true);
            //SongData sr = new SongData(@"J:\Taiko\wii5 sheets\sheet\newsht\solo\castle_m.bin", true);
           
            foreach (BarData bar in sr.Bars) {
                //Console.WriteLine(bar.EndOffsetInt - 17000);
                Console.WriteLine();
                Console.WriteLine("Bar # " + bar.BarIndex);
                if (bar.SpeedAfterBar > 0)
                {
                    Console.WriteLine("speed : " + bar.Speed);
                    Console.WriteLine("SCROLL SPEED CHANGE TO : " + bar.SpeedAfterBar);
                }
                Console.WriteLine("start offset: " + bar.Offset);

                foreach (List<NoteData> noteList in bar.NoteGroups) {

                    Console.WriteLine("  " + noteList.Count + " notes");
                    foreach (NoteData note in noteList) {
                        string noteType = "";
                        noteType = Enum.GetName(typeof(TaikoNoteType), note.NoteType);

                        int x = (int)(note.NoteOffset > 0 ? (note.NoteOffset) : 0);
                        float xPercent = (float)(Convert.ToDouble(x) / 892);

                        if (xPercent < .5f) {
                            xPercent *= 0.5f;
                        }
                        else if (xPercent > .5f) {
                            xPercent *= 1.5f;
                        }

                        Console.WriteLine("    " + (noteType == null ? "Unknown (" + (int)note.NoteType + ")" : noteType) + " " + note.NoteOffsetString + " " + x + " " + xPercent + " " + note.NoteLength.ToString());
                    }
                }

                Console.WriteLine(bar.BarFooter);
            }
            
            taikoBartest1.Songdata = sr;
            //BarData bTest = new BarData(sr.Bars[12].BarHex, true);
        }
    }
}
