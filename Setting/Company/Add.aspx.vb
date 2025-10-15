Partial Class Setting_Company_Add
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
            If txtId.Text = "" Then
                Call MessageError(True, "COMPANY CODE IS REQUIRED !")
                txtId.CssClass ="form-control  is-invalid"
                txtId.Focus()
                Exit Sub
            End If

            If txtId.Text = publicCfg.GetItemData("SELECT Id FROM Company WHERE Id = '" + txtId.Text + "'") Then
                Call MessageError(True, "COMPANY CODE ALREADY EXISTS !")
                txtId.CssClass ="form-control  is-invalid"
                txtId.Focus()
                Exit Sub
            End If

            If txtName.Text = "" Then
                Call MessageError(True, "COMPANY NAME IS REQUIRED !")
                txtName.CssClass ="form-control  is-invalid"
                txtName.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT COMPANY. ID : " & txtId.Text)

                Response.Redirect("~/setting/company", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/company", False)
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        txtId.CssClass = "form-control "
        txtName.CssClass = "form-control "
        txtDescription.CssClass = "form-control"
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
