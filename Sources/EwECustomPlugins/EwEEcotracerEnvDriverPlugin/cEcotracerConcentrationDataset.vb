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
Imports System.Drawing
Imports System.Xml
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData

#End Region ' Imports

Public Class cEcotracerConcentrationDataset
    Implements ISpatialDataSetPlugin
    Implements IConfigurable

    Private m_core As cCore = Nothing

#Region " Construction "

    Public Sub New()
        Me.CustomName = My.Resources.CCONC_DATASET_TYPE
    End Sub

    Public Sub Initialize(core As Object) _
        Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

#End Region ' Construction

#Region " Generics "

    Public Property Group As Integer

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return Me.CustomDescription
        End Get
    End Property

    Public Property GUID As System.Guid _
        Implements ISpatialDataSet.GUID

    Public ReadOnly Property Name As String _
        Implements IPlugin.Name
        Get
            Return "EwECore.Dataset.EcoTracerConcentration"
        End Get
    End Property

    Public ReadOnly Property Name2 As String _
        Implements IPlugin.DisplayName
        Get
            Return String.Format("Concentration {0}", Me.Group)
        End Get
    End Property

    Public Property CustomName As String _
        Implements ISpatialDataSet.CustomName

    Public Property CustomDescription As String _
        Implements ISpatialDataSet.CustomDescription

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

        Return New cCConcSpatialRaster(Me.m_core.EcospaceBasemap, Me.m_core.m_EcospaceData, Me.Group)

    End Function

#End Region ' Actual raster generation

#Region " Configuration "

    Public Function GetConfigUI() As Object _
        Implements IConfigurable.GetConfigUI
        Return New ucConfigureCConcDriver(Me)
    End Function

    Public Function IsConfigured() As Boolean _
        Implements ISpatialDataSet.IsConfigured, IConfigurable.IsConfigured
        Return (Me.m_core.StateMonitor.HasEcotracerLoaded) And (Me.Group > 0) And (Me.Group < Me.m_core.nGroups)
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
    ''' Write dataset configuration to XML.
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
        xn.InnerText = Me.CustomName
        xnMaster.AppendChild(xn)

        xn = doc.CreateElement("Description")
        xn.InnerText = Me.CustomDescription
        xnMaster.AppendChild(xn)

        xn = doc.CreateElement("Group")
        xn.InnerText = CStr(Me.Group)
        xnMaster.AppendChild(xn)

        Return xnMaster

    End Function

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
    Private Function FromXML(ByVal doc As XmlDocument,
                             ByVal node As XmlNode) As Boolean

        Dim xn As XmlNode = Nothing
        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

        If (node Is Nothing) Then Return False
        If (String.Compare(node.Name, "Configuration") <> 0) Then Return False

        Try

            For Each xn In node.ChildNodes
                Select Case xn.Name.ToLower()
                    Case "name"
                        Me.CustomName = xn.InnerText

                    Case "description"
                        Me.CustomDescription = xn.InnerText

                    Case "group"
                        Me.Group = CInt(xn.InnerText)

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

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Data is only available when Ecotracer is enabled for Ecospace.
    ''' </summary>
    ''' <param name="runtype"></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Public Function IsDataAvailable(runtype As IRunType) As Boolean _
        Implements IExternalDataSource.IsDataAvailable

        Dim parms As cEcospaceModelParameters = Me.m_core.EcospaceModelParameters
        Return Me.IsConfigured() And parms.ContaminantTracing

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

    Public ReadOnly Property DialogReadFilter(bRaster As Boolean, bImage As Boolean, bVector As Boolean, bAllFiles As Boolean) As String _
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
