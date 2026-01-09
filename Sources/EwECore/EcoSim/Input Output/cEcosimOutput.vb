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



Public Class cEcosimOutput
    Inherits cCoreInputOutputBase

    Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)

        Me.AllowValidation = False

        Me.DBID = cCore.NULL_VALUE
        Me.m_dataType = eDataTypes.EcosimOutput
        Me.m_coreComponent = eCoreComponentType.Ecosim

    End Sub

    ''' <summary>
    ''' Get/set the fishing in-balance (FIB) index.
    ''' </summary>
    Public ReadOnly Property FIB(iTimeStep As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.FIB, iTimeStep))
        End Get
    End Property

    Public ReadOnly Property FIBStatus(iTimeStep As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.FIB, iTimeStep)
        End Get
    End Property

    Public ReadOnly Property TLCatch(iTimeStep As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TLCatch, iTimeStep))
        End Get
    End Property

    Public ReadOnly Property TLCatchStatus(iTimeStep As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.TLCatch, iTimeStep)
        End Get
    End Property

    Public ReadOnly Property TotalCatch(iTimeStep As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TotalCatch, iTimeStep))
        End Get
    End Property

    Public ReadOnly Property TotalCatchStatus(iTimeStep As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.TotalCatch, iTimeStep)
        End Get
    End Property

    Public ReadOnly Property KemptonsQ(iTimeStep As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.KemptonsQ, iTimeStep))
        End Get
    End Property

    Public ReadOnly Property KemptonsQStatus(iTimeStep As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.KemptonsQ, iTimeStep)
        End Get
    End Property

    Public ReadOnly Property ShannonDiversity(iTimeStep As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.ShannonDiversity, iTimeStep))
        End Get
    End Property

    Public ReadOnly Property ShannonDiversityStatus(iTimeStep As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.ShannonDiversity, iTimeStep)
        End Get
    End Property

    Public ReadOnly Property DiversityIndex(iTimeStep As Integer) As Single
        Get
            Select Case Me.m_core.m_EcopathData.DiversityIndexType
                Case eDiversityIndexType.Shannon
                    Return Me.ShannonDiversity(iTimeStep)
                Case eDiversityIndexType.KemptonsQ
                    Return Me.KemptonsQ(iTimeStep)
                Case Else
                    Debug.Assert(False, "Diversity index type not supported")
            End Select
            Return cCore.NULL_VALUE
        End Get
    End Property

    Public ReadOnly Property DiversityIndexStatus(iTimeStep As Integer) As eStatusFlags
        Get
            Select Case Me.m_core.m_EcopathData.DiversityIndexType
                Case eDiversityIndexType.Shannon
                    Return Me.ShannonDiversityStatus(iTimeStep)
                Case eDiversityIndexType.KemptonsQ
                    Return Me.KemptonsQStatus(iTimeStep)
                Case Else
                    Debug.Assert(False, "Diversity index type not supported")
            End Select
            Return eStatusFlags.Null
        End Get
    End Property


    Public Overrides Function GetVariable(VarName As eVarNameFlags,
                                          Optional iIndex As Integer = -9999,
                                          Optional iIndex2 As Integer = -9999,
                                          Optional iIndex3 As Integer = -9999) As Object

        Try

            Select Case VarName
                Case eVarNameFlags.FIB
                    Return Me.m_core.m_EcoSimData.FIB(iIndex)
                Case eVarNameFlags.TLCatch
                    Return Me.m_core.m_EcoSimData.TLC(iIndex)
                Case eVarNameFlags.KemptonsQ
                    Return Me.m_core.m_EcoSimData.Kemptons(iIndex)
                Case eVarNameFlags.ShannonDiversity
                    Return Me.m_core.m_EcoSimData.ShannonDiversity(iIndex)
                Case eVarNameFlags.TotalCatch
                    Return Me.m_core.m_EcoSimData.CatchSim(iIndex)

            End Select

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return cCore.NULL_VALUE

    End Function

    Public Overrides Function GetStatus(VarName As eVarNameFlags, Optional iIndex As Integer = -9999, Optional iThirdIndex As Integer = -9999) As eStatusFlags
        Return eStatusFlags.NotEditable And eStatusFlags.ValueComputed
    End Function

End Class
