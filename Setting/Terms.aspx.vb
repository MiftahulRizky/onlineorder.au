Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Terms
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Customer" And Not Session("RoleName") = "Representative" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        lblCustomerId.Text = Session("CustomerId")

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblCustomerId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerQuotes SET Terms=@Terms WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblCustomerId.Text)
                        myCmd.Parameters.AddWithValue("@Terms", txtTerms.Text.Trim())

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"CustomerQuotes", lblCustomerId.Text, Session("LoginId").ToString(), "Terms & Condition Updated"}
                settingCfg.Log_Customer(dataLog)

                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                mailCfg.MailError(Page.Title, "BindData", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order/detail", False)
    End Sub

    Private Sub BindData(Id As String)
        Try
            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerQuotes WHERE Id = '" + Id + "'")

            txtTerms.Text = myData.Tables(0).Rows(0).Item("Terms").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                mailCfg.MailError(Page.Title, "BindData", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
