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

        ToolStripMenuItem[] menu2 = {
            new ToolStripMenuItem("Ax=0", null, System),
            new ToolStripMenuItem("Rank", null, Rank),
            new ToolStripMenuItem("Determinant", null, Determinant),
            new ToolStripMenuItem("All", null, All)
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

        ToolStripMenuItem[] menu4 = {
            new ToolStripMenuItem("add Matrix", null, addM),
            new ToolStripMenuItem("add Vector", null, addV),
            new ToolStripMenuItem("add Scalar", null, addS)
        };
        
        ToolStripMenuItem[] topItems = {
            new ToolStripMenuItem("File", null, menu1),
            new ToolStripMenuItem("Numerical Computations", null, menu2),
            new ToolStripMenuItem("Matrix Computations", null, menu3),
            new ToolStripMenuItem("+, -, *", null, menu4)
        };

        MenuStrip menu = new MenuStrip();
        foreach (var item in topItems)
            menu.Items.Add(item);

        Controls.Add(menu);     
    }
    #endregion

    #region Menu1 Functions
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

    #region Menu2 Functions
    bool determinant, rank, system, all;
    string rnk,det,sys;
    List<string> listOfAll = new List<string>();
    void System(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        MFalse();
        AFalse();
        system = true;
        change();
        mOriginal = null;
        sys = m.System();
        Invalidate();
    }
    void Rank(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        MFalse();
        AFalse();
        rank = true;
        change();
        mOriginal = null;
        rnk = m.Rank();
        Invalidate();
    }
    void Determinant(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        MFalse();
        AFalse();
        determinant = true;
        change();
        mOriginal = null;
        det = m.Determinant();
        Invalidate();
    }

     void All(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        MFalse();
        AFalse();
        all = true;
        change();
        listOfAll.Add(m.Determinant());
        listOfAll.Add(m.Rank());
        listOfAll.Add(m.System());
        Invalidate();
    }
    #endregion
    
    #region Menu3 Functions
    bool RREF, inverse, transpose, adjoint;
    List<Matrix> listOfStepsForRREF;
    void Adjoint(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        AFalse();
        adjoint = true;
        change();
        m = m.Adjoint();
        Invalidate();
    }
    void Transpose(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        AFalse();
        transpose = true;
        change();
        m = m.Transpose();
        Invalidate();
    }
    void ResultForRREF(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        AFalse();
        RREF = true;
        change();
        m = m.RREF(out List<Matrix> remember);
        Invalidate();
    }
    void SbsForRREF(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        AFalse();
        RREF = true;
        change();
        m.RREF(out listOfStepsForRREF);
        m = listOfStepsForRREF[0];
        Invalidate();
    }
    void Inverse(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        AFalse();
        if(mOriginal != null){
            m = mOriginal.clone();
        }
        Matrix temp = m.Inverse();
        if (temp.size == 1){
            MessageBox.Show("This matrix is not invertible");
            mOriginal = null;
        }else{
            mOriginal = m;
            m = temp;
            inverse = true;
        }
        Invalidate();
    }
    #endregion
    
    #region Menu4 Functions
    bool anotherM, anotherV, anotherS;
    void addM(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        MFalse();
        AFalse();
        anotherM = true;
        Invalidate();
    }
    void addV(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        MFalse();
        AFalse();
        anotherV = true;
        Invalidate();
    }
    void addS(object sender, EventArgs args){
        closeEdit();
        NumFalse();
        MFalse();
        AFalse();
        anotherS = true;
        Invalidate();
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
        if(!anotherM && !anotherV && !anotherS){
            minor = !minor;
            if(!minor){
                minor = false;
                clickedForMinor = false;
            }
            Invalidate();
        }
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
        if(anotherM || anotherV || anotherS){
            createAnother(args, g);
        }
    }
    
    void closeEdit() {
        if (editBox != null) {
            if(editBox.Text.Length == 0){
                m[editY, editX] = 0;
            }else{
                m[editY, editX] = double.Parse(editBox.Text);
            }
            Controls.Remove(editBox);
            editBox = null;
            Invalidate();
        }
    }
    
    void openEdit() {
        if(mOriginal == null && !determinant || rank || system && !minor){
            editBox = new TextBox();
            editBox.Font = font();
            Point ul = upperLeft();
            if(anotherM || anotherV || anotherS){
                ul = FirstHalfUpperLeft();
            }
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
        
        if (c != (char) Keys.Back && c != '-' && c != '.' && !char.IsDigit(c))
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
        CheckIfClicked(e, upperLeft());
        if(anotherM || anotherS || anotherV){
            CheckIfClicked(e, FirstHalfUpperLeft());
            Point temp = anotherM ? SecondHalfUpperLeft() : anotherV ? VectorUpperLeft() : ScalarUpperLeft();
            CheckIfClicked(e, temp);
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

    void CheckIfClicked(MouseEventArgs e, Point ul){
        if (e.X < ul.X || e.Y < ul.Y)
            return;
        
        int x = (e.X - ul.X) / oneSquareSize;
        int y = (e.Y - ul.Y) / oneSquareSize;
        if (x < m.size && y < m.size) {
            editX = x; editY = y;
            openEdit();
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
    Point FirstscreenQuater(){
        return new Point(1200/4, 720/2);
    }
    Point FirstHalfUpperLeft(){
        Point screenQuater = FirstscreenQuater();
        return new Point(screenQuater.X - matrixPixelNumber/2,  screenQuater.Y - matrixPixelNumber/2);
    }
    Point SecondscreenQuater(){
        return new Point(1200/2 + 1200/4, 720/2);
    }
    Point SecondHalfUpperLeft(){
        Point screenQuater = SecondscreenQuater();
        return new Point(screenQuater.X - matrixPixelNumber/2,  screenQuater.Y - matrixPixelNumber/2);
    }
    Point ScalarUpperLeft(){
        Point screenQuater = SecondscreenQuater();
        return new Point(screenQuater.X - oneSquareSize/2 - 100, screenQuater.Y - oneSquareSize/2);
    }
    Point VectorUpperLeft(){
        Point screenQuater = SecondscreenQuater();
        return new Point(screenQuater.X - oneSquareSize/2 - 100, screenQuater.Y - matrixPixelNumber/2);
    }
    Point originalUpperLeft(){
        //matrix original will be in the upper left corner of the window, these will be the coordinates for the upper left corner:
        return new Point(50,50);
    }
    PointF[] lPoints(){
        PointF lp1 = new PointF(1200/2 - 40, 720/2 + 180 + 30);
        PointF lp2 = new PointF(1200/2 - 40, 720/2 + 180 + 30 + 30);
        PointF lp3 = new PointF(1200/2 - 40 - 30, 720/2 + 180 + 30 + 30 - 15);
        PointF[] lpoints = {lp1, lp2, lp3};
        return lpoints;
    }
    PointF[] rPoints(){
        PointF rp1 = new PointF(1200/2 + 40, 720/2 + 180 + 30);
        PointF rp2 = new PointF(1200/2 + 40, 720/2 + 180 + 30 + 30);
        PointF rp3 = new PointF(1200/2 + 40 + 30, 720/2 + 180 + 30 + 30 - 15);
        PointF[] rpoints = {rp1, rp2, rp3};
        return rpoints;
    }
    //these points were intentionally left like this for purpuse of being able to retrace how we got to them
    Font font() => new Font ("Times New Roman", oneSquareSize/4 + 2);
    Font fontOriginal() => new Font ("Times New Roman", originalOneSquareSize/4 + 2);    
    void createMatrixGrid(PaintEventArgs args, Graphics g){
        Point matrixCoordinates = upperLeft();
        if (determinant || rank || system || all || anotherM || anotherV || anotherS){
            matrixCoordinates = FirstHalfUpperLeft();
        }
        int pixelNumber = matrixPixelNumber;
        int oneSquare = oneSquareSize;
        Pen pen = new Pen (Brushes.GreenYellow, 1);
        if(clickedForMinor){
            pen = new Pen (Brushes.DarkSalmon, 3);
        }
        _createMatrixGrid(args, g, matrixCoordinates, pixelNumber, oneSquare, pen, true);
        if (mOriginal != null){
            Point originalMatrixCoordinates = originalUpperLeft();
            int originalPixelNumber = matrixOriginalPixelNumber;
            int originalOneSquare = originalOneSquareSize;
            Pen originalPen = new Pen (Brushes.Orange, 1);

            _createMatrixGrid(args, g, originalMatrixCoordinates, originalPixelNumber, originalOneSquare, originalPen, true);
        }
    }

    void _createMatrixGrid(PaintEventArgs args, Graphics g, Point matrixCoordinates, int pixelNumber, int oneSquare, Pen pen, bool isSquare){
        //first we make the outline of the matrix
        Rectangle rect = new Rectangle (matrixCoordinates.X, matrixCoordinates.Y, pixelNumber, pixelNumber);
        if (!isSquare){
            rect = new Rectangle (matrixCoordinates.X, matrixCoordinates.Y, oneSquareSize, pixelNumber);
        }
        g.DrawRectangle(pen, rect);
        //now we draw lines of the grid with given oneSquareSize
        for (int i = 0; i <= pixelNumber; i += oneSquare){
            if(isSquare){
                Point fromVertical = new Point (matrixCoordinates.X + i, matrixCoordinates.Y);
                Point toVertical = new Point (matrixCoordinates.X + i, matrixCoordinates.Y + pixelNumber);
                g.DrawLine(pen, fromVertical, toVertical);
            }
            Point fromHorizontal = new Point (matrixCoordinates.X, matrixCoordinates.Y + i);
            Point toHorizontal = new Point (matrixCoordinates.X + pixelNumber, matrixCoordinates.Y + i);
            if(!isSquare){
                toHorizontal = new Point(matrixCoordinates.X + oneSquareSize, matrixCoordinates.Y + i);
            }
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
        if (determinant || rank || system || all || anotherM || anotherV || anotherS){
            ul = FirstHalfUpperLeft();
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
            Point allUpperLeft = new Point(650,300);
            Font font = new Font("Georgia", oneSquareSize/4 + 2);
            for(int i = 0; i < listOfAll.Count; i++){
                Rectangle rect = new Rectangle(allUpperLeft.X, allUpperLeft.Y + i*40, 450, oneSquareSize);
                g.DrawString (listOfAll[i], font, Brushes.White, rect, format);
            }
        }else if (determinant || rank || system){
            //since matrix is in the center of the left half of the screen, calculation will be in the center of the right half
            Point otherUpperLeft = new Point(700,300);//360 is the center, but since height of the rect is 120, we start 60 pixels before
            Font font = new Font("Times New Roman", 30);
            Rectangle rect = new Rectangle(otherUpperLeft.X, otherUpperLeft.Y, 400, 200);
            string show = rank ? rnk : determinant ? det : system ? sys : "";
            g.DrawString (show, font, Brushes.White, rect, format);
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
        format.LineAlignment = StringAlignment.Center;
        format.Alignment = StringAlignment.Center;
        Font font = new Font("Times New Roman", 14);
        Rectangle rect = new Rectangle(1200/2 - 25, 720/2 + 180 + 30, 55, 30);
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
    
    void createAnother(PaintEventArgs args, Graphics g){
        Point matrixCoordinates = anotherM ? SecondHalfUpperLeft() : anotherV ? VectorUpperLeft() : ScalarUpperLeft();
        int pixelNumber = anotherS ? oneSquareSize : matrixPixelNumber;
        int oneSquare = oneSquareSize;
        Pen pen = new Pen (Brushes.GreenYellow, 1);
        bool square = anotherV ? false : true;
        _createMatrixGrid(args, g, matrixCoordinates, pixelNumber, oneSquare, pen,square);
        Point from = new Point(1200/2 - 40, 720/2 + oneSquareSize/2);
        Point to = new Point(1200/2 + 40, 720/2 + oneSquareSize/2);
        g.DrawLine(pen, from, to);
    }
    #endregion
//click to change sign - default
//deafault 0 in everything
//make clickable
//fix the bug when clicking
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
        NumFalse();
        MFalse();
        sys = rnk = det = null;
        listOfAll.Clear();
        m = new Matrix(size);
        mOriginal = null;
        listOfStepsForRREF = null;
        step = 0;
        clickedForMinor = false;
        Invalidate();
    }

    void NumFalse(){
        determinant = rank = system = all = minor = false;
    }

    void MFalse(){
        transpose = adjoint = inverse = RREF = false;
    }

    void AFalse(){
        anotherM = anotherV = anotherS = false;
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

    void change(){
        if(mOriginal == null && !rank && !determinant && !system && !all){
            mOriginal = m.clone();
        }
        else if(mOriginal != null){
            m = mOriginal.clone();
        }
    }
    #endregion
}
