using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dragon_
{
    public partial class dragLoad : Form
    {
        public dragLoad()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            Refresh();

            CheckForIllegalCrossThreadCalls = false;
        }

        private Rectangle sizeGripRectangle;
        int paintReps = 0;

        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);
            ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, sizeGripRectangle);
            System.Threading.Thread.Sleep(0);

            if (paintReps++ % 500 == 0)
                Application.DoEvents();

        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();

            base.OnScroll(se);

            base.DoubleBuffered = true;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            base.OnScroll(se);

        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;

                return cp;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void dragLoad_Load(object sender, EventArgs e)
        {

            Random rnd = new Random();
            int xLoad = rnd.Next(2000, 7000);

            finishLoading.Interval = xLoad;
            finishLoading.Start();

        }

        private void finishLoading_Tick(object sender, EventArgs e)
        {
            finishLoading.Stop();

            this.Hide();

            DragonIDE dg = new DragonIDE();
            dg.Show();


        }
    }
}
