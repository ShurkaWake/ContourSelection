using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ORO_Lb3;
public class ContourParser
{
    int[,] mask = new int[,]
    {
            {-1, -1, -1},
            {-1, 8, -1},
            {-1, -1, -1}
    };
    bool isReverse;
    double averageImage = 128;

    Bitmap source;
    bool[,] boolMatrix;
    byte[,] intensityMatrix;
    public readonly Bitmap Result;

    public ContourParser(Bitmap source, bool isReverse, bool isBinary, int[,] mask = null)
    {
        this.isReverse = isReverse;
        this.mask = mask;

        this.source = new Bitmap(source);

        boolMatrix = new bool[this.source.Width, this.source.Height];
        intensityMatrix = new byte[this.source.Width, this.source.Height];
        Result = new Bitmap(this.source.Width, this.source.Height);

        averageImage = 96;//ImageAverage();
        Parse();

        if (isBinary)
        {
            FillResultFromBoolMatrix();
        }
        else
        {
            FillResultFromIntensityMatrix();
        }
    }

    private double Average(int x, int y)
    {
        var pixel = source.GetPixel(x, y);
        return Math.Sqrt((Math.Pow(pixel.R, 2) + Math.Pow(pixel.G, 2) + Math.Pow(pixel.B, 2)) / 3);
        //return (pixel.R + pixel.G + pixel.B) / 3.0;
    }

    private void Parse()
    {
        for (int i = 0; i < source.Width; i++)
        {

            for (int j = 0; j < source.Height; j++)
            {
                if (mask == null)
                {
                    Sobel(i, j);
                }
                else
                {
                    UseMask(i, j);
                }
            }
        }
    }

    private void FillResultFromBoolMatrix()
    {
        for (int i = 0; i < Result.Width; i++)
        {
            for (int j = 0; j < Result.Height; j++)
            {
                if (isReverse)
                {
                    Color color = boolMatrix[i, j] ? Color.White : Color.Black;
                    Result.SetPixel(i, j, color);
                }
                else
                {
                    Color color = boolMatrix[i, j] ? Color.Black : Color.White;
                    Result.SetPixel(i, j, color);
                }
            }
        }
    }

    private void FillResultFromIntensityMatrix()
    {
        for (int i = 0; i < Result.Width; i++)
        {
            for (int j = 0; j < Result.Height; j++)
            {
                Color color;
                if (isReverse)
                {
                    byte intensity = (byte) Math.Abs(intensityMatrix[i, j] - 255);
                    color = Color.FromArgb(intensity, intensity, intensity);
                    Result.SetPixel(i, j, color);
                }
                else
                {
                    byte intensity = intensityMatrix[i, j];
                    color = Color.FromArgb(intensity, intensity, intensity);
                    Result.SetPixel(i, j, color);
                }
            }
        }
    }

    private double ImageAverage()
    {
        double sum = 0;
        for(int i = 0; i < source.Width; i++)
        {
            for(int j = 0; j < source.Height; j++)
            {
                sum += Average(i, j);
            }
        }
        sum /= (source.Width * source.Height);
        return sum;
    }

    private byte DoubleToByteBounds(double number)
    {
        byte result = 0;
        if(number < 0)
        {
            result = 0;
        }
        else if(number > 255)
        {
            result = 255;
        }
        else
        {
            result = (byte) number;
        }

        return result;
    }

    private void Sobel(int x, int y)
    {
        int[,] vertMask = new int[,]
        {
                {1, 0, -1},
                {2, 0, -2},
                {1, 0, -1}
        };

        int[,] horMask = new int[,]
        {
                {1, 2, 1},
                {0, 0, 0},
                {-1, -2, -1}
        };

        double Gx = 0;
        double Gy = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (x - 1 + i >= 0 && x - 1 + i < source.Width &&
                    y - 1 + j >= 0 && y - 1 + j < source.Height)
                {
                    var avg = Average(x - 1 + i, y - 1 + j); ;
                    Gx += horMask[i, j] * avg;
                    Gy += vertMask[i, j] * avg;
                }
            }
        }

        double res = Math.Sqrt(Math.Pow(Gx, 2) + Math.Pow(Gy, 2));
        
        boolMatrix[x, y] = res < averageImage;
        intensityMatrix[x, y] = DoubleToByteBounds(res);
    }

    private void UseMask(int x, int y)
    {
        double sum = 0;
        for (int i = 0; i < mask.GetLength(0); i++)
        {
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                if (x - 1 + i >= 0 && x - 1 + i < source.Width &&
                    y - 1 + j >= 0 && y - 1 + j < source.Height)
                {
                    sum += mask[i, j] * Average(x - 1 + i, y - 1 + j);
                }
            }
        }

        boolMatrix[x, y] = sum < averageImage;
        intensityMatrix[x, y] = DoubleToByteBounds(sum);
    }
}
