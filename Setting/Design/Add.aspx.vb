Partial Class Setting_Design_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/design", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()

            Call BindDataCompany()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtName.Text = "" Then
                Call MessageError(True, "DESIGN NAME IS REQUIRED !")
                txtName.CssClass = "form-control  is-invalid"
                txtName.Focus()
                Exit Sub
            End If

            If ddlCompany.SelectedValue = "" Then
                Call MessageError(True, "COMPANY IS REQUIRED !")
                ddlCompany.CssClass = "form-select  is-invalid"
                ddlCompany.Focus()
                Exit Sub
            End If

            If txtPage.Text = "" Then
                Call MessageError(True, "PAGE IS REQUIRED !")
                txtPage.CssClass = "form-control  is-invalid"
                txtPage.Focus()
                Exit Sub
            End If

            If ddlActive.SelectedValue = "" Then
                Call MessageError(True, "DESIGN ACTIVE IS REQUIRED !")
                ddlActive.CssClass = "form-select  is-invalid"
                ddlActive.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT NEW DESIGN TYPE. NAME : " & txtName.Text)

                Response.Redirect("~/setting/design", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/design", False)
    End Sub

    Private Sub BindDataCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = publicCfg.GetListData("SELECT * FROM Company ORDER BY Name ASC")
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()
            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtName.CssClass ="form-control "
        ddlCompany.CssClass ="form-select "
        txtPage.CssClass ="form-control "
        txtDescription.CssClass ="form-control"
        ddlActive.CssClass ="form-select "
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
