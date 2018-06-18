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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports System.Xml
Imports EwECore
Imports EwECore.Auxiliary
Imports EwECore.Core
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map

#End Region ' Imports

Namespace Style

    ''' <summary>
    ''' Utility class to import / export visual styles from Ecospace layers to file.
    ''' </summary>
    Public Class cImportExportStyle

#Region " Internal admin "

        Public Class cLayerEntry
            Public Sub New(name As String, style As cVisualStyle, varname As eVarNameFlags, index As Integer)
                Me.Name = name
                Me.VisualStyle = style
                Me.VarName = varname
                Me.Index = index
            End Sub
            Public ReadOnly Property Name As String = ""
            Public ReadOnly Property VisualStyle As cVisualStyle = Nothing
            Public ReadOnly Property VarName As eVarNameFlags = eVarNameFlags.NotSet
            Public ReadOnly Property Index As Integer = 0
        End Class

#End Region ' Internal admin

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_dtEntries As New Dictionary(Of String, cLayerEntry)

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(uic As cUIContext)
            Me.m_uic = uic
        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <vent>pfff</vent>
        ''' -------------------------------------------------------------------
        Public Function Entries() As cLayerEntry()
            Dim lEntries As New List(Of cLayerEntry)
            lEntries.AddRange(Me.m_dtEntries.Values)
            Return lEntries.ToArray()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Bulk add all layers from a given <see cref="IEcospaceLayerManager">layer manager</see>.
        ''' </summary>
        ''' <param name="man">The layer manager to obtain layers from.</param>
        ''' <param name="vn">Optional variable name to filter by.</param>
        ''' -------------------------------------------------------------------
        Public Sub Add(man As IEcospaceLayerManager, Optional vn As eVarNameFlags = eVarNameFlags.NotSet)
            For Each l As cEcospaceLayer In man.Layers(vn)
                Add(l)
            Next
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a single layer.
        ''' </summary>
        ''' <param name="l"></param>
        ''' -------------------------------------------------------------------
        Public Sub Add(l As cEcospaceLayer)

            If (l Is Nothing) Then Return

            Dim DBID As Integer = 0
            Select Case l.VarName
                Case eVarNameFlags.LayerHabitat
                    DBID = Me.m_uic.Core.EcospaceHabitats(l.Index).DBID
                Case eVarNameFlags.LayerMPA
                    DBID = Me.m_uic.Core.EcospaceMPAs(l.Index).DBID
            End Select

            Dim key As New cValueID(l.DataType, DBID, eVarNameFlags.Name)
            Dim ad As cAuxiliaryData = Me.m_uic.Core.AuxillaryData(key)
            If (ad Is Nothing) Then Return

            Dim vs As cVisualStyle = ad.VisualStyle
            If (vs Is Nothing) Then Return

            Add(l.Name, vs, l.VarName, l.Index)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Manually add a layer definition.
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="vs"></param>
        ''' <param name="vn"></param>
        ''' <param name="index"></param>
        ''' -------------------------------------------------------------------
        Public Sub Add(name As String, vs As cVisualStyle, vn As eVarNameFlags, index As Integer)
            Me.m_dtEntries(name) = New cLayerEntry(name, vs, eVarNameFlags.VariableName, index)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Bulk remove all layers.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub RemoveAll()
            Me.m_dtEntries.Clear()
        End Sub


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a single layer.
        ''' </summary>
        ''' <param name="l"></param>
        ''' -------------------------------------------------------------------
        Public Sub Remove(l As cEcospaceLayer)

            If (l Is Nothing) Then Return
            Remove(l.Name)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a single layer with a given name.
        ''' </summary>
        ''' <param name="name"></param>
        ''' -------------------------------------------------------------------
        Public Sub Remove(name As String)
            If Me.m_dtEntries.ContainsKey(name) Then Me.m_dtEntries.Remove(name)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load layers from file.
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Load(file As String) As Boolean

            ' ToDo: add a whack of error handling
            ' ToDo: serialize as XML

            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
            Dim doc As New XmlDocument()

            doc.Load(file)

            For Each cn As XmlElement In doc.GetElementsByTagName("LayerStyle")
                Dim name As String = cn.GetAttribute("name")
                Dim vn As eVarNameFlags = cin.GetVarName(cn.GetAttribute("varname"))
                Dim index As Integer = CInt(cn.GetAttribute("index"))
                Dim value As String = cn.GetAttribute("style")
                Me.Add(name, cVisualStyleReader.StringToStyle(value), vn, index)
            Next
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save layers to file.
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Save(file As String) As Boolean

            ' ToDo: add a whack of error handling
            ' ToDo: serialize as XML

            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
            Dim xnRoot As XmlNode = Nothing
            Dim xn As XmlElement = Nothing
            Dim xa As XmlAttribute = Nothing
            Dim doc As XmlDocument = cXMLUtils.NewDoc("LayerStyles", xnRoot)

            For Each name As String In Me.m_dtEntries.Keys

                Dim data As cLayerEntry = Me.m_dtEntries(name)

                xn = doc.CreateElement("LayerStyle")

                xa = doc.CreateAttribute("name")
                xa.InnerText = name
                xn.Attributes.Append(xa)

                xa = doc.CreateAttribute("varname")
                xa.InnerText = cin.GetVarName(data.VarName)
                xn.Attributes.Append(xa)

                xa = doc.CreateAttribute("index")
                xa.InnerText = CStr(data.Index)
                xn.Attributes.Append(xa)

                xa = doc.CreateAttribute("style")
                xa.InnerText = cVisualStyleReader.StyleToString(data.VisualStyle)
                xn.Attributes.Append(xa)

                xnRoot.AppendChild(xn)
            Next
            doc.Save(file)
            Return True

        End Function


#End Region ' Public access

    End Class

End Namespace
