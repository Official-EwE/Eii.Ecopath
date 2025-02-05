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
Imports EwECore.Auxiliary
Imports System.IO
Imports System.Reflection
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters
Imports System.Runtime.Serialization.Formatters.Binary
Imports EwEUtils.Database
Imports System.Diagnostics.Eventing
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Runtime.InteropServices
Imports EwEUtils.Utilities
Imports System.Drawing.Imaging


#End Region ' Imports 
''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.16:</para>
''' <para>
''' Made hab cap gradient correction flag persistent.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_16
    Inherits cDBUpdate

    Private Class cVisualStyleNamespaceMapper
        Inherits SerializationBinder

        Public Overrides Function BindToType(assemblyName As String, strType As String) As System.Type

            If (strType.Contains("EwECore")) Then
                Select Case strType
                    Case "EwECore.cVisualStyle"
                        strType = GetType(cVisualStyle).ToString
                End Select
            End If

            For Each ass As Assembly In AppDomain.CurrentDomain.GetAssemblies()
                Dim t As Type = ass.GetType(strType, False, True)
                If (t IsNot Nothing) Then
                    Return t
                End If
            Next
            Return Nothing

        End Function

    End Class

    ''' ===========================================================================
    ''' <summary>
    ''' Helper class for converting a visual style from the old deprecated
    ''' binary serialization style to the new JSON serialization style.
    ''' </summary>
    ''' ===========================================================================
    Private Class cVisualStyleConverter

        Public Shared Function BinaryToJSON(str As String) As String

            If String.IsNullOrEmpty(str) Then Return String.Empty

            ' Read the old format
            Dim bf As New BinaryFormatter()
            Dim ms As MemoryStream = Nothing
            Dim ab As Byte() = Nothing
            Dim vsResult As cVisualStyle = Nothing

            ' Ignore assembly version differences
            bf.AssemblyFormat = FormatterAssemblyStyle.Simple
            ' Perform type mapping
            bf.Binder = New cVisualStyleNamespaceMapper()

            Try
                ab = System.Convert.FromBase64String(str)
                ms = New MemoryStream(ab)
                vsResult = CType(bf.Deserialize(ms), cVisualStyle)

                ' Test image format
                If (vsResult.Image IsNot Nothing) Then
#If DEBUG Then
                    Dim img = vsResult.Image
                    Dim fmt = cVisualStyle.FixedImageFormat
                    Dim fn As String = cFileUtils.MakeTempFile(".png")
                    fn = fn.Replace(".png", "~1.png")
                    img.Save(fn, fmt)

                    ' Test conversion
                    Dim strTest As String = cVisualStyleReader.StyleToString(vsResult)
                    Dim vsClone As cVisualStyle = cVisualStyleReader.StringToStyle(strTest)

                    fn = fn.Replace("~1", "~2")
                    vsClone.Image.Save(fn, fmt)
#End If
                End If
            Catch ex As Exception
                Return ""
            End Try

            Return cVisualStyleReader.StyleToString(vsResult)
        End Function

    End Class

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700016!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Changed cVisualStyle serialization format"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Go for it.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = db.GetWriter("Auxillary")
        Dim dt As DataTable = writer.GetDataTable()

        For Each drow As DataRow In dt.Rows
            Dim objResult As Object = drow("VisualStyle")
            If (Not Convert.IsDBNull(objResult)) Then
                drow.BeginEdit()
                drow("VisualStyle") = cVisualStyleConverter.BinaryToJSON(CStr(objResult))
                drow.EndEdit()
            End If
        Next

        Return db.ReleaseWriter(writer, True)

    End Function

End Class
