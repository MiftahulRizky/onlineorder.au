Imports System.Data

Partial Class Export_Sunlight
    Inherits Page

    Dim exactCfg As New ExactConfig
    Dim mailCfg As New MailConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            BackColor()

            btnSubmit.Visible = False : btnCancel.Visible = False
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
                btnSubmit.Visible = True : btnCancel.Visible = True
            End If
            If Session("RoleName") = "Sunlight Product" Then
                btnSubmit.Visible = True : btnCancel.Visible = True
            End If
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If ddlOrderStatus.SelectedValue = "" Then
                MessageError(True, "ORDER STATUS IS REQUIRED !")
                ddlOrderStatus.BackColor = Drawing.Color.Red
                ddlOrderStatus.Focus()
                Exit Sub
            End If

            If txtJobDate.Text = "" Then
                MessageError(True, "JOB DATE IS REQUIRED !")
                txtJobDate.BackColor = Drawing.Color.Red
                txtJobDate.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisData As DataSet = exactCfg.GetListData("SELECT * FROM OrderHeaders WHERE Status = '" + ddlOrderStatus.SelectedValue + "' AND OrderType = 'Panorama' AND CONVERT(DATE, OrderHeaders.JobDate) = '" + txtJobDate.Text + "' AND Active=1")

                If thisData.Tables(0).Rows.Count = 0 Then
                    MessageError(True, "TIDAK ADA DATA !")
                    Exit Sub
                End If

                Dim url As String = String.Format("xmlexact?status={0}&jobdate={1}", ddlOrderStatus.SelectedValue, txtJobDate.Text)

                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSubmit_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Private Sub BackColor()
        MessageError(False, String.Empty)

        ddlOrderStatus.BackColor = Drawing.Color.Empty
        txtJobDate.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
