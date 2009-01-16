'==============================================================================
'
' $Log: cEcosimStats.vb,v $
' Revision 1.2  2009/01/16 18:30:17  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:20  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/07/02 01:55:25  jeroens
' Added option to force status flag total reset (fixes bug 503)
'
' Revision 1.4  2008/06/20 19:43:19  joeb
' Added SSGroup to EcosimStats
'
' Revision 1.3  2008/05/29 22:22:43  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.2  2007/06/22 16:09:06  joeb
' Fixed CVS log
'
' Revision 1.1  2007/06/22 16:06:07  joeb
' Added cEcosimStats file
'
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcosimStats
    Inherits cCoreInputOutputBase

    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Me.DBID = DBID
        m_dataType = eDataTypes.EcospaceGroup
        m_coreComponent = eCoreComponentType.EcoSpace

        Dim val As cValue

        Try

            m_dataType = eDataTypes.EcospaceGroup
            m_coreComponent = eCoreComponentType.EcoSpace

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimGroupInput, eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)
            'SS
            val = New cValue(New Single, eVarNameFlags.EcosimSS, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            m_values.Add(val.varName, val)

            val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimSSGroup, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, _
                         AddressOf m_core.GetCoreCounter)
            m_values.Add(val.varName, val)



            'set status flags to their default values
            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceGroup.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceGroup. Error: " & ex.Message)
        End Try

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        'tell the base class to do the default values
        MyBase.ResetStatusFlags(bForceReset)

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.PointArray, eValueTypes.BoolArray
                        For i = 0 To value.Length
                            value.Status(i) = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        Next i

                    Case eValueTypes.Sng, eValueTypes.Int
                        value.Status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed

                End Select
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function


    Public Property SS() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimSS))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimSS, value)
        End Set
    End Property


    Public Property SSGroup(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimSSGroup, iGroup))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimSSGroup, value, iGroup)
        End Set
    End Property

    Public Property SSStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimSS)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimSS, value)
        End Set
    End Property


    Public Property SSGroupStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimSS, iGroup)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimSS, value, iGroup)
        End Set
    End Property
End Class
