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

Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.09:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Habitat capacity calculation type made per group.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_09
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500009!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added capacity calculation types per group"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        If Not db.Execute("ALTER TABLE EcospaceScenarioGroup ADD COLUMN CapacityCalType SHORT") Then
            Return False
        End If

        Dim reader As IDataReader = db.GetReader("SELECT * FROM EcospaceScenario")
        Dim ct As eEcospaceCapacityCalType = eEcospaceCapacityCalType.Habitat
        Dim bSuccess As Boolean = True
        Dim iScenarioID As Integer = 0
        Dim bHasHabitats As Boolean = False

        If (reader IsNot Nothing) Then
            While reader.Read

                iScenarioID = CInt(reader("ScenarioID"))
                bHasHabitats = (CInt(db.GetValue(String.Format("SELECT COUNT(*) FROM EcospaceScenarioHabitat WHERE ScenarioID={0}", iScenarioID), 0)) > 0)

                ' Assume that new model uses env responses
                ct = eEcospaceCapacityCalType.Habitat

                ' Unless capacity calculation is set to habitats AND there are habitats defined for this scenario
                If (CInt(db.ReadSafe(reader, "CapacityCalType", 0)) = 1) Then
                    If bHasHabitats Then
                        ct = eEcospaceCapacityCalType.Both
                    Else
                        ct = eEcospaceCapacityCalType.EnvResponses
                    End If
                End If

                bSuccess = bSuccess And db.Execute(String.Format("UPDATE EcospaceScenarioGroup SET CapacityCalType={0} WHERE ScenarioID={1}", CInt(ct), iScenarioID))
            End While
            db.ReleaseReader(reader)
        End If

        If bSuccess Then
            bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario DROP COLUMN CapacityCalType")
        End If
        Return bSuccess

    End Function

End Class
