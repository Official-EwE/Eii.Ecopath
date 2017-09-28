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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Web
Imports System.Xml
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cComplexityDataset
    Implements ISpatialDataSetPlugin
    Implements IConfigurable

    Private m_core As cCore = Nothing
    Private m_lRules As New List(Of cComplexityRule)

#Region " Construction "

    Public Sub New()
        Me.DisplayName = "EcoEngineer driver"
        Me.DataDescription = "An engine that calculates a habitat complexity map from various ecosystem engineer biomasses."
    End Sub

    Public Sub Initialize(core As Object) _
        Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

#End Region ' Construction

#Region " Generics "

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public Property GUID As System.Guid _
        Implements ISpatialDataSet.GUID

    Public ReadOnly Property Name As String _
        Implements IPlugin.Name
        Get
            Return "EwECore.Dataset.EcoEngineer"
        End Get
    End Property

    Public Property DisplayName As String _
        Implements ISpatialDataSet.DisplayName

    Public Property DataDescription As String _
        Implements ISpatialDataSet.DataDescription

    Public ReadOnly Property Summary As String _
        Implements ISummarizable.Summary
        Get
            ' ToDo later - for key run purposes
            Return ""
        End Get
    End Property

#End Region ' Generics

#Region " Metadata and attributes "

    Public Function GetAttributeDataTypes() As System.Type() _
        Implements ISpatialDataSet.GetAttributeDataTypes
        ' Not supported
        Return Nothing
    End Function

    Public Function GetAttributes() As String() _
        Implements ISpatialDataSet.GetAttributes
        ' Not supported
        Return Nothing
    End Function

    Public Function GetAttributeValues() As System.Data.DataTable _
        Implements ISpatialDataSet.GetAttributeValues
        ' Not supported
        Return Nothing
    End Function

#End Region ' Metadata and attributes

#Region " Actual raster generation "

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="converter"></param>
    ''' <param name="strLayerName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRaster(converter As ISpatialDataConverter, strLayerName As String) As ISpatialRaster _
        Implements ISpatialDataSet.GetRaster

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim nRows As Integer = bm.InRow
        Dim nCols As Integer = bm.InCol
        Dim data(nRows, nCols) As Single
        Dim ds As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData

        ' Perform the conversion
        For iRow As Integer = 1 To nRows
            For iCol As Integer = 1 To nCols
                If (bm.IsModelledCell(iRow, iCol)) Then
                    ' Calculate cummulative complexity from all valid rules
                    Dim complexity As Single = 0
                    For Each r As cComplexityRule In Me.m_lRules
                        If (r.IsValid) Then
                            complexity += r.ArchitecturalComplexity(ds.Bcell(iRow, iCol, r.Group))
                        End If
                    Next

                    data(iRow, iCol) = complexity
                End If
            Next iCol
        Next iRow

        Return New cSpatialRaster(bm, data)

    End Function

#End Region ' Actual raster generation

#Region " Configuration "

    Public Function Rules() As List(Of cComplexityRule)
        Return Me.m_lRules
    End Function

    Public Function IsConfigured() As Boolean _
        Implements ISpatialDataSet.IsConfigured, IConfigurable.IsConfigured
        Return (Me.m_lRules.Count > 0)
    End Function

    Public Function GetConfigUI() As System.Windows.Forms.Control _
        Implements IConfigurable.GetConfigUI
        Return New ucEcoEngineerConfigUI(Me)
    End Function

    Public Property Configuration(doc As System.Xml.XmlDocument, strFolderRoot As String) As System.Xml.XmlNode _
        Implements ISpatialDataSet.Configuration
        Get
            Return Me.ToXML(doc)
        End Get
        Set(value As System.Xml.XmlNode)
            Me.FromXML(doc, value)
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Write content to XML.
    ''' </summary>
    ''' <param name="doc">The doc to generate nodes for.</param>
    ''' <returns>
    ''' An XML node that contains the content of the dataset.
    ''' </returns>
    ''' -------------------------------------------------------------------
    Private Function ToXML(ByVal doc As XmlDocument) As XmlNode

        Dim xnMaster As XmlNode = Nothing
        Dim xn As XmlNode = Nothing
        Dim xnRule As XmlNode = Nothing
        Dim xaRule As XmlAttribute = Nothing
        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

        xnMaster = doc.CreateElement("Configuration")

        xn = doc.CreateElement("Name")
        xn.InnerText = Me.DisplayName
        xnMaster.AppendChild(xn)

        xn = doc.CreateElement("Description")
        xn.InnerText = Me.DataDescription
        xnMaster.AppendChild(xn)

        For Each r As cComplexityRule In Me.m_lRules
            xnRule = doc.CreateElement("Rule")

            xaRule = doc.CreateAttribute("GroupDBID")
            xaRule.InnerText = CStr(Me.m_core.EcoPathGroupInputs(r.Group).DBID)
            xnRule.Attributes.Append(xaRule)

            ' Store name as a URL encoded string to avoid quote character confusion
            xaRule = doc.CreateAttribute("Name")
            xaRule.InnerText = HttpUtility.UrlEncode(r.Name)
            xnRule.Attributes.Append(xaRule)

            xaRule = doc.CreateAttribute("A")
            xaRule.InnerText = cStringUtils.FormatNumber(r.A)
            xnRule.Attributes.Append(xaRule)

            xaRule = doc.CreateAttribute("B")
            xaRule.InnerText = cStringUtils.FormatNumber(r.B)
            xnRule.Attributes.Append(xaRule)

            xaRule = doc.CreateAttribute("C")
            xaRule.InnerText = cStringUtils.FormatNumber(r.C)
            xnRule.Attributes.Append(xaRule)

            xnMaster.AppendChild(xnRule)
        Next

        Return xnMaster

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Read content from XML.
    ''' </summary>
    ''' <param name="doc">The doc to read nodes from.</param>
    ''' <param name="node">The configuration node that contains the content
    ''' of the dataset. Happy, happy, happy.</param>
    ''' <returns>
    ''' True if successful.
    ''' </returns>
    ''' -------------------------------------------------------------------
    Private Function FromXML(ByVal doc As XmlDocument, _
                             ByVal node As XmlNode) As Boolean

        Dim xn As XmlNode = Nothing
        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

        If (node Is Nothing) Then Return False
        If (String.Compare(node.Name, "Configuration") <> 0) Then Return False

        Me.m_lRules.Clear()

        Try

            For Each xn In node.ChildNodes
                Select Case xn.Name.ToLower()
                    Case "name"
                        Me.DisplayName = xn.InnerText

                    Case "description"
                        Me.DataDescription = xn.InnerText

                    Case "rule"
                        Dim r As New cComplexityRule()
                        Dim dbid As Integer = CInt(xn.Attributes("GroupDBID").InnerText)
                        For i As Integer = 1 To Me.m_core.nGroups
                            If Me.m_core.EcoPathGroupInputs(i).DBID = dbid Then
                                r.Group = i
                            End If
                        Next
                        If (xn.Attributes("Name") IsNot Nothing) Then
                            r.Name = HttpUtility.UrlDecode(xn.Attributes("Name").InnerText)
                        Else
                            r.Name = My.Resources.FUNCTION_NONAME
                        End If
                        r.A = cStringUtils.ConvertToSingle(xn.Attributes("A").InnerText)
                        r.B = cStringUtils.ConvertToSingle(xn.Attributes("B").InnerText)
                        r.C = cStringUtils.ConvertToSingle(xn.Attributes("C").InnerText)

                        If (r.IsValid) Then
                            Me.m_lRules.Add(r)
                        End If

                End Select
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

#End Region ' Configuration

#Region " Data access "

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="IExternalDataSource.EnableData"/>
    ''' -------------------------------------------------------------------
    Public Property EnableData(runtype As IRunType) As Boolean _
        Implements IExternalDataSource.EnableData
        Get
            Return True
        End Get
        Set(value As Boolean)
            ' NOP
        End Set
    End Property

    Public Function IsDataAvailable(runtype As IRunType) As Boolean _
        Implements IExternalDataSource.IsDataAvailable
        ' Data is always available when this plug-in is correctly configured
        Return Me.IsConfigured()
    End Function

    Public Function HasDataAtT(datetime As Date) As Boolean _
        Implements ISpatialDataSet.HasDataAtT
        Return True
    End Function

    Public Function GetExtentAtT(datetime As Date, ByRef ptfTL As PointF, ByRef ptfBR As PointF) As Boolean _
        Implements ISpatialDataSet.GetExtentAtT
        ' Always return full Ecospace extent
        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        ptfTL = bm.PosTopLeft
        ptfBR = bm.PosBottomRight
        Return True
    End Function

    Public ReadOnly Property TimeStart As Date _
        Implements ISpatialDataSet.TimeStart
        Get
            ' Beginning of any Ecospace run time
            Return Me.m_core.EcospaceTimestepToAbsoluteTime(1)
        End Get
    End Property

    Public ReadOnly Property TimeEnd As Date _
        Implements ISpatialDataSet.TimeEnd
        Get
            ' End of any Ecospace run time
            Dim parms As cEcospaceModelParameters = Me.m_core.EcospaceModelParameters
            Return Me.m_core.EcospaceTimestepToAbsoluteTime(Me.m_core.nEcospaceTimeSteps + CInt(parms.NumberOfTimeStepsPerYear))
        End Get
    End Property

    Public Function LockDataAtT(datetime As Date, dCellSize As Double, ptfNE As PointF, ptfSW As PointF, strProjectionString As String) As Boolean _
       Implements ISpatialDataSet.LockDataAtT
        ' Not needed, just report a success
        Return True
    End Function

    Public Function IsLocked() As Boolean _
        Implements ISpatialDataSet.IsLocked
        ' Not needed, just report a success
        Return True
    End Function

    Public Function Unlock() As Boolean _
        Implements ISpatialDataSet.Unlock
        ' Not needed, just report a success
        Return True
    End Function

#End Region ' Data access

#Region " Indexing "

    Public Sub UpdateIndexAtT(datetime As Date) _
        Implements ISpatialDataSet.UpdateIndexAtT
        ' NOP
    End Sub

    Public Function IndexStatusAtT(datetime As Date) As ISpatialDataSet.eIndexStatus _
        Implements ISpatialDataSet.IndexStatusAtT
        ' Always indexed
        Return ISpatialDataSet.eIndexStatus.Indexed
    End Function

#End Region ' Indexing

#Region " Other config bits "

    Public ReadOnly Property DialogReadFilter(bRaster As Boolean, bImage As Boolean, bVector As Boolean) As String _
        Implements ISpatialDataSet.DialogReadFilter
        Get
            ' Does not apply
            Return ""
        End Get
    End Property

    Public ReadOnly Property ConversionFormat As String _
        Implements ISpatialDataSet.ConversionFormat
        Get
            ' No need to convert
            Return ""
        End Get
    End Property

    Public Property Source As String _
        Implements ISpatialDataSet.Source
        Get
            Return "internal"
        End Get
        Set(value As String)
            ' NOP
        End Set
    End Property

    Public Property VarName As eVarNameFlags _
        Implements ISpatialDataSet.VarName
        Get
            Return eVarNameFlags.LayerDriver
        End Get
        Set(value As eVarNameFlags)
            ' NOP
        End Set
    End Property

    Public Property Cache As ISpatialDataCache _
        Implements ISpatialDataSet.Cache
        Get
            Return Nothing
        End Get
        Set(value As ISpatialDataCache)
            ' NOP
        End Set
    End Property

    Public Function ExportTo(strPath As String) As ISpatialDataSet _
        Implements ISpatialDataSet.ExportTo
        ' No data to copy, no fancy things to do here
        Return Me
    End Function

#End Region ' Other config bits

End Class
