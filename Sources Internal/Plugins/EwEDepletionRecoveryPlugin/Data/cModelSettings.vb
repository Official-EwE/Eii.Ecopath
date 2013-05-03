#Region " Imports "

Option Strict On
Imports System.Xml

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Settings for a single model in the plug-in
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cModelSettings

#Region " Private vars "

    Private m_strFileName As String = ""
    Private m_lgroupcategories([Enum].GetValues(GetType(eGroupCategoryTypes)).Length) As List(Of Integer)
    Private m_lfleetcategories([Enum].GetValues(GetType(eFleetCategoryTypes)).Length) As List(Of Integer)
    Private m_ilScenarios As New List(Of Integer)

#End Region ' Private vars

#Region " Construction "

    Public Sub New(ByVal strFileName As String)
        Me.m_strFileName = strFileName

        For Each cat As eGroupCategoryTypes In [Enum].GetValues(GetType(eGroupCategoryTypes))
            Me.m_lgroupcategories(cat) = New List(Of Integer)
        Next

        For Each cat As eFleetCategoryTypes In [Enum].GetValues(GetType(eFleetCategoryTypes))
            Me.m_lfleetcategories(cat) = New List(Of Integer)
        Next
    End Sub

#End Region ' Construction

#Region " Variable access "

    Public Property Groups(ByVal category As eGroupCategoryTypes) As List(Of Integer)
        Get
            Return Me.m_lgroupcategories(category)
        End Get
        Set(ByVal value As List(Of Integer))
            Me.m_lgroupcategories(category) = value
        End Set
    End Property

    Public Property Fleets(ByVal category As eFleetCategoryTypes) As List(Of Integer)
        Get
            Return Me.m_lfleetcategories(category)
        End Get
        Set(ByVal value As List(Of Integer))
            Me.m_lfleetcategories(category) = value
        End Set
    End Property

    Public Property Scenarios() As List(Of Integer)
        Get
            Return Me.m_ilScenarios
        End Get
        Set(ByVal value As List(Of Integer))
            Me.m_ilScenarios = value
        End Set
    End Property

    Public ReadOnly Property FileName() As String
        Get
            Return Me.m_strFileName
        End Get
    End Property

#End Region ' Variable access

#Region " Presentation "

    Public Overrides Function ToString() As String
        Return Me.m_strFileName
    End Function

#End Region ' Presentation

End Class
