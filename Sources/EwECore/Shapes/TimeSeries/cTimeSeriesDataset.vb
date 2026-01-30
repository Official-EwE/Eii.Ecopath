' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' ---------------------------------------------------------------------------
''' <summary>
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTimeSeriesDataset
    Inherits cCoreInputOutputBase

    Private m_iNumTimeSeries As Integer = 0
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cTimeSeriesDataset)()

#Region " Constructor "

    Public Sub New(core As cCore, nTimeSeries As Integer)
        MyBase.New(core)

        Dim val As cValue
        Dim desc() As Char

        Try

            Me.m_dataType = eDataTypes.TimeSeriesDataset
            Me.m_iNumTimeSeries = nTimeSeries

            ' Definition changes do not affect the running state of the model
            Me.m_coreComponent = eCoreComponentType.DataSource

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' Description
            val = New cValue(core, New String(desc), eVarNameFlags.Description, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str)
            Me.m_values.Add(val.varName, val)

            ' Author
            val = New cValue(core, New String(desc), eVarNameFlags.Author, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            Me.m_values.Add(val.varName, val)

            ' Contact
            val = New cValue(core, New String(desc), eVarNameFlags.Contact, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            Me.m_values.Add(val.varName, val)

            'set status flags to default values
            Me.ResetStatusFlags()
            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cTimeSeriesDataset.")
            m_logger.LogError(ex, ".New(nGroups) Error creating new cTimeSeriesDataset. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructor

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' All variables non-editable (for now, 11feb08)
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        Me.AllowValidation = False
        For Each vs As cValue In Me.m_values.Values
            vs.ValidationStatus = eStatusFlags.OK Or eStatusFlags.NotEditable
        Next
        Me.AllowValidation = True
        Return True
    End Function

#End Region ' Overrides

#Region " Variable via dot(.) operator"

    Public Property Description() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Description))
        End Get

        Friend Set(str As String)
            Me.SetVariable(eVarNameFlags.Description, str)
        End Set
    End Property

    Public Property Author() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Author))
        End Get

        Friend Set(str As String)
            Me.SetVariable(eVarNameFlags.Author, str)
        End Set
    End Property

    Public Property Contact() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Contact))
        End Get

        Friend Set(str As String)
            Me.SetVariable(eVarNameFlags.Contact, str)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of time series in a dataaset, as obtained from the datasource.
    ''' </summary>
    ''' <remarks>
    ''' <para>This value is read from the database and provides an estimate of the number of
    ''' time series for this dataset PRIOR TO when the time series are loaded.</para>
    ''' <para>As soon as the dataset is loaded, the method will give the actual number of 
    ''' time series loaded in the core.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property nTimeSeries() As Integer
        Get
            ' Return the cached number of time series if no series have been loaded yet
            Dim iTot As Integer = Me.m_core.EcosimGroupTimeseries.Count + Me.m_core.EcosimFleetTimeseries.Count
            If (iTot = 0) Then Return Me.m_iNumTimeSeries
            Return iTot
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the first year in the time series dataset.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property FirstYear() As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of points in the time series dataset. Data points may
    ''' be spaced <see cref="eTSDataSetInterval.Annual">annually</see> or is given for each
    ''' <see cref="eTSDataSetInterval.TimeStep">time step</see>. The size of the 
    ''' interval can be checked via <see cref="TimeSeriesInterval"/>.
    ''' <seealso cref="TimeSeriesInterval"/>
    ''' <seealso cref="eTSDataSetInterval"/>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumPoints() As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a time series from the dataset.
    ''' </summary>
    ''' <param name="iSeries">One-based index of the time series to retrieve.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property TimeSeries(iSeries As Integer) As cTimeSeries
        Get
            Return Me.m_core.EcosimTimeSeries(iSeries)
        End Get
    End Property

    Public Property TimeSeriesInterval As eTSDataSetInterval = eTSDataSetInterval.Annual

#End Region ' Variable via dot(.) operator

#Region " Status Flags via dot(.) operator"

    Public Property DescriptionStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.Description)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Description, value)
        End Set

    End Property

    Public Property AuthorStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.Author)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Author, value)
        End Set

    End Property

    Public Property ContactStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.Description)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Description, value)
        End Set

    End Property

#End Region ' Status Flags via dot(.) operator

#Region " Public interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update all time series in the dataset
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Update()
        'For Each ts As cTimeSeries In Me.m_lTimeSeries
        '    ts.Enabled = True
        'Next
        Me.m_core.UpdateTimeSeries()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns>
    ''' <para>Return values are:</para>
    ''' <list>
    ''' <item><term>True</term><description>All of the time series in the Dataset are applied</description></item>
    ''' <item><term>False</term><description>None of the time series in the Dataset are applied</description></item>
    ''' <item><term>UseDefault</term><description>Some of the time series in the Dataset are applied</description></item>
    ''' </list>
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function IsEnabled() As TriState

        Dim iEnableCount As Integer = 0
        Dim iTot As Integer = Me.nTimeSeries
        For Each ts As cTimeSeries In Me.m_core.EcosimGroupTimeseries
            If (ts IsNot Nothing) Then
                If ts.Enabled Then iEnableCount += 1
            End If
        Next
        For Each ts As cTimeSeries In Me.m_core.EcosimFleetTimeseries
            If (ts IsNot Nothing) Then
                If ts.Enabled Then iEnableCount += 1
            End If
        Next
        If iEnableCount = 0 Then Return TriState.False
        If iEnableCount = iTot Then Return TriState.True
        Return TriState.UseDefault

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the dataset is currently loaded.
    ''' </summary>
    ''' <returns>True if the dataset is currently loaded.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsLoaded() As Boolean
        Return (Me.m_core.ActiveTimeSeriesDatasetIndex = Me.Index)
    End Function

#End Region ' Public interfaces

End Class
