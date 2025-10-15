
Partial Class Account_Feedback
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblUserId.Text = UCase(Session("UserId")).ToString()
        If Not IsPostBack Then
            Call MessageError(False, String.Empty)
            Call MessageSuccess(False, String.Empty)
        End If
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/account", False)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Call MessageSuccess(False, String.Empty)
        Try
            txtDescription.BackColor = Drawing.Color.Empty
            If txtDescription.Text = "" Then
                Call MessageError(True, "YOUR FEEDBACK IS REQUIRED !")
                txtDescription.BackColor = Drawing.Color.Red
                txtDescription.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Insert()

                Call MessageSuccess(True, "YOUR FEEDBACK HAS BEEN SUCCESSFULLY RECEIVED !")
                txtDescription.Text = ""
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(lblUserId.Text, Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False
        msgError.InnerText = Msg
        If Show = True Then
            divError.Visible = True
        End If
    End Sub

    Private Sub MessageSuccess(Show As Boolean, Msg As String)
        divSuccess.Visible = False
        msgSuccess.InnerText = Msg
        If Show = True Then
            divSuccess.Visible = True
        End If
    End Sub
End Class
