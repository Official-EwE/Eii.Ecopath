#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports System.Collections.Generic

#End Region ' Imports

Public Class cPedigreeManager
    Implements ICoreInterface

#Region " Private classes "

    Private Class cPedigreeLevelListSorter
        Implements IComparer(Of cPedigreeLevel)

        Public Function Compare(ByVal x As cPedigreeLevel, ByVal y As cPedigreeLevel) As Integer _
            Implements IComparer(Of cPedigreeLevel).Compare
            If x.Index < y.Index Then Return -1
            If x.Index > y.Index Then Return 1
            If x.ConfidenceInterval < y.ConfidenceInterval Then Return -1
            If x.ConfidenceInterval > y.ConfidenceInterval Then Return 1
            Return 0
        End Function

    End Class

#End Region ' Private classes

    Private Shared g_varNameSupported As eVarNameFlags() = {eVarNameFlags.Biomass, eVarNameFlags.PBInput, eVarNameFlags.QBInput, eVarNameFlags.DietComp}
    Private m_core As cCore = Nothing
    Private m_varName As eVarNameFlags = eVarNameFlags.NotSet
    Private m_levels As New List(Of cPedigreeLevel)
    Private m_dataType As eDataTypes = eDataTypes.PedigreeLevel
    Private m_messageSource As eCoreComponentType = eCoreComponentType.Core

    Friend Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags)
        Me.m_core = core
        Me.m_varName = varName
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the list of variables for which pedigree information is supported.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function SupportVariables() As eVarNameFlags()
        Return cPedigreeManager.g_varNameSupported
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strDescription"></param>
    ''' <param name="iPosition">The position of the level to assign in the manager.</param>
    ''' <param name="varName"></param>
    ''' <param name="sIndexValue"></param>
    ''' <param name="sConfidence"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function AddLevel(ByVal strDescription As String, _
                                         ByVal iPosition As Integer, _
                                         ByVal varName As eVarNameFlags, _
                                         ByVal sIndexValue As Single, _
                                         ByVal sConfidence As Single, _
                                         ByRef iDBID As Integer) As Boolean

        Dim level As cPedigreeLevel = Nothing
        Dim bSucces As Boolean = True

        If m_core.AddPedigreeLevel(varName, iPosition, sIndexValue, sConfidence, strDescription, iDBID) Then
            ' Reload me
            bSucces = Me.Load()
            ' Give core a chance to respond
            Me.m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)
        End If

        Return bSucces

    End Function

    Public Function MoveLevel(ByVal iPosFrom As Integer, ByVal iPosTo As Integer) As Boolean
        ' Hih
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="level"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveLevel(ByVal level As cPedigreeLevel) As Boolean

        Dim bSucces As Boolean = True

        If m_core.RemovePedigreeLevel(level.DBID) Then
            ' Reload me
            bSucces = Me.Load()
            ' Give core a chance to respond
            Me.m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)
        End If

        Return bSucces

    End Function

#Region " Saving, loading and updating "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create and load pedigree levels.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Load() As Boolean
        Try
            Dim level As cPedigreeLevel = Nothing
            Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData

            Me.m_levels.Clear()
            For iLevel As Integer = 1 To data.NumPedigreeLevels
                If data.PedigreeLevelVarName(iLevel) = Me.m_varName Then

                    level = New cPedigreeLevel(Me.m_core, Me, data.PedigreeLevelDBID(iLevel))

                    level.AllowValidation = False
                    level.Index = iLevel
                    level.IndexValue = data.PedigreeLevelIndexValue(iLevel)
                    level.ConfidenceInterval = data.PedigreeLevelConfidence(iLevel)
                    level.Description = data.PedigreeLevelDescription(iLevel)
                    level.AllowValidation = True

                    Me.m_levels.Add(level)

                End If
            Next

        Catch ex As Exception
            Return False
            Debug.Assert(False, Me.ToString & ".Load() Error: " & ex.Message)
        End Try

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Commit pedigree levels to the EwE core.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Update() As Boolean

        Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim level As cPedigreeLevel = Nothing

        Try
            For iLevel As Integer = 1 To data.NumPedigreeLevels

                level = Me.m_levels(iLevel)
                Try
                    data.PedigreeLevelIndexValue(level.Index) = level.IndexValue
                    data.PedigreeLevelConfidence(level.Index) = level.ConfidenceInterval
                    data.PedigreeLevelDescription(level.Index) = level.Description

                Catch ex As Exception
                    cLog.Write(Me.ToString & ".Update() level failed to update DBID=" & level.DBID)
                    Debug.Assert(False, Me.ToString & ".Update() level failed to update DBID=" & level.DBID)
                End Try

            Next iLevel

            Me.m_core.onChanged(Me, eMessageType.DataModified)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() Error: " & ex.Message)
            Return False
        End Try

        Return True

    End Function

#End Region ' Saving, loading and updating

#Region " ICoreInterface Implementation "

    Public ReadOnly Property DataType() As EwEUtils.Core.eDataTypes Implements ICoreInterface.DataType
        Get
            Return Me.m_dataType
        End Get
    End Property

    Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
        Get
            Return Me.m_messageSource
        End Get
    End Property

    Public Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

    Public Function GetID() As String Implements ICoreInterface.GetID
        Dim id As Integer = CType(m_dataType, Integer)
        Return cValueID.getDataTypeID(m_dataType, id)
    End Function

    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return Me.ToString
        End Get
        Set(ByVal value As String)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

#End Region ' ICoreInterface Implementation

#Region " Public methods "

    Public ReadOnly Property NumLevels() As Integer
        Get
            Return Me.m_levels.Count
        End Get
    End Property

    Public ReadOnly Property Level(ByVal iLevel As Integer) As cPedigreeLevel
        Get
            Return Me.m_levels(iLevel)
        End Get
    End Property

#End Region ' Public methods

End Class
