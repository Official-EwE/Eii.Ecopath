
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Xml

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

Imports EwESpatialAssetsPlugin
Imports EwESpatialAssetsPlugin.SpatialData


Public Class cRelativePathDataSetPlugin
    Inherits cASCIIFilesDataSetPlugin


    Public Sub New()
        MyBase.New()

    End Sub


    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Read dataset configuration from XML.
    ''' </summary>
    ''' <param name="doc">The doc to read nodes from.</param>
    ''' <param name="node">The configuration node that contains the content
    ''' of the dataset. Happy, happy, happy.</param>
    ''' <returns>
    ''' True if successful.
    ''' </returns>
    ''' -------------------------------------------------------------------
    Protected Overrides Function FromXML(ByVal doc As XmlDocument, ByVal node As XmlNode) As Boolean

        Dim xn As XmlNode = Nothing
        Dim xnFile As XmlNode = Nothing
        Dim xaFile As XmlAttribute = Nothing

        If (String.Compare(node.Name, "Configuration") <> 0) Then Return False

        Try
            Me.m_bCanSort = False
            For Each xn In node.ChildNodes
                Select Case xn.Name
                    Case "Name" : Me.m_strName = xn.InnerText
                    Case "Description" : Me.Description = xn.InnerText
                    Case "Source"
                        'xxxxxxxxxxxxxxxxxx HACK xxxxxxxxxxxxxxxxxxxxx
                        'Use the path to the Spatial Config file(XmlDocument) as the root path to the data
                        'this makes the xml file the Source (root path) for all the data files
                        Dim docpath As String = Path.GetDirectoryName(doc.BaseURI.Replace("file:///", ""))
                        Me.Source = Path.Combine(docpath, xn.InnerText)
                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                    Case "Variable" : Me.VarName = DirectCast(CInt(xn.InnerText), eVarNameFlags)
                    Case "Annual" : Convert.ToBoolean(xn.InnerText)
                    Case "Files"
                        For Each xnFile In xn.ChildNodes
                            Dim strName As String = xnFile.Attributes("Name").InnerText
                            Dim strDate As String = xnFile.Attributes("Date").InnerText
                            Dim dt As DateTime = DateTime.FromOADate(Convert.ToDouble(strDate))

                            'Source is the path to the xml Spatial Config file and will act as the root to all files 
                            'Data files need to contain the path UP to the Spatial Config file
                            'for example " Scenario-1\Datafile.asc"
                            Dim fullPath As String = Path.Combine(Me.Source, strName)
                            Debug.Assert(System.IO.File.Exists(fullPath), "Spatial Config file invalid path '" + fullPath + "'")
                            Dim f As New cTemporalFile(dt, fullPath)

                            f.IndexStatus = ISpatialDataSet.eIndexStatus.NotIndexed
                            If (xnFile.Attributes.GetNamedItem("Indexed") IsNot Nothing) Then
                                If (Boolean.Parse(xnFile.Attributes("Indexed").InnerText)) Then
                                    f.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed
                                    f.TopLeft = New PointF(CSng(cStringUtils.ConvertToNumber(xnFile.Attributes("lonmin").InnerText, GetType(Single))), _
                                                        CSng(cStringUtils.ConvertToNumber(xnFile.Attributes("latmax").InnerText, GetType(Single))))
                                    f.BottomRight = New PointF(CSng(cStringUtils.ConvertToNumber(xnFile.Attributes("lonmax").InnerText, GetType(Single))), _
                                                        CSng(cStringUtils.ConvertToNumber(xnFile.Attributes("latmin").InnerText, GetType(Single))))
                                End If
                            End If
                            Me.m_lFiles.Add(f)
                        Next
                End Select
            Next
            Me.m_bCanSort = True

        Catch ex As Exception
            Me.Clear()
            Return False
        End Try

        Return True

    End Function

End Class
