Partial Class Setting_Query
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColour()
        End If
    End Sub

    Private Sub BackColour()
        Call MessageError(False, String.Empty)

        ddlAction.BackColor = Drawing.Color.Empty
        txtQuery.BackColor = Drawing.Color.Empty
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColour()
        Try
            If ddlAction.SelectedValue = "" Then
                Call MessageError(True, "QUERY ACTION IS REQUIRED !")
                ddlAction.BackColor = Drawing.Color.Red
                ddlAction.Focus()
                Exit Sub
            End If

            If txtQuery.Text = "" Then
                Call MessageError(True, "YOUR QUERY IS REQUIRED !")
                txtQuery.BackColor = Drawing.Color.Red
                txtQuery.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If ddlAction.SelectedValue = "Create" Then
                    sdsPage.InsertCommand = txtQuery.Text
                    sdsPage.Insert()
                End If

                If ddlAction.SelectedValue = "Read" Then
                    gvList.DataSource = publicCfg.GetListData(txtQuery.Text)
                    gvList.DataBind()
                End If

                If ddlAction.SelectedValue = "Update" Then
                    sdsPage.UpdateCommand = txtQuery.Text
                    sdsPage.Update()
                End If

                If ddlAction.SelectedValue = "Delete" Then
                    sdsPage.DeleteCommand = txtQuery.Text
                    sdsPage.Delete()
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        divErrors.Visible = False : msgErrors.InnerText = Msg
        If Show = True Then : divError.Visible = True : divErrors.Visible = True : End If
    End Sub
End Class