using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class MainForm : Form
    {
        private readonly Game game;

        public MainForm()
        {
            InitializeComponent();

            this.KeyPreview = true;
            btnReset.Enabled = false;

            this.game = new Game(this);
            game.Load();
        }

        private void btnStartPause_Click(object sender, EventArgs e)
        {
            if (!game.isLoaded)
                return;

            if (!game.isStarted)
            {
                game.Run();
                btnReset.Enabled = true;
                btnStartPause.Text = "Pause";
            }
            else if (game.isStarted && !game.isPaused)
            {
                game.Pause();
                btnReset.Enabled = true;
                btnStartPause.Text = "Resume";
            }
            else if (game.isStarted && game.isPaused)
            {
                game.Run();
                btnReset.Enabled = true;
                btnStartPause.Text = "Pause";
            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (game.isStarted)
            {
                game.Reset();

                btnStartPause.Text = "Start";
                lblScoreValue.Text = "0";

                btnReset.Enabled = false;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.SuppressKeyPress = true; // do this to 'eat' the keystroke

            if (game != null && game.isStarted)
            {
                game.SetUserInput(e);
            }

            base.OnKeyDown(e);
        }
    }
}
