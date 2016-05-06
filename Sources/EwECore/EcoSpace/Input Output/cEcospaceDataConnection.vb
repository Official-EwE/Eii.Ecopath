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
' Copyright 1991-2013 UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceDataConnection
    Inherits cCoreInputOutputBase

#Region " Constructor "

    Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Try

            Me.m_dataType = eDataTypes.EcospaceDataConnection
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.DBID = iDBID

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceDataConnection.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceDataConnection. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructor

#Region " Properties by dot (.) operator "

    ' Index will be the # of the connection within the adapter
    ' Here to put the dataset, converter, varname, scale, scale type
    ' Also add link to adapter, way to check compatibility of dataset, and IsConfigured

#End Region ' Properties by dot (.) operator

#Region " Status by dot (.) operator "

#End Region ' Status by dot (.) operator 

End Class
