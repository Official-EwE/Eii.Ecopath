'==============================================================================
'
' $Log: cEcoPathGroupOutput.vb,v $
' Revision 1.7  2009/03/11 00:14:28  joeh
' Add PSD calculation
'
' Revision 1.6  2009/03/06 00:47:56  joeh
' Add Ecopath output data (Weight, Number, Biomass) over time
'
' Revision 1.5  2009/03/03 01:42:55  joeh
' Tcatch no longer has input and output pair
'
' Revision 1.4  2009/03/02 20:09:36  joeh
' VBK no longer has input and output pair
'
' Revision 1.3  2009/02/28 00:17:51  joeh
' Added PSD foundation
'
' Revision 1.2  2009/01/16 18:30:15  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:18  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.50  2008/07/02 01:55:23  jeroens
' Added option to force status flag total reset (fixes bug 503)
'
' Revision 1.49  2008/05/29 22:22:42  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.48  2008/03/06 02:37:43  jeroens
' Fixed enum names
'
' Revision 1.47  2008/03/02 15:19:14  jeroens
' Fixed issue 435
'
' Revision 1.46  2008/02/18 16:01:01  jeroens
' Fixed null flag testing
'
' Revision 1.45  2008/01/11 10:16:25  jeroens
' cEcopathGroupOutput.SetOutputStatus discontinued; instead performed by ResetStatusFlags
'
' Revision 1.44  2008/01/10 12:04:23  jeroens
' Reinstated NULL check
'
' Revision 1.43  2007/09/13 15:50:02  joeb
' SetNullFlag added test for Null_Value to if bZeroOnly = True
'
' Revision 1.42  2007/09/10 01:41:51  jeroens
' * Allowed negative values (fixed bugs 165, 166, 180)
'
' Revision 1.41  2007/08/07 16:34:50  jeroens
' + BiomAccumRate now propery represented
' * SetNullFlag able to handle explicit =0 nulls
'
' Revision 1.40  2007/06/23 00:27:06  jeroens
' * Mortality coefficients no longer flagged as null when negative; negative values are valid for this variable
'
' Revision 1.39  2007/05/22 13:25:37  jeroens
' * Nitty-gritty
'
' Revision 1.38  2007/05/18 01:52:48  jeroens
' * Renamed isReadOnly protected var
'
' Revision 1.37  2007/05/04 15:31:34  jeroens
' + Uses invalid messages source to prevent unwanted updates
'
' Revision 1.36  2007/03/28 01:16:31  jeroens
' * Changed all status modification access from Public to Friend
'
' Revision 1.35  2007/01/19 18:31:08  joeb
' Changes to cValueArray constructor
'
' Revision 1.34  2007/01/19 00:49:51  joeb
' Changes to cValueArray Constructor
'
' Revision 1.33  2006/09/29 18:17:36  joeb
' Change setting of EE Status Flag
'
' Revision 1.32  2006/09/29 17:38:50  joeb
' Setting of status flags to null for zero values
'
' Revision 1.31  2006/09/29 02:51:06  jeroens
' * BiomassArea split into input and output
'
' Revision 1.30  2006/09/21 01:00:23  jeroens
' * Updated to cCoreGroupBase
'
' Revision 1.29  2006/08/22 19:03:05  joeb
' Renaming of Input and Output objects
'
' Revision 1.28  2006/08/22 04:11:07  jeroens
' + Exposed a few more Ecopath output variables, once again
' + Borrowed Alpha from Ecoranger
'
' Revision 1.27  2006/08/20 02:07:27  jeroens
' * Strict On
' + Exposed a few more variables
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcoPathGroupOutput
    Inherits cCoreGroupBase

    Private m_nGroups As Integer
    Private m_pathData As cEcopathDataStructures
    Private m_coreData As New Dictionary(Of eVarNameFlags, IResultsWrapper)

    ' m_Area = 'A()
    ' m_Biomass = 'B()
    ' m_BiomassArea = 'BH()  Biomass/Area in t/km2 
    ' m_BioAccum = 'BA()  Biomass Accumulation 
    ' m_PB = ' PB() Production/Biomass
    ' m_QB = 'QB() consumption/biomass
    ' m_EE =
    ' m_GE = GE() 'Production/Consumption
    ' m_GS =  GS()'Unassimilated food
    ' m_DetImport = 
    ' m_predmort() = 

#Region "Functionality specific to this class"

    Private Enum eNullTestTypes As Integer
        ''' <summary>Value is not allowed to be 0 or core Null.</summary>
        NonZero
        ''' <summary>Value must be larger than 0 (and not core Null).</summary>
        GreaterThanZero
        ''' <summary>Value must not be core Null.</summary>
        NonCoreNull
    End Enum

    ''' <summary>
    ''' Set the status flag of this variable to NULL if it is less than zero
    ''' </summary>
    ''' <param name="varName">Name of the variable that will get the status flag set</param>
    ''' <param name="sValueToTest">
    ''' <para>Value of the variable to test.</para>
    ''' <para>The value is passed in so that the calling method can use either the core data structures or the output object. 
    ''' If just the eVarNameFlags is used then only the getVariable() method can be used to retrieve the value.</para>
    ''' </param>
    ''' <param name="Index">Optional variable index.</param>
    ''' <param name="nullTest">Flag stating how to test for NULL values.</param>
    Private Sub SetNullFlag(ByVal varName As eVarNameFlags, ByVal sValueToTest As Single, _
            Optional ByVal Index As Integer = -9999, Optional ByVal nullTest As eNullTestTypes = eNullTestTypes.GreaterThanZero)

        Dim bIsNull As Boolean = False

        Select Case nullTest
            Case eNullTestTypes.NonZero
                'jb added test for NULL_VALUE
                bIsNull = (sValueToTest = 0.0!) Or (sValueToTest = cCore.NULL_VALUE)
            Case eNullTestTypes.GreaterThanZero
                bIsNull = (sValueToTest <= 0.0!)
            Case eNullTestTypes.NonCoreNull
                bIsNull = (sValueToTest = cCore.NULL_VALUE)
        End Select

        Try
            If bIsNull Then
                Me.SetStatusFlags(varName, eStatusFlags.Null, Index)
            Else
                Me.ClearStatusFlags(varName, eStatusFlags.Null, Index)
            End If
        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub


#End Region

#Region "Must Override Methods"

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim sg As cStanzaGroup = Nothing

        MyBase.ResetStatusFlags(bForceReset)

        Try

            'Set the Status Flags to ValueComputed for input/output pairs 
            'if the modeled value is different than the input value.
            'The original data structure is needed to perform this.
            If m_core.m_EcoPathData.EE(Me.Index) <> m_core.m_EcoPathData.EEinput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.EEOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.EEOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.EEOutput, m_core.m_EcoPathData.EE(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            If m_core.m_EcoPathData.PB(Me.Index) <> m_core.m_EcoPathData.PBinput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.PBOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.PBOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.PBOutput, m_core.m_EcoPathData.PB(Me.Index))

            If m_core.m_EcoPathData.QB(Me.Index) <> m_core.m_EcoPathData.QBinput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.QBOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.QBOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.QBOutput, m_core.m_EcoPathData.QB(Me.Index))

            If m_core.m_EcoPathData.GE(Me.Index) <> m_core.m_EcoPathData.GEinput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.GEOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.GEOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.GEOutput, m_core.m_EcoPathData.GE(Me.Index))

            If m_core.m_EcoPathData.B(Me.Index) <> m_core.m_EcoPathData.Binput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.Biomass, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.Biomass, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.Biomass, m_core.m_EcoPathData.B(Me.Index))

            If m_core.m_EcoPathData.BH(Me.Index) <> m_core.m_EcoPathData.BHinput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.BiomassAreaOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.BiomassAreaOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.BiomassAreaOutput, m_core.m_EcoPathData.BH(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'Joeh
            'A in LW
            If m_core.m_EcoPathData.AinLW(Me.Index) <> m_core.m_EcoPathData.AinLWInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.AinLWOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.AinLWOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.AinLWOutput, m_core.m_EcoPathData.AinLW(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'B in LW
            If m_core.m_EcoPathData.BinLW(Me.Index) <> m_core.m_EcoPathData.BinLWInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.BinLWOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.BinLWOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.BinLWOutput, m_core.m_EcoPathData.BinLW(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'Loo 
            If m_core.m_EcoPathData.Loo(Me.Index) <> m_core.m_EcoPathData.LooInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.LooOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.LooOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.LooOutput, m_core.m_EcoPathData.Loo(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'Winf 
            If m_core.m_EcoPathData.Winf(Me.Index) <> m_core.m_EcoPathData.WinfInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.WinfOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.WinfOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.WinfOutput, m_core.m_EcoPathData.Winf(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            't0
            If m_core.m_EcoPathData.t0(Me.Index) <> m_core.m_EcoPathData.t0Input(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.t0Output, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.t0Output, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.t0Output, m_core.m_EcoPathData.t0(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'Tmax
            If m_core.m_EcoPathData.Tmax(Me.Index) <> m_core.m_EcoPathData.TmaxInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.TmaxOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.TmaxOutput, eStatusFlags.ValueComputed)
            End If
            SetNullFlag(eVarNameFlags.TmaxOutput, m_core.m_EcoPathData.Tmax(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)
            'End Joeh

            'test for NULL values in other variables
            SetNullFlag(eVarNameFlags.BioAccum, m_core.m_EcoPathData.BA(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonZero)
            SetNullFlag(eVarNameFlags.BioAccumRatePerYear, Me.BioAccumRatePerYear, cCore.NULL_VALUE, eNullTestTypes.NonZero)

            SetNullFlag(eVarNameFlags.MortCoBioAcumRate, Me.MortCoBioAcumRate, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            SetNullFlag(eVarNameFlags.MortCoFishRate, Me.MortCoFishRate, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            SetNullFlag(eVarNameFlags.MortCoNetMig, Me.MortCoNetMig)
            ' This value can be negative
            SetNullFlag(eVarNameFlags.MortCoOtherMort, Me.MortCoOtherMort, cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)
            SetNullFlag(eVarNameFlags.MortCoPB, Me.MortCoPB)
            SetNullFlag(eVarNameFlags.MortCoPredMort, Me.MortCoPredMort)

            ' Key indices
            SetNullFlag(eVarNameFlags.NetMigration, Me.NetMigration, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            SetNullFlag(eVarNameFlags.FlowToDet, Me.FlowToDet, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            SetNullFlag(eVarNameFlags.NetEfficiency, Me.NetEfficiency, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            SetNullFlag(eVarNameFlags.OmnivoryIndex, Me.OmnivoryIndex, cCore.NULL_VALUE, eNullTestTypes.NonZero)

            SetNullFlag(eVarNameFlags.Assimilation, Me.Assimilation)
            SetNullFlag(eVarNameFlags.Respiration, Me.Respiration)
            SetNullFlag(eVarNameFlags.RespAssim, Me.RespAssim, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            SetNullFlag(eVarNameFlags.ProdResp, Me.ProdResp)
            SetNullFlag(eVarNameFlags.RespBiom, Me.RespBiom)

            For i As Integer = 1 To m_nGroups
                SetNullFlag(eVarNameFlags.Consumption, Me.Consumption(i), i)
                SetNullFlag(eVarNameFlags.PredMort, Me.PredMort(i), i, eNullTestTypes.NonZero)
                SetNullFlag(eVarNameFlags.SearchRate, Me.SearchRate(i), i, eNullTestTypes.NonZero)

                ' Set highlight on cannibalism cells (fixes bug 435)
                If i = Me.Index Then
                    Me.SetStatusFlags(eVarNameFlags.PredMort, eStatusFlags.CoreHighlight, i)
                End If
            Next

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Function

#End Region

#Region "Construction and Initialization"

    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue

        'default is readonly
        m_bReadOnly = True
        AllowValidation = False

        'get the number of groups from the core delegate
        m_nGroups = m_core.GetCoreCounter(eCoreCounterTypes.nGroups)
        m_dataType = eDataTypes.EcoPathGroupOutput

        ' Outputs should never send out messages
        m_coreComponent = eCoreComponentType.NotSet

        'default OK status used for SetVariable
        'see comment SetVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoPathGroupOutput, _
                                        eCoreComponentType.EcoPath, Index, cCore.NULL_VALUE)

        Me.DBID = DBID

        val = New cValue(New Single, eVarNameFlags.EEOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.PBOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.QBOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.GEOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.Area, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.BioAccum, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.Biomass, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.BiomassAreaOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'jb June-13-06 Added to ouputs
        val = New cValue(New Single, eVarNameFlags.BioAccumRatePerYear, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.DetImp, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.GS, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.TTLX, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.ImportedConsumption, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'mortality
        val = New cValue(New Single, eVarNameFlags.MortCoPB, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.MortCoFishRate, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.MortCoPredMort, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.MortCoBioAcumRate, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.MortCoNetMig, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.MortCoOtherMort, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.NetMigration, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.FlowToDet, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.NetEfficiency, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.OmnivoryIndex, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.Respiration, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.Assimilation, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.ProdResp, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.RespAssim, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.RespBiom, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'arrayed values
        'val will contain an array of nGroup elements 
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.Consumption, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.PredMort, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.SearchRate, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.Hlap, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.Plap, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.Alpha, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        'Joeh
        val = New cValue(New Single, eVarNameFlags.VBK, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.Tcatch, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.AinLWOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.BinLWOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.LooOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.WinfOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.t0Output, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)
        val = New cValue(New Single, eVarNameFlags.TmaxOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcopathWeight, eStatusFlags.NotEditable, eCoreCounterTypes.nEcopathTimeSteps, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcopathNumber, eStatusFlags.NotEditable, eCoreCounterTypes.nEcopathTimeSteps, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcopathBiomass, eStatusFlags.NotEditable, eCoreCounterTypes.nEcopathTimeSteps, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.PSD, eStatusFlags.NotEditable, eCoreCounterTypes.nWeightClasses, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)
        'End Joeh

    End Sub

#End Region

#Region "Variables as Public Properties Via dot(.) operator"

    Public Property Area() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Area))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Area, newValue)
            End If
        End Set

    End Property

    Public Property Biomass() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.Biomass))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Biomass, newValue)
            End If
        End Set

    End Property

    Public Property BiomassArea() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.BiomassAreaOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.BiomassAreaOutput, newValue)
            End If
        End Set

    End Property

    Public Property BioAccum() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.BioAccum))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.BioAccum, newValue)
            End If
        End Set

    End Property

    Public Property BioAccumRatePerYear() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.BioAccumRatePerYear))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.BioAccumRatePerYear, newValue)
            End If
        End Set

    End Property

    Public Property QBOutput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.QBOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.QBOutput, newValue)
            End If
        End Set

    End Property

    Public Property PBOutput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.PBOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.PBOutput, newValue)
            End If
        End Set

    End Property

    Public Property EEOutput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EEOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.EEOutput, newValue)
            End If
        End Set

    End Property

    Public Property GEOutput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.GEOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.GEOutput, newValue)
            End If
        End Set

    End Property

    'Joeh
    Public Property VBK() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.VBK))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.VBK, newValue)
            End If
        End Set
    End Property

    Public Property Tcatch() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Tcatch))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Tcatch, newValue)
            End If
        End Set

    End Property

    Public Property AinLWOutput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.AinLWOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.AinLWOutput, newValue)
            End If
        End Set

    End Property

    Public Property BinLWOutput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.BinLWOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.BinLWOutput, newValue)
            End If
        End Set

    End Property

    Public Property LooOutput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.LooOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.LooOutput, newValue)
            End If
        End Set

    End Property

    Public Property WinfOutput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.WinfOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.WinfOutput, newValue)
            End If
        End Set

    End Property

    Public Property t0Output() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.t0Output))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.t0Output, newValue)
            End If
        End Set

    End Property

    Public Property TmaxOutput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.TmaxOutput))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.TmaxOutput, newValue)
            End If
        End Set

    End Property

    Public Property EcopathWeight(ByVal iTimeStep As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathWeight, iTimeStep))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.EcopathWeight, newValue, iTimeStep)
            End If
        End Set
    End Property

    Public Property EcopathWeight() As Single()
        Get
            Return DirectCast(GetVariable(eVarNameFlags.EcopathWeight), Single())
        End Get
        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.EcopathWeight, newValue)
            End If
        End Set
    End Property

    Public Property EcopathNumber(ByVal iTimeStep As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathNumber, iTimeStep))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.EcopathNumber, newValue, iTimeStep)
            End If
        End Set
    End Property

    Public Property EcopathNumber() As Single()
        Get
            Return DirectCast(GetVariable(eVarNameFlags.EcopathNumber), Single())
        End Get
        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.EcopathNumber, newValue)
            End If
        End Set
    End Property

    Public Property EcopathBiomass(ByVal iTimeStep As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathBiomass, iTimeStep))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.EcopathBiomass, newValue, iTimeStep)
            End If
        End Set
    End Property

    Public Property EcopathBiomass() As Single()
        Get
            Return DirectCast(GetVariable(eVarNameFlags.EcopathBiomass), Single())
        End Get
        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.EcopathBiomass, newValue)
            End If
        End Set
    End Property

    Public Property PSD(ByVal iWeightClass As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.PSD, iWeightClass))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.PSD, newValue, iWeightClass)
            End If
        End Set
    End Property

    Public Property PSD() As Single()
        Get
            Return DirectCast(GetVariable(eVarNameFlags.PSD), Single())
        End Get
        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.PSD, newValue)
            End If
        End Set
    End Property
    'End Joeh

    Public Property GS() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.GS))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.GS, newValue)
            End If
        End Set
    End Property

    Public Property TTLX() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.TTLX))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.TTLX, newValue)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Predation mortality on this group caused by this ipred
    ''' </summary>
    ''' <param name="iPred">iPredator group </param>
    ''' <value>Returns predation mortality on this group caused by this iPredator</value>
    ''' <remarks>
    ''' B(pred) * QB(pred) * DC(pred, prey) / B(prey) 
    '''</remarks>
    Public Property PredMort(ByVal iPred As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.PredMort, iPred))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.PredMort, newValue, iPred)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Predation mortality array
    ''' </summary>
    ''' <value>Returns an array of predation mortalities for this group</value>
    ''' <remarks> B(pred) * QB(pred) * DC(pred, prey) / B(prey) </remarks>
    Public Property PredMort() As Single()
        Get
            Return DirectCast(GetVariable(eVarNameFlags.PredMort), Single())
        End Get

        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.PredMort, newValue)
            End If
        End Set
    End Property

    ''' <summary> PB(iGroup) </summary>
    Public Property MortCoPB() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MortCoPB))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MortCoPB, newValue)
            End If
        End Set
    End Property

    ''' <summary> Catch(i) / B(i) </summary>
    Public Property MortCoFishRate() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MortCoFishRate))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MortCoFishRate, newValue)
            End If
        End Set
    End Property

    ''' <summary> M2(i) </summary>
    Public Property MortCoPredMort() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MortCoPredMort))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MortCoPredMort, newValue)
            End If
        End Set

    End Property

    Public Property MortCoOtherMort() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MortCoOtherMort))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MortCoOtherMort, newValue)
            End If
        End Set

    End Property

    ''' <summary> BA(i) / B(i) </summary>
    Public Property MortCoBioAcumRate() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MortCoBioAcumRate))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MortCoBioAcumRate, newValue)
            End If
        End Set

    End Property

    ''' <summary> (Emigration(i) - Immig(i)) / B(i) </summary>
    Public Property MortCoNetMig() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MortCoNetMig))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MortCoNetMig, newValue)
            End If
        End Set

    End Property



    Public Property Consumption() As Single()

        Get
            Return DirectCast(GetVariable(eVarNameFlags.Consumption), Single())
        End Get

        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Consumption, newValue)
            End If
        End Set

    End Property

    Public Property Consumption(ByVal iGroup As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.Consumption, iGroup))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Consumption, newValue, iGroup)
            End If
        End Set
    End Property

    Public Property ImportedConsumption() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.ImportedConsumption))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.ImportedConsumption, newValue)
            End If
        End Set

    End Property

    Public Property NetMigration() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.NetMigration))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.NetMigration, newValue)
            End If
        End Set
    End Property

    Public Property FlowToDet() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.FlowToDet))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.FlowToDet, newValue)
            End If
        End Set
    End Property

    Public Property NetEfficiency() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.NetEfficiency))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.NetEfficiency, newValue)
            End If
        End Set
    End Property

    Public Property OmnivoryIndex() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.OmnivoryIndex))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.OmnivoryIndex, newValue)
            End If
        End Set
    End Property

    Public Property Respiration() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Respiration))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Respiration, newValue)
            End If
        End Set
    End Property

    Public Property Assimilation() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Assimilation))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Assimilation, newValue)
            End If
        End Set
    End Property

    Public Property RespAssim() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.RespAssim))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.RespAssim, newValue)
            End If
        End Set
    End Property

    Public Property ProdResp() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.ProdResp))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.ProdResp, newValue)
            End If
        End Set
    End Property

    Public Property RespBiom() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.RespBiom))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.RespBiom, newValue)
            End If
        End Set
    End Property

    Public Property SearchRate(ByVal iPred As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.SearchRate, iPred))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.SearchRate, newValue, iPred)
            End If
        End Set
    End Property

    Public Property SearchRate() As Single()
        Get
            Return DirectCast(GetVariable(eVarNameFlags.SearchRate), Single())
        End Get
        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.SearchRate, newValue)
            End If
        End Set
    End Property

    Public Property Hlap(ByVal iPred As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Hlap, iPred))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Hlap, newValue, iPred)
            End If
        End Set
    End Property

    Public Property Hlap() As Single()
        Get
            Return DirectCast(GetVariable(eVarNameFlags.Hlap), Single())
        End Get
        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Hlap, newValue)
            End If
        End Set
    End Property

    Public Property Plap(ByVal iPred As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Plap, iPred))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Plap, newValue, iPred)
            End If
        End Set
    End Property

    Public Property Plap() As Single()
        Get
            Return DirectCast(GetVariable(eVarNameFlags.Plap), Single())
        End Get
        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Plap, newValue)
            End If
        End Set
    End Property

    Public Property Alpha(ByVal iPred As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Alpha, iPred))
        End Get
        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Alpha, newValue, iPred)
            End If
        End Set
    End Property

    Public Property Alpha() As Single()
        Get
            Return DirectCast(GetVariable(eVarNameFlags.Alpha), Single())
        End Get
        Set(ByVal newValue As Single())
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.Alpha, newValue)
            End If
        End Set
    End Property

#End Region

#Region "Status Flags Via dot (.) operator"

    Public Property AreaStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Area)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Area, value)
        End Set

    End Property

    Public Property BiomassAccumStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.BioAccum)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.BioAccum, value)
        End Set

    End Property

    Public Property BiomassStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Biomass)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Biomass, value)
        End Set

    End Property

    Public Property BiomassAreaStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.BiomassAreaOutput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.BiomassAreaOutput, value)
        End Set

    End Property

    Public Property EEOutputStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.EEOutput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EEOutput, value)
        End Set

    End Property

    Public Property GEOutputStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.GEOutput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.GEOutput, value)
        End Set

    End Property

    Public Property GSStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.GS)

        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.GS, value)
        End Set

    End Property

    Public Property ImportedConsumptionStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.ImportedConsumption)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.ImportedConsumption, value)
        End Set

    End Property

    Public Property MortCoBioAcumRateStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.MortCoBioAcumRate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MortCoBioAcumRate, value)
        End Set

    End Property

    Public Property MortCoFishRateStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.MortCoFishRate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MortCoFishRate, value)
        End Set


    End Property

    Public Property MortCoNetMigStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.MortCoNetMig)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MortCoNetMig, value)
        End Set

    End Property

    Public Property MortCoOtherMortStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.MortCoOtherMort)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MortCoOtherMort, value)
        End Set

    End Property

    Public Property MostCoPBStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.MortCoPB)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MortCoPB, value)
        End Set

    End Property

    Public Property MostCoPredMortStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.MortCoPredMort)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MortCoPredMort, value)
        End Set

    End Property

    Public Property PBOutputStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.PBOutput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.PBInput, value)
        End Set

    End Property

    Public Property QBStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.QBOutput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.QBOutput, value)
        End Set

    End Property

    Public Property TTLXStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.TTLX)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.TTLX, value)
        End Set

    End Property

    Public Property PredMortStatus(ByVal iPred As Integer) As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.PredMort)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.PredMort, value)
        End Set

    End Property

    Public Property NetMigrationStatus(ByVal iGroup As Integer) As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.NetMigration)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.NetMigration, value)
        End Set

    End Property

    Public Property FlowToDetStatus(ByVal iGroup As Integer) As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.FlowToDet)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.FlowToDet, value)
        End Set

    End Property

    Public Property NetEfficiencyStatus(ByVal iGroup As Integer) As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.NetEfficiency)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.NetEfficiency, value)
        End Set

    End Property

    Public Property OmnivoryIndexStatus(ByVal iGroup As Integer) As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.OmnivoryIndex)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.OmnivoryIndex, value)
        End Set

    End Property

    Public Property RespirationStatus(ByVal iGroup As Integer) As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Respiration)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Respiration, value)
        End Set

    End Property

    Public Property AssimilationStatus(ByVal iGroup As Integer) As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Assimilation)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Assimilation, value)
        End Set

    End Property

    Public Property SearchRateStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.SearchRate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.SearchRate, value)
        End Set

    End Property

    Public Property SearchRateStatus(ByVal iPred As Integer) As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.SearchRate, iPred)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.SearchRate, value, iPred)
        End Set

    End Property

#End Region

End Class


