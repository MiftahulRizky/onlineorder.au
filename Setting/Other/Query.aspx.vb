
Imports System.Data.SqlClient

Partial Class Setting_Other_Query
    Inherits Page

    Dim settingCfg As New SettingConfig
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

            If txtQuery.Text = "" Then
                MessageError(True, "YOUR QUERY IS REQUIRED !")
                txtQuery.BackColor = Drawing.Color.Red
                txtQuery.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If ddlAction.SelectedValue = "Create" Or ddlAction.SelectedValue = "Update" Or ddlAction.SelectedValue = "Delete" Then
                    Dim result As Integer = 0
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand(txtQuery.Text.Trim())
                            myCmd.Connection = thisConn
                            thisConn.Open()
                            result = myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    If result = 1 Then

                    End If
                End If
                If ddlAction.SelectedValue = "Read" Then
                    gvList.DataSource = settingCfg.GetListData(txtQuery.Text)
                    gvList.DataBind()
                End If
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
        txtQuery.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
