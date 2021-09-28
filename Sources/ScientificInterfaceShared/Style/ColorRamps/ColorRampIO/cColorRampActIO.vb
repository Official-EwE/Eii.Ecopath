' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports ScientificInterfaceShared.Style

#End Region ' Imports

' ToDo: use factory, use plug-ins for readers/writers

''' <summary>
''' Reader/writer to the .act file format
''' </summary>
''' <remarks>
''' https://www.adobe.com/devnet-apps/photoshop/fileformatashtml/#50577411_pgfId-1070626
''' Load of ramps at
''' https://www.giss.nasa.gov/tools/panoply/colorbars/
''' File formats at
''' http://www.selapa.net/swatches/colors/fileformats.php
''' </remarks>
Public Class cColorRampActIO

    Public Function Read(fn As String) As cBinaryColorRamp

        Dim colors As New List(Of Color)
        Dim name As String = Path.GetFileNameWithoutExtension(fn)

        Dim md5Hasher As MD5 = MD5.Create()
        Dim hashed As Byte() = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(name))
        Dim id As Integer = BitConverter.ToInt32(hashed, 0)

        Using stream As New FileStream(fn, FileMode.Open)
            Using sr As New BinaryReader(stream)
                Dim bytes As Byte() = sr.ReadBytes(3)
                Dim done As Boolean = False
                While Not done
                    If (bytes.Length = 3) Then
                        colors.Add(Color.FromArgb(255, bytes(0), bytes(1), bytes(2)))
                    Else
                        done = True
                    End If
                    bytes = sr.ReadBytes(3)
                End While
            End Using
        End Using
        Return New cBinaryColorRamp(id, name, colors.ToArray())

    End Function

End Class
