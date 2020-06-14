using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
class MyForm : Form {
    #region Menu + initialize buttons
    public MyForm(){
        ClientSize = new Size(1200, 720);
        BackColor = Color.Black;

        Button Random = CreateDynamicButton("Random", 940, 25, 50, 120, Color.GreenYellow, Color.Black);
        Random.Click += new EventHandler(OnRandomClick); 
        this.Controls.Add(Random);
        Button Reset = CreateDynamicButton("Reset", 1070, 25, 50, 120, Color.GreenYellow, Color.Black);
        Reset.Click += new EventHandler(OnResetClick); 
        this.Controls.Add(Reset);
        Button Minor = CreateDynamicButton("Minor", 1070, 100, 50, 120, Color.GreenYellow, Color.Black);
        Minor.Click += new EventHandler(OnMinorClick); 
        this.Controls.Add(Minor);
        Button Help = CreateDynamicButton("?", 1140, 660, 50, 50, Color.GreenYellow, Color.Black);
        Help.Click += new EventHandler(OnHelpClick); 
        this.Controls.Add(Help);

        ToolStripMenuItem[] chooseSize = {
            new ToolStripMenuItem("2x2", null, MatrixSize2x2),
            new ToolStripMenuItem("3x3", null, MatrixSize3x3),
            new ToolStripMenuItem("4x4", null, MatrixSize4x4)
        };

        ToolStripMenuItem[] menu1 = {
            new ToolStripMenuItem("New", null, chooseSize),
            new ToolStripMenuItem("Quit", null, Quit)
        }; 

        ToolStripMenuItem[] RREF = {
            new ToolStripMenuItem("Result", null, ResultForRREF),
            new ToolStripMenuItem("Step by step", null, SbsForRREF),
        }; 

        ToolStripMenuItem[] menu3 = {
            new ToolStripMenuItem("Transpose", null, Transpose),
            new ToolStripMenuItem("Adjoint", null, Adjoint),
            new ToolStripMenuItem("RREF", null, RREF),
            new ToolStripMenuItem("Inverse", null, Inverse)
        };

        ToolStripMenuItem[] menu2 = {
            new ToolStripMenuItem("Ax=0", null, System),
            new ToolStripMenuItem("Rank", null, Rank),
            new ToolStripMenuItem("Determinant", null, Determinant),
            new ToolStripMenuItem("All", null, All)
        };
        
        ToolStripMenuItem[] topItems = {
            new ToolStripMenuItem("File", null, menu1),
            new ToolStripMenuItem("Numerical Computations", null, menu2),
            new ToolStripMenuItem("Matrix Computations", null, menu3),
        }; 

        MenuStrip menu = new MenuStrip();
        foreach (var item in topItems)
            menu.Items.Add(item);

        Controls.Add(menu);     
    }
    #endregion

    #region Menu Functions
    Matrix m = new Matrix(4);
    Matrix mOriginal; //this matrix will be used for RREF and Inverse to display the original
    void Quit(object sender, EventArgs args){
        Application.Exit();
    }
    void MatrixSize2x2(object sender, EventArgs args){
        Reset(2);
    }
    void MatrixSize3x3(object sender, EventArgs args){
        Reset(3);
    }
    void MatrixSize4x4(object sender, EventArgs args){
       Reset(4);
    }
    #endregion
    
    #region  Buttons
    bool minor = false;
    public Button CreateDynamicButton(string buttonName, int x, int y, int height, int width, Color forecolor, Color backcolor)  {  
        Button dynamicButton = new Button();
        dynamicButton.Height = height;
        dynamicButton.Width = width;
        dynamicButton.ForeColor = forecolor;
        dynamicButton.BackColor = backcolor;
        dynamicButton.Location = new Point(x, y);
        dynamicButton.Text = buttonName;
        dynamicButton.Name = buttonName;
        dynamicButton.Font = new Font ("Times New Roman", 13);
        return dynamicButton;
    }

    public void OnRandomClick (object sender, EventArgs args){
        Reset(m.size);
        Random rnd = new Random();
        for(int i = 0; i < m.size; i++){
            for (int j = 0; j < m.size; j++){
                m[i,j] = rnd.Next(-20,20);
            }
        }
        Invalidate();
    }
    public void OnResetClick (object sender, EventArgs args){
        Reset(m.size);
    }
    public void OnHelpClick(object sender, EventArgs args){
        Form help = new Form();
        help.Height = 500;
        help.Width = 500;
        help.Text = "Help";

        Label Shortcuts = new Label();
        Shortcuts.Name = "Shortcuts";
        Shortcuts.Height = 350;
        Shortcuts.Width = 400;
        Shortcuts.Location = new Point(50,15);
        Shortcuts.Font = new Font("Georgia", 13);
        Shortcuts.Text = @" 
        Mouse click on matrix - choose a square to write


        Tab - select everything in chosen square


        Left and right arrow keys:

            - move through matrix


        Minor:

            - if enabled, it will show the minor of any chosen position";
        Button exit = CreateDynamicButton("OK", 150, 375, 50, 200, Color.Black, Color.Transparent);
        exit.Click += (sender, e) => { help.Close(); };

        help.Controls.Add(Shortcuts);
        help.Controls.Add(exit);
        help.ShowDialog();
    }

    public void OnMinorClick(object sender, EventArgs args){
        minor = !minor;
        if(!minor){
            minor = false;
            clickedForMinor = false;
        }
        Invalidate();
    }
    #endregion

    #region Event handlers 
    int editX, editY;
    TextBox editBox;
    bool clickedForMinor;
    int minorX, minorY;
    protected override void OnPaint(PaintEventArgs args) {
        Graphics g = args.Graphics;
        createMatrixGrid(args, g);
        if(listOfStepsForRREF != null){
            SbsRREF(args, g);
        }else{
            displayNumbers(args, g);
        }
        displayOther(args, g);
        if(minor){
            drawMinor(args, g);
        }
    }
    
    void closeEdit() {
        if (editBox != null) {
            if(editBox.Text.Length == 0){
                m[editY, editX] = 0;
            }else{
                m[editY, editX] = int.Parse(editBox.Text);
            }
            Controls.Remove(editBox);
            editBox = null;
            Invalidate();
        }
    }
    
    void openEdit() {
        if(mOriginal == null && !other && !minor){
            editBox = new TextBox();
            editBox.Font = font();
            Point ul = upperLeft();
            editBox.SetBounds(ul.X + oneSquareSize * editX, ul.Y + oneSquareSize * editY,
                            oneSquareSize, oneSquareSize);
            editBox.Text = m[editY, editX].ToString();
            Controls.Add(editBox);
            editBox.Focus();
            editBox.KeyPress += OnEditBoxKeyPress;
            editBox.PreviewKeyDown += OnEditBoxPreviewKeyDown;
        }
    }
    
    void OnEditBoxKeyPress(object sender, KeyPressEventArgs args) {
        char c = args.KeyChar;
        
        if (c == (char) Keys.Return)
            closeEdit();
        
        if (c != (char) Keys.Back && c != '-' && !char.IsDigit(c))
            args.Handled = true;   // ignore keypresses that are not digits or '-'
    }
    
    // The tab key is special and does not trigger a KeyPress event, but we
    // can detect it via a PreviewKeyDown event.
    void OnEditBoxPreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
        if (e.KeyCode == Keys.Left) {
            moveLR(-1);
        } 
        else if (e.KeyCode == Keys.Right) {
            moveLR(1);
        }
    }
    protected override bool ProcessTabKey(bool forward) {
        if (editBox != null)
           editBox.SelectAll();
        return true;
    }

    protected override void OnMouseDown(MouseEventArgs e){
        closeEdit();    // close existing box if any
        Point ul = upperLeft();
        if (e.X < ul.X || e.Y < ul.Y)
            return;
        
        int x = (e.X - ul.X) / oneSquareSize;
        int y = (e.Y - ul.Y) / oneSquareSize;
        if (x < m.size && y < m.size) {
            editX = x; editY = y;
            openEdit();
        }
        if(insideTriangle(rPoints(), e.X, e.Y)){
            if(step+1 == listOfStepsForRREF.Count){
                step = listOfStepsForRREF.Count-1;
            }else{
                step++;
            }
            Invalidate();
        }
        else if(insideTriangle(lPoints(), e.X, e.Y)){
            if(step-1 < 0){
                step = 0;
            }else{
                step--;
            }
            Invalidate();
        }
        if(minor){
            for(int i = 0; i < m.size; i++){
                for(int j = 0; j < m.size; j++){
                    if(insideSquare(i, j, e.X, e.Y)){
                        minorX = i;
                        minorY = j;
                        clickedForMinor = true;
                        Invalidate();
                    }
                }
            }
        }
    }

    #endregion

    #region Drawing
    
    public int matrixPixelNumber = 360;
    public int matrixOriginalPixelNumber = 240;
    int oneSquareSize => matrixPixelNumber / m.size;
    int originalOneSquareSize => matrixOriginalPixelNumber/ m.size; 
    int step = 0;
    Point upperLeft() {
        //since the matrix has to be in the middle, these will be the coordinates for the upper left corner:
        Point screenCenter = new Point(1200/2, 720/2);
        return new Point(screenCenter.X - matrixPixelNumber/2,  screenCenter.Y - matrixPixelNumber/2);
    }
    Point secondUpperLeft(){
        Point screenQuater = new Point(1200/4, 720/2);
        return new Point(screenQuater.X - matrixPixelNumber/2,  screenQuater.Y - matrixPixelNumber/2);
    }
    Point originalUpperLeft(){
        //matrix original will be in the upper left corner of the window, these will be the coordinates for the upper left corner:
        return new Point(50,50);
    }
    PointF[] lPoints(){
        PointF lp1 = new PointF(1200/2 - 30, 720/2 + 180 + 30);
        PointF lp2 = new PointF(1200/2 - 30, 720/2 + 180 + 30 + 30);
        PointF lp3 = new PointF(1200/2 - 30 - 30, 720/2 + 180 + 30 + 30 - 15);
        PointF[] lpoints = {lp1, lp2, lp3};
        return lpoints;
    }
    PointF[] rPoints(){
        PointF rp1 = new PointF(1200/2 + 30, 720/2 + 180 + 30);
        PointF rp2 = new PointF(1200/2 + 30, 720/2 + 180 + 30 + 30);
        PointF rp3 = new PointF(1200/2 + 30 + 30, 720/2 + 180 + 30 + 30 - 15);
        PointF[] rpoints = {rp1, rp2, rp3};
        return rpoints;
    }
    //these points were intentionally left like this for purpuse of being able to retrace how we got to them
    Font font() => new Font ("Times New Roman", oneSquareSize/4 + 2);
    Font fontOriginal() => new Font ("Times New Roman", originalOneSquareSize/4 + 2);    
    void createMatrixGrid(PaintEventArgs args, Graphics g){
        Point matrixCoordinates = upperLeft();
        if (other){
            matrixCoordinates = secondUpperLeft();
        }
        int pixelNumber = matrixPixelNumber;
        int oneSquare = oneSquareSize;
        Pen pen = new Pen (Brushes.GreenYellow, 1);
        if(clickedForMinor){
            pen = new Pen (Brushes.DarkSalmon, 3);
        }
        _createMatrixGrid(args, g, matrixCoordinates, pixelNumber, oneSquare, pen);
        if (mOriginal != null){
            Point originalMatrixCoordinates = originalUpperLeft();
            int originalPixelNumber = matrixOriginalPixelNumber;
            int originalOneSquare = originalOneSquareSize;
            Pen originalPen = new Pen (Brushes.Orange, 1);

            _createMatrixGrid(args, g, originalMatrixCoordinates, originalPixelNumber, originalOneSquare, originalPen);
        }
    }

    void _createMatrixGrid(PaintEventArgs args, Graphics g, Point matrixCoordinates, int pixelNumber, int oneSquare, Pen pen){
        //first we make the outline of the matrix
        Rectangle rect = new Rectangle (matrixCoordinates.X, matrixCoordinates.Y, pixelNumber, pixelNumber);
        g.DrawRectangle(pen, rect);
        //now we draw lines of the grid with given oneSquareSize
        for (int i = 0; i <= pixelNumber; i += oneSquare){
            Point fromVertical = new Point (matrixCoordinates.X + i, matrixCoordinates.Y);
            Point toVertical = new Point (matrixCoordinates.X + i, matrixCoordinates.Y + pixelNumber);
            g.DrawLine(pen, fromVertical, toVertical);
            Point fromHorizontal = new Point (matrixCoordinates.X, matrixCoordinates.Y + i);
            Point toHorizontal = new Point (matrixCoordinates.X + pixelNumber, matrixCoordinates.Y + i);
            g.DrawLine(pen, fromHorizontal, toHorizontal);
        }

    }

    void displayNumbers(PaintEventArgs args, Graphics g){
        Font f = font();
        Font originalFont = fontOriginal();
        StringFormat format = new StringFormat();
        format.LineAlignment = StringAlignment.Center;
        format.Alignment = StringAlignment.Center;
        Point ul = upperLeft();
        if (other){
            ul = secondUpperLeft();
        }
        Point originalul = originalUpperLeft();
        displayText(args, g, ul, originalul, format);
        for (int x = 0 ; x < m.size ; ++x){
            for (int y = 0 ; y < m.size ; ++y) {
                Rectangle rect = new Rectangle (ul.X + x * oneSquareSize, ul.Y + y * oneSquareSize,
                                                oneSquareSize, oneSquareSize);
                string s = Math.Round(m[y, x],2).ToString();
                if(s == "-0"){
                    s = "0";
                }
                g.DrawString (s, f, Brushes.White, rect, format);
                if (mOriginal != null){
                    Rectangle originalRect = new Rectangle (originalul.X + x * originalOneSquareSize, originalul.Y + y * originalOneSquareSize,
                                                        originalOneSquareSize, originalOneSquareSize);
                    string s2 = Math.Round(mOriginal[y, x],2).ToString();
                    if(s2 == "-0"){
                        s2 = "0";
                    }
                    g.DrawString (s2, originalFont, Brushes.White, originalRect, format);
                }
            }
        }
        RREF = false;
        inverse = false;  
        transpose = false;  
    }

    void displayText(PaintEventArgs args, Graphics g, Point ul, Point originalul, StringFormat format){
        Font font = new Font("Times New Roman", 25);
        Font originalFont = new Font("Times New Roman", 15);
        Rectangle originalRect = new Rectangle(originalul.X, originalul.Y - 20, matrixOriginalPixelNumber, 20);
        Rectangle rect = new Rectangle(ul.X, ul.Y - 40, matrixPixelNumber, 30);
        if (RREF){
            g.DrawString ("Original", originalFont, Brushes.Orange, originalRect, format);
            g.DrawString ("RREF", font, Brushes.GreenYellow, rect, format);
        }
        else if (inverse){
            g.DrawString ("Original", originalFont, Brushes.Orange, originalRect, format);
            g.DrawString ("Inverse", font, Brushes.GreenYellow, rect, format);
        }
        else if (transpose){
            g.DrawString ("Original", originalFont, Brushes.Orange, originalRect, format);
            g.DrawString ("Transpose", font, Brushes.GreenYellow, rect, format);
        }
        else if (adjoint){
            g.DrawString ("Original", originalFont, Brushes.Orange, originalRect, format);
            g.DrawString ("Adjoint", font, Brushes.GreenYellow, rect, format);
        }
        else{
            g.DrawString ("Original", font, Brushes.GreenYellow, rect, format);
        }
    }

    void displayOther(PaintEventArgs args, Graphics g){
        StringFormat format = new StringFormat();
        format.LineAlignment = StringAlignment.Near;
        format.Alignment = StringAlignment.Near;
        if (all){
            //since matrix is in the center of the left half of the screen
            //all of the computations will be of the other half, starting 100 pixels from the upper left corner of the right half 
            Point allUpperLeft = new Point(700,300);
            Font font = new Font("Georgia", oneSquareSize/4 + 2);
            for(int i = 0; i < listOfOther.Count; i++){
                Rectangle rect = new Rectangle(allUpperLeft.X, allUpperLeft.Y + i*40, 400, oneSquareSize/2);
                g.DrawString (listOfOther[i], font, Brushes.White, rect, format);
            }
        }else if (other){
            //since matrix is in the center of the left half of the screen, "other" will be in the center of the right half
            Point otherUpperLeft = new Point(700,300);//360 is the center, but since height of the rect is 120, we start 60 pixels before
            Font font = new Font("Times New Roman", 30);
            Rectangle rect = new Rectangle(otherUpperLeft.X, otherUpperLeft.Y, 400, 120);
            g.DrawString (listOfOther[0], font, Brushes.White, rect, format);
        }
    }

    void SbsRREF(PaintEventArgs args, Graphics g){
        Pen p = new Pen (Brushes.GreenYellow, 1);
        //right triangle
        PointF[] rpoints = rPoints();
        g.DrawPolygon(p, rpoints);
        //left triangle
        PointF[] lpoints = lPoints();
        g.DrawPolygon(p, lpoints);
        //draw number of steps
        StringFormat format = new StringFormat();
        format.LineAlignment = StringAlignment.Near;
        format.Alignment = StringAlignment.Near;
        Font font = new Font("Times New Roman", 14);
        Rectangle rect = new Rectangle(1200/2 - 15, 720/2 + 180 + 30, 40, 30);
        g.DrawString (step+1 + "/" + listOfStepsForRREF.Count, font, Brushes.GreenYellow, rect, format);
        //show numbers
        m = listOfStepsForRREF[step];
        displayNumbers(args,g);
        RREF = true;
    }

    void drawMinor(PaintEventArgs args, Graphics g){
        Rectangle rect1 = new Rectangle (1050,120,12,12);
        g.FillEllipse(Brushes.GreenYellow,rect1);
        if(clickedForMinor){
            Matrix _minor = m.clone();
            List<Point> l = new List<Point>();
            _minor = m.Minor(m,minorX,minorY, out l);
            Pen p2 = new Pen(Brushes.DarkSalmon,5);
            Point ul = upperLeft();
            Rectangle rect2 = new Rectangle(ul.X + minorX*oneSquareSize,ul.Y + minorY*oneSquareSize, oneSquareSize, oneSquareSize);
            g.DrawRectangle(p2,rect2);
            for (int i = 0; i < l.Count; i++){
                Pen p3 = new Pen(Brushes.Red,5);
                Rectangle rect3 = new Rectangle(ul.X + l[i].X*oneSquareSize,ul.Y + l[i].Y*oneSquareSize, oneSquareSize, oneSquareSize);
                g.DrawRectangle(p3,rect3);
            }
            clickedForMinor = false;
        }
    }
    #endregion

    #region Computations
    bool RREF, inverse, transpose, adjoint, other, all;
    List<string> listOfOther = new List<string>();
    List<Matrix> listOfStepsForRREF;
    void System(object sender, EventArgs args){
        closeEdit();
        other = true;
        listOfOther.Add(m.System());
        Invalidate();
    }
    void Rank(object sender, EventArgs args){
        closeEdit();
        other = true;
        listOfOther.Add(m.Rank());
        Invalidate();
    }
    void Determinant(object sender, EventArgs args){
        closeEdit();
        other = true;
        listOfOther.Add(m.Determinant());
        Invalidate();
    }
    void Adjoint(object sender, EventArgs args){
        closeEdit();
        adjoint = true;
        mOriginal = m;
        m = m.Adjoint();
        Invalidate();
    }
    void Transpose(object sender, EventArgs args){
        closeEdit();
        transpose = true;
        mOriginal = m.clone();
        m = m.Transpose();
        Invalidate();
    }
    void ResultForRREF(object sender, EventArgs args){
        closeEdit();
        RREF = true;
        mOriginal = m.clone();
        m = m.RREF(out List<Matrix> remember);
        Invalidate();
    }
    void SbsForRREF(object sender, EventArgs args){
        closeEdit();
        RREF = true;
        mOriginal = m.clone();
        m.RREF(out listOfStepsForRREF);
        m = listOfStepsForRREF[0];
        Invalidate();
    }
    void Inverse(object sender, EventArgs args){
        closeEdit();
        Matrix temp = m.Inverse();
        if (temp.size == 1){
            MessageBox.Show("This matrix is not invertable");
        }else{
            mOriginal = m;
            m = temp;
            inverse = true;
        }
        Invalidate();
    }
    void All(object sender, EventArgs args){
        closeEdit();
        listOfOther.Add(m.Determinant());
        listOfOther.Add(m.Rank());
        listOfOther.Add(m.System());
        all = other = true;
        Invalidate();
    }
    #endregion

    #region Helper functions
    void moveLR(int i){
        closeEdit();
        editX += i;
        if (editX >= m.size) {
            editX = 0;
            editY += 1;
            if (editY >= m.size)
                editY = 0;
        }
        else if (editX < 0) {
            editX = m.size - 1;
            editY -= 1;
            if (editY < 0)
                editY = m.size - 1;
        }
        openEdit();
    }

    void Reset(int size){
        closeEdit();
        other = transpose = adjoint = inverse = RREF = all = minor = false;
        listOfOther.Clear();
        m = new Matrix(size);
        mOriginal = null;
        listOfStepsForRREF = null;
        step = 0;
        clickedForMinor = false;
        Invalidate();
    }

    bool insideTriangle(PointF[] pointFs, int x, int y){
        //formula I am using: A = [ x1(y2 – y3) + x2(y3 – y1) + x3(y1-y2)]/2
        double area = Math.Abs((pointFs[0].X * (pointFs[1].Y - pointFs[2].Y) + pointFs[1].X * (pointFs[2].Y - pointFs[0].Y) + pointFs[2].X * (pointFs[0].Y - pointFs[1].Y))/2);
        double a1 = Math.Abs((x * (pointFs[1].Y - pointFs[2].Y) + pointFs[1].X * (pointFs[2].Y - y) + pointFs[2].X * (y - pointFs[1].Y))/2);
        double a2 = Math.Abs((x * (pointFs[0].Y - pointFs[2].Y) + pointFs[0].X * (pointFs[2].Y - y) + pointFs[2].X * (y - pointFs[0].Y))/2);
        double a3 = Math.Abs((x * (pointFs[0].Y - pointFs[1].Y) + pointFs[0].X * (pointFs[1].Y - y) + pointFs[1].X * (y - pointFs[0].Y))/2);
        return a1 + a2 + a3 == area; 
    }

    bool insideSquare(int i, int j, int eX, int eY){
        Point ul = upperLeft();
        return (ul.X + i*oneSquareSize) < eX && (ul.X + (i+1)*oneSquareSize) > eX && (ul.Y + j*oneSquareSize) < eY && (ul.Y + (j+1)*oneSquareSize) > eY;
    }
    #endregion
}
