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

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cEcospaceHabitat
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Sub New(core As cCore, DBID As Integer)
        MyBase.New(core)

        Me.DBID = DBID
        Me.m_dataType = eDataTypes.EcospaceHabitat
        Me.m_coreComponent = eCoreComponentType.Ecospace

        Dim val As cValue

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' HabAreaProportion
            val = New cValue(core, New Single, eVarNameFlags.HabAreaProportion, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            Me.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceHabitat.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceHabitat. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Properties by dot (.) operator "

    Public Property HabAreaProportion() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.HabAreaProportion))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.HabAreaProportion, value)
        End Set
    End Property

#End Region

#Region "Status by dot (.) operator"

    Public Property HabAreaProportionStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.HabAreaProportion)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.HabAreaProportion, value)
        End Set
    End Property

#End Region

End Class
