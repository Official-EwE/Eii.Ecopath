Option Strict On
Imports System.Drawing
Imports System.IO

' A fun utity to create a basemap from an image
Module Module1

    Sub Main()

        ' Control vars
        Dim bmpIn As String = "P:\Projects\EwE\Resources\EwE stock art\Logos\Ecopath_noshade_large.jpg"
        Dim mapOut As String = Path.Combine(".", "BmpBasemap_" & Path.GetFileNameWithoutExtension(bmpIn) & ".asc")
        Dim nocells As Integer = 50

        ' Read image and grab some properties
        Dim bmp As New Bitmap(bmpIn)
        Dim w As Integer = bmp.Width
        Dim h As Integer = bmp.Height

        ' Make up an arbitrary depth scale for the image pixels
        Dim rnd As New Random()
        Dim scale As Single = CSng(rnd.NextDouble * 5)

        ' Decide on no. of pix that will feed each basemap cell, based on the min. no of desired cells
        Dim nopix As Integer = CInt(Math.Min(w / nocells, h / nocells))

        ' This will give the basemap with and height
        Dim dx As Integer = CInt(w / nopix)
        Dim dy As Integer = CInt(h / nopix)

        ' Write the ASCII file, processing the bitmap as we go along
        Using sw As New StreamWriter(mapOut)

            ' Write ASCII header
            sw.WriteLine("nrows        {0}", dy)
            sw.WriteLine("ncols        {0}", dx)
            sw.WriteLine("xllcorner    {0}", 0)
            sw.WriteLine("yllcorner    {0}", 0)
            sw.WriteLine("cellsize     {0}", 10 / nopix) ' The map represents a small area ;-)
            sw.WriteLine("NODATA_value {0}", -9999)

            ' Process bitmap info, one spatial cell at the time in the order of writing to the ASCII file
            For irow As Integer = 0 To dy - 1
                For icol As Integer = 0 To dx - 1

                    ' For each cell, sum and count up total R + G + B colour values within
                    Dim colTot As Long = 0
                    Dim nTot As Integer = 0

                    ' Process the bitmap pixels for the cell
                    For sx As Integer = 0 To nopix - 1
                        For sy As Integer = 0 To nopix - 1
                            ' Count up the R, G and B colours
                            Dim px As Color = bmp.GetPixel(icol * nopix + sx, irow * nopix + sy)
                            colTot = colTot + px.R + px.G + px.B
                            nTot += 3
                        Next
                    Next

                    ' Decide on mean grayscale value, which will be the map value
                    colTot = CInt(colTot / nTot)

                    ' Bit of funnery: grayscale values under 55 will be treated as 0 (land); all other values will be rescaled to arbitrary max depth
                    Dim sMapVal As Single = Math.Max(0, colTot - 55) * scale

                    ' ASCII writing
                    If (icol > 0) Then sw.Write(sMapVal)
                    sw.Write(colTot)
                Next
                sw.WriteLine()
            Next

            ' Make sure ASCII file is fully written
            sw.Flush()
        End Using

    End Sub

End Module
