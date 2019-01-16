using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using MathNet.Numerics.LinearAlgebra;

namespace MES_Console
{
    public partial class Plot : Form
    {

        double[,] data;
        int nH, nL;
        double ambientT;

        public Plot()
        {
            InitializeComponent();
        }

        public Plot(double[,] data, int nH, int nL, double ambientT)
        {
            this.data = data;
            this.nH = nH;
            this.nL = nL;
            this.ambientT = ambientT;
            InitializeComponent();
        }

        private void Plot_Load(object sender, EventArgs e)
        {
            //Create Plotview object
            PlotView myPlot = new PlotView();

            var model = new PlotModel { Title = "GRID" };


            // Color axis


            model.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Jet(1200), Maximum = ambientT, Minimum = 100 });


            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = 16,
                Y0 = 0,
                Y1 = 16,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Rectangles,
                LabelFontSize = 0.2, // neccessary to display the label
                Data = data,
            };

            model.Series.Add(heatMapSeries);

            //Assign PlotModel to PlotView
            myPlot.Model = model;

            //Set up plot for display
            //myPlot.Dock = System.Windows.Forms.DockStyle.Bottom;
            myPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            myPlot.Location = new System.Drawing.Point(0, 0);
            myPlot.Size = new System.Drawing.Size(500, 500);
            myPlot.TabIndex = 0;

            //Add plot control to form
            Controls.Add(myPlot);
        }
    }
}
