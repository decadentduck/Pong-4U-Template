﻿/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //paddle position variables
        int paddle1Y, paddle2Y;

        //ball position variables
        int ballX, ballY;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions
        Boolean ballMoveRight = false;
        Boolean ballMoveDown = false;

        //constants used to set size and speed of paddles 
        const int PADDLE_LENGTH = 40;
        const int PADDLE_WIDTH = 10;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle
        const int PADDLE_SPEED = 4;

        //constants used to set size and speed of ball 
        const int BALL_SIZE = 10;
        const int BALL_SPEED = 4;

        //player scores
        int player1Score = 0;
        int player2Score = 0;

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        //game winning score
        int gameWinScore = 5;

        //brush for paint method
        SolidBrush drawBrush = new SolidBrush(Color.White);

        Boolean ballRed = false;
        int redWallX = 0;
        int blueWallX = 0;

        #endregion

        public Form1()
        {
            InitializeComponent();
            SetParameters();
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                    if (newGameOk)
                    {
                        GameStart();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
                case Keys.Space:
                    if (newGameOk)
                    {
                        GameStart();
                    }
                    break;
                default:
                    break;
            }
        }
        
        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets up the game objects in their start position, resets the scores, displays a 
        /// countdown, and then starts the game timer.
        /// </summary>
        private void GameStart()
        {
            newGameOk = true;
            SetParameters();
            
            Graphics formGraphics = this.CreateGraphics();            SolidBrush theBrush = new SolidBrush(Color.Black);            Font drawfont = new Font("Courier New", 35);
            startLabel.Visible = false;
            Refresh();
            
            //countdown to start of game
            for (int i = 3; i > 0; i-- ) 
            {
                formGraphics.DrawString(Convert.ToString(i), drawfont, theBrush, (this.Width/2) - 16, (this.Height/2) - 63);
                Thread.Sleep(1000);
                this.Refresh();
            }
            
            gameUpdateLoop.Start();
            newGameOk = false;
        }
  
        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                // sets location of score labels to the middle of the screen
                player1Label.Location = new Point(this.Width / 2 - player1Label.Size.Width - 10, player1Label.Location.Y);
                player2Label.Location = new Point(this.Width / 2 + 10, player2Label.Location.Y);

                //set label, score variables, and ball position
                player1Score = player2Score = 0;
                player1Label.Text = "RED :  " + player1Score;
                player2Label.Text = "BLUE :  " + player2Score;

                paddle1Y = paddle2Y = this.Height / 2 - PADDLE_LENGTH / 2;
                redWallX = 0;
                blueWallX = 0;

            }
            
            ballY = (this.Height/2) - (BALL_SIZE/2);
            ballX = (this.Width / 2) - (BALL_SIZE/2);

        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            //sound player to be used for all in game sounds initially set to collision sound
            SoundPlayer player = new SoundPlayer();
            player = new SoundPlayer(Properties.Resources.Loud_Bang);

            #region update ball position
            
            if(ballMoveRight == true)
            {
                ballX = ballX + BALL_SPEED;
            }
            else
            {
                ballX = ballX - BALL_SPEED;
            }

            if (ballMoveDown == true)
            {
                ballY = ballY + BALL_SPEED;
            }
            else
            {
                ballY = ballY - BALL_SPEED;
            }
            #endregion

            #region update paddle positions

            if (aKeyDown == true && paddle1Y > 0)
            {
                paddle1Y = paddle1Y - PADDLE_SPEED;
            }
            if(zKeyDown == true && paddle1Y < this.Height - PADDLE_LENGTH)
            {
                paddle1Y = paddle1Y + PADDLE_SPEED;
            }
   
            if(jKeyDown == true && paddle2Y > 0)
            {
                paddle2Y = paddle2Y - PADDLE_SPEED;
            }
            if(mKeyDown == true && paddle2Y < this.Height - PADDLE_LENGTH)
            {
                paddle2Y = paddle2Y + PADDLE_SPEED;
            }
            #endregion

            #region ball collision with top and bottom lines

            if (ballY < 0) // if ball hits top line
            {
                ballMoveDown = true;
                player.Play();
            }
            else if (ballY > (this.Height - BALL_SIZE))
            {
                ballMoveDown = false;
                player.Play();
            }

            #endregion

            #region ball collision with paddles

            if (ballY > paddle1Y && ballY < paddle1Y + PADDLE_LENGTH && ballX < PADDLE_EDGE + PADDLE_WIDTH) // left paddle collision
            {
                player.Play();
                ballMoveRight = true;
                ballRed = true;
            }
            else if (ballY > paddle2Y && ballY < paddle2Y + PADDLE_LENGTH && ballX + BALL_SIZE > this.Width - PADDLE_EDGE - PADDLE_WIDTH / 2) // right paddle collision
            {
                player.Play();
                ballMoveRight = false;
                ballRed = false;
            }

            #endregion

            #region ball collision with side walls (point scored)

            player = new SoundPlayer(Properties.Resources.score);

            if (ballX < 0)  
            {
                player.Play();
                player2Score++;
                player2Label.Text = "BLUE: " + Convert.ToString (player2Score);
                Refresh();
                blueWallX = blueWallX + 35;

                if(blueWallX > this.Width / 2)
                {
                    GameOver("RED!");
                }
                else
                {
                    SetParameters();
                }
            }
            
            if (ballX > this.Width - BALL_SIZE )
            {
                player.Play();
                player1Score++;
                player1Label.Text = "RED: " + Convert.ToString(player1Score);
                Refresh();
                redWallX = redWallX + 35;

                if (redWallX > this.Width / 2)
                {
                    GameOver("BLUE!");
                }
                else
                {
                    SetParameters();
                }
            }

            #endregion
            
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;
            startLabel.Visible = true;          
            gameUpdateLoop.Stop();
            
            startLabel.Text = "SUCK IT " + winner ;
            Refresh();
            Thread.Sleep(2000);
            startLabel.Text = "Play Again? Y / N";

        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            drawBrush.Color = Color.DarkRed;
            e.Graphics.FillRectangle(drawBrush, 0, 0, redWallX, this.Height);
            drawBrush.Color = Color.Red;
            e.Graphics.FillRectangle(drawBrush, PADDLE_EDGE, paddle1Y, PADDLE_WIDTH, PADDLE_LENGTH);
            
            drawBrush.Color = Color.DarkBlue;
            e.Graphics.FillRectangle(drawBrush, this.Width - blueWallX, 0, this.Width, this.Height);
            drawBrush.Color = Color.Blue;
            e.Graphics.FillRectangle(drawBrush, (this.Width - PADDLE_WIDTH - PADDLE_EDGE), paddle2Y, PADDLE_WIDTH, PADDLE_LENGTH);
            
            if (ballRed == true)
            {
                drawBrush.Color = Color.Red;
            }
            else { drawBrush.Color = Color.Blue; }
            e.Graphics.FillRectangle(drawBrush, ballX, ballY, BALL_SIZE, BALL_SIZE);
        }

    }
}
