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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Xml
Imports System.IO
Imports EwECore

#End Region ' Imports

Public Class cSession

    Private m_lModelSettings As New List(Of cModelSettings)
    Private m_strFileName As String = ""
    Private m_strOutputPath As String = ""
    Private m_strDirectoryMask As String = ""
    Private m_iNumYears As Integer = 0
    Private m_bAnnualAverages As Boolean = True

    Public Sub New()
        Me.Reset()
    End Sub

#Region " Public interfaces "

    Public Sub Reset()
        Me.m_lModelSettings.Clear()
        Me.m_iNumYears = 100
        Me.m_bAnnualAverages = True
        Me.m_strDirectoryMask = "[model:6]-[scenario:6]-[category]-[timeseries]"
        Me.m_strOutputPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    End Sub

    Public Function Model(ByVal strModelPath As String) As cModelSettings
        For Each ms As cModelSettings In Me.m_lModelSettings
            If String.Compare(ms.FileName, strModelPath, False) = 0 Then
                Return ms
            End If
        Next
        Return Nothing
    End Function

    Public Function AddModel(ByVal ms As cModelSettings) As Boolean
        If Me.m_lModelSettings.IndexOf(ms) > -1 Then Return False
        Me.m_lModelSettings.Add(ms)
        Return True
    End Function

    Public Function RemoveModel(ByVal ms As cModelSettings) As Boolean
        If Me.m_lModelSettings.IndexOf(ms) = -1 Then Return False
        Me.m_lModelSettings.Remove(ms)
        Return True
    End Function

    Public Function Models() As List(Of cModelSettings)
        Return Me.m_lModelSettings
    End Function

    Public Function NumSteps() As Integer
        Dim iNumSteps As Integer = 0
        Dim iNumGroupCats As Integer = [Enum].GetValues(GetType(eGroupCategoryTypes)).Length
        Dim iNumFleetCats As Integer = [Enum].GetValues(GetType(eGroupCategoryTypes)).Length
        For Each ms As cModelSettings In Me.m_lModelSettings
            iNumSteps += (ms.Scenarios.Count * iNumGroupCats * iNumFleetCats)
        Next
        Return iNumSteps
    End Function

#End Region ' Public interfaces

#Region " Public Properties "

    Public ReadOnly Property FileName() As String
        Get
            Return Me.m_strFileName
        End Get
    End Property

    Public Property OutputPath() As String
        Get
            Return Me.m_strOutputPath
        End Get
        Set(ByVal value As String)
            Me.m_strOutputPath = value
        End Set
    End Property

    Public Property DirectoryMask() As String
        Get
            Return Me.m_strDirectoryMask
        End Get
        Set(ByVal value As String)
            Me.m_strDirectoryMask = value
        End Set
    End Property

    Public Property NumberOfYears() As Integer
        Get
            Return Me.m_iNumYears
        End Get
        Set(ByVal value As Integer)
            Me.m_iNumYears = value
        End Set
    End Property

    Public Property AnnualAverages() As Boolean
        Get
            Return Me.m_bAnnualAverages
        End Get
        Set(ByVal bAnnualAverages As Boolean)
            Me.m_bAnnualAverages = bAnnualAverages
        End Set
    End Property

#End Region ' Public Properties

#Region " Load /save "

    Public Function Save(ByVal strFileName As String) As Boolean

        Dim doc As XmlDocument = Me.CreateXMLDocument()
        Dim xn As XmlNode = Nothing
        Dim xa As XmlAttribute = Nothing
        Dim modelsettings As cModelSettings = Nothing

        Me.m_strFileName = strFileName

        ' Create session node
        xn = doc.CreateElement("Session")

        ' Add output path
        xa = doc.CreateAttribute("OutputPath")
        xa.Value = Me.OutputPath
        xn.Attributes.Append(xa)

        ' Add dir mask
        xa = doc.CreateAttribute("DirectoryMask")
        xa.Value = Me.DirectoryMask
        xn.Attributes.Append(xa)

        ' Add annual averages
        xa = doc.CreateAttribute("AnnualAvg")
        xa.Value = Convert.ToString(Me.AnnualAverages)
        xn.Attributes.Append(xa)

        ' Add numyears
        xa = doc.CreateAttribute("NumYears")
        xa.Value = Convert.ToString(Me.NumberOfYears)
        xn.Attributes.Append(xa)

        ' Done
        doc.AppendChild(xn)

        For Each ms As cModelSettings In Me.Models
            xn.AppendChild(BuildModelSettingsNode(ms, doc))
        Next

        doc.Save(strFileName)
        Return True

    End Function

    Public Function Load(ByVal strFileName As String) As Boolean

        Dim doc As XmlDocument = Me.CreateXMLDocument()
        Dim bSucces As Boolean = True

        Try
            doc.Load(strFileName)
            Me.Reset()

            For Each xn As XmlNode In doc.ChildNodes
                If (String.Compare(xn.Name, "Session", True) = 0) Then

                    For Each xa As XmlAttribute In xn.Attributes
                        Try
                            Select Case xa.Name.ToLower()
                                Case "outputpath" : Me.m_strOutputPath = xa.Value
                                Case "directorymask" : Me.m_strDirectoryMask = xa.Value
                                Case "annualavg" : Me.AnnualAverages = Convert.ToBoolean(xa.Value)
                                Case "numyears" : Me.NumberOfYears = Convert.ToInt16(xa.Value)
                            End Select
                        Catch ex As Exception

                        End Try
                    Next xa

                    For Each xnChild As XmlNode In xn.ChildNodes
                        If (String.Compare(xnChild.Name, "Model", True) = 0) Then
                            bSucces = bSucces And Me.ReadModelSettingsNode(xnChild)
                        End If
                    Next xnChild

                    Exit For
                End If
            Next
        Catch ex As Exception
            bSucces = False
        End Try

        If Not bSucces Then Me.Reset()
        Return bSucces

    End Function

    Private Function CreateXMLDocument() As XmlDocument
        Dim doc As New XmlDocument()
        doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", Nothing))
        doc.PreserveWhitespace = True
        Return doc
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Format a single time step, single habitat as a XML document
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function BuildModelSettingsNode(ByVal ms As cModelSettings, ByVal doc As XmlDocument) As XmlNode

        Dim xn As XmlNode = Nothing
        Dim xa As XmlAttribute = Nothing

        ' Create model node
        xn = doc.CreateNode(XmlNodeType.Element, "Model", "")

        ' Append file name attribute
        xa = doc.CreateAttribute("File")
        xa.Value = ms.FileName
        xn.Attributes.Append(xa)

        ' Append group categories
        For Each cat As eGroupCategoryTypes In [Enum].GetValues(GetType(eGroupCategoryTypes))
            If ms.Groups(cat).Count > 0 Then
                xn.AppendChild(BuildGroupCategoryNode(ms, cat, doc))
            End If
        Next

        ' Append fleet categories
        For Each cat As eFleetCategoryTypes In [Enum].GetValues(GetType(eFleetCategoryTypes))
            If ms.Fleets(cat).Count > 0 Then
                xn.AppendChild(BuildFleetCategoryNode(ms, cat, doc))
            End If
        Next

        ' Append scenarios
        xn.AppendChild(BuildScenariosNode(ms, doc))

        Return xn

    End Function

    Private Function ReadModelSettingsNode(ByVal xn As XmlNode) As Boolean

        Dim xa As XmlAttribute = Nothing
        Dim ms As cModelSettings = Nothing
        Dim bSucces As Boolean = True

        ' Ignore name

        xa = xn.Attributes("File")
        If (xa Is Nothing) Then Return False
        ms = New cModelSettings(xa.Value)

        ' Read group categories
        For Each xnChild As XmlNode In xn.ChildNodes
            Select Case xnChild.Name.ToLower
                Case "groupcategory", "category"
                    bSucces = bSucces And Me.ReadGroupCategoryNode(xnChild, ms)
                Case "fleetcategory"
                    bSucces = bSucces And Me.ReadFleetCategoryNode(xnChild, ms)
                Case "scenarios"
                    bSucces = bSucces And Me.ReadScenariosNode(xnChild, ms)
            End Select
        Next

        If bSucces Then
            Me.AddModel(ms)
        End If
        Return bSucces

    End Function

#Region " Group nodes "

    Private Function BuildGroupCategoryNode(ByVal ms As cModelSettings, ByVal cat As eGroupCategoryTypes, ByVal doc As XmlDocument) As XmlNode

        Dim xn As XmlNode = Nothing
        Dim xa As XmlAttribute = Nothing

        ' Create model node
        xn = doc.CreateNode(XmlNodeType.Element, "GroupCategory", "")

        ' Append category numerical value attribute
        xa = doc.CreateAttribute("Value")
        xa.Value = CStr(CInt(cat))
        xn.Attributes.Append(xa)

        ' Append category name attribute
        xa = doc.CreateAttribute("Name")
        xa.Value = cat.ToString()
        xn.Attributes.Append(xa)

        ' Append group categories
        For Each i As Integer In ms.Groups(cat)
            xn.AppendChild(BuildIndexNode("Group", i, doc))
        Next
        Return xn

    End Function

    Private Function ReadGroupCategoryNode(ByVal xn As XmlNode, ByVal ms As cModelSettings) As Boolean

        Dim xa As XmlAttribute = Nothing
        Dim cat As eGroupCategoryTypes = eGroupCategoryTypes.All
        Dim li As New List(Of Integer)
        Dim bSucces As Boolean = True

        ' Ignore name

        xa = xn.Attributes("Value")
        If (xa Is Nothing) Then Return False
        Try
            cat = DirectCast(Integer.Parse(xa.Value), eGroupCategoryTypes)
        Catch ex As Exception
            Return False
        End Try

        ' Read group categories
        For Each xnChild As XmlNode In xn.ChildNodes
            Dim iGroup As Integer = Me.ReadIndexNode("Group", xnChild)
            If iGroup <> cCore.NULL_VALUE Then
                li.Add(iGroup)
            Else
                bSucces = False
            End If
        Next

        If bSucces Then
            ms.Groups(cat) = li
        End If

        Return bSucces

    End Function

#End Region ' Group nodes

#Region " Fleet nodes "

    Private Function BuildFleetCategoryNode(ByVal ms As cModelSettings, _
                                            ByVal cat As eFleetCategoryTypes, _
                                            ByVal doc As XmlDocument) As XmlNode

        Dim xn As XmlNode = Nothing
        Dim xa As XmlAttribute = Nothing

        ' Create model node
        xn = doc.CreateNode(XmlNodeType.Element, "FleetCategory", "")

        ' Append category numerical value attribute
        xa = doc.CreateAttribute("Value")
        xa.Value = CStr(CInt(cat))
        xn.Attributes.Append(xa)

        ' Append category name attribute
        xa = doc.CreateAttribute("Name")
        xa.Value = cat.ToString()
        xn.Attributes.Append(xa)

        ' Append fleet categories
        For Each i As Integer In ms.Fleets(cat)
            xn.AppendChild(BuildIndexNode("Fleet", i, doc))
        Next
        Return xn

    End Function

    Private Function ReadFleetCategoryNode(ByVal xn As XmlNode, ByVal ms As cModelSettings) As Boolean

        Dim xa As XmlAttribute = Nothing
        Dim cat As eFleetCategoryTypes = eFleetCategoryTypes.All
        Dim li As New List(Of Integer)
        Dim bSucces As Boolean = True

        ' Ignore name

        xa = xn.Attributes("Value")
        If (xa Is Nothing) Then Return False
        Try
            cat = DirectCast(Integer.Parse(xa.Value), eFleetCategoryTypes)
        Catch ex As Exception
            Return False
        End Try

        ' Read fleet nodes
        For Each xnChild As XmlNode In xn.ChildNodes
            Dim iFleet As Integer = Me.ReadIndexNode("Fleet", xnChild)
            If iFleet <> cCore.NULL_VALUE Then
                li.Add(iFleet)
            Else
                bSucces = False
            End If
        Next

        If bSucces Then
            ms.Fleets(cat) = li
        End If

        Return bSucces

    End Function

#End Region ' Fleet nodes

#Region " Scenario nodes "

    Private Function BuildScenariosNode(ByVal ms As cModelSettings, ByVal doc As XmlDocument) As XmlNode

        Dim xn As XmlNode = Nothing
        Dim xa As XmlAttribute = Nothing

        ' Create model node
        xn = doc.CreateNode(XmlNodeType.Element, "Scenarios", "")

        ' Append group categories
        For Each i As Integer In ms.Scenarios
            xn.AppendChild(BuildIndexNode("Scenario", i, doc))
        Next
        Return xn

    End Function

    Private Function ReadScenariosNode(ByVal xn As XmlNode, ByVal ms As cModelSettings) As Boolean

        Dim xa As XmlAttribute = Nothing
        Dim li As New List(Of Integer)
        Dim bSucces As Boolean = True

        ' Ignore name

        ' Read fleet nodes
        For Each xnChild As XmlNode In xn.ChildNodes
            Dim iScenario As Integer = Me.ReadIndexNode("Scenario", xnChild)
            If iScenario <> cCore.NULL_VALUE Then
                li.Add(iScenario)
            Else
                bSucces = False
            End If
        Next

        If bSucces Then
            ms.Scenarios = li
        End If

        Return bSucces

    End Function

#End Region ' Scenario nodes

#Region " Generic "

    Private Function BuildIndexNode(ByVal strName As String, ByVal iIndex As Integer, ByVal doc As XmlDocument) As XmlNode

        Dim xn As XmlNode = Nothing
        Dim xa As XmlAttribute = Nothing

        ' Create model node
        xn = doc.CreateNode(XmlNodeType.Element, strName, "")

        ' Append index
        xa = doc.CreateAttribute("Index")
        xa.Value = CStr(CInt(iIndex))
        xn.Attributes.Append(xa)

        Return xn

    End Function

    Private Function ReadIndexNode(ByVal strName As String, ByVal xn As XmlNode) As Integer

        ' Read index
        Dim xa As XmlAttribute = xn.Attributes("Index")
        If xa IsNot Nothing Then Return CInt(xa.Value)
        Return cCore.NULL_VALUE

    End Function

#End Region ' Generic

#End Region ' Load /save

End Class
