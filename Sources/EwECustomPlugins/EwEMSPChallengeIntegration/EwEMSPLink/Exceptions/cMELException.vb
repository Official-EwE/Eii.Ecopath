' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' Boink.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cMELException
    Inherits Exception

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new <see cref="cMELException"/>.
    ''' </summary>
    ''' <param name="strDetails">The exception details.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(strDetails As String)
        MyBase.New(strDetails)
        Debug.WriteLine("MEL Exception: " & strDetails)
    End Sub

End Class
