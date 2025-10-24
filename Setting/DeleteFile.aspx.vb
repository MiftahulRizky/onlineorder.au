Imports System.IO

Partial Class Setting_DeleteFile
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Call DeleteAction()
        End If
    End Sub

    Private Sub DeleteAction()
        Call MessageError(False, String.Empty)
        Try
            Dim dirMail As New DirectoryInfo(Server.MapPath("~/File/Order/Mail/"))
            Call EmptyFolder_Preview(dirMail)

            Dim dirPreview As New DirectoryInfo(Server.MapPath("~/File/Order/Preview/"))
            Call EmptyFolder_Preview(dirPreview)

            Dim dirJobs As New DirectoryInfo(Server.MapPath("~/File/Order/Job/"))
            Call EmptyFolder_Print(dirJobs)

            Dim dirQuote As New DirectoryInfo(Server.MapPath("~/File/Order/Quote/"))
            Call EmptyFolder_Quote(dirQuote)

            If msgError.InnerText = "" Then
                Response.Redirect("~/", False)
                Exit Sub
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub EmptyFolder_Mail(directory As DirectoryInfo)
        For Each file As FileInfo In directory.GetFiles()
            file.Delete()
        Next
    End Sub

    Private Sub EmptyFolder_Preview(directory As DirectoryInfo)
        For Each file As FileInfo In directory.GetFiles()
            file.Delete()
        Next
    End Sub

    Private Sub EmptyFolder_Print(directory As DirectoryInfo)
        For Each file As FileInfo In directory.GetFiles()
            file.Delete()
        Next
    End Sub

    Private Sub EmptyFolder_Quote(directory As DirectoryInfo)
        For Each file As FileInfo In directory.GetFiles()
            file.Delete()
        Next
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
