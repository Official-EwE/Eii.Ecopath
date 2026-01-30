' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Namespace MSY

    ''' <summary>
    ''' Parameters to configure an MSY search.
    ''' </summary>
    Public Class cMSYParameters
        Inherits cCoreInputOutputBase

#Region " Private variables "

        Private m_msyData As cMSYDataStructures

#End Region ' Private variables

#Region "Construction Initialization"

        Public Sub New(core As cCore, MSYData As cMSYDataStructures)
            MyBase.New(core)

            Dim val As cValue = Nothing

            Me.m_dataType = eDataTypes.MSYParameters
            Me.m_coreComponent = eCoreComponentType.MSY

            'create and set the status object to this source and OK
            Me.m_ValidationStatus = New cVariableStatus
            Me.m_ValidationStatus.CoreDataObject = Me
            Me.AllowValidation = False

            Me.DBID = cCore.NULL_VALUE

            Me.m_msyData = MSYData

            ' FSelection
            val = New cValue(core, New Integer, eVarNameFlags.MSYFSelection, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' FSelectionMode
            val = New cValue(core, New Integer, eVarNameFlags.MSYFSelectionMode, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' Assessment (frozen pools or not)
            val = New cValue(core, New Boolean, eVarNameFlags.MSYAssessment, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            ' RunLengthMode
            val = New cValue(core, New Integer, eVarNameFlags.MSYRunLengthMode, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' MaxFishingRate
            val = New cValue(core, New Single, eVarNameFlags.MSYMaxFishingRate, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' NumTrialYears
            val = New cValue(core, New Integer, eVarNameFlags.MSYNumTrialYears, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' EquilibriumStepSize
            val = New cValue(core, New Single, eVarNameFlags.MSYEquilibriumStepSize, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

        End Sub

#End Region

#Region " .operators "

        Public Property SelGroupFleetIndex As Integer
            Set(value As Integer)
                Me.m_msyData.iSelGroupFleet = value
            End Set
            Get
                Return Me.m_msyData.iSelGroupFleet
            End Get
        End Property

        Public Property FSelectionMode As eMSYFSelectionModeType
            Get
                Return Me.m_msyData.FSelectionMode
            End Get
            Set(value As eMSYFSelectionModeType)
                Me.m_msyData.FSelectionMode = value
            End Set
        End Property

        Public Property Assessment As eMSYAssessmentTypes
            Set(value As eMSYAssessmentTypes)
                Me.m_msyData.AssessmentType = value
            End Set
            Get
                Return Me.m_msyData.AssessmentType
            End Get
        End Property

        Public Property RunLengthMode As eMSYRunLengthModeTypes
            Get
                Return Me.m_msyData.RunLengthMode
            End Get
            Set(value As eMSYRunLengthModeTypes)
                Me.m_msyData.RunLengthMode = value
            End Set
        End Property

        Public Property MaxFishingRate As Single
            Get
                Return Me.m_msyData.MaxRelF
            End Get
            Set(value As Single)
                Me.m_msyData.MaxRelF = value
            End Set
        End Property

        Public Property NumTrialYears As Integer
            Get
                Return Me.m_msyData.nYearsPerTrial
            End Get
            Set(value As Integer)
                Me.m_msyData.nYearsPerTrial = value
            End Set
        End Property

        Public Property EquilibriumStepSize As Single
            Get
                Return Me.m_msyData.FStepSize
            End Get
            Set(value As Single)
                Me.m_msyData.FStepSize = value
            End Set
        End Property

#End Region ' .operators

    End Class

End Namespace
