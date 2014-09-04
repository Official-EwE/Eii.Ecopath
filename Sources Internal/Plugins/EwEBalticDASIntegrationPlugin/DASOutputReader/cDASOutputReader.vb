' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Class that performs the actual file generation.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cDASOutputReader

    Public Class cDatapoint

        Private m_values As Single()

        Public Sub New(nVariables As Integer)
            ReDim Me.m_values(nVariables)
        End Sub

        Public Property Lat As Single
        Public Property Lon As Single

        Public Function Values() As Single()
            Return Me.m_values
        End Function

    End Class

    Private m_lVariables As New List(Of String)
    Private m_lDataPoints As New List(Of cDatapoint)

    Public Sub New()

    End Sub

    Public Function Load(ByVal strFile As String) As Boolean

        Me.m_lVariables.Clear()
        Me.m_lDataPoints.Clear()

        Dim reader As New StreamReader(strFile)

        Return True

    End Function

    Public ReadOnly Property Variables As String()
        Get
            Return Me.m_lVariables.ToArray()
        End Get
    End Property

End Class
