
Partial Class Setting_Other_Email
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/additional", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            BackColor()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If ddlAction.SelectedValue = "" Then
                MessageError(True, "QUERY ACTION IS REQUIRED !")
                ddlAction.BackColor = Drawing.Color.Red
                ddlAction.Focus()
                Exit Sub
            End If

            If txtTo.Text = "" Then
                MessageError(True, "YOUR QUERY IS REQUIRED !")
                txtTo.BackColor = Drawing.Color.Red
                txtTo.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                mailCfg.MailTest(ddlAction.SelectedValue, txtTo.Text.Trim())
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/other", False)
    End Sub

    Private Sub BackColor()
        MessageError(False, String.Empty)

        ddlAction.BackColor = Drawing.Color.Empty
        txtTo.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
