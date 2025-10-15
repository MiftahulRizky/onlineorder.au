Partial Class Setting_Region_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtAliasName.Text = "" Then
                Call MessageError(True, "ALIAS NAME IS REQUIRED !")
                txtAliasName.CssClass = "form-control  is-invalid"
                txtAliasName.Focus()
                Exit Sub
            End If

            If txtFullName.Text = "" Then
                Call MessageError(True, "FULL NAME IS REQUIRED !")
                txtFullName.CssClass = "form-control  is-invalid"
                txtFullName.Focus()
                Exit Sub
            End If

            If ddlOperator.SelectedValue = "" Then
                Call MessageError(True, "OPERATOR IS REQUIRED !")
                ddlOperator.CssClass = "form-select  is-invalid"
                ddlOperator.Focus()
                Exit Sub
            End If

            If ddlActive.SelectedValue = "" Then
                Call MessageError(True, "ACTIVE IS REQUIRED !")
                ddlActive.CssClass = "form-select  is-invalid"
                ddlActive.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT REGION. ALIAS NAME : " & txtAliasName.Text)

                Response.Redirect("~/setting/region", False)
                Exit Sub
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/region", False)
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtAliasName.CssClass = "form-control "
        txtFullName.CssClass = "form-control "
        ddlOperator.CssClass = "form-control "
        txtDescription.CssClass = "form-select"
        ddlActive.CssClass = "form-select "
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        ' If Show = True Then : divError.Visible = True : End If
        If Show = True Then
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
