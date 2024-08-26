using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using PaintProject.Shapes;
using Rectangle = PaintProject.Shapes.Rectangle;

namespace PaintProject
{
    public partial class Form1 : Form
    {

        //Fields
        private List<Shape> _shapes = new List<Shape>();

        private Rectangle _rectangle;

        private Ellipse _ellipse;

        private Line _line;

        private bool _paint = false;
        private int _index = 1;
        private int _x, _y, _sx, _sy, _cx, _cy;
        private Color _colorP;
        private Point _pointX, _pointY;
        private Bitmap _bitmapN;
        private Graphics _graphics;
        private readonly Pen _pen = new Pen(Color.Black,2);
        private readonly Pen _eraser = new Pen(Color.White, 2);

        private readonly ColorDialog _colorDialog= new ColorDialog();
        private readonly Stack<Bitmap> _undoStack = new Stack<Bitmap>();
        private readonly Stack<Bitmap> _redoStack = new Stack<Bitmap>();

        private void SaveStateToUndoStack()
        {
            _undoStack.Push((Bitmap)_bitmapN.Clone());
        }

        private void BtnUndo_Click(object sender, EventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                _redoStack.Push(_bitmapN);

                _bitmapN = _undoStack.Pop();

                _graphics = Graphics.FromImage(_bitmapN);
                Pic.Image = _bitmapN;
                
            }
            else
            {
                MessageBox.Show(@"There are no more actions to undo.");
            }
        }

        private void BtnRedo_Click(object sender, EventArgs e)
        {
            if (_redoStack.Count > 0)
            {
                _undoStack.Push(_bitmapN);

                _bitmapN = _redoStack.Pop();

                
                _graphics = Graphics.FromImage(_bitmapN);
                Pic.Image = _bitmapN;
                
            }
            else
            {
                MessageBox.Show(@"There are no more actions to redo.");
            }
        }


        private static Point SetPoint(PictureBox pictureBox,Point point)
        {
            float pX = 1f * pictureBox.Image.Width / pictureBox.Width;
            float pY = 1f * pictureBox.Image.Height / pictureBox.Height;

            return new Point((int)(point.X * pX),(int)(point.Y * pY));
        }
        private void Validate(Bitmap bitmap,Stack<Point>pointStack,int x,int y,Color colorNew,Color colorOld)
        {
            Color cx = bitmap.GetPixel(x,y);
            if(cx == colorOld)
            {
                pointStack.Push(new Point(x,y));
                bitmap.SetPixel(x, y, colorNew);
            }
        }
        public void FillUp(Bitmap bitmap,int x, int y,Color newColor)
        {
            Color oldColor=bitmap.GetPixel(x,y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x,y));
            bitmap.SetPixel(x, y, newColor);
            if (oldColor == newColor) return;
            while (pixel.Count > 0)
            {
                Point point=(Point)pixel.Pop();
                if (point.X>0 && point.Y>0 && point.X<bitmap.Width-1 && point.Y<bitmap.Height-1 ) 
                {
                    Validate(bitmap,pixel,point.X-1,point.Y,newColor,oldColor);
                    Validate(bitmap, pixel, point.X, point.Y-1,newColor, oldColor);
                    Validate(bitmap, pixel, point.X+1, point.Y,newColor, oldColor);
                    Validate(bitmap, pixel, point.X, point.Y+1,newColor, oldColor);

                }
            }
        }

        


        public Form1()
        {
            InitializeComponent();
            _bitmapN= new Bitmap(Pic.Width,Pic.Height);
            _graphics=Graphics.FromImage(_bitmapN);
            _graphics.Clear(Color.White);
            Pic.Image = _bitmapN;
            ButtonPencil.BackColor = ButtonPencilWidth1.BackColor = Color.LightGreen;
        }

        private void BtnColor_Click(object sender, EventArgs e)
        {
            PictureBox capyPictureBox = (PictureBox)sender;
            PenColor.BackColor = _pen.Color = _colorP = capyPictureBox.BackColor;
            SaveStateToUndoStack();


        }
        private void BtnColorSet_Click(object sender, EventArgs e)
        {
            _colorDialog.ShowDialog();
            _colorP = PenColor.BackColor = _pen.Color = _colorDialog.Color;
            SaveStateToUndoStack();
        }

        private void BtnPenWidth_Click(object sender, EventArgs e)
        {
            foreach (var btn in PnlPenWidth.Controls.OfType<Button>())
                btn.BackColor = Color.WhiteSmoke;
            Button capyButton = (Button)sender;
            capyButton.BackColor = Color.LightGreen;
            _pen.Width = _eraser.Width = Convert.ToInt32(capyButton.Tag);
            SaveStateToUndoStack();

        }


        private void Btn_Click(object sender, EventArgs e)
        {
            foreach (var btn in tableLayoutPanel1.Controls.OfType<Button>())
                btn.BackColor = Color.WhiteSmoke;
            Button capyButton = (Button)sender;
            capyButton.BackColor = Color.LightGreen;
            _index = Convert.ToInt32(capyButton.Tag);

        }
        private void Pic_MouseUp(object sender, MouseEventArgs e)
        {
            _paint = false;
            _sx = _x - _cx;
            _sy = _y - _cy;
            if (_index == 4)
            {
                _graphics.DrawLine(_pen, _cx, _cy, _x, _y);
                SaveStateToUndoStack();
            }
            if (_index == 5)
            {
                
                _graphics.DrawRectangle(_pen, _cx, _cy, _sx, _sy);
                SaveStateToUndoStack();
            }
            if (_index == 6)
            {
                _graphics.DrawEllipse(_pen, _cx, _cy, _sx, _sy);
                SaveStateToUndoStack();
            }
            

        }

        private void Pic_MouseDown(object sender, MouseEventArgs e)
        {
            _paint = true;
            _pointY = e.Location;

            _cx = e.X;
            _cy = e.Y;

           



        }

        private void Pic_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = SetPoint(Pic, e.Location);
            if (_index == 3)
            {
                
                FillUp(_bitmapN, point.X, point.Y, _colorP);
                SaveStateToUndoStack();

            }

        }

        private void Pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (_paint)
            {
                if (_index == 1 || _index == 2)
                {
                    
                    _pointX = e.Location;
                    if (_index == 1)
                    {
                        _graphics.DrawLine(_pen, _pointX, _pointY);
                    }
                    else if (_index == 2)
                    {
                        _graphics.DrawLine(_eraser, _pointX, _pointY);
                    }
                    _pointY = _pointX;
                }
            }
            Pic.Refresh();
            _x = e.X;
            _y = e.Y;
            _sx = e.X - _cx;
            _sy = e.Y - _cy;

        }
        private void Pic_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphicsPaint=e.Graphics;
            if (_paint)
            {
                if (_index == 4)
                {
                    _line?.Draw(e.Graphics);
                }
                if (_index == 5)
                {
                    _rectangle?.Draw(e.Graphics);
                }
                if (_index == 6)
                {
                    _ellipse?.Draw(e.Graphics);
                }
            }
        }
        private void ButtonClear_Click(object sender, EventArgs e)
        {
            _graphics.Clear(Color.White);
            Pic.Image = _bitmapN;
            foreach (var btn in tableLayoutPanel1.Controls.OfType<Button>())
                btn.BackColor = Color.WhiteSmoke;
            foreach (var btn in PnlPenWidth.Controls.OfType<Button>())
                btn.BackColor = Color.WhiteSmoke;
            ButtonPencil.BackColor = ButtonPencilWidth1.BackColor = Color.LightGreen;
            _pen.Width = _eraser.Width = 2;
            _index = 1;
            SaveStateToUndoStack();
        }
        
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png Image|*.png";
            ImageFormat format = ImageFormat.Png;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Pic.Image.Save(saveFileDialog.FileName, format);
            }

            SaveStateToUndoStack();
        }
        
        

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;

        }
        private void ButtonX_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ButtonMinus_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Minimized;
        }

        private void Picture_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
