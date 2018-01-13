using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Taiko;

namespace TaikoSheet {
    public partial class TaikoBartest : UserControl {
        SongData sd = null;
        public SongData Songdata { get { return sd; } set { sd = value; } }

        public int startoffset = 0;

        int barwidth = 406;

        public TaikoBartest() {
            InitializeComponent();
            this.Paint += new PaintEventHandler(TaikoBartest_Paint);
        }

        void TaikoBartest_Paint(object sender, PaintEventArgs e) {
            e.Graphics.Clear(Color.LightGray);
            //e.Graphics.DrawLine(Pens.Black, 0, 0, 0, this.Height);
            //e.Graphics.DrawLine(Pens.Black, 85, 0, 85, this.Height);

            if (sd != null) {
                
                int end = 0;
                //foreach (BarData bd in sd.Bars) {
                for (int i = 4; i < sd.Bars.Count; i++) { BarData bd = sd.Bars[i];
                    float start = sd.Bars[i].Offset / 100;
                int lastx = 0;



                    if (bd.NoteGroups.Count > 0) {
                        List<NoteData> noteList = bd.NoteGroups[0];
                        //end += (bd.EndOffsetInt - 17000);

                        end += barwidth;
                        
                        foreach (NoteData nd in noteList) {
                            Pen lineCol = Pens.Red;
                            bool roll = false;

                            switch (nd.NoteType) {
                                case TaikoNoteType.Don:
                                    lineCol = Pens.Red;
                                    break;

                                case TaikoNoteType.Don_Large:
                                    lineCol = new Pen(Brushes.Red, 4.0f);
                                    break;

                                case TaikoNoteType.Katsu:
                                    lineCol = Pens.Blue;
                                    break;

                                case TaikoNoteType.Katsu_Large:
                                    lineCol = new Pen(Brushes.Blue, 4.0f);
                                    break;

                                case TaikoNoteType.Drum_Roll:
                                case TaikoNoteType.Drum_Roll_Large:
                                    roll = true;
                                    lineCol = Pens.Yellow;
                                    break;
                            }
                            
                            float x = (start + (nd.NoteOffset)) / 100;

                            //x += lastx;

 
                                                        
                            /*
                            if (xPercent < .5f) {
                                xPercent *= 0.75f;
                            }
                            else if (xPercent > .5f) {
                                xPercent *= 1.25f;
                            }*/
                            
                            //Console.WriteLine(xPercent);
                            //Console.WriteLine(nd.NoteOffset);

                            //Console.WriteLine(start + x);

                           // Console.WriteLine(xPercent * (end - start));

                            float pos = x;//(xPercent * (barwidth));

                            //if (start + x < this.Width)
                                e.Graphics.DrawLine(lineCol, pos, 0, pos, this.Height);

                                //e.Graphics.DrawLine(lineCol, start + ((xPercent * 150) * 2), 0, start + ((xPercent * 150) * 2), this.Height);
                                //e.Graphics.DrawLine(lineCol, start + x, 0, start + x, this.Height);

                            e.Graphics.DrawLine(new Pen(Brushes.Black, 2), start, 0, start, 5);
                            
                        }

                        //start += (bd.EndOffsetInt - 17000);
                        //start += barwidth;
                        //Console.WriteLine();
                        //Console.WriteLine(start);
                    }
                }
            }
        }
    }
}
