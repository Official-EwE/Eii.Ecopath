Imports System.Drawing
Imports System.IO

Module Module1

    Sub Main()

        Dim bmpIn As String = "P:\Jeroen\Personal\jeroen.jpg"
        Dim mapOut As String = ".\bitmapbasemap_js.asc"
        Dim nocells As Integer = 50
        Dim bmp As New Bitmap(bmpIn)
        Dim w As Integer = bmp.Width
        Dim h As Integer = bmp.Height
        Dim rnd As New Random()
        Dim scale As Single = rnd.NextDouble * 5
        Dim nopix As Integer = Math.Min(CInt(w / nocells), (h / nocells))
        Dim dx As Integer = CInt(w / nopix)
        Dim dy As Integer = CInt(h / nopix)

        Using sw As New StreamWriter(mapOut)
            sw.WriteLine("nrows      {0}", dy)
            sw.WriteLine("ncols      {0}", dx)
            sw.WriteLine("xllcorner  {0}", 0)
            sw.WriteLine("yllcorner  {0}", 0)
            sw.WriteLine("cellsize   {0}", nopix * 100)
            sw.WriteLine("NODATA_value {0}", -9999)

            For irow As Integer = 0 To dy - 1
                For icol As Integer = 0 To dx - 1
                    Dim colTot As Long = 0
                    Dim nTot As Integer = 0

                    For sx As Integer = 0 To nopix - 1
                        For sy As Integer = 0 To nopix - 1
                            Dim px As Color = bmp.GetPixel(icol * nopix + sx, irow * nopix + sy)
                            colTot = colTot + px.R + px.G + px.B
                            nTot += 3
                        Next
                    Next
                    colTot = colTot / nTot

                    If (icol > 0) Then sw.Write(" ")
                    sw.Write(Math.Max(0, colTot - 55) * scale)
                Next
                sw.WriteLine()
            Next
            sw.Flush()
        End Using

    End Sub

End Module
