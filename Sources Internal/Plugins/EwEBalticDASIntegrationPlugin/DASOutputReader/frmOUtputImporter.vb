' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Main (and only) interface to the DAS region file generator utility thing bit.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmOutputImporter

#Region " Private vars "

    Private m_uic As cUIContext = Nothing

#End Region 'Private vars

#Region " Construction "

    Public Sub New(ByVal uic As cUIContext)
        Me.InitializeComponent()
        Me.m_uic = uic
    End Sub

#End Region ' Construction

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        ' Sanity check
        If (Me.m_uic Is Nothing) Then Return

        Dim core As cCore = Me.m_uic.Core

        ' Initialize enabled state of UI elements
        Me.UpdateControls()

        Me.CenterToScreen()

    End Sub

    Protected Overrides Sub OnClosed(e As System.EventArgs)

        My.Settings.Save()

        ' Bye
        MyBase.OnClosed(e)

    End Sub

#End Region 'Form overrides

#Region " Control events "


#End Region ' Control events

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state of UI elements.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Visit one of the sponsors.
    ''' </summary>
    ''' <param name="strURL"></param>
    ''' -----------------------------------------------------------------------
    Private Sub VisitSponsor(ByVal strURL As String)

        ' Use EwE UI command system
        Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
        Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
        cmd.Invoke(strURL)

    End Sub

#End Region ' Internals

End Class