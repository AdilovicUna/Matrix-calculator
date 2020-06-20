using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

class Matrix{
    int matrixSize = 0;
    double[,] matrix;
    
    public Matrix(int ms){
        matrixSize = ms;
        matrix = new double[matrixSize, matrixSize];
    } 
    
    public double this[int x, int y] {
        get => matrix[x, y];
        
        set {
            matrix[x, y] = value;
        }
    }

    public int size => matrixSize;


#region Helper functions
public int sign(int i, int j){
    if ((i+j) % 2 == 0){
        return 1;
    }
    return -1;
}

public void minusZero (double i){
    if(i == -0){
        i = 0;
    }
}

public Matrix clone(){
    Matrix clone = new Matrix(this.size);
    for(int i = 0 ; i < this.size; i++){
        for (int j = 0; j < this.size; j++){
            clone[i,j] = this[i,j];
        }
    }
    return clone;
}

public bool changed(Matrix m){
    for(int i = 0 ; i < this.size; i++){
        for (int j = 0; j < this.size; j++){
           if(m[i,j] != this[i,j]){
               return true;
           }
        }
    }
    return false;
}
#endregion

#region RREF
    public Matrix RREF(out List<Matrix> remember){
        remember = new List<Matrix>(); 
        remember.Add(new Matrix(this.size));
        remember[remember.Count - 1] = this.clone();
        int leadingZero = 0;
        for (int i = 0; i < this.size; i++){
            if (this.size <= leadingZero){
                break;
            }
            int r = i;
            while (this[r,leadingZero] == 0){
                r++;
                if (r == this.size){
                    r = i;
                    leadingZero++;
                    if (this.size == leadingZero){
                        leadingZero--;
                        break;
                    }
                }
            }
            for (int j = 0; j < this.size; j++){
                double temp = this[i,j];
                this[i,j] = this[r,j];
                this[r,j] = temp;
            }
            if(this.changed(remember[remember.Count-1])){
                remember.Add(new Matrix(this.size));
                remember[remember.Count - 1] = this.clone();
            }
            double divide = this[i,leadingZero];
            if (divide != 0){
                for (int k = 0; k < this.size; k++){
                    this[i,k] /= divide; 
                }       
            }
            if(this.changed(remember[remember.Count-1])){
                remember.Add(new Matrix(this.size));
                remember[remember.Count - 1] = this.clone();
            }
            Matrix tempMatrix = this.clone();
            for (int l = 0; l < this.size; l++){
                if (l != i){
                    for (int h = 0; h < this.size; h++){
                        double mul = tempMatrix[l,leadingZero] * tempMatrix[i,h];
                        this[l,h] -= mul;
                    }
                    if(this.changed(remember[remember.Count-1])){
                        remember.Add(new Matrix(this.size));
                        remember[remember.Count - 1] = this.clone();
                    }
                }
                minusZero(this[i,l]);
            }
            if(this.changed(remember[remember.Count-1])){
                remember.Add(new Matrix(this.size));
                remember[remember.Count - 1] = this.clone();
            }
            leadingZero++;
        }
        return this;
    }
#endregion

#region Transpose
    public Matrix Transpose(){
        for (int i = 0; i < this.size; i++){
            for (int j = 0; j < this.size; j++){
                if (i > j){
                    double temp = this[i,j];
                    this[i,j] = this[j,i];
                    this[j,i] = temp;
                }
            }
        }
        return this;
    }
#endregion

#region Adjoint
    public Matrix Adjoint(){
        Matrix m = this;
        Matrix adj = new Matrix(this.size);
        for (int i = 0; i < this.size; i++){
            for (int j = 0; j < this.size; j++){
                adj[i,j] = sign(i,j) * _Determinant(Minor(m,i,j,out List<Point> l),0);
                minusZero(adj[i,j]);
            }
        }
        adj.Transpose();
        return adj;
    } 
#endregion

#region Inverse
    public Matrix Inverse(){
        Matrix m = this;
        Matrix inverse = new Matrix(m.size);
        double det = _Determinant(m,0);
        if (det == 0){
            return new Matrix(1);
        }
        Matrix adj = m.Adjoint();
        for (int i = 0; i < this.size; i++){
            for (int j = 0; j < this.size; j++){
                inverse[i,j] = (1/det) * adj[i,j];
            }
        } 
        return inverse;
    }
#endregion

#region Minor
    public Matrix Minor(Matrix matrix, int x, int y, out List<Point> l){
        Matrix m = new Matrix(matrix.size-1);
        l = new List<Point>();
        int addX, addY;
        for (int i = 0; i < matrix.size; i++){
            if (i > x){
                addX = i - 1;
            }else{
                addX = i;
            }
            for (int j = 0; j < matrix.size; j++){
                if (j > y){
                    addY = j - 1;
                }else{
                    addY = j;
                }
                if (i != x && j != y){
                    m[addX,addY] = matrix[i,j];
                    l.Add(new Point(i,j));
                }
            }
        }
        return m;
    }
#endregion

#region  Determinant
    public string Determinant(){
        Matrix matrix = this;
        double toReturn = _Determinant(matrix, 0);
        return "Determinant = " + toReturn;
    }
    public double _Determinant(Matrix matrix, double s){
        int i = 0;
        if (matrix.size == 1){
            return matrix[0,0];
        }
        if (matrix.size == 2){
            return (matrix[0,0]*matrix[1,1] - matrix[0,1]*matrix[1,0]);
        }
        for (int j = 0; j < matrix.size; j++){
            s = s + matrix[i,j]*sign(i,j)*_Determinant(Minor(matrix, i, j, out List<Point> l), s);
        }
        return s;
    }
#endregion

#region Rank
    public string Rank(){
        Matrix matrix = this.clone();
        int toReturn = _Rank(matrix);
        return "Rank = " + toReturn;
    }

    public int _Rank(Matrix matrix){
        matrix = matrix.RREF(out List<Matrix> list);
        int result = 0;
        for(int j = 0; j < matrix.size; j++){
            bool t = true;
            for(int i = 0; i < matrix.size; i++){
                if(i == j && matrix[i,j] != 1){
                    t = false;
                }
                if(i != j && matrix[i,j] != 0){
                    t = false;
                }
            }
            if(t){
                result++;
            }
        }
        return result;
    }
#endregion

#region Ax=0
    public string System(){
        Matrix matrix = this.clone();
        int rank = _Rank(matrix);
        string toReturn = "";
        if(rank == matrix.size){
            toReturn += "x1=0";
            for(int i = 1; i < matrix.size; i++){
                toReturn += ", x" + (i+1) + "=0";
            } 
        }
        return toReturn;
    }
#endregion
}