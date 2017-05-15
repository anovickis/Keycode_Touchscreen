using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// BlankForm - display a uniformly blank screen 
    ///             for use to display while 
    ///             switching between screens to prevent 
    ///             the desktop from showing
    /// </summary>
    public partial class BlankForm : Form
    {
        public BlankForm()
        {
            InitializeComponent();
        }
    }
}
