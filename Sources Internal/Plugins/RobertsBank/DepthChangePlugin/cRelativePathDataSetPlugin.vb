
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

        Me.m_strName = "Relative Dataset"
        Me.Description = "ASCII file dataset containing files in a relative path"

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
                        'In the normal data set the Source node is the full path to the data files
                        'Here it CAN be just the path from the XMLDocument to the data files
                        'Making this data set "Relative" to the XMLDocument itself

                        'Use the path to the Spatial Config file(XmlDocument) as the root path to the data
                        Dim docpath As String = Path.GetDirectoryName(doc.BaseURI.Replace("file:///", ""))
                        'Source node should be the directory structure under the XMLDocument that contains the data files
                        'Combine the XMLDocument path and Source node for the full data path to this dataset
                        If Not Path.IsPathRooted(xn.InnerText) Then
                            Me.Source = Path.Combine(docpath, xn.InnerText)
                        Else
                            Me.Source = xn.InnerText
                        End If
                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                    Case "Variable" : Me.VarName = DirectCast(CInt(xn.InnerText), eVarNameFlags)
                    Case "Annual" : Convert.ToBoolean(xn.InnerText)
                    Case "Files"
                        For Each xnFile In xn.ChildNodes
                            Dim strName As String = xnFile.Attributes("Name").InnerText
                            Dim strDate As String = xnFile.Attributes("Date").InnerText
                            Dim dt As DateTime = DateTime.FromOADate(Convert.ToDouble(strDate))

                            'Source is the root to all files built from the XMLDocument path and the Source node
                            'It's relative to the XMLDocument 
                            'The Name node can be just the file name or it could contain a path from the Source node
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
